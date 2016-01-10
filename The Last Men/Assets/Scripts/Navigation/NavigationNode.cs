using UnityEngine;
using System;

public class NavigationNode {
    
    public enum nodeTypes
    {
        None,
        Free,
        Obst,
        Restricted,
        Cover,
        Enemy
    };

    public struct Edge
    {
        public int nodeID;
        public int cost;
    };

    public static Color[] nodeColors = {
        new Color(0.0f, 0.0f, 0.0f, 1.0f),
        new Color(1.0f, 1.0f, 1.0f, 1.0f),
        new Color(1.0f, 0.0f, 0.0f, 1.0f),
        new Color(1.0f, 1.0f, 0.0f, 1.0f),
        new Color(0.5f, 0.5f, 0.5f, 1.0f),
        new Color(0.0f, 1.0f, 1.0f, 1.0f),
    };

    public nodeTypes nodeType { get; set; }

    private Vector2i gridIndices;

    private NavigationGrid grid = null;

    // Save all neighbours of a node numblock style (x = this node)
    // 0 1 2
    // 3 x 4
    // 5 6 7
    public Edge[] neighbours = new Edge[8];

    private int id;

    public NavigationNode(NavigationGrid grid, Vector2i indices)
    {
        this.grid = grid;
        gridIndices = indices;
        id = CalculateID(indices);
        SetNodeType(nodeTypes.None);

        // Initalize edges with start values
        for(int i = 0; i < 8; i++)
        {
            neighbours[i].nodeID = int.MinValue;
            neighbours[i].cost = 0;
        }
    }

    public void SetNodeType(nodeTypes type)
    {
        this.nodeType = type;
    }

    public Vector2i GetGridIndices()
    {
        return gridIndices;
    }

    public int GetID()
    {
        return id;
    }

    public static int CalculateID(Vector2i indices)
    {
        // magic number 1000 which is big enough to not have two of the same values
        return indices.x + 1000 * indices.y;
    }

    public static int GetManhattenDistance(NavigationNode lhs, NavigationNode rhs)
    {
        return Math.Abs(lhs.gridIndices.x - rhs.gridIndices.x) + Math.Abs(lhs.gridIndices.y - rhs.gridIndices.y);
    }
}
