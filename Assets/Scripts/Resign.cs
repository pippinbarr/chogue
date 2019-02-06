using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Resign : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoResign () {
        PlayerPrefs.SetString("Executor", "resigned");
        SceneManager.LoadScene("GameOver");
        Debug.Log("RESIGN!");
    }

	public void OnMouseDown()
	{
        DoResign();
	}
}
