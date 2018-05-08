using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCollidingThings : MonoBehaviour {

    public bool PawnWallCheck = false;
    public List<TileType> CollidingTileList = new List<TileType>();




    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        /*foreach(TileType tile in CollidingTileList)
        {
            if (tile == null)
            {
                CollidingTileList.Remove(tile);
            }
        }*/
		
	}



    private void OnTriggerStay(Collider other)
    {
        
        if (((other.transform.tag == "tile")|| other.transform.tag == "wall" || other.transform.tag == "piece") &&(!CollidingTileList.Contains(other.GetComponent<TileType>())))
        {
            CollidingTileList.Add(other.GetComponent<TileType>());
            //Has the pawn touched a wall?
            if(PawnWallCheck && (other.transform.tag == "wall") &&(!transform.parent.GetComponent<Piece>().NewQueen))
            {
                Debug.Log("prepare for new queen");
                transform.parent.GetComponent<Piece>().NewQueen = true;
            }

        }

    }
    private void OnTriggerExit(Collider other)
    {

        if (CollidingTileList.Contains(other.GetComponent<TileType>()))
        {
            CollidingTileList.Remove(other.GetComponent<TileType>());
        }

    }
}
