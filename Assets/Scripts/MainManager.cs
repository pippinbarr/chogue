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
    public bool firstscene = false;


    private bool dothisonce = true; //hack
    public bool restartgame = false;

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
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
            //knight is chevalier to distinguish and rook is tour
            
        }
        if (firstscene)
        {
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
        }
        if (!firstscene)
        { 
            statusline.text = "level " + PlayerPrefs.GetInt("level") + "| High Score: " + PlayerPrefs.GetInt("maxlevel");
    

            //create a level
            GetComponent<MapMaker>().CreateLevel();
        }
        //select a default human piece
        foreach (Piece piece in PieceList)
        {

            if (piece.human)
            {
                CurrentActivePiece = piece;
                
            }
        }

            //update visibility manually
        

        WaitingForPlayerMove = true;
    }
	
	// Update is called once per frame
	void Update () {

        if (dothisonce)
        {

            UpdateVisibility();
            dothisonce = false;
        }



        //Get click on destination (only if in "move" mode)
        if ((WaitingForPlayerMove)&&(!WaitingForMove))
        {
            if (!PieceSelected)
            {
                
                if (LastSelectedPiece != null)
                {
                    CurrentActivePiece = LastSelectedPiece;
                    CurrentActivePiece.FindAvailableDestinations();
                    CurrentActivePiece.ShowDestinations();
                    PieceSelected = true;
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                

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
                        CurrentActivePiece.HideDestinations();
                        CurrentActivePiece = hit.transform.GetComponent<Piece>();
                        CurrentActivePiece.FindAvailableDestinations();
                        CurrentActivePiece.ShowDestinations();
                        LastSelectedPiece = CurrentActivePiece;
                        PieceSelected = true;
                    }

                }
            }
        }
        else if((!WaitingForCPUMove)&&(!WaitingForMove))
        {
           // Debug.Log("calling play black");
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
            piece.FindAvailableDestinations();
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
                piece.DecideMove();
                if (piece.BestMove > bestmove)
                {
                    bestmove = piece.BestMove;
                    bestpiece = piece;
                }
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
   public  IEnumerator MoveToTile(TileType tile)
    {
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
        yield return new WaitForSeconds(0.01f);
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




        WaitingForMove = false;

        //was this a piece? then eat it!
        if (tile.transform.tag == "piece")
        {
            EatPiece(tile.GetComponent<Piece>());
            Debug.Log("eat piece at destination");
            
        }
        /*else if (tile.CurrentPiece != null)
        {
            //not eating myself
            if (tile.CurrentPiece != CurrentActivePiece.transform)
            {
                EatPiece(tile.CurrentPiece.GetComponent<Piece>());
                Debug.Log("found a piece at destination tile");
            }
            

        }*/
        if (CurrentActivePiece.NewQueen)
        {
            CurrentActivePiece.Queen();
        }
        UpdateVisibility();
        UpdateThreats();
        
        
    }

    public void EatPiece(Piece piece)
    {
        //was it the player's king?
        if ((piece.human) && (piece.PieceType == "king"))
        {
            Debug.Log("game over");
            gameover = true;
            GameOver();
        }
        if ((!piece.human) && (piece.PieceType == "king"))
        {
            Debug.Log("game over");
            gameover = true;
            Win();
        }
        //Debug.Log("I am human? : " + CurrentActivePiece.human);
       // Debug.Log("Eaten piece is human ?: " + piece.human);
        if (piece.PieceColor == "red")
        {
           // WaitingForCPUMove = false;
            piece.human = true;
            //piece.PieceColor = "white";
            //WaitingForPlayerMove = false;
            CurrentActivePiece = piece;
            
            piece.DecideMove();
            piece.MakeMove();
            piece.PowerUp();

            // WaitingForCPUMove = true;

        }
        else if (piece.human!=CurrentActivePiece.human)
        {
            PieceList.Remove(piece);
            Destroy(piece.gameObject);
            GetComponent<AudioSource>().clip = gulp;
            GetComponent<AudioSource>().Play();
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
}
