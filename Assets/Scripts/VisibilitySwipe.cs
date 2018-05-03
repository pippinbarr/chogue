using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilitySwipe : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "tile")
        {
            //Debug.Log("swipe tile");
            other.GetComponent<TileType>().visited = true;
            other.GetComponent<TileType>().visible = true;
            other.GetComponent<TileType>().SetVisibility();
        }
        if(other.tag == "wall")
        {
            other.GetComponent<Renderer>().enabled = true;
        }

    }
}
