using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Piece : MonoBehaviour {

    public bool human = true; //is this a human controlled piece or not?
    public bool manual = false; //turn on if it creates itself
    public int turn = 0; //a priori seulement pour le pion, pour savoir s'il peut avancer deux fois
    public string PieceType = "rook";
    public string PieceColor = "white";
    public Transform PieceWhiteModel;
    public Transform PieceBlackModel;
    public Transform QueenPrefab;
    public bool NewQueen = false;


    //Array of colliders
    public Collider[] Colliders;

    Transform PieceModel;
    List<TileType> TileList = new List<TileType>();


	// Use this for initialization
	void Start () {
        if (manual)
        {
            CreateModel(PieceColor);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateModel(string pieceColor)
    {
        if (pieceColor == "white") {
            PieceModel = Instantiate(PieceWhiteModel, transform.position, PieceWhiteModel.rotation);
            PieceModel.parent = transform;
            PieceColor = "white";
        }
        else
        {
            PieceModel = Instantiate(PieceBlackModel, transform.position, PieceBlackModel.rotation);
            PieceModel.parent = transform;
            PieceColor = "black";
        }
        
    }

    public void FindAvailableDestinations()
    {
        //le pion est vraiment un cas spécial
        if (PieceType == "pawn")
        {
            FindAvailableDestinationsForPawn();
        }
        else
        {
            TileList.Clear();
            //Crawl through each collider
            foreach (Collider col in Colliders)
            {
                List<TileType> TempTileList = col.transform.GetComponent<GetCollidingThings>().CollidingTileList;
                //for each tile, find out the distance to this piece
                foreach (TileType tile in TempTileList.ToList<TileType>())
                {
                    //in case it was a piece that was eaten
                    if (tile == null)
                    {
                        TileList.Remove(tile);
                        TempTileList.Remove(tile);

                    }
                    else
                    {
                        tile.GetDistanceTo(transform);
                    }

                }

                //sort this list by distance

                TempTileList = TempTileList.OrderBy(tile => tile.DistanceToPiece).ToList();

                //then add tiles to main list until you find a wall
                foreach (TileType tile in TempTileList)
                {
                    if (tile.transform.tag == "tile")
                    {
                        TileList.Add(tile);
                    }
                    else if (tile.transform.tag == "wall")
                    {
                        break;
                    }
                    else if (tile.transform.tag == "piece")
                    {
                        TileList.Add(tile);
                        break;

                    }
                }

            }
        }
    }

    public void FindAvailableDestinationsForPawn()
    {
        TileList.Clear();

        //can I move forward 1? The first collider is the move forward 1.
        List<TileType> TempTileList = Colliders[0].transform.GetComponent<GetCollidingThings>().CollidingTileList;
        TempTileList = TempTileList.OrderBy(tile => tile.DistanceToPiece).ToList();
        bool nope = false;
        foreach (TileType tile in TempTileList)
        {

            if ((tile.transform.tag == "piece")|| (tile.transform.tag == "wall"))
            {
                nope = true;
            }
           
        }
        if ((!nope)&&(TempTileList.Count>0))
        {
            TileList.Add(TempTileList[0]);
        }

        //if tile 1 is a free tile then maybe I can go to tile 2 if it's my first turn
        if ((turn == 0) && (TileList.Count > 0))
        {
            TempTileList = Colliders[3].transform.GetComponent<GetCollidingThings>().CollidingTileList;
            TempTileList = TempTileList.OrderBy(tile => tile.DistanceToPiece).ToList();
            nope = false;
            foreach (TileType tile in TempTileList)
            {

                if ((tile.transform.tag == "piece") || (tile.transform.tag == "wall"))
                {
                    nope = true;
                }

            }
            if  ((!nope) && (TempTileList.Count > 0))
                {
                TileList.Add(TempTileList[0]);
            }
        }

        //if a piece is in my diagonal, then I can eat it. But I can't move.
        TempTileList = Colliders[1].transform.GetComponent<GetCollidingThings>().CollidingTileList;
        TempTileList.AddRange(Colliders[2].transform.GetComponent<GetCollidingThings>().CollidingTileList);
        foreach (TileType tile in TempTileList)
        {
 
           if (tile.transform.tag == "piece")
            {
                TileList.Add(tile);
            }
        }

    }
    public void Queen()
    {
        Debug.Log("queen");
        Transform TempPiece = Instantiate(QueenPrefab, transform.position, transform.rotation);
        TempPiece.GetComponent<Piece>().CreateModel(PieceColor);
        TempPiece.GetComponent<Piece>().human = human;
        TempPiece.GetComponent<Piece>().NewQueen = true;
        GameObject.Find("MainManager").GetComponent<MainManager>().PieceList.Insert(0,TempPiece.GetComponent<Piece>());
        GameObject.Find("MainManager").GetComponent<MainManager>().PieceList.Remove(this);
        Destroy(gameObject);
    }
    public void ShowDestinations()
    {
        foreach (TileType tile in TileList)
        {
            tile.Highlight(true);
        }
    }
    public void HideDestinations()
    {
        foreach (TileType tile in TileList)
        {
            tile.Highlight(false);
        }
    }
    public void DecideMove()
    {
        turn++;
        
        //Find available destinations
        FindAvailableDestinations();
        
        //See if the human is there
        foreach(TileType tile in TileList)
        {
            if (tile.transform.tag == "piece")
            {
                if (tile.GetComponent<Piece>().human)
                {
                    //Debug.Log("eat human");
                    GameObject.Find("MainManager").GetComponent<MainManager>().MoveToTile(tile);
                    return;
                }
            }
        }

        //Get the human coordinates
        //******Currently this is hard coded... the player is always first in this list
        Piece HumanPlayer = GameObject.Find("MainManager").GetComponent<MainManager>().PieceList[0];

        //Find distance of all available tiles to human
        foreach (TileType tile in TileList)
        {
            tile.GetDistanceTo(HumanPlayer.transform);
        }


        //Sort them
        TileList = TileList.OrderBy(tile => tile.DistanceToPiece).ToList();

        //go to first one if not zéro
        if (TileList.Count > 0)
        {
            GameObject.Find("MainManager").GetComponent<MainManager>().MoveToTile(TileList[0]);
        }
        



    }


}
