using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType : MonoBehaviour {

    public Color WhiteGround;
    public Color BlackGround;
    public Color Background;
    public Color HighlightColor;
    public int Type = 0;
    public float DistanceToPiece = 1000000;
    public bool AvailableDestination = false;
    public bool manual = false;

    //Function to set the tile type
    public void SetTileType(int type)
    {
        Type = type;
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
        // type 2 : Wall
    }

    public void Highlight(bool OnOrOff)
    {
        if (OnOrOff)
        {
            
            GetComponent<Renderer>().material.color = (GetComponent<Renderer>().material.color + HighlightColor) / 2;
            AvailableDestination = true;
        }
        else
        {
            SetTileType(Type);
            AvailableDestination = false;
        }

    }

    public void GetDistanceTo(Transform piece)
    {
        DistanceToPiece = Vector3.Distance(transform.position, piece.position);
        //Debug.Log("Distance to piece is " + DistanceToPiece);
    }

	// Use this for initialization
	void Start () {
        if (manual)
        {
            SetTileType(Type);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
