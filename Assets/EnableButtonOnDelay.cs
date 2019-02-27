using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableButtonOnDelay : MonoBehaviour {

    public GameObject button;
    public float delay = 1f;

    private float currentDelay = 0;

	// Use this for initialization
	void Start () {
        if (button != null)
        {
            button.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        currentDelay += Time.deltaTime;
        if (currentDelay > delay && button != null) {
            button.SetActive(true);
        }
	}
}
