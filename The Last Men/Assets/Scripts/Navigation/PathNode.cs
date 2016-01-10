using System.Collections;
using System;

class PathNode : IEquatable<PathNode>
{
    public int nodeID { get; set; }
    public PathNode predecessor { get; set; }
    public int cost { get; set; }

    public PathNode(int _nodeID, PathNode _predecessor, int _cost)
    {
        this.nodeID = _nodeID;
        this.cost = _cost;
        this.predecessor = _predecessor;
    }

    // Returns path of NavigationNodes
    public ArrayList GetPath()
    {
        ArrayList path = new ArrayList();

        PathNode curr = this;

        path.Add(curr.nodeID);

        while (curr.predecessor != null)
        {
            curr = curr.predecessor;
            path.Add(curr.nodeID);
        }

        path.Reverse();

        return path;
    }

    // Interface implementation
    public override int GetHashCode()
    {
        return this.nodeID;
    }
    public bool Equals(PathNode other)
    {
        return (this.nodeID == other.nodeID);
    }
};
