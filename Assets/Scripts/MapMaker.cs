using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour {

    Texture2D Level;
    public Transform TilePrefab;
    public Transform WallPrefab;
    public Transform Rook;
    public Transform Bishop;
    public Transform Queen;
    bool LevelDone = false; //this is stupid but for now I do this to solve the JS CS order of compilation

	// Use this for initialization
	public void CreateLevel () {

        //get MainManage to update Piece List
        MainManager mm = GetComponent<MainManager>();
        bool PlayerCreated = false; //set true when a player piece has been created

        //Get the generated level
        Level = GetComponent<levelgen>().level;
        int sizeX = GetComponent<levelgen>().sizeX;
        int sizeY = GetComponent<levelgen>().sizeY;

        bool blacktile = true; //used to alternate colors

        //Go through all the pixels, if it is white, create a tile from tile prefab
        //The type of tile is not defined yet (checkered, etc)
        for (int posy = 0;  posy<sizeY; posy++)
        {
            for(int posx = 0; posx < sizeX; posx++)
            {
                if(Level.GetPixel(posx, posy).r == 1)
                {
                    Transform tempTile = Instantiate(TilePrefab, new Vector3(posx, posy, 0), transform.rotation);
                    if (blacktile)
                    {
                       // Debug.Log("white tile");
                        tempTile.GetComponent<TileType>().SetTileType(0);
                    }
                    else
                    {
                        //Debug.Log("black tile");
                        tempTile.GetComponent<TileType>().SetTileType(1);
                    }

                    //Here we add some pieces give a threshold chance
                    if (Random.value < 0.05)
                    {
                        //create the player first
                        if (!PlayerCreated)
                        {
                            Transform TempPiece = Instantiate(Rook, tempTile.position + new Vector3(0, 0, -.2f), Rook.rotation);
                            TempPiece.GetComponent<Piece>().CreateModel("white");
                            TempPiece.GetComponent<Piece>().human = true;
                            PlayerCreated = true;
                            mm.PieceList.Add(TempPiece.GetComponent<Piece>());
                            mm.CurrentActivePiece = TempPiece.GetComponent<Piece>();
                        }
                        //if we have a player, create some other piece
                        else
                        {
                            Transform TempPiece;
                            //select randomly between available pieces
                            float random = Random.value;
                            if (random > 0.66)
                            {
                                TempPiece = Instantiate(Rook, tempTile.position + new Vector3(0, 0, -.2f), Rook.rotation);
                            }
                            else if (random > 0.33)
                            {
                                TempPiece = Instantiate(Bishop, tempTile.position + new Vector3(0, 0, -.2f), Bishop.rotation);
                            }
                            else
                            {
                                TempPiece = Instantiate(Queen, tempTile.position + new Vector3(0, 0, -.2f), Queen.rotation);
                            }
                            
                            TempPiece.GetComponent<Piece>().CreateModel("black");
                            TempPiece.GetComponent<Piece>().human = false;
                            mm.PieceList.Add(TempPiece.GetComponent<Piece>());

                        }
                        
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
        if ((GetComponent<levelgen>().level != null)&&!LevelDone)
        {
            CreateLevel();
            LevelDone = true;
        }
	}
}
