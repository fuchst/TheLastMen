using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class s_FireHook : MonoBehaviour {

    public GameObject m_hook;
    public Camera cam;
    private bool fired;
    private bool set;
    SpringJoint rope;
    public GameObject a_hook;
    private LineRenderer lr;
    private RigidbodyFirstPersonControllerSpherical controller;
    
    // Use this for initialization
	void Start () {
        fired = false;
        rope = GetComponent<SpringJoint>();
        lr = GetComponent<LineRenderer>();
        controller = GetComponent<RigidbodyFirstPersonControllerSpherical>();
        //rope.spring = 0;
        set = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (CrossPlatformInputManager.GetButtonDown("Hook")&&!fired)
        {
            a_hook = Instantiate(m_hook, transform.position, transform.rotation) as GameObject;
            a_hook.GetComponent<s_Hook>().direction = cam.transform.forward;
            a_hook.GetComponent<s_Hook>().parentRB = GetComponent<Rigidbody>();
            rope.connectedBody = a_hook.GetComponent<Rigidbody>();
            //a_hook.GetComponent<s_Hook>().spring = rope;
            //spring.spring = 0;
            
            //rope.anchor = transform.position;
            //rope.connectedAnchor = a_hook.transform.position;
            Debug.Log("pressed_Hook");
            fired = true;
            lr.enabled = true;
        }
        else if(CrossPlatformInputManager.GetButtonDown("Hook")&&fired){
            fired = false;
            rope.spring = 0;
            set = false;
            lr.enabled = false;
            Destroy(a_hook);
            Debug.Log("Defired Hook");
            controller.m_Hooked = false;
        }
        else if(fired)
        {
            bool flyingtmp = a_hook.GetComponent<s_Hook>().flying;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, a_hook.transform.position);
            Vector3 v1 = transform.position;
            Vector3 v2 = a_hook.transform.position;
            float distance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z), 2));
            //Debug.Log(distance);
            if (!flyingtmp && !set)
            {
                rope.spring = 100000000;
                //  rope.damper = 3.402823e+15F;          
                rope.maxDistance = distance;
                rope.minDistance = distance;
                set = true;
                lr.enabled = true;
                
                controller.m_Hooked = true;
                Debug.Log("set");
            }
            if(distance > 20 && flyingtmp)
            {
                rope.spring = 0;
                lr.enabled = false;
                set = false;
                fired = false;
                flyingtmp = false;
                controller.m_Hooked = true;
                Destroy(a_hook);
                Debug.Log("Hook out of range");
            }
            //Debug.Log("flying: " + flyingtmp);
            
        }
            
	}
}
