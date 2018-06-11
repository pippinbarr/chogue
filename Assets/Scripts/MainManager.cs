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
    public bool WaitingForMoveBack = false;
    public List<Piece> PieceList = new List<Piece>();
    public List<TileType> TileList = new List<TileType>();
    int CurrentPieceIndex = 0;
    bool gameover = false;
    public bool HumanTurn = false;
    public AudioClip gulp;
    public AudioClip sliding;
    public AudioClip putdown;
    public AudioClip tic;
    public Text statusline;
    public Text msgline;
    public bool firstscene = false;


    private bool dothisonce = true; //hack
    public bool restartgame = false;
    public bool HitPointVersion = false;

    // Message and notation vars
    int Turn = 1;
    private string TempMessage = "";
    private string WhiteMoveNotation = "";
    private string WhiteMoveMessage = "";

    private string[] files = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "bb", "cc", "dd", "ee", "ff", "gg", "hh", "ii", "jj", "kk", "ll", "mm", "nn", "oo", "pp", "qq", "rr", "ss", "tt", "uu", "vv", "ww", "xx", "yy", "zz" };
    private string[] hits = new string[] { "hits", "swings and hits", "has injured", "scored an excellent hit on" };
    private string[] misses = new string[] { "misses", "swings and misses", "barely misses", "doesn't hit" };

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
                    CurrentActivePiece = LastSelectedPiece;
                    CurrentActivePiece.SetActive(true);
                    CurrentActivePiece.FindAvailableDestinations();
                    CurrentActivePiece.ShowDestinations();
                    PieceSelected = true;
                    UpdateStatus();
                }
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
                        
                        WaitingForPlayerMove = false;
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
        else if((!WaitingForCPUMove)&&(!WaitingForMove))
        {
            // Debug.Log("calling play black");
            UpdateStatus();
            PlayBlack();
           


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
        Debug.Log("CPU's turn");
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
        //Debug.Log("best move is " + bestmove);
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
        
        CurrentActivePiece.MakeMove();
        
        WaitingForCPUMove = false;
        



        WaitingForPlayerMove = true;
        Debug.Log("Waiting for player's turn");
        PieceSelected = false;


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
        //UpdateThreats();
        //Am I going to a tile that has a piece?
        if (tile.CurrentPiece != null)
        {
            Debug.Log("Found a piece at the tile, going there instead");
            tile = tile.CurrentPiece.GetComponent<TileType>();
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
        CurrentActivePiece.SetActive(true);
        while (( Vector3.Distance(destination, CurrentActivePiece.transform.position)>0.2f)&&WaitingForMove)
        {
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

        //string OriginCoordinate = OriginPosition.position.x.ToString() + OriginPosition.position.y.ToString();

        string PieceSymbol = CurrentActivePiece.PieceSymbol;
        string OriginCoordinate = OriginPosition.position.x.ToString() + OriginPosition.position.y.ToString();

        string DestinationCoordinate = tile.transform.position.x.ToString() + tile.transform.position.y.ToString();
        string ActionSymbol = ""; // will be set to x or * by EatPiece()
        string DestinationPiece = "";
        string InCheck = "";

        //check whether piece has enemy king in check
        CurrentActivePiece.FindAvailableDestinations();
       CurrentActivePiece.SetActive(false);

       

        
        if (CurrentActivePiece.check)
        {
            InCheck = "+";
        }

        TempMessage = ""; //reset message
        //was this a piece? then eat it!
        if (tile.transform.tag == "piece")
        {
            ActionSymbol = EatPiece(tile.GetComponent<Piece>());
            Debug.Log("eat piece at destination");
            DestinationPiece = tile.GetComponent<Piece>().PieceType;
            
        }



        // The eating/attacking message is defined in TempMessage by the EatPiece() function
        //Now building the full notation
        //string FullNotation = "(" + PieceType + OriginCoordinate + ActionSymbol + DestinationPiece + DestinationCoordinate + InCheck + ") ";


        // Calculate notation for the squares

        string startRank = (OriginPosition.position.y).ToString();
        int startFileIndex = Mathf.FloorToInt(OriginPosition.position.x);
        string startFile = files[startFileIndex];

        string destRank = (tile.transform.position.y).ToString();
        int destFileIndex = Mathf.FloorToInt(tile.transform.position.x);
        string destFile = files[destFileIndex];

        // Bug: InCheck didn't work for a black knight versus my kind...
        string MoveNotation = PieceSymbol + ActionSymbol + destFile + destRank + InCheck;

        //Now assembling complete message
        string FullMoveMessage;
        if (!nomessage)
        {
            if (CurrentActivePiece.human)
            {
                Turn++;
                FullMoveMessage = Turn.ToString() + ". " + MoveNotation;
                string AttackFlavour = TempMessage;
                if (AttackFlavour != "")
                {
                    FullMoveMessage += " (" + AttackFlavour + ")";
                }

                WhiteMoveMessage = FullMoveMessage; //keeping it for black's turn
            }
            else
            {
                FullMoveMessage = WhiteMoveMessage + " " + MoveNotation;
                string AttackFlavour = TempMessage;
                if (AttackFlavour != "")
                {
                    FullMoveMessage += " (" + AttackFlavour + ")";
                }

            }



            //display message
            DisplayMsg(FullMoveMessage);
        }
 


        if (CurrentActivePiece.NewQueen)
        {
            CurrentActivePiece.Queen();
            CurrentActivePiece = PieceList[0];
        }
        


        WaitingForMove = false;
        //if attack but not kill, send piece back to where it came from!
        if (WaitingForMoveBack)
        {
            WaitingForMove = true;

            WaitingForMoveBack = false;
            StartCoroutine(MoveToTile(OriginPosition.GetComponent<TileType>(), true));
            while (WaitingForMove)
            {
               
                yield return new WaitForSeconds(0.01f);
               
            }
            

        }
        UpdateVisibility();
        UpdateThreats();

    }

    public string EatPiece(Piece piece)
    {


        //Calculate damage (0 to MaxHP)
        int DMG = (int)(Random.Range(0, CurrentActivePiece.MaxHP+1));
        Debug.Log("damage:" + DMG);

        //If this is the clean take version, just add tons of damage
        if (!HitPointVersion)
        {
            DMG = 1000000000;
        }

        //apply damage
        piece.HP -= DMG;

        //Dead?
        if ((piece.HP < 1)||(piece.PieceColor=="red"))
        {
            if (CurrentActivePiece.human && !piece.human && (piece.PieceType != "coin"))
            {
                if (HitPointVersion)
                {
                    TempMessage = "Your " + CurrentActivePiece.PieceType + " defeated the black " + piece.PieceType;
                }
                else
                {
                    TempMessage = "Your " + CurrentActivePiece.PieceType + " captured the black " + piece.PieceType;
                }
            }
            if (!CurrentActivePiece.human && piece.human)
            {
                if (HitPointVersion)
                {
                    TempMessage = "The black " + CurrentActivePiece.PieceType + " defeated your " + piece.PieceType;
                }
                else
                {
                    TempMessage = "The black " + CurrentActivePiece.PieceType + " captured your " + piece.PieceType;
                }
            }
            if (CurrentActivePiece.human && (piece.PieceColor == "red") && piece.PieceType != "coin")
            {
                TempMessage = "You now have a new " + piece.PieceType + "!";
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
                    // CurrentActivePiece = piece;

                    // piece.DecideMove();
                    // piece.MakeMove();
                    WaitingForMoveBack = true;
                    piece.PowerUp();
                }
                else
                {
                    int gold = (int)(Random.value * 50);
                    TempMessage = "You found " + gold + " gold!";
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

              /*  int gold = (int)(Random.value * 50);
                TempMessage = "You found " + gold + " gold !";
                PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold")+gold);*/

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
            string verb = "xxxxxx";
            if (DMG == 0)
            {
                verb = misses[Random.Range(0, misses.Length - 1)];
            }
            else
            {
                verb = hits[Random.Range(0, hits.Length - 1)];
            }

            if (CurrentActivePiece.human)
            {
                TempMessage = "Your " + CurrentActivePiece.PieceType + " " + verb + " the " + piece.PieceColor + " " + piece.PieceType;

            }
            else
            {
                TempMessage = "The " + CurrentActivePiece.PieceColor + " " + CurrentActivePiece.PieceType + " " + verb + " your " + piece.PieceType; ;
            }

            //TempMessage = "Some damage but no kill";
            //you have to go back to where you came from!
            WaitingForMoveBack = true;
            GetComponent<AudioSource>().clip = tic;
            GetComponent<AudioSource>().Play();
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
