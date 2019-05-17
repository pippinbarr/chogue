using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //login to google game center
        GameServices.Instance.LogIn(LoginComplete);
        GameServices.Instance.ShowLeaderboadsUI();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Debug.Log("Login success: " + success);
        GleyGameServices.ScreenWriter.Write("Login success: " + success);
    }
}
