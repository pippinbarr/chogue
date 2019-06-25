using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class LeaderBoard : MonoBehaviour
{
    //high score stuff
    private LeaderboardNames[] allLeaderboards;
    // Start is called before the first frame update
    void Start()
    {
        //login to google game center
        if (!GameServices.Instance.IsLoggedIn())
        {
            GameServices.Instance.LogIn(LoginComplete);
        }
        //make a list of all leaderboards
        int nrOfLeaderboards = System.Enum.GetValues(typeof(LeaderboardNames)).Length;
        allLeaderboards = new LeaderboardNames[nrOfLeaderboards];
        for (int i = 0; i < nrOfLeaderboards; i++)
        {
            allLeaderboards[i] = ((LeaderboardNames)i);
        }

        //if this script exists, this means this is the end of the game, we can submit the choguelo
        SubmitChoguelo();

    }

    public void ShowLeaderBoard()
    {
        GameServices.Instance.ShowLeaderboadsUI();
    }

    public void ShowAchievements()
    {
        GameServices.Instance.ShowAchievementsUI();
    }

    public void SubmitChoguelo()
    {
        //PlayerPrefs.SetInt("Choguelo", 1450);
        //Debug.Log("choguelo:" + PlayerPrefs.GetInt("Choguelo"));
        long choguelo = PlayerPrefs.GetInt("Choguelo");

        //Debug.Log("choguelo:" + choguelo);
        //GleyGameServices.ScreenWriter.Write("Submitting score: " + choguelo);
        GameServices.Instance.SubmitScore(choguelo, allLeaderboards[0], ScoreSubmitted);

        Social.LoadAchievements(achievements => {
            if ((achievements!=null)&&(achievements.Length > 0))
            {
                foreach (IAchievement achievement in achievements)
                {
                    if (achievement.id == "com.pippinbarr.chogue.achievement.grandmaster" || achievement.id == "CgkIoNP6vo8GEAIQAw")
                    {
                        if (achievement.completed)
                        {
                            //Debug.Log("LeaderBoard.SubmitChoguelo setting grandmaster to 1");
                            PlayerPrefs.SetInt("grandmaster", 1);
                        }
                    }

                    else
                    {
                        Debug.Log("No achievements returned");
                    }
                }
            }
            if (PlayerPrefs.GetInt("grandmaster", 0) == 1)
            {
                // Grandmaster
                GameServices.Instance.SubmitScore(choguelo, allLeaderboards[1], ScoreSubmitted);
            }
        });


    }
    private void LoginComplete(bool success)
    {
        if (success == true)
        {
            //Login was succesful

        }
        else
        {
            //Login failed
        }
       // Debug.Log("Login success: " + success);
        //GleyGameServices.ScreenWriter.Write("Login success: " + success);
    }
    //Automatically called when a score was submitted 
    private void ScoreSubmitted(bool success, GameServicesError error)
    {
        if (success)
        {
            //score successfully submitted
        }
        else
        {
            //an error occurred
            //Debug.LogError("Score failed to submit: " + error);
        }
        //Debug.Log("Submit score result: " + success + " message:" + error);
        //GleyGameServices.ScreenWriter.Write("Submit score result: " + success + " message:" + error);
    }

}
