using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NavigationGrid : MonoBehaviour {

    class PathNode : IEqualityComparer<PathNode>
    {
        public NavigationNode node { get; set; }
        public PathNode predecessor;
        public int cost { get; set; }

        public PathNode(NavigationNode _node, PathNode _predecessor, int _cost)
        {
            this.node = _node;
            this.cost = _cost;
            this.predecessor = _predecessor;
        }

        public bool Equals(PathNode x, PathNode y)
        {
            return (x.node == y.node);
        }

        public int GetHashCode(PathNode obj)
        {
            return obj.node.GetHashCode();
        }

        // Returns path of NavigationNodes
        public ArrayList GetPath()
        {
            ArrayList path = new ArrayList();

            PathNode curr = this;

            path.Add(curr.node);

            while(curr.predecessor != null)
            {
                curr = curr.predecessor;
                path.Add(curr.node);
            }

            path.Reverse();

            return path;
        }
    };

    public GameObject[] obst;

    private const int sizeX = 16;
    private const int sizeY = 16;

    public float stepSize = 2.0f;
    public int edgeCost = 1;
    public float maxHeight = 100.0f;

    private NavigationNode[,] nodes;
    private GameObject island;

    public void createGrid()
    {
        nodes = new NavigationNode[sizeX, sizeY];

        RaycastHit hit;

        if (island.GetComponent<Collider>().Raycast(new Ray(island.transform.position + maxHeight * island.transform.up, -island.transform.up), out hit, 2.0f * maxHeight))
        {
            this.transform.position = hit.point + 0.5f * island.transform.up;
        }
        else
        {
            this.transform.position = this.transform.position + 0.5f * this.transform.up;
        }

        // Set transform.position to position of first node in the array (corner)
        float shiftX = (sizeX % 2 == 0) ? (sizeX - 1) / 2.0f : sizeX / 2.0f;
        float shiftY = (sizeY % 2 == 0) ? (sizeY - 1) / 2.0f : sizeY / 2.0f;
        this.transform.position -= island.transform.forward * shiftY * stepSize;
        this.transform.position -= island.transform.right * shiftX * stepSize;

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                nodes[i, j] = new NavigationNode(new Vector2i(i, j));

                // Check if above island
                if (island.GetComponent<Collider>().Raycast(new Ray(GetNodeWorldPos(nodes[i, j]), -island.transform.up), out hit, 10.0f))
                {
                    nodes[i, j].SetNodeType(NavigationNode.nodeTypes.Free);

                    // Check if inside of obstacle
                    for (int k = 0; k < obst.Length; k++)
                    {
                        if(!isInside(GetNodeWorldPos(nodes[i, j]), obst[k].gameObject.GetComponent<Collider>()))
                        {
                            nodes[i, j].SetNodeType(NavigationNode.nodeTypes.Obst);
                        }
                    }    
                }
                else
                {
                    nodes[i, j].SetNodeType(NavigationNode.nodeTypes.None);
                }
            }
        }
    }

    // Only for convex colliders
    private bool isInside(Vector3 point, Collider coll)
    {
        Vector3 end = coll.bounds.center;
        Vector3 direction = end - point;
        direction.Normalize();

        RaycastHit hit;
        bool result = coll.Raycast(new Ray(point, direction), out hit, 100.0f);
        
        return result;
    }

    public ArrayList findPath(NavigationNode start, NavigationNode end)
    {
        PriorityQueue<PathNode> openlist = new PriorityQueue<PathNode>();
        HashSet<PathNode> closedlist = new HashSet<PathNode>();

        // 0 = left, 1 = right, 2 = up, 3 = down
        PathNode[] successors = new PathNode[4];

        openlist.Enqueue(0, new PathNode(start, null, 0));

        while(!openlist.IsEmpty())
        {
            PriorityQueue<PathNode>.PriorityQueueElement curr = openlist.Dequeue();
            if ( curr.m_value.node == end )
            {
                return curr.m_value.GetPath();
            }

            closedlist.Add(curr.m_value);

            // Expand to neighbouring cells
            Vector2i indices = curr.m_value.node.GetGridIndices();
            
            if (IndicesOnGrid(indices.x - 1, indices.y))
            {
                successors[0] = new PathNode(nodes[indices.x - 1, indices.y], null, 0);
            }
            if (IndicesOnGrid(indices.x + 1, indices.y) && !closedlist.Contains(new PathNode(nodes[indices.x + 1, indices.y], null, 0)))
            {
                successors[1] = new PathNode(nodes[indices.x + 1, indices.y], null, 0);
            }
            if (IndicesOnGrid(indices.x, indices.y + 1) && !closedlist.Contains(new PathNode(nodes[indices.x, indices.y + 1], null, 0)))
            {
                successors[2] = new PathNode(nodes[indices.x, indices.y + 1], null, 0);
            }
            if (IndicesOnGrid(indices.x, indices.y - 1) && !closedlist.Contains(new PathNode(nodes[indices.x, indices.y - 1], null, 0)))
            {
                successors[3] = new PathNode(nodes[indices.x, indices.y - 1], null, 0);
            }

            foreach (PathNode successor in successors)
            {
                if(!closedlist.Contains(successor))
                {
                    if(successor.node.GetNodeType() == NavigationNode.nodeTypes.Restricted)
                    {
                        closedlist.Add(successor);
                        continue;
                    }

                    int tentative_cost = curr.m_value.cost + edgeCost;

                    PriorityQueue<PathNode>.PriorityQueueElement elem;

                    if(openlist.Contains(successor, out elem) && tentative_cost >= elem.m_value.cost)
                    {
                        continue;
                    }
                    else
                    {
                        elem = new PriorityQueue<PathNode>.PriorityQueueElement(0, successor);
                    }

                    elem.m_value.predecessor = curr.m_value;
                    elem.m_value.cost = tentative_cost;
                    elem.m_key = tentative_cost + NavigationNode.GetManhattenDistance(elem.m_value.node, end);

                    if(openlist.Contains(successor))
                    {
                        openlist.UpdateElement(elem);
                    }
                    else
                    {
                        openlist.Enqueue(elem);
                    }
                }
            }
        }

        return null;

    }

    public NavigationNode GetClosestNode(Vector3 position)
    {
        //Plane plane = new Plane(this.transform.up, this.transform.position);
        //float dist = plane.GetDistanceToPoint(position);
        //Vector3 pointOnPlane = dist * -this.transform.up + position;
        Vector3 direction = position - this.transform.position;
        float distRight = Vector3.Dot(direction, this.transform.right);
        float distForward = Vector3.Dot(direction, this.transform.forward);

        int indexRight = (int)(distRight / stepSize);
        int indexForward = (int)(distForward / stepSize);

        if(!IndicesOnGrid(indexRight, indexForward))
        {
            //Debug.Log("Position not on NavigationGrid");
            return null;
        }
        else
        {
            //Debug.Log("Position on NavigationGrid (" + indexRight + "," + indexForward + ")");
            return nodes[indexRight, indexForward];
        }
    }

    public bool IndicesOnGrid(int x, int y)
    {
        if (x >= sizeX || x < 0 || y >= sizeY || y < 0)
            return false;
        else
            return true;
    }

    public Vector3 GetNodeWorldPos(NavigationNode node)
    {
        return this.transform.position + this.transform.forward * node.GetGridIndices().y * stepSize + this.transform.right * node.GetGridIndices().x * stepSize;
    }

    void Start()
    {
        this.island = this.transform.parent.gameObject;
        createGrid();
    }

    void OnDrawGizmos()
    {
        if(nodes != null)
        {
            foreach (NavigationNode node in nodes)
            {
                Gizmos.color = NavigationNode.nodeColors[(int)node.GetNodeType()];
                Gizmos.DrawCube(GetNodeWorldPos(node), new Vector3(0.3f, 0.3f, 0.3f));
            }
        }
    }
}
