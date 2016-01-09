using System.Collections;
using System;

class PathNode : IEquatable<PathNode>
{
    public NavigationNode node { get; set; }
    public PathNode predecessor { get; set; }
    public int cost { get; set; }

    public PathNode(NavigationNode _node, PathNode _predecessor, int _cost)
    {
        this.node = _node;
        this.cost = _cost;
        this.predecessor = _predecessor;
    }

    // Returns path of NavigationNodes
    public ArrayList GetPath()
    {
        ArrayList path = new ArrayList();

        PathNode curr = this;

        path.Add(curr.node);

        while (curr.predecessor != null)
        {
            curr = curr.predecessor;
            path.Add(curr.node);
        }

        path.Reverse();

        return path;
    }

    // Interface implementation
    public override int GetHashCode()
    {
        return this.node.GetID();
    }
    public bool Equals(PathNode other)
    {
        return (this.node.GetID() == other.node.GetID());
    }
};
