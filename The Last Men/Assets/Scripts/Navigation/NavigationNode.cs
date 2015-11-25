using UnityEngine;
using System.Collections;

public class NavigationNode {

    public GameObject gizmo;

    public enum nodeTypes
    {
        None,
        Free,
        Obst,
        Restricted,
        Cover
    };

    private nodeTypes type;

    private static Color[] colors = { new Color(0.0f, 0.0f, 0.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f) };

    public Vector3 position;

    public NavigationNode()
    {
   
    }

    public nodeTypes getNodeType()
    {
        return type;
    }

    public void setType(nodeTypes type)
    {
        this.type = type;
        if(gizmo != null)
        {
            gizmo.GetComponent<Renderer>().material.color = colors[(int)this.type];
        }      
    }

    public void toggleRendering()
    {
        gizmo.SetActive(!gizmo.activeSelf);
    }
}
