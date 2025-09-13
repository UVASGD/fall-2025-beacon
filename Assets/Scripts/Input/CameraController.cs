using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;         // Speed of camera movement
    public float zoomSpeed = 500f;        // Speed of zooming in/out
    public float minZoom = 20f;           // Minimum zoom (camera height)
    public float maxZoom = 100f;          // Maximum zoom (camera height)

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            direction += Vector3.back;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            direction += Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            direction += Vector3.right;

        // Move relative to world space (XZ plane)
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newHeight = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
            cam.orthographicSize = newHeight;
        }
    }
}
