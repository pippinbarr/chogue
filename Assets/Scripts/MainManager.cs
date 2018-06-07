using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {

    //The current active piece, currently manually assigned
    public Piece CurrentActivePiece;
    private Piece LastSelectedPiece;
    public  bool PieceSelected = false;
    public bool WaitingForPlayerMove = false;
    public bool WaitingForCPUMove = false;
    public bool WaitingForMove = false;
    public List<Piece> PieceList = new List<Piece>();
    public List<TileType> TileList = new List<TileType>();
    int CurrentPieceIndex = 0;
    bool gameover = false;
    public bool HumanTurn = false;
    public AudioClip gulp;
    public AudioClip sliding;
    public AudioClip putdown;
    public Text statusline;
    public Text msgline;
    public bool firstscene = false;
    


    private bool dothisonce = true; //hack
    public bool restartgame = false;

    // Message and notation vars
    int Turn = 1;
    private string TempMessage = "";
    private string WhiteMoveNotation = "";
    private string WhiteMoveMessage = "";

	// Use this for initialization
	void Start () {

        if (restartgame)
        {
            PlayerPrefs.DeleteAll();
        }
        //if this is the first time, setup basic vars if not load them from playerprefs
        if ((PlayerPrefs.GetInt("maxlevel",0) == 0))
        {
            Debug.Log("new game");
            PlayerPrefs.SetInt("maxlevel", 1);
            PlayerPrefs.SetInt("maxtaken", 0);
            PlayerPrefs.SetInt("gold", 0);
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
            //knight is chevalier to distinguish and rook is tour
            
        }
        if (firstscene)
        {
            PlayerPrefs.SetInt("level", 0);
            PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
            PlayerPrefs.SetInt("taken", 0);
            PlayerPrefs.SetInt("gold", 0);
        }
        if (!firstscene)
        {
            
    

            //create a level
            GetComponent<MapMaker>().CreateLevel();
        }
        //select a default human piece
        foreach (Piece piece in PieceList)
        {

            if (piece.human)
            {
                CurrentActivePiece = piece;
                CurrentActivePiece.SetActive(true);
                
            }
        }

            //update visibility manually
        

        WaitingForPlayerMove = true;
        Debug.Log("player's turn");
        
    }
	
	// Update is called once per frame
	void Update () {

        if (dothisonce)
        {

            UpdateVisibility();
            dothisonce = false;
            UpdateStatus();
        }
        


        //Get click on destination (only if in "move" mode)
        if ((WaitingForPlayerMove)&&(!WaitingForMove))
        {
            if (!PieceSelected)
            {
                if (LastSelectedPiece != null)
                {
                    Debug.Log("finding last selected piece");
                    CurrentActivePiece = LastSelectedPiece;
                    CurrentActivePiece.SetActive(true);
                    CurrentActivePiece.FindAvailableDestinations();
                    CurrentActivePiece.ShowDestinations();
                    PieceSelected = true;
                    UpdateStatus();
                }
            }
            else
            {
                CurrentActivePiece.HideDestinations();
                CurrentActivePiece.FindAvailableDestinations();
                CurrentActivePiece.ShowDestinations();
            }

            if (Input.GetMouseButtonDown(0))
            {
                DisplayMsg("");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int layerMask = 1 << 8;
                if (Physics.Raycast(ray, out hit, 100,layerMask))
                {
                    //Debug.Log("hit something ");
                    //if this is an available destination, move!
                    Transform gototile = hit.transform;
                    if ((gototile.GetComponent<TileType>()!=null)&&(gototile.GetComponent<TileType>().AvailableDestination))
                    {
                        if (gototile.GetComponent<TileType>().CurrentPiece != null)
                        {
                            Debug.Log("going to piece instead of tile");
                            gototile = gototile.GetComponent<TileType>().CurrentPiece;

                        }
                        if(gototile.tag=="tile")
                        {
                            Debug.Log("going to a tile");
                        }
                        else if (gototile.tag == "piece")
                        {
                            Debug.Log("going to a piece");
                        }
                        StartCoroutine(MoveToTile(gototile.GetComponent<TileType>()));
                        
                        //WaitingForPlayerMove = false;
                        UpdateVisibility();
                        

                    }
                    //if this is a human piece, select it
                    if((hit.transform.GetComponent<Piece>()!=null) &&(hit.transform.GetComponent<Piece>().human))
                    {
                        if (CurrentActivePiece != null)
                        {
                            CurrentActivePiece.SetActive(false);
                            CurrentActivePiece.HideDestinations();
                        }

                        CurrentActivePiece = hit.transform.GetComponent<Piece>();
                        CurrentActivePiece.SetActive(true);
                        CurrentActivePiece.FindAvailableDestinations();
                        CurrentActivePiece.ShowDestinations();
                        LastSelectedPiece = CurrentActivePiece;
                        PieceSelected = true;
                        UpdateStatus();
                    }

                }
            }
        }


    }

    public void UpdateThreats()
    {
        //clear all tiles of their "covered and threatened"
        foreach (TileType tile in TileList)
        {
            tile.covered = false;
            tile.threatened = false;
        }
        //first, find available destinations for all pieces to updated "protected" and "covered" statuses
        foreach (Piece piece in PieceList)
        {
            piece.SetActive(true);
            piece.FindAvailableDestinations();
            piece.SetActive(false);
        }
    }

    void  PlayBlack()
    {
        //give a few seconds
        WaitingForCPUMove = true;
        //yield return new WaitForSeconds(0.5f);
        //select a black piece to move

        UpdateThreats();

        //first ask every piece what is its best move
        int bestmove = 0;
        Piece bestpiece = PieceList[0];
        foreach (Piece piece in PieceList)
        {
            //if ((piece.PieceColor=="black")&&(piece.CurrentTile.GetComponent<TileType>().visible))
            if (piece.PieceColor == "black")
             {
                piece.SetActive(true);
                piece.DecideMove();
                if (piece.BestMove > bestmove)
                {
                    bestmove = piece.BestMove;
                    bestpiece = piece;
                }
                piece.SetActive(false);
                // break;
            }
        }
        Debug.Log("best move is " + bestmove);
        //we can only move, select the one that can move closest to king

        if (bestmove < 2)

        {
            float leastdist = 1000000;
           // Debug.Log("Can only move, get closest to king");
            foreach (Piece piece in PieceList)
            {
                if (!piece.human)
                {
                    if (piece.LeastDistanceToKing < leastdist)
                    {
                        leastdist = piece.LeastDistanceToKing;
                        //Debug.Log("Least distance is " + leastdist);
                        bestpiece = piece;
                    }
                    // break;
                }
            }
        }
        CurrentActivePiece = bestpiece;
        Debug.Log(" best piece is " + CurrentActivePiece);

        if (CurrentActivePiece.BestMoveTarget != null)
        {
            Debug.Log("Do it");
            CurrentActivePiece.MakeMove();
        }
        else
        {
            ChangeTurn();
        }

        
      //  WaitingForCPUMove = false;
        



        //WaitingForPlayerMove = true;
        //Debug.Log("Waiting for player's turn");
        //PieceSelected = false;


    }

    public void UpdateVisibility()
    {
        //Debug.Log("updating visibility of " + TileList.Count + "tiles");
        foreach (TileType tile in TileList)
        {
            tile.SetVisibility();
        }
        foreach (Piece piece in PieceList)
        {
            piece.SetVisibility();
        }
    }


    //make this a coroutine with actual movement, will collide with tiles and make them visible (maybe?!)
    public IEnumerator MoveToTile(TileType tile, bool nomessage = false)
    {
        //Am I going to a tile that has a piece?
        if (tile.CurrentPiece != null)
        {
            if (tile.CurrentPiece != CurrentActivePiece.transform)
            {
                tile = tile.CurrentPiece.GetComponent<TileType>();
            }
            
        }

        //saving original position (for message)
        Transform OriginPosition = CurrentActivePiece.CurrentTile;
        
        //Debug.Log("destination tile type: "+tile.GetComponent<TileType>().Type);
        WaitingForMove = true;
        CurrentActivePiece.CurrentTile.GetComponent<Collider>().enabled = true;
        CurrentActivePiece.turn++;
        Vector3 destination = new Vector3(tile.transform.position.x, tile.transform.position.y, CurrentActivePiece.transform.position.z);
        CurrentActivePiece.HideDestinations();
        Vector3 movestep = (destination - CurrentActivePiece.transform.position) / (Vector3.Distance(destination, CurrentActivePiece.transform.position)*3);
        
        if(Vector3.Distance(destination, CurrentActivePiece.transform.position) > 4)
        {
            GetComponent<AudioSource>().clip = sliding;
            GetComponent<AudioSource>().Play();
        }
        if (!nomessage)
        {
            CurrentActivePiece.SetActive(true);
        }
        while (( Vector3.Distance(destination, CurrentActivePiece.transform.position)>0.2f)&&WaitingForMove)
        {
            WaitingForMove = true;
            //Debug.Log("distance: "+Vector3.Distance(destination, CurrentActivePiece.transform.position));
            CurrentActivePiece.transform.position = CurrentActivePiece.transform.position + movestep;
            yield return new WaitForSeconds(0.01f);
            //UpdateVisibility();
        }

        GetComponent<AudioSource>().clip = putdown;
        GetComponent<AudioSource>().Play();
        CurrentActivePiece.transform.position = destination;
        //did we land on stairs?

        if (tile.GetComponent<TileType>().Type == 3)
        {
            Debug.Log("landed on stairs");
            CurrentActivePiece.gameObject.SetActive(false);
            if(CurrentActivePiece.PieceColor == "white")
            {
                LastSelectedPiece = null;
                ChangeLevel();
            }
            else
            {
                //remove king
                PieceList.Remove(CurrentActivePiece);
                Destroy(CurrentActivePiece.gameObject);
                //CurrentActivePiece = PieceList[0];
            }
        }

        

        //Start preparing the displayed message
        //Describe the move in chess notation


        string PieceType = CurrentActivePiece.PieceType;
        string OriginCoordinate = OriginPosition.position.x.ToString() + OriginPosition.position.y.ToString();
        string DestinationCoordinate = tile.transform.position.x.ToString() + tile.transform.position.y.ToString();
        string ActionSymbol = ""; // will be set to x or * by EatPiece()
        string DestinationPiece = "";
        string InCheck = "";

        //check whether piece has enemy king in check
        CurrentActivePiece.FindAvailableDestinations();
        CurrentActivePiece.SetActive(false);

        WaitingForMove = false;

        
        if (CurrentActivePiece.check)
        {
            InCheck = "+";
        }

        TempMessage = ""; //reset message
        //was this a piece? then eat it!
        if ((tile.transform.tag == "piece")&&(!nomessage))
        {
            ActionSymbol = EatPiece(tile.GetComponent<Piece>());
            Debug.Log("eat piece at destination");
            DestinationPiece = tile.GetComponent<Piece>().PieceType;
            
        }

        //if attack but not kill, send piece back to where it came from!
        if (ActionSymbol == "*")
        {
            Piece TempPiece = CurrentActivePiece;
            StartCoroutine(MoveToTile(OriginPosition.GetComponent<TileType>(),true));
            //wait for the piece to get there
            WaitingForMove = true;
            while (WaitingForMove)
            {
                yield return new WaitForSeconds(0.1f);
            }
            CurrentActivePiece = TempPiece;
            
        }
        

        // The eating/attacking message is defined in TempMessage by the EatPiece() function
        //Now building the full notation
        string FullNotation = "(" + PieceType + OriginCoordinate + ActionSymbol + DestinationPiece + DestinationCoordinate + InCheck+") ";

        //Now assembling complete message
        //output is different whether it's the human turn or not
        //tempmessage is the attacking flavor defined in EatPiece();
        if (!nomessage)
        {
            if (CurrentActivePiece.human)
            {
                Turn++;
                WhiteMoveNotation = FullNotation; //keeping it for black's turn
                WhiteMoveMessage = TempMessage;
                TempMessage = Turn.ToString() + ". " + FullNotation + TempMessage;
            }
            else
            {
                //if no black message, then put back the white message
                if (TempMessage == "")
                {
                    TempMessage = WhiteMoveMessage;
                }
                TempMessage = Turn.ToString() + ". " + WhiteMoveNotation + " " + FullNotation + TempMessage;

            }
            //display message
            DisplayMsg(TempMessage);
        }

       


        if (CurrentActivePiece.NewQueen)
        {
            CurrentActivePiece.Queen();
        }
        if (!nomessage)
        {
            ChangeTurn();
        }


        
        
    }
    public void ChangeTurn()
    {

        if (WaitingForPlayerMove)
        {
            Debug.Log("CPU's turn");
            WaitingForPlayerMove = false;
            PlayBlack();
        }
        else
        {
            Debug.Log("player's turn");
            WaitingForCPUMove = false;
            WaitingForPlayerMove = true;
            PieceSelected = false;
        }

    }
    public string EatPiece(Piece piece)
    {

        //Calculate damage (0 to MaxHP)
        int DMG = (int)(Random.Range(0, CurrentActivePiece.MaxHP+1));
        Debug.Log("damage:" + DMG);
        //apply damage
        piece.HP -= DMG;

        //Dead?
        if (piece.HP < 1)
        {

            if (CurrentActivePiece.human && !piece.human && (piece.PieceType != "coin"))
            {
                TempMessage = " Your " + CurrentActivePiece.PieceType + " scored an excellent hit on the " + piece.PieceType;
            }
            if (!CurrentActivePiece.human && piece.human)
            {
                TempMessage = " The " + CurrentActivePiece.PieceType + " scored an excellent hit on your " + piece.PieceType;
            }
            if (CurrentActivePiece.human && (piece.PieceColor == "red") && piece.PieceType != "coin")
            {
                TempMessage = " You now have a new " + piece.PieceType + " !";
            }


            //was it the player's king?
            if ((piece.human) && (piece.PieceType == "king"))
            {
                Debug.Log("game over");
                gameover = true;
                GameOver();
            }
            if ((!piece.human) && (piece.PieceType == "king") && (piece.PieceColor != "red"))
            {
                Debug.Log("game over");
                gameover = true;
                Win();
            }
            //Debug.Log("I am human? : " + CurrentActivePiece.human);
            // Debug.Log("Eaten piece is human ?: " + piece.human);
            if (piece.PieceColor == "red")
            {
                if (piece.PieceType != "coin")
                {
                    // WaitingForCPUMove = false;
                    piece.human = true;
                    //piece.PieceColor = "white";
                    //WaitingForPlayerMove = false;
                    CurrentActivePiece = piece;

                    piece.DecideMove();
                    piece.MakeMove();
                    piece.PowerUp();
                }
                else
                {
                    int gold = (int)(Random.value * 50);
                    TempMessage = " You found " + gold + " gold !";
                    PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold") + gold);
                    PieceList.Remove(piece);
                    Destroy(piece.gameObject);
                    GetComponent<AudioSource>().clip = gulp;
                    GetComponent<AudioSource>().Play();
                }




                // WaitingForCPUMove = true;

            }
            else if (piece.human != CurrentActivePiece.human)
            {
                PieceList.Remove(piece);
                Destroy(piece.gameObject);
                GetComponent<AudioSource>().clip = gulp;
                GetComponent<AudioSource>().Play();
            }
            if (CurrentActivePiece.human)
            {
                PlayerPrefs.SetInt("taken", PlayerPrefs.GetInt("taken") + 1);
                if (PlayerPrefs.GetInt("taken") > PlayerPrefs.GetInt("maxtaken"))
                {
                    PlayerPrefs.SetInt("maxtaken", PlayerPrefs.GetInt("taken"));
                }
            }

            return "x"; //will return * if partial hit in the HP branch
        }
        //not dead!
        else
        {
            TempMessage = "Some damage but no kill";
            //you have to go back to where you came from!
            return "*";
            
        }
        
    }



    private void ChangeLevel()
    {
        string outgoingpieces = "";
        foreach(Piece piece in PieceList)
        {
            if (piece.PieceColor == "white")
            {
                if (piece.PieceType == "pawn")
                {
                    outgoingpieces = outgoingpieces + "p";
                }
                else if (piece.PieceType == "bishop")
                {
                    outgoingpieces = outgoingpieces + "b";
                }
                else if (piece.PieceType == "knight")
                {
                    outgoingpieces = outgoingpieces + "c";
                }
                else if (piece.PieceType == "rook")
                {
                    outgoingpieces = outgoingpieces + "t";
                }
                else if (piece.PieceType == "king")
                {
                    outgoingpieces = outgoingpieces + "k";
                }
                else if (piece.PieceType == "queen")
                {
                    outgoingpieces = outgoingpieces + "q";
                }
            }

        }
        PlayerPrefs.SetString("IncomingPieces", outgoingpieces);
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        if (PlayerPrefs.GetInt("level") > PlayerPrefs.GetInt("maxlevel"))
        {
            PlayerPrefs.SetInt("maxlevel", PlayerPrefs.GetInt("level"));
        }

        SceneManager.LoadScene("LevelGen"); 
    }
    public void GameOver()
    {
        
        SceneManager.LoadScene("GameOver");

    }
    public void Win()
    {

        SceneManager.LoadScene("Victory");

    }
    public void UpdateStatus()
    {
        int CurrentPieces = 0;
        foreach(Piece piece in PieceList)
        {
            if (piece.human)
            {
                CurrentPieces++;
            }
        }
        if (statusline != null)
        {
            statusline.text = " Level:" + PlayerPrefs.GetInt("level") + " (highest "+ PlayerPrefs.GetInt("maxlevel")+")     Hits:"+CurrentActivePiece.HP+"("+CurrentActivePiece.MaxHP+")"+"     Pieces:" + CurrentPieces + "     Captured:" + PlayerPrefs.GetInt("taken") + "     Gold:"+ PlayerPrefs.GetInt("gold");
        }
        
    }
    public void DisplayMsg(string msg)
    {
        if (msgline != null)
        {
            msgline.text = msg;
        }
    }
}
