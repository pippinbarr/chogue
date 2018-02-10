﻿using System.Collections;
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
		
	}



    private void OnTriggerEnter(Collider other)
    {
        
        if (((other.transform.tag == "tile")|| other.transform.tag == "wall" || other.transform.tag == "piece") &&(!CollidingTileList.Contains(other.GetComponent<TileType>())))
        {
            CollidingTileList.Add(other.GetComponent<TileType>());
            //Has the pawn touched a wall?
            if(PawnWallCheck && (other.transform.tag == "wall"))
            {
                Debug.Log("queen");
                transform.parent.GetComponent<Piece>().Queen();
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
