using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class FireGrapplingHook : MonoBehaviour
{
    public float hookSpeed = 50.0f;
    public float minRopeLength = 2.0f;
    public float maxRopeLength = 50.0f;
    public GameObject grapplingHookPrefab;

    private bool fired = false;
    private bool hooked = false;

    public Transform cameraTransform;
    //private SpringJoint springJoint;
    private ConfigurableJoint confJoint;
    private GameObject grapplingHook;
    private GrapplingHook grapplingHookScript;
    private RigidbodyFirstPersonControllerSpherical controller;
    private Rigidbody rb;

    void Awake()
    {
        //springJoint = GetComponent<SpringJoint>();
        confJoint = GetComponent<ConfigurableJoint>();
        controller = GetComponent<RigidbodyFirstPersonControllerSpherical>();
        rb = GetComponent<Rigidbody>();
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
                Destroy(grapplingHook);
                RemoveRope();
            }
        }
        if (hooked && Input.GetAxis("RopeLength") != 0) {
            //Debug.Log("rope length axis != 0");
            float newLength = Mathf.Clamp(GetCurrentRopeLength() + Input.GetAxis("RopeLength"), minRopeLength, maxRopeLength);
            UpdateRopeLength(newLength);
        }
    }

    public void Unfire()
    {
        fired = false;
        hooked = false;
        controller.SetHooked(false);
    }

    public void SetRope () {
        hooked = true;
        controller.SetHooked(true);
        float distance = Mathf.Max(minRopeLength, Vector3.Distance(transform.position, grapplingHook.transform.position));
        //springJoint.maxDistance = distance;
        //springJoint.minDistance = distance;
        //springJoint.spring = float.PositiveInfinity;
        //springJoint.connectedBody = grapplingHook.GetComponent<Rigidbody>();
        //confJoint.linearLimit.limit = distance;
        
        confJoint.connectedBody = grapplingHook.GetComponent<Rigidbody>();
        confJoint.xMotion = confJoint.yMotion = confJoint.zMotion = ConfigurableJointMotion.Limited;
        UpdateRopeLength(distance);
    }

    public void RemoveRope () {
        confJoint.connectedBody = null;
        confJoint.xMotion = confJoint.yMotion = confJoint.zMotion = ConfigurableJointMotion.Free;
        UpdateRopeLength(0.0f, false);
        if (hooked) { //give player a little extra upwards velocity upon unhooking
            controller.Jump(true);
        }
        Unfire();
    }

    private void UpdateRopeLength (float distance, bool checkConstraint = true) {
        SoftJointLimit linLim = confJoint.linearLimit;
        float oldDistance = linLim.limit;
        linLim.limit = distance;
        confJoint.linearLimit = linLim;

        //as the configurable joint does not enforce linear distance limit when limit is changed to less than current actual distance*, so this is done here explicitely
        //(* unless by adding a spring force, which makes it rather bouncy / less controllable, like the regular spring joint)
        if (checkConstraint && oldDistance > distance){
            rb.MovePosition(rb.position + Vector3.ClampMagnitude(confJoint.connectedBody.transform.position - transform.position, oldDistance - distance));
        }   
        
        //SoftJointLimit linLim = new SoftJointLimit();
        //linLim.contactDistance = 0.1f;
        //linLim.bounciness = 0.1f;
        //linLim.limit = distance;
        //return linLim;
    }

    public float GetCurrentRopeLength () {
        return confJoint.linearLimit.limit;
    }
}
