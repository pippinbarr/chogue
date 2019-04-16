using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Piece : MonoBehaviour {

    public bool human = true; //is this a human controlled piece or not?
    public bool manual = false; //turn on if it creates itself
    public int turn = 0; //a priori seulement pour le pion, pour savoir s'il peut avancer deux fois
    public int HP = 0;
    public int MaxHP = 0;
    public string PieceType = "rook";
    public string PieceColor = "white";
    public string PieceSymbol = "R";
    public Transform PieceWhiteModel;
    public Transform PieceBlackModel;
    public Transform PieceRedModel;
    public Transform QueenPrefab;
    public bool NewQueen = false;
    public Transform CurrentTile;
    public int BestMove = 0; //0 : can't move, 1: can move, 2: can eat, 3: can eat white king
    public Transform BestMoveTarget; //
    public float LeastDistanceToKing = 1000000000;//
    public Color CurrentTileRoomColor;
    public bool threatened = false; // piece is in the line of enemy piece
    //public bool covered = false; //
    public bool guarding = false; //piece could recapture 
    public bool check = false; //Does this piece have the king in check?
    public bool eaten = false;
    public bool guarded = false; //a friendly piece can recapture if eaten
    public int protecting = 0; //the point value of a piece that would be threatened if I moved

    private MainManager MM;


    //Array of colliders
    public Collider[] Colliders;

    Transform PieceModel;
    public List<TileType> TileList = new List<TileType>();


	// Use this for initialization
	void Start () {
        MM = GameObject.Find("MainManager").GetComponent<MainManager>();
        if (manual)
        {
            CreateModel(PieceColor);
        }

        if (PieceType == "king") PieceSymbol = "K";
        else if (PieceType == "queen") PieceSymbol = "Q";
        else if (PieceType == "rook") PieceSymbol = "R";
        else if (PieceType == "bishop") PieceSymbol = "B";
        else if (PieceType == "knight") PieceSymbol = "N";
        else if (PieceType == "pawn") PieceSymbol = "";
	}
	
	// Update is called once per frame
	void Update () {
        /*if( (NewQueen)&&(!MM.WaitingForMove))
        {
            Queen();
            MM.PieceSelected = false;
        }*/
    }

    public void CreateModel(string pieceColor)
    {
        if (pieceColor == "white") {
            PieceModel = Instantiate(PieceWhiteModel, transform.position, PieceWhiteModel.rotation);
            PieceModel.parent = transform;
            PieceColor = "white";
            if (GetComponentInChildren<VisibilitySwipe>() != null)
            {
               // Debug.Log("set swipe active");
                GetComponentInChildren<VisibilitySwipe>().gameObject.GetComponent<Collider>().enabled = true ;
            }
           
        }
        else if(pieceColor=="black")
        {
            PieceModel = Instantiate(PieceBlackModel, transform.position, PieceBlackModel.rotation);
            PieceModel.parent = transform;
            PieceColor = "black";
            
        }
        //this is a powerup
        else
        {
            PieceModel = Instantiate(PieceRedModel, transform.position, PieceRedModel.rotation);
            PieceModel.parent = transform;
            PieceColor = "red";
        }

        
    }

    public void FindAvailableDestinations()
    {
        check = false;
    //    guarding = false;
     //   guarded = false;
       // threatened = false;
       // covered = false;
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
                List<TileType> TempTileList = new List<TileType>();
                foreach (TileType tile in col.transform.GetComponent<GetCollidingThings>().CollidingTileList)
                {
                   if (tile.CurrentPiece == null)
                    {
                        TempTileList.Add(tile);
                    }
                    else
                    {
                        
                        TempTileList.Add(tile.CurrentPiece.GetComponent<TileType>());
                    }
                }
                //col.transform.GetComponent<GetCollidingThings>().CollidingTileList;
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

                bool LookingForProtectedPiece = false;
                Piece PossiblyProtectingPiece = this;
                //int tilenum = 0;
                foreach (TileType tile in TempTileList)
                {

                  //  Debug.Log("tile number: " + tilenum);
                 //   tilenum++;
                    if ((tile.transform.tag == "wall")|| ((tile.Type == 3)&&(PieceColor=="black")&&(PieceType!="king")))
                    {
                        break;
                    }
                    else if ((tile.transform.tag == "piece"))
                    {

                        
                        if ((tile.GetComponent<Piece>().human != human))
                        {
                            if (!LookingForProtectedPiece)
                            {
                                TileList.Add(tile);
                                tile.GetComponent<Piece>().threatened = true;
                                PossiblyProtectingPiece = tile.GetComponent<Piece>();
                                Debug.Log("protecting piece tyoe = " + PossiblyProtectingPiece.PieceType);
                                tile.threatened = true;
                                if (tile.GetComponent<Piece>().PieceType == "king")
                                {
                                    Debug.Log("king in check");
                                    check = true;
                                }
                            }
                            else
                            {
                                if(PossiblyProtectingPiece!= tile.GetComponent<Piece>())
                                {
                                    PossiblyProtectingPiece.protecting = tile.GetComponent<Piece>().MaxHP;
                                    Debug.Log("protected piece type = " + tile.GetComponent<Piece>().PieceType);
                                    break;
                                }
                                
                            }


                        }
                        else 
                        {
                            if (PossiblyProtectingPiece)
                            {
                                break;
                            }                              
                            tile.GetComponent<Piece>().guarded = true;
                            guarding = true;
                        }

                        if (TileList.Contains(tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>()))
                        {
                            TileList.Remove(tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>());
                        }
                        LookingForProtectedPiece = true;
                       // break;

                    }
                    else if (tile.transform.tag == "tile")
                    {
                        if (!LookingForProtectedPiece)
                        {
                            TileList.Add(tile);
                            //if we are white, set visibility around
                            if (PieceColor == "white")
                            {
                                tile.GetDistanceTo(transform);
                                if (tile.DistanceToPiece < 3)
                                {
                                    tile.visited = true;
                                    tile.visible = true;
                                    tile.SetVisibility();

                                }
                                tile.threatened = true;
                            }
                        }
                       /*
                        else 
                        {
                            tile.threatened = true;

                            //break;
                        }
                        */
                    }

                }

            }
        }
        if (TileList.Count > 0)
        {
            //update covered and threatened
            foreach(TileType tile in TileList)
            {
                if (PieceColor == "white")
                {
                    tile.threatened = true;
                    if (tile.GetComponent<Piece>() != null)
                    {
                        tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>().threatened = true;
                    }
                    if (tile.CurrentPiece != null)
                    {
                        tile.CurrentPiece.GetComponent<Piece>().threatened = true;
                    }
                }
                else
                {
                    tile.covered = true;
                    if (tile.GetComponent<Piece>() != null)
                    {
                        tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>().covered = true;
                    }
                }
            }
        }
    }

    public void FindAvailableDestinationsForPawn()
    {
        TileList.Clear();

        //can I move forward 1? The first collider is the move forward 1.
        List<TileType> TempTileList = new List<TileType>();
        foreach(TileType tile in Colliders[0].transform.GetComponent<GetCollidingThings>().CollidingTileList)
        {
            TempTileList.Add(tile);
        }

           // Colliders[0].transform.GetComponent<GetCollidingThings>().CollidingTileList;
        foreach (TileType tile in TempTileList.ToList<TileType>())
        {
            //in case it was a piece that was eaten
            if (tile == null)
            {
                TileList.Remove(tile);
                TempTileList.Remove(tile);

            }

        }
        bool nope = false;
        foreach (TileType tile in TempTileList)
        {

            if (tile.CurrentPiece != null)
            {
                nope = true;
                if (TileList.Contains(tile))
                {
                    TileList.Remove(tile);
                }
            }
            if (tile.transform.tag == "piece")
            {

                if (TileList.Contains(tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>()))
                {
                    TileList.Remove(tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>());
                }


                nope = true;
            }
            if (tile.transform.tag == "wall")
            {

                if (TileList.Contains(tile))
                {
                    TileList.Remove(tile);
                }


                nope = true;
            }
        }
        if ((!nope)&&(TempTileList.Count>0))
        {
            //Debug.Log("how many tiles: " + TempTileList.Count);
            TileList.Add(TempTileList[0]);
        }

        //if tile 1 is a free tile then maybe I can go to tile 2 if it's my first turn
        if ((turn == 0) && (TileList.Count > 0))
        {
            TempTileList = new List<TileType>();
            foreach(TileType tile in Colliders[3].transform.GetComponent<GetCollidingThings>().CollidingTileList)
            {
                TempTileList.Add(tile);
            }
                //Colliders[3].transform.GetComponent<GetCollidingThings>().CollidingTileList;
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
            TempTileList = TempTileList.OrderBy(tile => tile.DistanceToPiece).ToList();
            nope = false;
            foreach (TileType tile in TempTileList)
            {
                if (tile.CurrentPiece != null)
                {
                    nope = true;
                    if (TileList.Contains(tile))
                    {
                        TileList.Remove(tile);
                    }
                }
                if (tile.transform.tag == "piece")
                {
                    if (TileList.Contains(tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>()))
                    {
                        TileList.Remove(tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>());
                    }
                    nope = true;
                }
                if (tile.transform.tag == "wall")
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
        //copy list

        TempTileList.Clear();
        foreach(TileType tile in Colliders[1].transform.GetComponent<GetCollidingThings>().CollidingTileList)
        {
            TempTileList.Add(tile);
        }
        foreach (TileType tile in Colliders[2].transform.GetComponent<GetCollidingThings>().CollidingTileList)
        {
            TempTileList.Add(tile);
        }
        
        foreach (TileType tile in TempTileList)
        {
            
           if ((tile!=null)&&(tile.transform.tag == "piece"))
            {
                if (tile.GetComponent<Piece>().human != human)
                {
                    TileList.Add(tile);
                    tile.GetComponent<Piece>().threatened = true;
                }
                else
                {
                    tile.GetComponent<Piece>().guarded = true;
                    guarding = true;
                }


               
            }
        }

    }
    public void Queen()
    {
        Debug.Log("queen");
        NewQueen = false;
        Transform TempPiece = Instantiate(QueenPrefab, transform.position, transform.rotation);
        TempPiece.GetComponent<Piece>().CreateModel(PieceColor);
        TempPiece.GetComponent<Piece>().human = human;
        TempPiece.GetComponent<Piece>().CurrentTile = CurrentTile;
        CurrentTile.GetComponent<TileType>().CurrentPiece = TempPiece;
        MM.PieceList.Insert(0,TempPiece.GetComponent<Piece>());
        MM.PieceList.Remove(this);
        //Debug.Log("Queen(), new PieceType is " + MM.PieceList[0].PieceType);

        //Destroy(gameObject);
        transform.position = new Vector3(10000f, 100000f, 10000f);
    }
    public void ShowDestinations()
    {
        foreach (TileType tile in TileList)
        {
            if (tile.gameObject.tag == "tile")
            {
                tile.Highlight(true);
            }
            else if(tile.gameObject.tag == "piece")
            {
                tile.Highlight(true);
                tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>().Highlight(true);
            }
            
        }
    }
    public void HideDestinations()
    {
        foreach (TileType tile in TileList)
        {
            if (tile != null)
            {
                if (tile.gameObject.tag == "tile")
                {
                    tile.Highlight(false);
                }
                else if (tile.gameObject.tag == "piece")
                {
                    tile.Highlight(false);
                    tile.Highlight(false);
                    tile.GetComponent<Piece>().CurrentTile.GetComponent<TileType>().Highlight(false);

                }
            }


        }
    }
   public void MakeMove()
    {
        Debug.Log("is bestmovetarget null?");
        if (BestMoveTarget != null)
        {
            Debug.Log("Moving to bestmovetarget");
            StartCoroutine(GameObject.Find("MainManager").GetComponent<MainManager>().MoveToTile(BestMoveTarget.GetComponent<TileType>()));
        }
        

    }
   public void DecideMove()
    {


        //Find available destinations
        FindAvailableDestinations();
        
        if (TileList.Count > 0)
        {
            BestMove = 0;
        }
        else
        {
            //if I'm king and I am threatened, this means I'm checkmated
            if (threatened)
            {
               // MM.Win();
                Debug.Log("Checkmate");
            }
            BestMove = -1;
            LeastDistanceToKing = 10000000;
        }
        //See if the human is there 
        //If I'm king I want to go to the stairs
        foreach(TileType tile in TileList)
        {
            if (tile.transform.tag == "piece")
            {
                if (tile.GetComponent<Piece>().human)
                {
                    //if I'm king and the piece I'm looking at is threatened, can't eat it
                    if (!((PieceType == "king") && (tile.GetComponent<Piece>().guarded)))
                    {
                        //if the piece is protected and worth less than me, don't do that
                        if ((tile.GetComponent<Piece>().MaxHP > MaxHP)|| (!tile.GetComponent<Piece>().guarded)){
                            BestMove = 3;
                            BestMoveTarget = tile.transform;
                            if ((tile.GetComponent<Piece>().PieceType == "king") || ((PieceType == "king") && threatened))
                            {
                                BestMove = 5;
                                return;
                            }
                        }

                    }                   
                    
                }
            }
            //special case for ennemy king, looking for an exit
            else if ((PieceType == "king") && (threatened)&&(tile.Type == 3))
            {
                BestMove = 6;
                BestMoveTarget = tile.transform;
            }

        }
        //am I threatened and uncovered?
       // threatened = CurrentTile.GetComponent<TileType>().threatened;
       // covered = CurrentTile.GetComponent<TileType>().covered;
       //was this useful?
       //todo: move if threatened (unless guarded and attacking piece is higher)
       //todo: know whether I am blocking a piece
        if ( (PieceType!="king")&&(BestMove == 0)&&(threatened)&&(protecting<MaxHP))
        {
           
            //Debug.Log("I'm threatened");
            //can I find a place where I'll be protected?
            foreach(TileType tile in TileList)
            {
                if (!tile.threatened)
                {
                    //Debug.Log("not threatened");
                    BestMove = 2;   
                    BestMoveTarget = tile.transform;
                    break;
                }
                /*else if(tile.covered)
                {
                    //Debug.Log("covered ");
                    BestMove = 1;
                    BestMoveTarget = tile.transform;
                }*/

            }
        }
        //If I'm king I absolutely want to be not threatened.
        if ((PieceType == "king") && (threatened) && (BestMove < 5))
        {
            Debug.Log("I'm king and threatened");
            foreach(TileType tile in TileList)
            {
                if (!tile.threatened)
                {
                    BestMove = 4;
                    BestMoveTarget = tile.transform;
                }
            }
            //if I haven't found anything, I'm checkmate
            if (BestMove < 4)
            {
                //MM.Win();
                Debug.Log("checkmate");
            }
        }

        //if we haven't found a piece to eat, find closest place
        if ((BestMove == 0))
        {
            //Get the human coordinates
            Piece HumanKing  = GameObject.Find("MainManager").GetComponent<MainManager>().PieceList[0];

            bool foundpiece = false;
            while (!foundpiece)
            {
                int randompiece = (int)(Random.value * MM.PieceList.Count);
                if (MM.PieceList[randompiece].human)
                {
                    foundpiece = true;
                    HumanKing = MM.PieceList[randompiece];
                }
            }

           /* foreach(Piece piece in GameObject.Find("MainManager").GetComponent<MainManager>().PieceList)
            {
                if (piece.human )
                {
                    HumanKing = piece;
                }
            }*/
            

            //Find distance of all available tiles to human
            foreach (TileType tile in TileList)
            {
                tile.GetDistanceTo(HumanKing.transform);
            }


            //Sort them
            TileList = TileList.OrderBy(tile => tile.DistanceToPiece).ToList();

            //try to find a tile that isn't threatened
            if ((TileList.Count > 0))
            {
                //BestMove = 0;
                LeastDistanceToKing = 1000000000;
                foreach (TileType tile in TileList)
                {
                    if (!tile.threatened)
                    {
                        if (PieceType == "pawn")
                        {
                            BestMove = -2;
                        }
                        else
                        {
                            BestMove = 1;

                        }
                        
                        BestMoveTarget = tile.transform;
                        LeastDistanceToKing = TileList[0].DistanceToPiece;
                        break;
                    }

                }
                
            }
            else
            {
                BestMove = 0;
                LeastDistanceToKing = 1000000000;
            }

        }
       /* if ((BestMove == 1) && (threatened))
        {
            BestMove = -1;
        }*/
        if (PieceColor == "red")
        {
            if (TileList.Count > 0)
            {
                BestMove = 5;
                BestMoveTarget = TileList[0].transform;

            }
        }
        if ((BestMoveTarget!=null)&&(BestMove==1)){
            if (BestMoveTarget.GetComponent<TileType>().threatened)
            {
                BestMove = -2;
            }
            if (BestMoveTarget.GetComponent<Piece>() != null)
            {
                if (BestMoveTarget.GetComponent<Piece>().threatened)
                {
                    BestMove = -2;
                }
            }
        }
        
        
    }

    public void SetActive(bool active)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {

            if ((child.transform.name.Contains("Cube")) || (child.transform.name.Contains("ollider"))|| (child.transform.name.Contains("ubwe")))
            {
                if (active)
                {
                    child.GetComponent<GetCollidingThings>().CollidingTileList.Clear();
                }
                child.GetComponent<Collider>().enabled = active;
                child.GetComponent<GetCollidingThings>().enabled = active;
            }

        }
    }

    public void SetVisibility()
    {
        //if I'm player piece and stumble upon corridor for the first time, make it visible
        if (CurrentTile != null)
        {
            if ((CurrentTile.GetComponent<TileType>().corridor) && (PieceColor == "white"))
            {
                CurrentTile.GetComponent<TileType>().visited = true;
                CurrentTile.GetComponent<TileType>().visible = true;
                CurrentTile.GetComponent<TileType>().SetVisibility();
            }
        }

        Renderer[] allChildren = GetComponentsInChildren<Renderer>();
        foreach (Renderer child in allChildren)
        {
            //if (CurrentTile != null)
          //  {
                if (CurrentTile.GetComponent<TileType>().visible)
                {
                    if ((!child.transform.name.Contains("Cube")) && (!child.transform.name.Contains("ollider")))
                    {
                        child.enabled = true;
                    }

                }
                else
                {
                    child.enabled = false;
                }
           // }

        }


    }

    private void OnTriggerStay(Collider collision)
    {
        //update my tile
        if (MM == null)
        {
            MM = GameObject.Find("MainManager").GetComponent<MainManager>();
        }
        if ((collision.transform.tag == "tile"))
        {
            CurrentTile = collision.transform;
            CurrentTileRoomColor = CurrentTile.GetComponent<TileType>().RoomColor;
            //CurrentTile.GetComponent<Collider>().enabled = false;
            //SetVisibility();
            
        }
        //was I eaten?
       /* if((collision.transform.tag == "piece")&&!MM.WaitingForMove)
        {
            //should not be a red piece
            if ((collision.GetComponent<Piece>().PieceColor != "red")&&(PieceColor!="red"))
            {
                
                //who ate who?
                if (MM.WaitingForCPUMove && human)
                {
                    Debug.Log("eating lingering white piece");
                    MM.EatPiece(this);
                }
                else if (MM.WaitingForPlayerMove && !human)
                {
                    Debug.Log("eating lingering black piece");
                    MM.EatPiece(this);
                }
            }

        }*/
    }

    public void PowerUp()
    {
        //destroy current model
        foreach (Transform child in transform)
        {
            if (child.name.Contains("lone"))
            {
                Destroy(child.gameObject);
            }
        }
        HP = MaxHP;
        CreateModel("white");
        human = true;

    }

    public void GoToNextLevel()
    {
        gameObject.active = false;
        //do something more about this!
    }


}
