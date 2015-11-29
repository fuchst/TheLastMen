using UnityEngine;
using System.Collections;

public struct Vector2i
{
    public int x;
    public int y;

    public Vector2i(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
};

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
}
