using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class FireGrapplingHook : MonoBehaviour
{
    public float hookSpeed = 50.0f;
    public float hookLength = 50.0f;
    public GameObject grapplingHookPrefab;

    private bool fired = false;
    private bool hooked = false;

    public Transform cameraTransform;
    private SpringJoint springJoint;
    private GameObject grapplingHook;
    private GrapplingHook grapplingHookScript;
    private RigidbodyFirstPersonControllerSpherical controller;

    void Awake()
    {
        springJoint = GetComponent<SpringJoint>();
        controller = GetComponent<RigidbodyFirstPersonControllerSpherical>();
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Hook"))
        {
            if (fired == false)
            {
                grapplingHook = Instantiate(grapplingHookPrefab, cameraTransform.position, cameraTransform.rotation) as GameObject;
                grapplingHookScript = grapplingHook.GetComponent<GrapplingHook>();
                grapplingHookScript.fireGrapplingHook = this;
                grapplingHookScript.player = transform;
                fired = true;
            }
            else
            {
                controller.SetHooked(false);
                Destroy(grapplingHook);
                Unfire();
            }
        }
    }

    public void Unfire()
    {
        fired = false;
        hooked = false;
    }

    public void SetRope()
    {
        controller.SetHooked(true);
        float distance = Vector3.Distance(transform.position, grapplingHook.transform.position);
        springJoint.maxDistance = distance;
        springJoint.minDistance = distance;
        springJoint.spring = float.PositiveInfinity;
        springJoint.connectedBody = grapplingHook.GetComponent<Rigidbody>();
    }
}
