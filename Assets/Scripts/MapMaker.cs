using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour {

    Texture2D Level;
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

        //Get the generated level
        Level = GetComponent<DungeonGenerator>().m_DungeonImage;
        int sizeX = GetComponent<DungeonGenerator>().m_DungeonWidth;
        int sizeY = GetComponent<DungeonGenerator>().m_DungeonHeight;

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
                    if (Level.GetPixel(posx, posy) == new Color(0f, 1f, 0f))
                    {
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
                        Debug.Log("incoming pieces : " + incomingpieces);
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
                        Debug.Log("incoming pieces : " + incomingpieces);
                       // PlayerPrefs.SetString("IncomingPieces", incomingpieces);
                    }
                    //Here we add the ennemies0
                    float enemythreshold = 0.04f + (((float)PlayerPrefs.GetInt("level")) / 100f);
                    if ((Random.value < enemythreshold) && (!(difcolor < 0.01)) && (Level.GetPixel(posx, posy) != Color.red))
                    {
                        Transform TempPiece;
                        //select randomly between available pieces
                        float random = Random.value;
                        Debug.Log("random1 : " + random);
                        random += (float)PlayerPrefs.GetInt("level") / 50f;
                        Debug.Log("random2 : " + random);
                        if ((random > 1.14) && (!AIKingCreated))
                        {
                            TempPiece = Instantiate(King, tempTile.position + new Vector3(0, 0, -.2f), Rook.rotation);
                            AIKingCreated = true;
                            Debug.Log("adding a king");
                        }
                        else if (random > 1f)
                        {
                            TempPiece = Instantiate(Queen, tempTile.position + new Vector3(0, 0, -.2f), Rook.rotation);
                            Debug.Log("adding a queen");
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
                        if (Random.value < (0.08f + (float)PlayerPrefs.GetInt("level") / 50f))
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
                    else if (Random.value < 0.01f)
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

        //update visibility
        

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
