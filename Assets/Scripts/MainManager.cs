using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    //The current active piece, currently manually assigned
    public Piece CurrentActivePiece;
    public bool WaitingForPlayerMove = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //Debug code: wait for space key to check destinations
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentActivePiece.FindAvailableDestinations();
            CurrentActivePiece.ShowDestinations();
            WaitingForPlayerMove = true;
        }

        //Get click on destination (only if in "move" mode)
        if (WaitingForPlayerMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    //if this is an available destination, move!
                    if (hit.transform.GetComponent<TileType>().AvailableDestination)
                    {
                        CurrentActivePiece.transform.position = new Vector3(hit.transform.position.x,hit.transform.position.y,CurrentActivePiece.transform.position.z);
                        CurrentActivePiece.HideDestinations();

                    }
                }
            }
        }

	}
}
