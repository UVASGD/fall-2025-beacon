using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconGrabber : MonoBehaviour
{
    public static BeaconGrabber Singleton;
    public Camera playerCamera;
    public LayerMask beaconLayer;

    [SerializeField]
    private GameObject grabbedBeacon;
    public GameObject GrabbedBeacon => grabbedBeacon;
    void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GrabBeacon();
        }
        else if (Input.GetMouseButton(0))
        {
            MoveBeacon();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseBeacon();
        }
    }

    void GrabBeacon()
    {
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; 
        if(Physics.Raycast(mouseRay, out hit, 10000f, beaconLayer)) {
            grabbedBeacon = hit.transform.gameObject;
        }
    }

    void MoveBeacon()
    {
        if (grabbedBeacon == null)
            return;
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distanceToPlane;
        if (groundPlane.Raycast(mouseRay, out distanceToPlane))
        {
            Vector3 point = mouseRay.GetPoint(distanceToPlane);
            grabbedBeacon.transform.position = point;
            Physics.SyncTransforms();
        }
    }

    void ReleaseBeacon()
    {
        grabbedBeacon = null;
    }
}
