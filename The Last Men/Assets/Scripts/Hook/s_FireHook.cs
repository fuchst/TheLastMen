using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;

public class s_FireHook : MonoBehaviour
{
    public float hookThrowSpeed = 50.0f;
    public float hookLength = 50.0f;
    public bool SubdivideRope = false;
    public int subDivAmount = 10;

    public bool CenteredHook = false;

    public GameObject m_hook;
    public GameObject m_ropeElement;
    public Camera cam;

    private bool fired = false;
    private bool set = false;
    SpringJoint rope;
    private GameObject a_hook;
    private RigidbodyFirstPersonControllerSpherical controller;
    public List<GameObject> ropeElements = new List<GameObject>();
    private LineRenderer lr;

    public void Awake()
    {
        rope = GetComponent<SpringJoint>();
        controller = GetComponent<RigidbodyFirstPersonControllerSpherical>();
        lr = GetComponent<LineRenderer>();
        if (SubdivideRope)
        {
            lr.SetVertexCount(subDivAmount + 2);
        }
        else
        {
            subDivAmount = 0;
            lr.SetVertexCount(2);
        }
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Hook") && !fired)
        {
            a_hook = Instantiate(m_hook, cam.gameObject.transform.position, cam.transform.rotation) as GameObject;
            s_Hook HookScript = a_hook.GetComponent<s_Hook>();
            HookScript.Speed = hookThrowSpeed;
            if (!CenteredHook)
            {
                a_hook.transform.forward = cam.transform.forward;
                //HookScript.direction = cam.transform.forward;
            }
            else
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, cam.transform.forward,out hit);
                Vector3 dir;
                if (hit.collider == null)
                {
                    dir = cam.transform.forward;
                }
                else
                {
                   dir = hit.collider.transform.position -  transform.position;
                }
                a_hook.transform.forward = dir;
                //HookScript.direction = dir;
            }
            //HookScript.parentRB = GetComponent<Rigidbody>();
            rope.connectedBody = a_hook.GetComponent<Rigidbody>();


