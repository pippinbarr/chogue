using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    //The current active piece, currently manually assigned
    public Piece CurrentActivePiece;
    public bool WaitingForPlayerMove = false;
    public List<Piece> PieceList = new List<Piece>();
    int CurrentPieceIndex = 0;
    bool gameover = false;

	// Use this for initialization
	void Start () {
        StartCoroutine(PlayGame());
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
                        WaitingForPlayerMove = false;

                        //was this a piece? then eat it!
                        if (hit.transform.tag == "piece")
                        {
                            PieceList.Remove(hit.transform.GetComponent<Piece>());
                            Destroy(hit.transform.gameObject);
                            GetComponent<AudioSource>().Play();
                        }

                    }
                }
            }
        }

	}

    IEnumerator PlayGame()
    {
        while (!gameover) {
            Debug.Log("Playing piece " + CurrentPieceIndex);
            yield return new WaitForSeconds(0.5f);
            CurrentActivePiece = PieceList[CurrentPieceIndex];
            if (CurrentActivePiece.human)
            {
                CurrentActivePiece.FindAvailableDestinations();
                CurrentActivePiece.ShowDestinations();
                WaitingForPlayerMove = true;
                while (WaitingForPlayerMove)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            
            }
            else
            {
                CurrentActivePiece.DecideMove();
            }
            //next piece
            CurrentPieceIndex++;
            if (CurrentPieceIndex > (PieceList.Count-1))
            {
                CurrentPieceIndex = 0;
            }
        }

    }
}
