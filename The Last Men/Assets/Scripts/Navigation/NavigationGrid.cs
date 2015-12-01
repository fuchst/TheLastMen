using UnityEngine;
using System.Collections;
using C5;

public class NavigationGrid : MonoBehaviour {

    public GameObject[] obst;

    private const int sizeX = 16;
    private const int sizeY = 16;

    public float stepSize = 2.0f;
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

    public bool findPath(NavigationNode start, NavigationNode end)
    {
        PriorityQueue<NavigationNode> openlist = new PriorityQueue<NavigationNode>();
        HashSet<NavigationNode> closedlist = new HashSet<NavigationNode>();

        openlist.Enqueue(NavigationNode.GetManhattenDistance(start, end), start);

        while(!openlist.IsEmpty())
        {
            PriorityQueue<NavigationNode>.PriorityQueueElement curr = openlist.Dequeue();
            if ( curr.value == end )
            {
                return true;
            }

            closedlist.Add(curr.value);

            // Add neighbouring cells to openlist
            Vector2i indices = curr.value.GetGridIndices();

            // Left
            if (IndicesOnGrid(indices.x - 1, indices.y) && !closedlist.Contains(nodes[indices.x - 1, indices.y]))
            {
                openlist.Enqueue(NavigationNode.GetManhattenDistance(nodes[indices.x - 1, indices.y], end) + curr.key, nodes[indices.x - 1, indices.y]);
            }
            // Right
            if (IndicesOnGrid(indices.x + 1, indices.y) && !closedlist.Contains(nodes[indices.x + 1, indices.y]))
            {
                openlist.Enqueue(NavigationNode.GetManhattenDistance(nodes[indices.x + 1, indices.y], end) + curr.key, nodes[indices.x + 1, indices.y]);
            }
            // Top
            if (IndicesOnGrid(indices.x, indices.y + 1) && !closedlist.Contains(nodes[indices.x, indices.y + 1]))
            {
                openlist.Enqueue(NavigationNode.GetManhattenDistance(nodes[indices.x, indices.y + 1], end) + curr.key, nodes[indices.x, indices.y + 1]);
            }
            // Bottom
            if (IndicesOnGrid(indices.x, indices.y - 1) && !closedlist.Contains(nodes[indices.x, indices.y - 1]))
            {
                openlist.Enqueue(NavigationNode.GetManhattenDistance(nodes[indices.x, indices.y - 1], end) + curr.key, nodes[indices.x, indices.y - 1]);
            }

        }

        return false;

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
            Debug.Log("Position not on NavigationGrid");
            return null;
        }
        else
        {
            Debug.Log("Position on NavigationGrid (" + indexRight + "," + indexForward + ")");
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
