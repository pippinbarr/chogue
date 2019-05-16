using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.
    public float speed = 1;
    private float startingOrthographicSize = 12.5f;

    private bool dragging = false;
    private Vector3 prevDragPosition;

    void Update()
    {
        if(Input.GetMouseButton(0)) 
        {
            if (!dragging)
            {
                dragging = true;
                prevDragPosition = Input.mousePosition;
            }
            Vector3 move = prevDragPosition - Input.mousePosition;
            Vector3 worldMove = Camera.main.ScreenToViewportPoint(move);
            float orthoRatio = Camera.main.orthographicSize / startingOrthographicSize;
            Camera.main.transform.Translate(worldMove.x * 10f * orthoRatio,worldMove.y * 20f * orthoRatio,0);
            prevDragPosition = Input.mousePosition;
        }
        else
        {
            dragging = false;
        }

        //if there is one touch on the device
        if(Input.touchCount == 1)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector3 touchPosition = new Vector3(Input.GetTouch(0).position.x,Input.GetTouch(0).position.y,0);
                if (!dragging) 
                {
                    dragging = true;
                    prevDragPosition = touchPosition;
                }
            Vector3 move = prevDragPosition - touchPosition;
            Vector3 worldMove = Camera.main.ScreenToViewportPoint(move);
            float orthoRatio = Camera.main.orthographicSize / startingOrthographicSize;
            Camera.main.transform.Translate(worldMove.x * 10f * orthoRatio,worldMove.y * 20f * orthoRatio,0);
            prevDragPosition = touchPosition;
            }
        }
        
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (Camera.current.orthographic)
            {
                Debug.Log(Camera.current.orthographicSize + " --> ");
                // ... change the orthographic size based on the change in distance between the touches.
                Camera.current.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                Camera.current.orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, 0.1f);

                Debug.Log(Camera.current.orthographicSize);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                GetComponent<Camera>().fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, 0.1f, 179.9f);
            }
            
        }
    }
}