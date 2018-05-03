using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour {

    public Text CurLevel;
    public Text BestLevel;
    // Use this for initialization

    public void Start()
    {
        if (PlayerPrefs.GetInt("level") > PlayerPrefs.GetInt("maxlevel"))
        {
            PlayerPrefs.SetInt("maxlevel", PlayerPrefs.GetInt("level") );
        }
        CurLevel.text = "Level reached: " + PlayerPrefs.GetInt("level");
        BestLevel.text = "Highest level reached: " + PlayerPrefs.GetInt("maxlevel");

        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("LevelGen");
    }
}
