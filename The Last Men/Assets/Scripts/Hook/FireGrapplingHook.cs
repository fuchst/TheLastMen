using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class FireGrapplingHook : MonoBehaviour
{
    public float hookSpeed = 50.0f;
    public float minRopeLength = 2.0f;
    public float maxRopeLength = 50.0f;
    public float ropeLengthChangingSpeed = 2.5f;
    public bool invertLengthChanging = false;
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
    private float ropeLengthChangeOld = 0;

    public bool Fired { get { return fired; } }
    public bool Hooked { get { return hooked; } }
    public float CurrentRopeLength { get { return confJoint.linearLimit.limit; } }
    public float MaximumRopeLength { get { return maxRopeLength; } }

    void Awake()
    {
        //springJoint = GetComponent<SpringJoint>();
        confJoint = GetComponent<ConfigurableJoint>();
        controller = GetComponent<RigidbodyFirstPersonControllerSpherical>();
        rb = GetComponent<Rigidbody>();
    }

    void Start () {
        UpdateRopeLength(0.0f, false);
    }

    void Update()
    {
        if(fired && !grapplingHook) {
            RemoveRope();
        }

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
                RemoveRope();
            }
        }
        if (hooked && (Input.GetAxis("RopeLength") != 0 || ropeLengthChangeOld != 0)) {
            //Debug.Log("rope length axis != 0");
            float ropeLengthChangeNew = ropeLengthChangingSpeed * Input.GetAxis("RopeLength") * (invertLengthChanging ? -1 : 1);
            float ropeLengthChangeTotal = ropeLengthChangeOld + ropeLengthChangeNew;
            ropeLengthChangeOld = 0;
            float newLength = Mathf.Clamp(CurrentRopeLength + ropeLengthChangeTotal, minRopeLength, maxRopeLength);
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
        //give player a little extra upwards velocity upon unhooking, except when already grounded on a near-flat surface (i.e. standing regularly)
        if (hooked) {
            controller.Jump(true);
        }
        Unfire();
        Destroy(grapplingHook);
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Tool);
    }

    private void UpdateRopeLength (float newLength, bool checkConstraint = true) {
        SoftJointLimit linLim = confJoint.linearLimit;
        float oldLength = linLim.limit;
        
        //as the configurable joint does not enforce linear distance limit when limit is changed to less than current actual distance*, so this is done here explicitely
        //(* unless by adding a spring force, which makes it rather bouncy / less controllable, like the regular spring joint)
        if (checkConstraint && oldLength > newLength){
            float lengthChange = oldLength - newLength;
            if(lengthChange > 0.5f) {
                ropeLengthChangeOld -= (lengthChange - 0.5f);
                lengthChange = 0.5f;
                newLength = oldLength - 0.5f;
            }

            rb.MovePosition(rb.position + Vector3.ClampMagnitude(confJoint.connectedBody.transform.position - transform.position, lengthChange));
            //rb.MovePosition(rb.position + Vector3.ClampMagnitude(confJoint.connectedBody.transform.position - transform.position, oldLength - newLength));
            //rb.AddForce(Vector3.ClampMagnitude(confJoint.connectedBody.transform.position - transform.position, oldDistance - distance), ForceMode.Impulse);
        }

        linLim.limit = newLength;
        confJoint.linearLimit = linLim;

        //SoftJointLimit linLim = new SoftJointLimit();
        //linLim.contactDistance = 0.1f;
        //linLim.bounciness = 0.1f;
        //linLim.limit = distance;
        //return linLim;
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Tool);
    }

    public override string ToString() {
        return "Length: " + CurrentRopeLength.ToString("0") + "/" + MaximumRopeLength.ToString("0");
    }
}
