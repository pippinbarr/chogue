using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType : MonoBehaviour {

    public Color WhiteGround;
    public Color BlackGround;
    public Color Background;
    public Color HighlightColor;
    public Color StairColor;
    public int Type = 0;
    public float DistanceToPiece = 1000000;
    public bool AvailableDestination = false;
    public bool manual = false;
    public bool visible = false;
    public bool visited = false;
    public bool corridor = false;
    public Color RoomColor;
    public bool threatened = false;
    public bool covered = false;

    private MainManager MM;

    // Use this for initialization
    void Start()
    {
        MM = GameObject.Find("MainManager").GetComponent<MainManager>();
        if (manual)
        {
            SetTileType(Type);
        }
    }

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
        // type 3 : Stairs
        if (type == 3)
        {
            GetComponent<Renderer>().material.color = StairColor;
            GetComponent<Renderer>().enabled = true;
        }
        if (!visited)
        {
            GetComponent<Renderer>().material.color = Color.black;
            if (type == 3)
            {
                GetComponent<Renderer>().enabled = false;
            }

        }

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

    public void SetVisibility()
    {
        //Debug.Log("setting visibility");
        
        MM = GameObject.Find("MainManager").GetComponent<MainManager>();
        if (!corridor)
        {
            //visible = false;
            foreach (Piece piece in MM.PieceList)
            {
                if ((RoomColor == piece.CurrentTileRoomColor) && (piece.PieceColor == "white"))
                {
                    //Debug.Log("Tile is visible");
                    visible = true;
                    //break;
                    visited = true;
                }
            }
        }

        SetTileType(Type);
    }


}
