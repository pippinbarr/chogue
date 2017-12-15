﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour {

    Texture2D Level;
    public Transform TilePrefab;
    bool LevelDone = false; //this is stupid but for now I do this to solve the JS CS order of compilation

	// Use this for initialization
	public void CreateLevel () {
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
