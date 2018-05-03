using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    //The current active piece, currently manually assigned
    public Piece CurrentActivePiece;
    public bool WaitingForPlayerMove = false;
    public bool WaitingForCPUMove = false;
    public bool WaitingForMove = false;
    public List<Piece> PieceList = new List<Piece>();
    public List<TileType> TileList = new List<TileType>();
    int CurrentPieceIndex = 0;
    bool gameover = false;
    public bool HumanTurn = false;


    private bool dothisonce = true; //hack

	// Use this for initialization
	void Start () {

        //manually set Playerprefs right now
        PlayerPrefs.SetString("IncomingPieces", "kqcbtppp");
        //knight is chevalier to distinguish and rook is tour

        //create a level
        GetComponent<MapMaker>().CreateLevel();    
            
        //select a default human piece
        foreach (Piece piece in PieceList)
        {
            if (piece.human)
            {
                CurrentActivePiece = piece;
                break;
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
            if (Input.GetMouseButtonDown(0))
            {
                

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    //Debug.Log("hit something ");
                    //if this is an available destination, move!
                    if ((hit.transform.GetComponent<TileType>()!=null)&&(hit.transform.GetComponent<TileType>().AvailableDestination))
                    {
                        WaitingForPlayerMove = false;
                        StartCoroutine(MoveToTile(hit.transform.GetComponent<TileType>()));
                        
                        UpdateVisibility();

                    }
                    //if this is a human piece, select it
                    if((hit.transform.GetComponent<Piece>()!=null) &&(hit.transform.GetComponent<Piece>().human))
                    {
                        CurrentActivePiece.HideDestinations();
                        CurrentActivePiece = hit.transform.GetComponent<Piece>();
                        CurrentActivePiece.FindAvailableDestinations();
                        CurrentActivePiece.ShowDestinations();
                    }

                }
            }
        }
        else if((!WaitingForCPUMove)&&(!WaitingForMove))
        {
            Debug.Log("calling play black");
            StartCoroutine( PlayBlack());
           


        }

    }
    IEnumerator PlayBlack()
    {
        //give a few seconds
        WaitingForCPUMove = true;
        //yield return new WaitForSeconds(0.5f);
        //select a black piece to move
        //first ask every piece what is its best move
        int bestmove = 0;
        Piece bestpiece = PieceList[0];
        foreach (Piece piece in PieceList)
        {
            if (!piece.human)
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
        Debug.Log("best move is " + bestmove);
        //we can only move, select the one that can move closest to king

        if (bestmove < 2)

        {
            float leastdist = 1000000;
            Debug.Log("Can only move, get closest to king");
            foreach (Piece piece in PieceList)
            {
                if (!piece.human)
                {
                    if (piece.LeastDistanceToKing < leastdist)
                    {
                        leastdist = piece.LeastDistanceToKing;
                        Debug.Log("Least distance is " + leastdist);
                        bestpiece = piece;
                    }
                    // break;
                }
            }
        }
        CurrentActivePiece = bestpiece;
        WaitingForCPUMove = false;
        CurrentActivePiece.MakeMove();
        yield return new WaitForSeconds(0.1f);

        WaitingForPlayerMove = true;
    }

    public void UpdateVisibility()
    {
        Debug.Log("updating visibility of " + TileList.Count + "tiles");
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
        CurrentActivePiece.turn++;
        Vector3 destination = new Vector3(tile.transform.position.x, tile.transform.position.y, CurrentActivePiece.transform.position.z);
        CurrentActivePiece.HideDestinations();
        Vector3 movestep = (destination - CurrentActivePiece.transform.position) / (Vector3.Distance(destination, CurrentActivePiece.transform.position)*3);
        WaitingForMove = true;
        while ( Vector3.Distance(destination, CurrentActivePiece.transform.position)>0.2f)
        {
            //Debug.Log("distance: "+Vector3.Distance(destination, CurrentActivePiece.transform.position));
            CurrentActivePiece.transform.position = CurrentActivePiece.transform.position + movestep;
            yield return new WaitForSeconds(0.05f);
            UpdateVisibility();
        }
        CurrentActivePiece.transform.position = destination;
        WaitingForMove = false;
        
        

        //was this a piece? then eat it!
        if (tile.transform.tag == "piece")
        {
            //was it the player's king?
            if ((tile.transform.GetComponent<Piece>().human)&&(tile.transform.GetComponent<Piece>().PieceType=="king"))
            {
                Debug.Log("game over");
                gameover = true;
            }

            PieceList.Remove(tile.transform.GetComponent<Piece>());
            Destroy(tile.transform.gameObject);
            GetComponent<AudioSource>().Play();
        }
        UpdateVisibility();
    }
}
