using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType : MonoBehaviour {

    public Color WhiteGround;
    public Color BlackGround;
    public Color Background;
   

    //Function to set the tile type
    public void SetTileType(int type)
    {
        //type 0 : WhiteGround
        if(type == 0)
        {
            GetComponent<Renderer>().material.color = WhiteGround;
        }
        //type 1 : BlackGround
        if (type == 1)
        {
            GetComponent<Renderer>().material.color = BlackGround;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
