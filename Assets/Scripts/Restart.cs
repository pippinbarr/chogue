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
        if (SceneManager.GetActiveScene().name == "GameOver")
        {
            Epitaph.text = "WHITE KING\n";

            if (PlayerPrefs.GetString("Executor") == "resigned")
            {
                Epitaph.text = Epitaph.text + "Resigned";
            }
            else
            {
                Epitaph.text = Epitaph.text + "Captured by a " + PlayerPrefs.GetString("Executor").ToUpper() ;
            }

            Epitaph.text = Epitaph.text + "\n\n" + System.DateTime.Today.ToShortDateString();

            Epitaph.text = Epitaph.text + "\n\n";


            if (PlayerPrefs.GetInt("continued") == 1)
            {
                Epitaph.text = Epitaph.text + "Captured the king of Yendor and reached level " + PlayerPrefs.GetInt("level");
            }
            else
            {           
                Epitaph.text = Epitaph.text + "Reached level " + PlayerPrefs.GetInt("level");
            }



            if (PlayerPrefs.GetInt("continued") == 0)
            {
                if (PlayerPrefs.GetInt("level") >= PlayerPrefs.GetInt("maxlevel"))
                {
                    //Epitaph.text = Epitaph.text + " (new record!)";
                    PlayerPrefs.SetInt("maxlevel", PlayerPrefs.GetInt("level"));
                }
            }
            else
            {
                int kinglevel = PlayerPrefs.GetInt("kinglevel");
                if (kinglevel + (kinglevel - (PlayerPrefs.GetInt("level"))) >= PlayerPrefs.GetInt("maxlevel"))
                {
                    //Epitaph.text = Epitaph.text + " (new record!)";
                    PlayerPrefs.SetInt("maxlevel", PlayerPrefs.GetInt("level"));
                }
            }


            //Epitaph.text = Epitaph.text + "\nwith " + PlayerPrefs.GetInt("gold") + " gold";
            if (PlayerPrefs.GetInt("gold") > PlayerPrefs.GetInt("maxgold"))
            {
                //Epitaph.text = Epitaph.text + " (new record!)";
                PlayerPrefs.SetInt("maxgold", PlayerPrefs.GetInt("gold"));
            }



            Epitaph.text = Epitaph.text + " with " + PlayerPrefs.GetInt("gold") + " gold, ";
            Epitaph.text = Epitaph.text + PlayerPrefs.GetInt("taken") + " captures, and ";
            Epitaph.text = Epitaph.text + "a rating of " + PlayerPrefs.GetInt("Choguelo", 0);
            if (PlayerPrefs.GetInt("Choguelo", 0) > PlayerPrefs.GetInt("MaxChoguelo", 0))
            {
                PlayerPrefs.SetInt("MaxChoguelo", PlayerPrefs.GetInt("Choguelo", 0));
                Epitaph.text = Epitaph.text + " (new record!)";
            }


            //CurLevel.text = "Level reached: " + PlayerPrefs.GetInt("level");
            //BestLevel.text = "Highest level reached: " + PlayerPrefs.GetInt("maxlevel");




            /* PlayerPrefs.SetInt("maxlevel", 1);
             PlayerPrefs.SetInt("hptaken", 0);
             PlayerPrefs.SetInt("gold", 0);
             PlayerPrefs.SetInt("level", 1);
             PlayerPrefs.SetString("IncomingPieces", "tcbkqbctpppppppp");
             PlayerPrefs.SetInt("continued", 0);
             PlayerPrefs.SetInt("Choguelo", 0);*/
            PlayerPrefs.SetInt("level", 1);
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
