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
    bool LevelDone = false; //this is stupid but for now I do this to solve the JS CS order of compilation

    private void Start()
    {
        //manually set Playerprefs right now
        PlayerPrefs.SetString("IncomingPieces", "kqcbtppp");
        //knight is chevalier to distinguish and rook is tour
    }

    // Use this for initialization
    public void CreateLevel () {

        //get MainManage to update Piece List
        MainManager mm = GetComponent<MainManager>();
        bool PlayerCreated = false; //set true when a player piece has been created

        //Get the generated level
        Level = GetComponent<DungeonGenerator>().m_DungeonImage;
        int sizeX = GetComponent<DungeonGenerator>().m_DungeonWidth;
        int sizeY = GetComponent<DungeonGenerator>().m_DungeonHeight;

        bool blacktile = true; //used to alternate colors

        //Go through all the pixels, if it is non-white, create a tile from tile prefab
        //The type of tile is not defined yet (checkered, etc)
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

                    //Is this the first room? if so add incoming pieces
                    string incomingpieces = PlayerPrefs.GetString("IncomingPieces");

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
                        else 
                        {
                            TempPiece = Instantiate(Pawn, tempTile.position + new Vector3(0, 0, -.2f), Pawn.rotation);
                        }
                        TempPiece.GetComponent<Piece>().CreateModel("white");
                        TempPiece.GetComponent<Piece>().human = true;
                        mm.PieceList.Add(TempPiece.GetComponent<Piece>());
                        incomingpieces = incomingpieces.Substring(1,(incomingpieces.Length-1));
                        Debug.Log("incoming pieces : " + incomingpieces);
                        PlayerPrefs.SetString("IncomingPieces", incomingpieces);
                    }
                    //Here we add the ennemies
                    if (Random.value < 0.03)
                    {
                        Transform TempPiece;
                        //select randomly between available pieces
                        float random = Random.value;
                        if (random > 0.80)
                        {
                            TempPiece = Instantiate(Rook, tempTile.position + new Vector3(0, 0, -.2f), Rook.rotation);
                        }
                        else if (random > 0.60)
                        {
                            TempPiece = Instantiate(Bishop, tempTile.position + new Vector3(0, 0, -.2f), Bishop.rotation);
                        }
                        else if(random > 0.40)
                        {
                            TempPiece = Instantiate(Queen, tempTile.position + new Vector3(0, 0, -.2f), Queen.rotation);
                        }
                        else if (random > 0.20)
                        {
                            TempPiece = Instantiate(Knight, tempTile.position + new Vector3(0, 0, -.2f), Knight.rotation);
                        }
                        else
                        {
                            TempPiece = Instantiate(Pawn, tempTile.position + new Vector3(0, 0, -.2f), Pawn.rotation);
                        }

                        TempPiece.GetComponent<Piece>().CreateModel("black");
                        TempPiece.GetComponent<Piece>().human = false;
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
		
	}
	
	// Update is called once per frame
	void Update () {
        if ((GetComponent<DungeonGenerator>().m_DungeonImage != null) && !LevelDone)
        {
            CreateLevel();
            LevelDone = true;
        }
	}
}
