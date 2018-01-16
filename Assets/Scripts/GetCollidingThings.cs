using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCollidingThings : MonoBehaviour {

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
