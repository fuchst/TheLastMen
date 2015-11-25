using UnityEngine;
using System.Collections;

public class NavigationMesh : MonoBehaviour {

    private GameObject gizmoPrefab;
    public static Material[] materials;

    public GameObject[] obst;

    private const int sizeX = 16;
    private const int sizeY = 16;

    private NavigationNode[,] nodes = new NavigationNode[sizeX,sizeY];
    private GameObject island;

    public void Awake() {
        gizmoPrefab = Resources.Load("Node", typeof(GameObject)) as GameObject;
    }

    public void createMesh(float stepSize)
    {
        RaycastHit hit;

        // Set transform.position to position of first node in the array (corner)
        if (island.GetComponent<Collider>().Raycast(new Ray(island.transform.position, island.transform.up), out hit, 10.0f))
        {
            this.transform.position = hit.point + 0.5f * island.transform.up - island.transform.position;
        }
        else
        {
            this.transform.position = this.transform.position + 0.5f * this.transform.up;
        }

        float shiftX = (sizeX % 2 == 0) ? (sizeX - 1) / 2.0f : sizeX / 2.0f;
        float shiftY = (sizeY % 2 == 0) ? (sizeY - 1) / 2.0f : sizeY / 2.0f;
        this.transform.position -= island.transform.forward * shiftY * stepSize;
        this.transform.position -= island.transform.right * shiftX * stepSize;

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                nodes[i, j] = new NavigationNode();
                nodes[i, j].position = this.transform.position + this.transform.forward * j * stepSize + this.transform.right * i * stepSize;

                nodes[i, j].gizmo = Instantiate(gizmoPrefab, nodes[i, j].position, this.transform.rotation) as GameObject;
                nodes[i, j].gizmo.transform.parent = this.gameObject.transform;

                // Check if above island
                if (island.GetComponent<Collider>().Raycast(new Ray(nodes[i, j].position, -island.transform.up), out hit, 10.0f))
                {
                    nodes[i, j].setType(NavigationNode.nodeTypes.Free);

                    // Check if inside of obstacle
                    for (int k = 0; k < obst.Length; k++)
                    {
                        if(!isInside(nodes[i, j].position, obst[k].gameObject.GetComponent<Collider>()))
                        {
                            nodes[i, j].setType(NavigationNode.nodeTypes.Obst);
                        }
                    }    
                }
                else
                {
                    nodes[i, j].setType(NavigationNode.nodeTypes.None);
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

    public void toggleRendering()
    {
        foreach(NavigationNode node in nodes)
        {
            node.toggleRendering();
        }
    }

    public NavigationNode[] findPath(NavigationNode start, NavigationNode end)
    {
        return null;
    }

    void Start()
    {
        this.island = this.transform.parent.gameObject;
        createMesh(2.0f);
    }
}
