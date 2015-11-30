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

    private nodeTypes type;

    private Vector2i gridIndices;

    public NavigationNode(Vector2i indices)
    {
        gridIndices = indices;
        SetNodeType(nodeTypes.None);
    }

    public nodeTypes GetNodeType()
    {
        return type;
    }

    public void SetNodeType(nodeTypes type)
    {
        this.type = type;
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
