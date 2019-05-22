using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour {

    Texture2D Level;
    public Texture2D ManualLevel;
    public bool manual = false;
    public int KingArrivesAtLevel = 1;
    public int KingFarFromStairsAtLevel =5;
    public Transform TilePrefab;
    public Transform WallPrefab;
    public Transform StairsPrefab;
    public Transform Rook;
    public Transform Bishop;
    public Transform Queen;
    public Transform Knight;
    public Transform King;
    public Transform Pawn;
    public Transform Gold;
    bool LevelDone = false; //this is stupid but for now I do this to solve the JS CS order of compilation

    private void Start()
    {

        
    }

    // Use this for initialization
    public void CreateLevel () {

        //get MainManage to update Piece List
        MainManager mm = GetComponent<MainManager>();
        bool PlayerCreated = false; //set true when a player piece has been created
        bool AIKingCreated = false;
        int sizeX;
        int sizeY;
        //Get the generated level
        if (manual)
        {
            Level = ManualLevel;
            sizeX = 40;// Level.width;
            sizeY = 25; // Level.height;
        }
        else
        {
            Level = GetComponent<DungeonGenerator>().m_DungeonImage;
            sizeX = GetComponent<DungeonGenerator>().m_DungeonWidth;
            sizeY = GetComponent<DungeonGenerator>().m_DungeonHeight;
        }

       // Debug.Log("size X " + sizeX);

        bool blacktile = true; //used to alternate colors

        //Go through all the pixels, if it is non-white, create a tile from tile prefab
        //The type of tile is not defined yet (checkered, etc)

        //get the incoming player pieces
        string incomingpieces = PlayerPrefs.GetString("IncomingPieces");

        for (int posy = 0;  posy<sizeY; posy++)
        {
            for(int posx = 0; posx < sizeX; posx++)
            {
                if(Level.GetPixel(posx, posy) != Color.white)
                {
                    Transform tempTile;
                    //is this a stairs?
                    bool stairs = false;
                    if (Level.GetPixel(posx, posy) == new Color(0f, 1f, 0f))
                    {
                        stairs = true;
                        Debug.Log("instantiating stairs");
                        tempTile = Instantiate(StairsPrefab, new Vector3(posx, posy, 0), transform.rotation);
                        tempTile.Rotate(new Vector3(180,0,0));
                        tempTile.GetComponent<TileType>().SetTileType(3);
                        //reset the pixel for right room color
                        if (Level.GetPixel(posx+1, posy) != Color.white)
                        {
                            Level.SetPixel(posx, posy, Level.GetPixel(posx + 1, posy));
                        }
                        else if (Level.GetPixel(posx - 1, posy) != Color.white)
                        {
                            Level.SetPixel(posx, posy, Level.GetPixel(posx - 1, posy)) ;
                        }
                        else if (Level.GetPixel(posx, posy+1) != Color.white)
                        {
                            Level.SetPixel(posx, posy, Level.GetPixel(posx, posy + 1));
                        }
                        else if (Level.GetPixel(posx, posy - 1) != Color.white)
                        {
                            Level.SetPixel(posx, posy, Level.GetPixel(posx, posy - 1));
                        }

                    }
                    else if (blacktile)
                    {
                        tempTile = Instantiate(TilePrefab, new Vector3(posx, posy, 0), transform.rotation);
                        tempTile.GetComponent<TileType>().SetTileType(0);
                    }
                    else
                    {
                        tempTile = Instantiate(TilePrefab, new Vector3(posx, posy, 0), transform.rotation);
                        tempTile.GetComponent<TileType>().SetTileType(1);
                    }
                    tempTile.GetComponent<TileType>().RoomColor = Level.GetPixel(posx, posy);
                    if(Level.GetPixel(posx, posy)==new Color(1f, 0, 0))
                    {
                        tempTile.GetComponent<TileType>().corridor = true;
                    }
                    mm.TileList.Add(tempTile.GetComponent<TileType>());
                    //Is this the first room? if so add incoming pieces
                    

                    //all this shit is because the image pixel colors are not EXACT! so looking for a minor difference
                    Color startroomcolor = GameObject.Find("MainManager").GetComponent<DungeonGenerator>().m_StartRoomColor;
                    Color currentcolor = Level.GetPixel(posx, posy);
                    float difcolor = Mathf.Abs((startroomcolor.r - currentcolor.r) + (startroomcolor.r - currentcolor.r) + (startroomcolor.r - currentcolor.r));
                    
                   // Debug.Log("color difference : " + difcolor);
                    if ((incomingpieces!="")&&(difcolor<0.01))
                    {
                        //Debug.Log("incoming pieces : " + incomingpieces);
                        Transform TempPiece;
                        if (incomingpieces[0].ToString() == "k")
                        {
                            TempPiece = Instantiate(King, tempTile.position + new Vector3(0, 0, -.2f), King.rotation);
                            PlayerCreated = true;
                            mm.CurrentActivePiece = TempPiece.GetComponent<Piece>();
                        }
                        else if (incomingpieces[0].ToString() == "t")
                        {
                            TempPiece = Instantiate(Rook, tempTile.position + new Vector3(0, 0, -.2f), Rook.rotation);
                        }
                        else if (incomingpieces[0].ToString() == "b")
                        {
                            TempPiece = Instantiate(Bishop, tempTile.position + new Vector3(0, 0, -.2f), Bishop.rotation);
                        }
                        else if (incomingpieces[0].ToString() == "q")
                        {
                            TempPiece = Instantiate(Queen, tempTile.position + new Vector3(0, 0, -.2f), Queen.rotation);
                        }
                        else if (incomingpieces[0].ToString() == "c")
                        {
                            TempPiece = Instantiate(Knight, tempTile.position + new Vector3(0, 0, -.2f), Knight.rotation);
                        }
                        else if (Level.GetPixel(posx, posy + 1) == Color.white)
                        {
                            TempPiece = Instantiate(Queen, tempTile.position + new Vector3(0, 0, -.2f), Queen.rotation);
                        }
                        else
                        {
                            TempPiece = Instantiate(Pawn, tempTile.position + new Vector3(0, 0, -.2f), Pawn.rotation);
                            
                        }
                        TempPiece.GetComponent<Piece>().CreateModel("white");
                        TempPiece.GetComponent<Piece>().human = true;
                        mm.PieceList.Add(TempPiece.GetComponent<Piece>());
                        incomingpieces = incomingpieces.Substring(1,(incomingpieces.Length-1));
                        //Debug.Log("incoming pieces : " + incomingpieces);
                       // PlayerPrefs.SetString("IncomingPieces", incomingpieces);
                    }

                    //Here we add the ennemies0
                    float enemythreshold = 0.025f + (((float)PlayerPrefs.GetInt("level")) / 100f);
                    if ((Random.value < enemythreshold) && (!(difcolor < 0.01)) && (Level.GetPixel(posx, posy) != Color.red) && (!stairs) && (mm.PieceList.Count<50))
                    {
                        Transform TempPiece;
                        //select randomly between available pieces
                        float random = Random.value;
                       // Debug.Log("random1 : " + random);
                        random += (float)PlayerPrefs.GetInt("level") / 50f;
                        //Debug.Log("random2 : " + random);
                        //Debug.Log("continued? " + PlayerPrefs.GetInt("continued"));
                       // Debug.Log("LEvel? " + (PlayerPrefs.GetInt("level")));
                       // Debug.Log("king should arrive at " + KingArrivesAtLevel);
                        if ((PlayerPrefs.GetInt("level")>=KingArrivesAtLevel) && (!AIKingCreated)&&(PlayerPrefs.GetInt("continued")!=1))
                        {

                            TempPiece = Instantiate(King, tempTile.position + new Vector3(0, 0, -.2f), Rook.rotation);
                            AIKingCreated = true;
                            Debug.Log("adding a king");
                        }
                        else if (random > 1f)
                        {
                            TempPiece = Instantiate(Queen, tempTile.position + new Vector3(0, 0, -.2f), Rook.rotation);
                            //Debug.Log("adding a queen");
                        }
                        else if (random > 0.95)
                        {
                            TempPiece = Instantiate(Rook, tempTile.position + new Vector3(0, 0, -.2f), Bishop.rotation);
                        }
                        else if (random > 0.70)
                        {
                            TempPiece = Instantiate(Bishop, tempTile.position + new Vector3(0, 0, -.2f), Queen.rotation);
                        }
                        else if (random > 0.55)
                        {
                            TempPiece = Instantiate(Knight, tempTile.position + new Vector3(0, 0, -.2f), Knight.rotation);
                        }
                        else if (Level.GetPixel(posx, posy - 1) != Color.white)
                        {
                            TempPiece = Instantiate(Pawn, tempTile.position + new Vector3(0, 0, -.2f), Pawn.rotation);
                            TempPiece.Rotate(new Vector3(180, 0, 0));
                        }
                        else
                        {
                            TempPiece = Instantiate(Knight, tempTile.position + new Vector3(0, 0, -.2f), Knight.rotation);
                        }

                        //chances that it is a powerup
                        int cappedlevel = PlayerPrefs.GetInt("level");
                        if (cappedlevel > 13)
                        {
                            cappedlevel = 13;
                        }
                        if ((Random.value < (0.08f + (float)cappedlevel / 50f))&&(TempPiece.GetComponent<Piece>().PieceType!="king"))
                        {

                            if (TempPiece.GetComponent<Piece>().PieceType == "pawn")
                            {
                                TempPiece.Rotate(new Vector3(-180, 0, 0));
                            }
                            TempPiece.GetComponent<Piece>().CreateModel("red");
                        }
                        else
                        {
                            TempPiece.GetComponent<Piece>().CreateModel("black");
                        }

                        TempPiece.GetComponent<Piece>().human = false;
                        mm.PieceList.Add(TempPiece.GetComponent<Piece>());


                    }
                    //we add coins
                    else if ((Random.value < 0.01f)&& (!(difcolor < 0.01))&&!stairs)
                    {
                        Transform TempPiece = Instantiate(Gold, tempTile.position + new Vector3(0, 0, -.2f), Gold.rotation);
                        TempPiece.GetComponent<Piece>().human = false;
                        TempPiece.GetComponent<Piece>().CreateModel("red");
                        mm.PieceList.Add(TempPiece.GetComponent<Piece>());
                    }


                }
                //let's assume this is a possible wall
                else
                {
                    Transform tempTile = Instantiate(WallPrefab, new Vector3(posx, posy, 0), transform.rotation);
                }
                blacktile = !blacktile;
            }
            blacktile = !blacktile;
        }

        //If we have a king, let's put him in the stairs room, but not exactly next to the stairs
        if (AIKingCreated)
        {
            //find the stairs
            TileType stairstile = null;
            foreach(TileType tile in mm.TileList)
            {
                if (tile.Type == 3)
                {
                    stairstile = tile;
                }
            }
            // find the king
            Piece king = null;
            foreach(Piece piece in mm.PieceList)
            {
                if ((piece.PieceType == "king") && (piece.PieceColor == "black"))
                {
                    king = piece;
                }
            }
            if (stairstile != null)
            {
                //find a tile that is between 2 and 3 units away and doesn't have a a piece on it
                float mindist = 1.9f;
                float maxdist = 2.6f;
                if (PlayerPrefs.GetInt("level") < KingFarFromStairsAtLevel)
                {
                    mindist = .9f;
                    maxdist = 1.1f;
                }
                foreach(TileType tile in mm.TileList)
                {
                    float dist = Vector3.Distance(tile.transform.position, stairstile.transform.position);
                    if ((dist < maxdist) && (dist > mindist)&&!tile.corridor)
                    {
                        bool notgood = false;
                        foreach (Piece piece in mm.PieceList)
                        {
                            if(Vector3.Distance(piece.transform.position,tile.transform.position)<0.5)
                            {
                                notgood = true;
                            }
                        }
                        if ((!notgood)&&(king!=null))
                        {
                            king.transform.position = tile.transform.position;
                        }
                    }
                }

            }

        }

        

	}
	
	// Update is called once per frame
	void Update () {
        /*if ((GetComponent<DungeonGenerator>().m_DungeonImage != null) && !LevelDone)
        {
            CreateLevel();
            LevelDone = true;
        }*/
	}
}
