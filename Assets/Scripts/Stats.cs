using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stats : MonoBehaviour
{
    public int CurrentGame = 0;
    public List<List<Move>> Moves = new List<List<Move>>();
    // Start is called before the first frame update

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        NextGame();
    }

    public void NextGame()
    {
        //save stats
        Debug.Log("Nb of moves " + Moves.Count);
        Moves.Clear();

        CurrentGame++;
        SceneManager.LoadScene("shatranj");
    }



}
