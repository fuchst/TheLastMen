using UnityEngine;
using System.Collections;
using System;

public class NavigationNode {
    
    public enum nodeTypes
    {
        None,
        Free,
        Obst,
        Restricted,
        Cover
    };

    public static Color[] nodeColors = {
        new Color(0.0f, 0.0f, 0.0f, 1.0f),
        new Color(1.0f, 1.0f, 1.0f, 1.0f),
        new Color(1.0f, 0.0f, 0.0f, 1.0f),
        new Color(1.0f, 1.0f, 0.0f, 1.0f),
        new Color(0.5f, 0.5f, 0.5f, 1.0f),
    };

    public nodeTypes nodeType { get; set; }

    private Vector2i gridIndices;

    private int index1D = 0;
    public int Index1D
    {
        get { return index1D; }
        protected set { index1D = value; }
    }

    public NavigationNode(Vector2i indices)
    {
        gridIndices = indices;
        index1D = indices.x + 16 * indices.y; // TODO: Change to be dependent on NavigationGrid Size!!!
        SetNodeType(nodeTypes.None);
    }

    public void SetNodeType(nodeTypes type)
    {
        this.nodeType = type;
    }

    public Vector2i GetGridIndices()
    {
        return gridIndices;
    }

    public static int GetManhattenDistance(NavigationNode lhs, NavigationNode rhs)
    {
        return Math.Abs(lhs.gridIndices.x - rhs.gridIndices.x) + Math.Abs(lhs.gridIndices.y - rhs.gridIndices.y);
    }
}
