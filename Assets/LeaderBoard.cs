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
        //make a list of all leaderboards
        int nrOfLeaderboards = System.Enum.GetValues(typeof(LeaderboardNames)).Length;
        allLeaderboards = new LeaderboardNames[nrOfLeaderboards];
        for (int i = 0; i < nrOfLeaderboards; i++)
        {
            allLeaderboards[i] = ((LeaderboardNames)i);
        }

        // Check for login to the game services in question
        if (!GameServices.Instance.IsLoggedIn())
        {
            // If not already logged in, try now (and then submit after success)
            GameServices.Instance.LogIn(LoginComplete);
        }
        else
        {
            // If we're already logged in we can just submit the score
            SubmitChoguelo();
        }
    }

    public void ShowLeaderBoard()
    {
        GameServices.Instance.ShowLeaderboadsUI();
    }

    public void ShowAchievements()
    {
        GameServices.Instance.ShowAchievementsUI();
    }


    private void LoginComplete(bool success)
    {
        if (success == true)
        {
            // If the login was successful, we submit the score
            SubmitChoguelo();
        }
        else
        {
            // If the login failed, we can't submit the score
            // So we just do... well, nothing.
        }
        //GleyGameServices.ScreenWriter.Write("Login success: " + success);
    }

    public void SubmitChoguelo()
    {
        //PlayerPrefs.SetInt("Choguelo", 1450);
        //Debug.Log("choguelo:" + PlayerPrefs.GetInt("Choguelo"));
        long choguelo = PlayerPrefs.GetInt("Choguelo");

        //Debug.Log("choguelo:" + choguelo);
        //GleyGameServices.ScreenWriter.Write("Submitting score: " + choguelo);
        GameServices.Instance.SubmitScore(choguelo, allLeaderboards[0], ScoreSubmitted);
        if (PlayerPrefs.GetInt("grandmaster", 0) == 1)
        {
            // Grandmaster
            GameServices.Instance.SubmitScore(choguelo, allLeaderboards[1], ScoreSubmitted);
        }
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
