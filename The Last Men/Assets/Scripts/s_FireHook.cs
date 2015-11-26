using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class s_FireHook : MonoBehaviour {

    public float hookLength = 50.0f;
    public GameObject m_hook;
    public Camera cam;
    private bool fired = false;
    private bool set = false;
    SpringJoint rope;
    public GameObject a_hook;
    private RigidbodyFirstPersonControllerSpherical controller;

    public float ropeThickness = 0.2f;
    private GameObject ropeObject;

    public void Awake() {
        rope = GetComponent<SpringJoint>();
        controller = GetComponent<RigidbodyFirstPersonControllerSpherical>();
    }

    void Update() {
        if (CrossPlatformInputManager.GetButtonDown("Hook") && !fired) {
            a_hook = Instantiate(m_hook, transform.position, transform.rotation) as GameObject;
            a_hook.GetComponent<s_Hook>().direction = cam.transform.forward;
            a_hook.GetComponent<s_Hook>().parentRB = GetComponent<Rigidbody>();
            rope.connectedBody = a_hook.GetComponent<Rigidbody>();
            //a_hook.GetComponent<s_Hook>().spring = rope;
            //spring.spring = 0;

            //rope.anchor = transform.position;
            //rope.connectedAnchor = a_hook.transform.position;
            //Debug.Log("pressed_Hook");
            fired = true;

            //Create rope
            ropeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(ropeObject.GetComponent<CapsuleCollider>());
            ropeObject.transform.LookAt(cam.transform.forward);
            ropeObject.transform.position = transform.position;
            ropeObject.transform.localScale = new Vector3(ropeThickness, 1.0f, ropeThickness);
        }
        else if (CrossPlatformInputManager.GetButtonDown("Hook") && fired) {
            controller.m_Hooked = false;
            DestroyRope();
            //Debug.Log("Defired Hook");
            
        }
        else if (fired) {
            bool flyingtmp = a_hook.GetComponent<s_Hook>().flying;
            Vector3 v1 = transform.position;
            Vector3 v2 = a_hook.transform.position;
            float distance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z), 2));
            //Debug.Log(distance);
            if (!flyingtmp && !set) {
                rope.spring = float.PositiveInfinity;
                //  rope.damper = 3.402823e+15F;          
                rope.maxDistance = distance;
                rope.minDistance = distance;
                set = true;

                controller.m_Hooked = true;
                //Debug.Log("set");
            }
            //Update Rope
            Vector3 ropeStart = transform.position + transform.forward * 0.5f;
            Vector3 ropePos = ropeStart + (0.5f * (a_hook.transform.position - ropeStart));
            ropeObject.transform.position = ropePos;
            ropeObject.transform.up = a_hook.transform.position - ropePos;
            float ropeLength = (a_hook.transform.position - ropePos).magnitude;
            ropeObject.transform.localScale = new Vector3(ropeObject.transform.localScale.x, ropeLength, ropeObject.transform.localScale.z);

            if (distance > hookLength && flyingtmp) {
                flyingtmp = false;
                controller.m_Hooked = true;
                DestroyRope();
                //Debug.Log("Hook out of range");
            }
            //Debug.Log("flying: " + flyingtmp);
        }

    }

    void DestroyRope() {
        fired = false;
        rope.spring = 0;
        set = false;
        Destroy(a_hook);
        Destroy(ropeObject);
    }
}
