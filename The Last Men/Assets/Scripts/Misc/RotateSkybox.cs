using UnityEngine;
using System.Collections;

public class RotateSkybox : MonoBehaviour {

    public float rotationSpeed = 2.0f;
    public SunLight sun;
    private Transform sunTransform;
    private Quaternion sunInitialRotation;
    public Camera skyboxCamera;
    private Transform skyboxCameraTransform;
    public Camera mainCamera;
    private Transform mainCameraTransform;

	// Use this for initialization
	void Start () {
        if(!mainCamera)
            mainCamera = Camera.main;
        mainCameraTransform = mainCamera.transform;
        skyboxCamera.depth = mainCamera.depth - 1;
        skyboxCameraTransform = skyboxCamera.transform;
        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<SunLight>();
        sunTransform = sun.transform;
        sunInitialRotation = sunTransform.rotation;

    }
	
	// Update is called once per frame
	void Update () {
        if (mainCamera.enabled) {
            skyboxCamera.enabled = true;
            skyboxCameraTransform.position = mainCameraTransform.position;
            skyboxCameraTransform.rotation = mainCameraTransform.rotation;
            skyboxCamera.fieldOfView = mainCamera.fieldOfView;
            skyboxCameraTransform.Rotate(Vector3.forward, rotationSpeed * Time.time, Space.World);
            sunTransform.rotation = sunInitialRotation;
            sunTransform.Rotate(Vector3.forward, -rotationSpeed * Time.time, Space.World);
        }
        else {
            skyboxCamera.enabled = false;
        }
        //sun.light.intensity = 0.5f to 2.0f;?
	}
}
