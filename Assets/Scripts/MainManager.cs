using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    //The current active piece, currently manually assigned
    public Piece CurrentActivePiece;
    public bool WaitingForPlayerMove = false;
    public bool WaitingForCPUMove = false;
    public List<Piece> PieceList = new List<Piece>();
    int CurrentPieceIndex = 0;
    bool gameover = false;
    public bool HumanTurn = false;

	// Use this for initialization
	void Start () {
        //select a default human piece
        foreach (Piece piece in PieceList)
        {
            if (piece.human)
            {
                CurrentActivePiece = piece;
                break;
            }
        }
        WaitingForPlayerMove = true;
    }
	
	// Update is called once per frame
	void Update () {

        //Get click on destination (only if in "move" mode)
        if (WaitingForPlayerMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    Debug.Log("hit something ");
                    //if this is an available destination, move!
                    if ((hit.transform.GetComponent<TileType>()!=null)&&(hit.transform.GetComponent<TileType>().AvailableDestination))
                    {

                        MoveToTile(hit.transform.GetComponent<TileType>());
                        WaitingForPlayerMove = false;

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
        else if(!WaitingForCPUMove)
        {
            Debug.Log("calling play black");
           StartCoroutine( PlayBlack());

        }

	}
    IEnumerator PlayBlack()
    {
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
        CurrentActivePiece = bestpiece;
        Debug.Log("pause");
        WaitingForCPUMove = true;
        yield return new WaitForSeconds(0.5f);
        WaitingForCPUMove = false;
        Debug.Log("ok go");
        CurrentActivePiece.MakeMove();
        
        WaitingForPlayerMove = true;
    }
    IEnumerator PlayGame()
    {
        while (!gameover) {
            //Debug.Log("Playing piece " + CurrentPieceIndex);
            yield return new WaitForSeconds(0.5f);
            //select a default white pieace

            if (HumanTurn)
            {
                foreach (Piece piece in PieceList)
                {
                    if (piece.human)
                    {
                        CurrentActivePiece = piece;
                        break;
                    }
                }
                WaitingForPlayerMove = true;
                
                while (WaitingForPlayerMove)
                {
                    yield return new WaitForSeconds(0.01f);
                }
            
            }
            else
            {
                //select a black piece to move
                //first ask every piece what is its best move
                int bestmove = 0;
                Piece bestpiece = PieceList[0];
                foreach(Piece piece in PieceList)
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
                CurrentActivePiece = bestpiece;

                CurrentActivePiece.MakeMove();
                    
            }

        }

    }

    public void MoveToTile(TileType tile)
    {
        CurrentActivePiece.turn++;
        CurrentActivePiece.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, CurrentActivePiece.transform.position.z);
        CurrentActivePiece.HideDestinations();
        

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
    }
}
