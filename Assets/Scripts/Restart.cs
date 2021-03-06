﻿using System.Collections;
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
        if (SceneManager.GetActiveScene().name == "GameOver")
        {
            if (PlayerPrefs.GetString("Executor") == "resigned")
            {
                Epitaph.text = "White King\nResigned";
            }
            else
            {
                Epitaph.text = "White King\nCaptured by a\n" + PlayerPrefs.GetString("Executor").ToUpper() ;
            }

            if (PlayerPrefs.GetInt("continued") == 1)
            {
                Epitaph.text = Epitaph.text + "\n\nCaptured the king of Yendor\nand ";
            }
            else
            {
                Epitaph.text = Epitaph.text + "\n\n";
            }

            if (PlayerPrefs.GetInt("level") == 0)
            {
                Epitaph.text = Epitaph.text + "on the chess board";
            }
            else
            {
                Epitaph.text = Epitaph.text + "reached level " + PlayerPrefs.GetInt("level");
            }


            if (PlayerPrefs.GetInt("continued") == 0)
            {
                if (PlayerPrefs.GetInt("level") >= PlayerPrefs.GetInt("maxlevel"))
                {
                    Epitaph.text = Epitaph.text + " (new record!)";
                    PlayerPrefs.SetInt("maxlevel", PlayerPrefs.GetInt("level"));
                }
            }
            else
            {
                int kinglevel = PlayerPrefs.GetInt("kinglevel");
                if (kinglevel + (kinglevel - (PlayerPrefs.GetInt("level"))) >= PlayerPrefs.GetInt("maxlevel"))
                {
                    Epitaph.text = Epitaph.text + " (new record!)";
                    PlayerPrefs.SetInt("maxlevel", PlayerPrefs.GetInt("level"));
                }
            }


            Epitaph.text = Epitaph.text + "\nwith " + PlayerPrefs.GetInt("gold") + " gold";
            if (PlayerPrefs.GetInt("gold") > PlayerPrefs.GetInt("maxgold"))
            {
                Epitaph.text = Epitaph.text + " (new record!)";
                PlayerPrefs.SetInt("maxgold", PlayerPrefs.GetInt("gold"));
            }
            Epitaph.text = Epitaph.text + "\n\n"+ System.DateTime.Today.ToShortDateString();
            //CurLevel.text = "Level reached: " + PlayerPrefs.GetInt("level");
            //BestLevel.text = "Highest level reached: " + PlayerPrefs.GetInt("maxlevel");

            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
            PlayerPrefs.SetInt("continued", 0);
        }
        else
        {
            Epitaph.text = "Black King\n\nCaptured by a\n" + PlayerPrefs.GetString("Executor").ToUpper() + "\n\n" + System.DateTime.Today.ToShortDateString();
        }
        

    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space)) &&(SceneManager.GetActiveScene().name == "GameOver"))
        {
            RestartGame();
        }
    }
    public void ContinueGame()
    {
        SceneManager.LoadScene("LevelGen");
        PlayerPrefs.SetInt("continued", 1);
    }
}
