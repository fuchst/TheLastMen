using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationGrid : MonoBehaviour {

    public float stepSize = 8.0f;
    public float maxHeight = 100.0f;

    public SortedList<int, NavigationNode> nodes = new SortedList<int, NavigationNode>();
    public List<int> freeNodeIDs = new List<int>();

    private List<Vector3> obstNodes = new List<Vector3>();

    private GameObject island;
    private GameObject[] obstacles;

    public static Vector2i[] neighbourIdxDiffs = {
        new Vector2i(-1, 1),
        new Vector2i( 0, 1),
        new Vector2i( 1, 1),
        new Vector2i(-1, 0),
        new Vector2i( 1, 0),
        new Vector2i(-1,-1),
        new Vector2i( 0,-1),
        new Vector2i( 1,-1),
    };

    public static int[] neighbourCost =
    {
        3,
        2,
        3,
        2,
        2,
        3,
        2,
        3
    };

    public void createGrid()
    {
        this.obstacles = Helper.FindChildrenWithTag(island.transform.parent, "Obstacle");
        //Debug.Log(obstacles.Length);

        RaycastHit hit;

        // Get first node position (height through raycast)
        if (island.GetComponent<Collider>().Raycast(new Ray(island.transform.position + maxHeight * island.transform.up, -island.transform.up), out hit, 2.0f * maxHeight))
        {
            this.transform.position = hit.point + 0.5f * island.transform.up;
        }
        else
        {
            this.transform.position = this.transform.position + 0.5f * this.transform.up;
        }

        nodes.Add(0, new NavigationNode(this, new Vector2i(0, 0)));

        if (island.GetComponent<Collider>().Raycast(new Ray(GetNodeWorldPos(nodes[0]), -island.transform.up), out hit, 10.0f))
        {
            nodes[0].SetNodeType(NavigationNode.nodeTypes.Free);
            freeNodeIDs.Add(0);

            // Check if inside of obstacle
            for (int k = 0; k < obstacles.Length; k++)
            {
                //if (!isInside(GetNodeWorldPos(nodes[0]), obstacles[k].gameObject.GetComponent<Collider>()))
                //{
                //    nodes[0].SetNodeType(NavigationNode.nodeTypes.Obst);
                //}
            }
        }

        SetNodeNeighbours(0);

        checkObst();
    }

    private void SetNodeNeighbours(int nodeID)
    {
        for (int i = 0; i < 8; i++)
        {
            // Check non yet visited neigbours
            if (nodes[nodeID].neighbours[i].cost == 0)
            {
                // Calculate neighbour grid indices
                Vector2i neighIdx = nodes[nodeID].GetGridIndices() + neighbourIdxDiffs[i];
                int neighID = NavigationNode.CalculateID(neighIdx);
                // Check if already available
                if (nodes.ContainsKey(neighID))
                {
                    nodes[nodeID].neighbours[i].nodeID = neighID;
                    nodes[nodeID].neighbours[i].cost = neighbourCost[i];
                    // Set according values in neighbour
                    nodes[neighID].neighbours[7 - i].nodeID = nodes[nodeID].GetID();
                    nodes[neighID].neighbours[7 - i].cost = neighbourCost[i];
                }
                else
                {
                    nodes.Add(neighID, new NavigationNode(this, neighIdx));
                    nodes[nodeID].neighbours[i].nodeID = neighID;

                    RaycastHit hit;

                    // Check if above island
                    if (island.GetComponent<Collider>().Raycast(new Ray(GetNodeWorldPos(nodes[neighID]), -island.transform.up), out hit, 10.0f))
                    {
                        nodes[neighID].SetNodeType(NavigationNode.nodeTypes.Free);
                        freeNodeIDs.Add(neighID);
                        nodes[nodeID].neighbours[i].cost = neighbourCost[i];

                        // Check if inside of obstacle
                        //for (int k = 0; k < obstacles.Length; k++)
                        //{
                            // Check if node in obst
                            //if (!isInside(GetNodeWorldPos(nodes[neighID]), obstacles[k].gameObject.GetComponent<Collider>()))
                            //{
                                //nodes[neighID].SetNodeType(NavigationNode.nodeTypes.Obst);
                                //nodes[neighID].neighbours[7 - i].cost = int.MaxValue;
                                //nodes[nodeID].neighbours[i].cost = int.MaxValue;
                            //}
                            // Check if obst is blocking path
                        //}
                    }
                    else
                    {
                        nodes[neighID].SetNodeType(NavigationNode.nodeTypes.None);
                        nodes[nodeID].neighbours[i].cost = int.MaxValue;
                        for(int j = 0; j < 8; j++)
                        {
                            nodes[neighID].neighbours[7 - i].cost = int.MaxValue;
                            nodes[neighID].neighbours[i].cost = int.MaxValue;
                        }
                    }

                    if(nodes[nodeID].neighbours[i].cost != int.MaxValue)
                        SetNodeNeighbours(neighID);
                }             
            }
        }
    }

    private void checkObst()
    {
        // Save all nodes to later sort them in a meaningful fashion:
        // 3 4
        // 1 2
        List<int> surrNodes = new List<int>();

        for (int i = 0; i < obstacles.Length; i++)
        {
            NavigationNode closestNode = GetClosestNode(obstacles[i].transform.position);

            if (Vector3.Distance(GetNodeWorldPos(closestNode), obstacles[i].transform.position) < stepSize)
            {
                //Debug.Log("Obst is close");

                surrNodes.Clear();
                surrNodes.AddRange(GetClosestNeighbourIndices(obstacles[i].transform.position));
                surrNodes.Sort();

                // The order in which neighbours are updated
                int[] order = { 1, 2, 4, 3, 0, 1, 6, 7, 4, 3, 5, 6 };

                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        nodes[surrNodes[j]].neighbours[order[j*3+k]].cost = int.MaxValue;
                        //if(!surrNodes.Contains(nodes[surrNodes[j]].neighbours[order[j * 3 + k]].nodeID))
                        //   Debug.Log("Fuck");
                    }

                    obstNodes.Add(GetNodeWorldPos(nodes[surrNodes[j]]));
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

    // slefNode = current occupied node of searcher
    public ArrayList findPath(int startID, int endID, int selfNodeID)
    {
        PriorityQueue<PathNode> openlist = new PriorityQueue<PathNode>();
        HashSet<PathNode> closedlist = new HashSet<PathNode>();

        // 8 nodes arround current node
        // 0 1 2
        // 3 x 4
        // 5 6 7
        PathNode[] successors = new PathNode[8];

        openlist.Enqueue(0, new PathNode(startID, null, 0));
        
        while(!openlist.IsEmpty())
        {
            PriorityQueue<PathNode>.PriorityQueueElement curr = openlist.Dequeue();

            if ( curr.m_value.nodeID == endID )
            {
                return curr.m_value.GetPath();
            }
            
            closedlist.Add(curr.m_value);

            // Expand to neighbouring cells
            Vector2i indices = nodes[curr.m_value.nodeID].GetGridIndices();
            
            for(int i = 0; i < successors.Length; i++)
            {
                successors[i] = null;
            }

            for(int i = 0; i < 8; i++)
            {
                Vector2i successorIdx = indices + neighbourIdxDiffs[i];

                if (IndicesOnGrid(successorIdx.x, successorIdx.y))
                {
                    successors[i] = new PathNode(nodes[NavigationNode.CalculateID(successorIdx)].GetID(), null, 0);
                }
            }

            //foreach (PathNode successor in successors)
            for (int i = 0; i < successors.Length; i++)
            {
                PathNode successor = successors[i];
                if (successor != null && !closedlist.Contains(successor))
                {
                    // Check if next node can be reached
                    if (nodes[successor.nodeID].nodeType != NavigationNode.nodeTypes.Free || nodes[successor.nodeID].neighbours[7 - i].cost == int.MaxValue)
                    {
                        closedlist.Add(successor);
                        continue;
                    }

                    int tentative_cost;

                    int neighbour_cost = nodes[successor.nodeID].neighbours[7 - i].cost;

                    if(neighbour_cost != int.MaxValue)
                    {
                        tentative_cost = curr.m_value.cost + nodes[successor.nodeID].neighbours[7 - i].cost;
                    }
                    else
                    {
                        tentative_cost = int.MaxValue;
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
                        if(tentative_cost != int.MaxValue)
                        {
                            openlist.Enqueue(new PriorityQueue<PathNode>.PriorityQueueElement(successor.cost + curr.m_value.cost, successor));
                        }
                        else
                        {
                            openlist.Enqueue(new PriorityQueue<PathNode>.PriorityQueueElement(successor.cost, successor));
                        }
                    }
                }
            }
        }

        return null;

    }

    // Return closest node position on grid or null if position not on grid
    public NavigationNode GetClosestNode(Vector3 position)
    {
        //Plane plane = new Plane(this.transform.up, this.transform.position);
        //float dist = plane.GetDistanceToPoint(position);
        //Vector3 pointOnPlane = dist * -this.transform.up + position;
        Vector3 direction = position - this.transform.position;
        float distRight = Vector3.Dot(direction, this.transform.right);
        float distForward = Vector3.Dot(direction, this.transform.forward);

        int indexRight = (int)(Mathf.Round(distRight / stepSize));
        int indexForward = (int)(Mathf.Round(distForward / stepSize));

        if(!IndicesOnGrid(indexRight, indexForward))
        {
            //Debug.Log("Position not on NavigationGrid");
            return null;
        }
        else
        {
            //Debug.Log("Position on NavigationGrid (" + indexRight + "," + indexForward + ")");
            return nodes[NavigationNode.CalculateID(new Vector2i(indexRight, indexForward))];
        }
    }

    public int[] GetClosestNeighbourIndices(Vector3 position)
    {
        NavigationNode node = GetClosestNode(position);
        Vector3 nodePosition = GetNodeWorldPos(node);

        Vector3 direction = (position - nodePosition).normalized;
        // Map direction onto navgrid plane
        direction -= Vector3.Dot(direction, transform.up) * transform.up;
        direction.Normalize();
        int forward = (int)Mathf.Sign(Vector3.Dot(direction, transform.forward));
        int right = (int)Mathf.Sign(Vector3.Dot(direction, transform.right));

        // Directional indices starting from closest node
        int[] relIndices = new int[3];
        relIndices[0] = (right > 0) ? 4 : 3;
        relIndices[1] = (forward > 0) ? 1 : 6;
        relIndices[2] = (right > 0) ? ((forward > 0) ? 2 : 7) : ((forward > 0) ? 0 : 5);

        int[] indices = new int[4];
        indices[0] = node.GetID();
        indices[1] = node.neighbours[relIndices[0]].nodeID;
        indices[2] = node.neighbours[relIndices[1]].nodeID;
        indices[3] = node.neighbours[relIndices[2]].nodeID;

        return indices;
    }

    public NavigationNode GetRandomFreeNode()
    {
        int index = UnityEngine.Random.Range(0, freeNodeIDs.Count - 1);

        return nodes.Values[index];
    }

    public NavigationNode GetNodeAtIndices(int x, int y)
    {
        if (IndicesOnGrid(x, y))
        {
            return nodes[NavigationNode.CalculateID(new Vector2i(x, y))];
        }

        return null;
    }

    public bool IndicesOnGrid(int x, int y)
    {
        return nodes.ContainsKey(NavigationNode.CalculateID(new Vector2i(x, y)));
    }

    public Vector3 GetNodeWorldPos(NavigationNode node)
    {
        // Hack returns 0,0,0 if node is null
        if (node != null)
            return this.transform.position + this.transform.forward * node.GetGridIndices().y * stepSize + this.transform.right * node.GetGridIndices().x * stepSize;
        else
            return new Vector3(0.0f, 0.0f, 0.0f);    
    }

    public void Init()
    {
        this.island = this.transform.parent.gameObject;
        this.createGrid();
    }

    //void OnDrawGizmos()
    //{
    //    if (nodes != null)
    //    {
    //        foreach (NavigationNode node in nodes.Values)
    //        {
    //            Gizmos.color = NavigationNode.nodeColors[(int)node.nodeType];
    //            Gizmos.DrawCube(GetNodeWorldPos(node), new Vector3(0.3f, 0.3f, 0.3f));
    //        }

    //        Color[] colors = { Color.blue, Color.green, Color.red, Color.white };
    //        for (int i = 0; i < obstNodes.Count; i++)
    //        {
    //            Gizmos.color = colors[i%4];
    //            Gizmos.DrawSphere(obstNodes[i], 1.0f);
    //        }
    //    }
    //}
}
