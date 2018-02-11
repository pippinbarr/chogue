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
                    Debug.Log("hit something ");
                    //if this is an available destination, move!
                    if (hit.transform.GetComponent<TileType>().AvailableDestination)
                    {
                        MoveToTile(hit.transform.GetComponent<TileType>());

                    }
                }
            }
        }

	}

    IEnumerator PlayGame()
    {
        while (!gameover) {
            //Debug.Log("Playing piece " + CurrentPieceIndex);
            yield return new WaitForSeconds(0.5f);
            CurrentActivePiece = PieceList[CurrentPieceIndex];
            if (CurrentActivePiece.human)
            {
                CurrentActivePiece.FindAvailableDestinations();
                CurrentActivePiece.ShowDestinations();
                if (CurrentActivePiece.TileList.Count > 0)
                {
                    WaitingForPlayerMove = true;
                }
                
                while (WaitingForPlayerMove)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            
            }
            else
            {
                if (!CurrentActivePiece.NewQueen)
                {
                    CurrentActivePiece.DecideMove();
                }
                else
                {
                    CurrentActivePiece.NewQueen = false;
                }
                    
            }
            //next piece
            Debug.Log("next piece");
            CurrentPieceIndex++;
            if (CurrentPieceIndex > (PieceList.Count-1))
            {
                CurrentPieceIndex = 0;
            }
        }

    }

    public void MoveToTile(TileType tile)
    {
        CurrentActivePiece.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, CurrentActivePiece.transform.position.z);
        CurrentActivePiece.HideDestinations();
        WaitingForPlayerMove = false;

        //was this a piece? then eat it!
        if (tile.transform.tag == "piece")
        {
            //was it the player's king?
            if ((tile.transform.GetComponent<Piece>().human)&&(tile.transform.GetComponent<Piece>().PieceType=="king"))
            {
                Debug.Log("game over");
                gameover = true;
            }

            PieceList.Remove(tile.transform.GetComponent<Piece>());
            Destroy(tile.transform.gameObject);
            GetComponent<AudioSource>().Play();
        }
    }
}
