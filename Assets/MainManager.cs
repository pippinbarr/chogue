using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    public Piece CurrentActivePiece;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentActivePiece.FindAvailableDestinations();
            CurrentActivePiece.ShowDestinations();
        }
	}
}
