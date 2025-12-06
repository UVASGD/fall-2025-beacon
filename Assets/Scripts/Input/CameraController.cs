using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float defaultMoveSpeed = 10f;  // Speed of camera movement
    public float fastMoveSpeed = 50f;     //cam speed if holding down shift
    public float zoomSpeed = 1000f;        // Speed of zooming in/out
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
        HandleClickdown();

        //also check for space bar
    }
    private bool mouseHeldDown = false;
    private Vector3 mouseDownPosition;
    private void HandleClickdown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //allow for dragging if the mouse button was also pressed down last frame
            if (mouseHeldDown)
            {
                //draw the rectangle at four points
            }
            else
            {
                mouseHeldDown = true;
                mouseDownPosition = Input.mousePosition;
            }
        }
        else
        {
            mouseHeldDown = false;
        }
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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += direction * fastMoveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += direction * defaultMoveSpeed * Time.deltaTime;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newHeight = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed * Time.unscaledDeltaTime, minZoom, maxZoom);
            cam.orthographicSize = newHeight;
        }
    }
}
