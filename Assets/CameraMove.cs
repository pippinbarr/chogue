﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour {

    MainManager MM;
    bool centered = false;
    public float ReferenceScreenWidth = 900;
    public float ReferenceScreenHeight = 1600;
    public float touchslowfactor = 0.01f;


    // Use this for initialization
    void Start () {
        MM = GameObject.Find("MainManager").GetComponent<MainManager>();

	}
	
	// Update is called once per frame
	void Update () {
        if (!centered)
        {
            foreach (Piece piece in MM.PieceList)
            {
                if ((piece.PieceType == "king") && (piece.human))
                {
                    Debug.Log("found human king");
                    transform.position = new Vector3(piece.transform.position.x, piece.transform.position.y+7, transform.position.z);
                    centered = true;
                }
            }
        }
       /* if (MM.MovingCamera)
        {
            //calculate factor difference between actual and reference resolution
            float screenratio = ReferenceScreenWidth / Screen.width;
            //Debug.Log("screen ratio " + screenratio);
            Vector3 MouseMove = (Input.mousePosition - MM.InitialMousePosition) * screenratio;
            transform.Translate(-MouseMove*touchslowfactor);
            MM.InitialMousePosition = Input.mousePosition;
           

        }*/
	}
}