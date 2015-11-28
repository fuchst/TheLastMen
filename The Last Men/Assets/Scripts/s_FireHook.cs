﻿using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;
using System.Collections.Generic;

public class s_FireHook : MonoBehaviour {

    public bool SubdivideRope = false;
    
    public float hookLength = 50.0f;
    public GameObject m_hook;
    public GameObject m_ropeElement;
    public Camera cam;
    private bool fired = false;
    private bool set = false;
    SpringJoint rope;
    public GameObject a_hook;
    private RigidbodyFirstPersonControllerSpherical controller;
    public List<GameObject> ropeElements = new List<GameObject>();
    private LineRenderer lr;
    public int subDivAmount;

    public float ropeThickness = 0.2f;
    private GameObject ropeObject;

    private bool subdivided = true;

    public void Awake() {
        rope = GetComponent<SpringJoint>();
        controller = GetComponent<RigidbodyFirstPersonControllerSpherical>();
        lr = GetComponent<LineRenderer>();
        if(SubdivideRope)
        {
            lr.SetVertexCount(subDivAmount + 2);
        }
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

            lr.enabled = true;

            //Create rope
           /* ropeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(ropeObject.GetComponent<CapsuleCollider>());
            ropeObject.transform.LookAt(cam.transform.forward);
            ropeObject.transform.position = transform.position;
            ropeObject.transform.localScale = new Vector3(ropeThickness, 1.0f, ropeThickness);*/
        }
        else if (CrossPlatformInputManager.GetButtonDown("Hook") && fired) {
            controller.m_Hooked = false;
            lr.enabled = false;
            DestroyRope();
            //Debug.Log("Defired Hook");
            
        }
        else if (fired) {
            bool flyingtmp = a_hook.GetComponent<s_Hook>().flying;
            Vector3 v1 = transform.position;
            Vector3 v2 = a_hook.transform.position;
            float distance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z), 2));

            updateLine();
         /*   lr.SetPosition(0, transform.position);
            lr.SetPosition(1, a_hook.transform.position);
            lr.SetPosition(2, a_hook.transform.position);
            lr.SetPosition(3, a_hook.transform.position);
            lr.SetPosition(4, a_hook.transform.position);
            lr.SetPosition(5, a_hook.transform.position);*/
            //Debug.Log(distance);
            if (!flyingtmp && !set) {
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
            Vector3 ropeStart = transform.position + transform.forward * 0.5f;
            Vector3 ropePos = ropeStart + (0.5f * (a_hook.transform.position - ropeStart));
           /* ropeObject.transform.position = ropePos;
            ropeObject.transform.up = a_hook.transform.position - ropePos;
            float ropeLength = (a_hook.transform.position - ropePos).magnitude;
            ropeObject.transform.localScale = new Vector3(ropeObject.transform.localScale.x, ropeLength, ropeObject.transform.localScale.z);*/

            if (distance > hookLength && flyingtmp) {
                flyingtmp = false;
                controller.m_Hooked = true;
                DestroyRope();
                lr.enabled = false;
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
       // Destroy(ropeObject);
        int c = ropeElements.Count;
        for(int i = 0; i<c; i++)
        {
            Debug.Log("Removed element: " + i);
           Destroy(ropeElements[0]);
           ropeElements.RemoveAt(0);
        }
    }

    void SetRope()
    {

        if (SubdivideRope)
        {
            Vector3 v1 = transform.position;
            Vector3 v2 = a_hook.transform.position;
            float distance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z), 2));
            rope.spring = float.PositiveInfinity;
            //  rope.damper = 3.402823e+15F;          
            // rope.maxDistance = distance;
            // rope.minDistance = distance;
            //add 5 rope elements
            GameObject go;
            GameObject previousElement = this.gameObject;
            Vector3 direction = v2 - v1;
            int total = subDivAmount + 2;
            for (int i = 1; i <= subDivAmount; i++)
            {
                go = Instantiate(m_ropeElement, transform.position + direction * ((1/total) * i), transform.rotation) as GameObject;
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
                if (i == subDivAmount)
                    go.GetComponent<SpringJoint>().connectedBody = a_hook.GetComponent<Rigidbody>();
                ropeElements.Add(go);
            }
            //lr.SetPosition(0, transform.position);
            //lr.SetPosition(4, a_hook.transform.position);

            rope.maxDistance = distance / total;
            rope.minDistance = distance / total;
            /*for (int i = 0; i < ropeElements.Length; i++)
            {
                ropeElements[i].GetComponent<SpringJoint>().spring = 1000;
            }*/
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

    void updateLine()
    {
        if(lr.enabled == false)
        {
            return;
        }
        if(ropeElements.Count == 0)
        {
            lr.SetPosition(0, transform.position);
            for (int i = 1; i < subDivAmount + 2; i++)
            {
                lr.SetPosition(i, a_hook.transform.position);
            }
        }
        else{
           // Debug.Log(ropeElements.Count);
            lr.SetPosition(0, transform.position);
            lr.SetPosition(subDivAmount+1, a_hook.transform.position);
            for(int i = 0; i<ropeElements.Count; i++)
            {
                lr.SetPosition(i+1, ropeElements[i].transform.position);
            }
        }
    }
}
