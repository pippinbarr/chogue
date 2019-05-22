using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMsg : MonoBehaviour
{
    public void ClosePopUp()
    {
        gameObject.SetActive(false);
        GameObject.FindObjectOfType<MainManager>().Win();
    }
}
