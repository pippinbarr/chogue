using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCameraToDevice : MonoBehaviour
{
    private Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();

        float unitsPerPixel = 25f / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        m_Camera.orthographicSize = desiredHalfHeight;
    }

    // Update is called once per frame
    void Update()
    {
        float unitsPerPixel = 23f / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        m_Camera.orthographicSize = desiredHalfHeight;

    }
}
