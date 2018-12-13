using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float mouseSensitivity = 1.0f;
    private Vector3 lastPosition;

    public float cameraDistanceMax = 20f;
    public float cameraDistanceMin = 3f;
    public float cameraDistance = 5f;
    public float scrollSpeed = 0.5f;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(-delta.x * mouseSensitivity, -delta.y * mouseSensitivity, 0);
            lastPosition = Input.mousePosition;
        }
        
        // camera position
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
        gameObject.GetComponent<Camera>().orthographicSize = cameraDistance;
        
    }
}
