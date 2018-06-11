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
    public Transform CurrentPiece;

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
        if (visible==(transform.tag!="wall"))
        {
            //make surrounding walls visible as well
            RaycastHit[] surrounding = Physics.BoxCastAll(transform.position, new Vector3(1.5f, 1.5f, 1.5f), new Vector3(0.0f, 0.0f, 0.1f));
            foreach (RaycastHit hit in surrounding)
            {
                
                if (hit.transform.tag=="wall")
                {
                    hit.transform.GetComponent<Renderer>().enabled = true;
                }
            }
        }

        SetTileType(Type);
    }
    private void OnTriggerStay(Collider collision)
    {
        if(MM==null)
        {
            MM = GameObject.Find("MainManager").GetComponent<MainManager>();
        }
        if (!MM.WaitingForMove)
        {
            //update my tile
            if ((collision.transform.tag == "piece") && (CurrentPiece == null))
            {
                CurrentPiece = collision.transform;

            }
        }


    }
    private void OnTriggerExit(Collider collision)
    {
        //update my tile
        if ((collision.transform.tag == "piece"))
        {
            CurrentPiece = null;

        }

    }

}
