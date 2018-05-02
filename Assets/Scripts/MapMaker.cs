﻿using System.Collections;
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

                    //Here we add some pieces give a threshold chance
                    if (Random.value < 0.05)
                    {
                        //create the player first
                        if (!PlayerCreated)
                        {
                            Transform TempPiece = Instantiate(King, tempTile.position + new Vector3(0, 0, -.2f), King.rotation);
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

                            //black or white?
                            if (Random.value < 0.25)
                            {
                                TempPiece.GetComponent<Piece>().CreateModel("white");
                                TempPiece.GetComponent<Piece>().human = true;
                            }
                            else
                            {
                                TempPiece.GetComponent<Piece>().CreateModel("black");
                                TempPiece.GetComponent<Piece>().human = false;
                            }

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
        if ((GetComponent<DungeonGenerator>().m_DungeonImage != null) && !LevelDone)
        {
            CreateLevel();
            LevelDone = true;
        }
	}
}
