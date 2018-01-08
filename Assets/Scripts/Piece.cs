using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Piece : MonoBehaviour {

    public bool human = true; //is this a human controlled piece or not?
    
    //Array of colliders
    public Collider[] Colliders;

    public List<TileType> TileList = new List<TileType>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FindAvailableDestinations()
    {
        TileList.Clear();
        //Crawl through each collider
        foreach (Collider col in Colliders)
        {
            List<TileType> TempTileList = col.transform.GetComponent<GetCollidingThings>().CollidingTileList;
            //for each tile, find out the distance to this piece
            foreach(TileType tile in TempTileList)
            {
                tile.GetDistanceTo(transform);
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
                else if(tile.transform.tag =="wall")
                {
                    break;
                }
            }

 

        }
    }

    public void ShowDestinations()
    {
        



        foreach (TileType tile in TileList)
        {
            tile.Highlight(true);
        }
    }


}