            fired = true;
            lr.enabled = true;
        }
        else if (CrossPlatformInputManager.GetButtonDown("Hook") && fired)
        {
            controller.m_Hooked = false;
            lr.enabled = false;
            DestroyRope();
        }
        else if (fired)
        {
            bool flyingtmp = a_hook.GetComponent<s_Hook>().flying;
            Vector3 v1 = transform.position;
            Vector3 v2 = a_hook.transform.position;
            float distance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z), 2));
            //debug rope length for subdiv
           /* Vector3 v3 =  ropeElements[0].transform.position;
            Vector3 v4 = ropeElements[1].transform.position;
            float distance2 = Mathf.Sqrt(Mathf.Pow((v3.x - v4.x), 2) + Mathf.Pow((v3.y - v4.y), 2) + Mathf.Pow((v3.z - v4.z), 2));
            Debug.Log("whole: " + distance + "inter: " + distance2);*/
            UpdateLine();
            /*   lr.SetPosition(0, transform.position);
               lr.SetPosition(1, a_hook.transform.position);
               lr.SetPosition(2, a_hook.transform.position);
               lr.SetPosition(3, a_hook.transform.position);
               lr.SetPosition(4, a_hook.transform.position);
               lr.SetPosition(5, a_hook.transform.position);*/
            //Debug.Log(distance);
            if (!flyingtmp && !set)
            {
                //rope.spring = float.PositiveInfinity;
                //  rope.damper = 3.402823e+15F;          
                // rope.maxDistance = distance;
                // rope.minDistance = distance;
                SetRope();
                set = true;
                lr.enabled = true;
                controller.m_Hooked = true;
                //Debug.Log("set");
            }
            //Update Rope
            //Vector3 ropeStart = transform.position + transform.forward * 0.5f;
            //Vector3 ropePos = ropeStart + (0.5f * (a_hook.transform.position - ropeStart));
            /* ropeObject.transform.position = ropePos;
             ropeObject.transform.up = a_hook.transform.position - ropePos;
             float ropeLength = (a_hook.transform.position - ropePos).magnitude;
             ropeObject.transform.localScale = new Vector3(ropeObject.transform.localScale.x, ropeLength, ropeObject.transform.localScale.z);*/

            if (distance > hookLength && flyingtmp)
            {
                flyingtmp = false;
                controller.m_Hooked = true;
                DestroyRope();
                lr.enabled = false;
                //Debug.Log("Hook out of range");
            }
            //Debug.Log("flying: " + flyingtmp);
        }

    }

    void DestroyRope()
    {
        fired = false;
        rope.spring = 0;
        set = false;
        Destroy(a_hook);
        int c = ropeElements.Count;
        for (int i = 0; i < c; i++)
        {
            //Debug.Log("Removed element: " + i);
            Destroy(ropeElements[0]);
            ropeElements.RemoveAt(0);
        }
    }

    void SetRope()
    {
        if (SubdivideRope)
        {
            Vector3 v1 = transform.position;
            Vector3 v2 = a_hook.transform.parent.position;
            float distance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z), 2));
            rope.spring = float.PositiveInfinity;
            //  rope.damper = 3.402823e+15F;          
            // rope.maxDistance = distance;
            // rope.minDistance = distance;
            //add 5 rope elements
            GameObject go;
            GameObject previousElement = this.gameObject;
            Vector3 direction = v2 - v1;
            Debug.Log(direction);
            int total = subDivAmount + 2;
            for (int i = 1; i <= subDivAmount; i++)
            {
               
               //Version for Springs 
                go = Instantiate(m_ropeElement, transform.position + direction * ((1.0F / total) * i), transform.rotation) as GameObject;
                go.name = "Element" + i;
                SpringJoint sj = go.GetComponent<SpringJoint>();
                sj.anchor = new Vector3(0, 0, 0);
                sj.connectedAnchor = new Vector3(0, 0, 0);
                //sj.connectedBody = previousElement.GetComponent<Rigidbody>();
                previousElement.GetComponent<SpringJoint>().connectedBody = go.GetComponent<Rigidbody>();
                sj.spring = 100;
                sj.maxDistance = distance / total;
                sj.minDistance = distance / total;
                // ropeElements[i] = go;

                previousElement = go;
                //lr.SetPosition(i, go.transform.position);
                //go.transform.parent = a_hook.transform;
                if (i == subDivAmount)
                    go.GetComponent<SpringJoint>().connectedBody = a_hook.GetComponent<Rigidbody>();
                ropeElements.Add(go);

                //Version for hinges
                /*go = Instantiate(m_ropeElement, transform.position + direction * ((1 / total) * i), transform.rotation) as GameObject;
                go.name = "Element" + i;
                HingeJoint hj = go.GetComponent<HingeJoint>();
                hj.anchor = new Vector3(0, 0, 0);
                hj.connectedAnchor = new Vector3(0, 0, 0);
                if(i==1)
                {
                    previousElement.GetComponent<SpringJoint>().connectedBody = go.GetComponent<Rigidbody>();
                    SpringJoint sj = previousElement.GetComponent<SpringJoint>();
                    sj.spring = 1000;
                    sj.maxDistance = distance / total;
                    sj.minDistance = distance / total;
                }
                else
                {
                    previousElement.GetComponent<HingeJoint>().connectedBody = go.GetComponent<Rigidbody>();
                }
              

                previousElement = go;
                //lr.SetPosition(i, go.transform.position);
                go.transform.parent = a_hook.transform;
                if (i == subDivAmount)
                    go.GetComponent<HingeJoint>().connectedBody = a_hook.GetComponent<Rigidbody>();
                ropeElements.Add(go);*/
            }
            
            //lr.SetPosition(0, transform.position);
            //lr.SetPosition(4, a_hook.transform.position);

            rope.maxDistance = distance / total;
            rope.minDistance = distance / total;
            /*for (int i = 0; i < ropeElements.Length; i++)
            {
                ropeElements[i].GetComponent<SpringJoint>().spring = 1000;
            }*/
            //Vector3 v3 = ropeElements[0].transform.position;
            //Vector3 v4 = ropeElements[1].transform.position;
            //float distance2 = Mathf.Sqrt(Mathf.Pow((v3.x - v4.x), 2) + Mathf.Pow((v3.y - v4.y), 2) + Mathf.Pow((v3.z - v4.z), 2));
           // Debug.Log("whole: " + distance + "inter: " + distance2);
        }
        else
        {
            Vector3 v1 = transform.position;
            Vector3 v2 = a_hook.transform.position;
            float distance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z), 2));
            rope.spring = float.PositiveInfinity;
            //  rope.damper = 3.402823e+15F;          
            rope.maxDistance = distance;
            rope.minDistance = distance;
        }
    }

    void UpdateLine()
    {
        if (lr.enabled == false)
        {
            return;
        }

        lr.SetPosition(0, transform.position);

        if (ropeElements.Count == 0)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, a_hook.transform.position);
        }
        else
        {
            lr.SetPosition(subDivAmount + 1, a_hook.transform.position);
            for (int i = 0; i < ropeElements.Count; i++)
            {
                lr.SetPosition(i + 1, ropeElements[i].transform.position);
            }
        }
    }
}
