using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NavigationGrid : MonoBehaviour {

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

            while(curr.predecessor != null)
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
            return this.node.Index1D;
        }
        public bool Equals(PathNode other)
        {
            return (this.node.Index1D == other.node.Index1D);
        }
    };

    // Change value in prefab
    public int sizeX = 32;
    public int sizeY = 32;

    public float stepSize = 0.5f;
    public int edgeCost = 2;
    public int edgeCostDiag = 3;
    public float maxHeight = 100.0f;

    public NavigationNode[,] nodes;
    private GameObject island;
    private GameObject[] obstacles;

    public void createGrid()
    {
        this.obstacles = Helper.FindChildrenWithTag(island, "Obstacle");

        this.nodes = new NavigationNode[sizeX, sizeY];

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

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                nodes[i, j] = new NavigationNode(new Vector2i(i, j));

                // Check if above island
                if (island.GetComponent<Collider>().Raycast(new Ray(GetNodeWorldPos(nodes[i, j]), -island.transform.up), out hit, 10.0f))
                {
                    nodes[i, j].SetNodeType(NavigationNode.nodeTypes.Free);

                    // Check if inside of obstacle
                    for (int k = 0; k < obstacles.Length; k++)
                    {
                        if(!isInside(GetNodeWorldPos(nodes[i, j]), obstacles[k].gameObject.GetComponent<Collider>()))
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

        // 8 nodes arround current node
        // 0 1 2
        // 3 x 4
        // 5 6 7
        PathNode[] successors = new PathNode[8];

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
            
            for(int i = 0; i < successors.Length; i++)
            {
                successors[i] = null;
            }

            // Top
            if (IndicesOnGrid(indices.x - 1, indices.y + 1))
            {
                successors[0] = new PathNode(nodes[indices.x - 1, indices.y + 1], null, 0);
            }
            if (IndicesOnGrid(indices.x, indices.y + 1))
            {
                successors[1] = new PathNode(nodes[indices.x, indices.y + 1], null, 0);
            }
            if (IndicesOnGrid(indices.x + 1, indices.y + 1))
            {
                successors[2] = new PathNode(nodes[indices.x + 1, indices.y + 1], null, 0);
            }
            // Middle
            if (IndicesOnGrid(indices.x - 1, indices.y))
            {
                successors[3] = new PathNode(nodes[indices.x - 1, indices.y], null, 0);
            }
            if (IndicesOnGrid(indices.x + 1, indices.y))
            {
                successors[4] = new PathNode(nodes[indices.x + 1, indices.y], null, 0);
            }
            // Bottom
            if (IndicesOnGrid(indices.x - 1, indices.y - 1))
            {
                successors[5] = new PathNode(nodes[indices.x - 1, indices.y - 1], null, 0);
            }
            if (IndicesOnGrid(indices.x, indices.y - 1))
            {
                successors[6] = new PathNode(nodes[indices.x, indices.y - 1], null, 0);
            }
            if (IndicesOnGrid(indices.x + 1, indices.y - 1))
            {
                successors[7] = new PathNode(nodes[indices.x + 1, indices.y - 1], null, 0);
            }

            //foreach (PathNode successor in successors)
            for(int i = 0; i < successors.Length; i++)
            {
                PathNode successor = successors[i];
                if (successor != null && !closedlist.Contains(successor))
                {
                    if (successor.node.nodeType == NavigationNode.nodeTypes.Obst || successor.node.nodeType == NavigationNode.nodeTypes.None)
                    {
                        closedlist.Add(successor);
                        continue;
                    }

                    int tentative_cost;

                    if (i == 0 || i == 2 || i == 5 || i == 7) {
                        tentative_cost = curr.m_value.cost + edgeCostDiag;
                    }
                    else {
                        tentative_cost = curr.m_value.cost + edgeCost;
                    }

                    PriorityQueue<PathNode>.PriorityQueueElement elem;

                    if (openlist.Contains(successor, out elem))
                    {
                        if (tentative_cost < elem.m_value.cost)
                        {
                            elem.m_value.cost = tentative_cost;
                            openlist.UpdateElement(elem);
                        }
                    }
                    else
                    {
                        successor.predecessor = curr.m_value;
                        successor.cost = tentative_cost;
                        openlist.Enqueue(new PriorityQueue<PathNode>.PriorityQueueElement(successor.cost + curr.m_value.cost, successor));
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

    public NavigationNode GetRandomFreeNode()
    {
        for(int i = 0; i < sizeX * sizeY; i++)
        {
            int x = UnityEngine.Random.Range(0, sizeX - 1);
            int y = UnityEngine.Random.Range(0, sizeY - 1);

            if(nodes != null && nodes[x,y].nodeType == NavigationNode.nodeTypes.Free)
            {
                return nodes[x,y];
            }
        }

        return null;
    }

    public NavigationNode GetNodeAtIndices(int x, int y)
    {
        if(IndicesOnGrid(x, y))
        {
            return nodes[x, y];
        }

        return null;
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

    public void Init()
    {
        this.island = this.transform.parent.gameObject;
        this.createGrid();
    }

    void OnDrawGizmos()
    {
        if(Camera.current.name == "MainCamera")
        {
            if (nodes != null)
            {
                foreach (NavigationNode node in nodes)
                {
                    Gizmos.color = NavigationNode.nodeColors[(int)node.nodeType];
                    Gizmos.DrawCube(GetNodeWorldPos(node), new Vector3(0.3f, 0.3f, 0.3f));
                }
            }
        }      
    }
}
