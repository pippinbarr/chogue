using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour {

    public Text CurLevel;
    public Text BestLevel;
    public Text Epitaph;
    // Use this for initialization

    public void Start()
    {
        Epitaph.text = "White King\n\nCaptured by a\n" + PlayerPrefs.GetString("Executor").ToUpper() + "\n\n" + System.DateTime.Today.ToShortDateString();
        
        if (PlayerPrefs.GetInt("level") > PlayerPrefs.GetInt("maxlevel"))
        {
            PlayerPrefs.SetInt("maxlevel", PlayerPrefs.GetInt("level") );
        }
        //CurLevel.text = "Level reached: " + PlayerPrefs.GetInt("level");
        //BestLevel.text = "Highest level reached: " + PlayerPrefs.GetInt("maxlevel");

        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }
    }
}
