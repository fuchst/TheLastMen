using UnityEngine;
using System.Collections.Generic;

public class WorldGeneration : MonoBehaviour {

    public float radius = 5.0f;
    public int cycles = 2;
    public int randomSeed = 1337;
    public int numberOfArtifacts = 8;
    public float destructionLevel = 0.6f;

    private string islandModel = "IslandSimple";

    private Vector3[] vertices;
    private Vector2[] edges;
    private int[,] edgesMatrix;
    private List<int> importantIslands = new List<int>();
    private Vector3 basePos;

    private Transform islandParent;

    public void CreateWorld() {
        Random.seed = randomSeed;
        new IcoSphere(radius, cycles, ref vertices, ref edges);
        CreateEdgeMatrix();
        CreateIslands();
        CreateBase();
        int[] artifacts = new int[numberOfArtifacts];
        CreateArtifacts(ref artifacts);
        CreateArtifactPaths(ref artifacts);
        DestroyUnneededIslands();
    }

    //void Update() {
    //    //show grid
    //    foreach (Vector2 e in edges) {
    //        Debug.DrawLine(vertices[(int)e.x], vertices[(int)e.y]);
    //    }
    //}

    void CreateIslands() {
        islandParent = new GameObject("Islands").transform;
        Rigidbody rb = islandParent.gameObject.AddComponent<Rigidbody>() as Rigidbody;
        rb.isKinematic = true;
        rb.useGravity = false;

        for (int i = 0; i < vertices.Length; i++) {
            GameObject GO = Instantiate(Resources.Load(islandModel, typeof(GameObject)), vertices[i], Quaternion.identity) as GameObject;
            GO.transform.up = GO.transform.position - new Vector3(0, 0, 0);
            GO.transform.parent = islandParent;
            GO.name = i.ToString();
        }
    }

    void CreateBase() {
        importantIslands.Add(0);
        GameObject baseIsland = islandParent.GetChild(0).gameObject;
        basePos = baseIsland.transform.position;
        baseIsland.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/BaseSimple", typeof(Material)) as Material;
        //Optimize: break look after 5 neighbors has been found
        for (int i = 1; i < edgesMatrix.GetLength(0); i++) {
            if (edgesMatrix[0, i] == 1) {
                importantIslands.Add(i);
                GameObject neighborIsland = islandParent.GetChild(i).gameObject;
                neighborIsland.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/MainPathSimple", typeof(Material)) as Material;
            }
        }
    }

    void CreateEdgeMatrix() {
        edgesMatrix = new int[edges.Length, edges.Length];
        foreach (Vector2 e in edges) {
            int x = (int)e.x;
            int y = (int)e.y;
            edgesMatrix[x, y] = 1;
            edgesMatrix[y, x] = 1;
        }
    }

    void CreateArtifacts(ref int[] artifacts) {
        //random distribution not good enough
        for (int i = 0; i < artifacts.Length; i++) {
            artifacts[i] = Random.Range(1, vertices.Length);
        }
        foreach (int x in artifacts) {
            GameObject artifactIsland = islandParent.GetChild(x).gameObject;
            artifactIsland.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/ArtifactSimple", typeof(Material)) as Material;
            importantIslands.Add(x);
        }
    }

    void CreateArtifactPaths(ref int[] artifacts) {
        Vector3 origin = islandParent.GetChild(0).transform.position;
        for (int i = 0; i < artifacts.Length; i++) {
            //create a path of islands from artifact isle to base isle
            //List<int> path = new List<int>();
            int current = artifacts[i];
            // reminder: base index is 0
            while (current != 0) {
                //find neighbors
                List<int> neighbors = new List<int>();
                int count = 0;
                int j = 0;
                while (count < 5 || j > vertices.Length) {
                    if (edgesMatrix[current, j] == 1) {
                        count++;
                        neighbors.Add(j);
                    }
                    j++;
                }
                //get closes neighbor
                while (neighbors.Count > 1) {
                    if (Vector3.Distance(vertices[neighbors[0]], origin) < Vector3.Distance(vertices[neighbors[1]], origin)) {
                        neighbors.RemoveAt(1);
                    }
                    else {
                        neighbors.RemoveAt(0);
                    }
                }
                current = neighbors[0];
                if (importantIslands.Contains(current) == false) {
                    importantIslands.Add(current);
                    islandParent.GetChild(current).gameObject.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/MainPathSimple", typeof(Material)) as Material;
                }
            }
        }
    }

    void DestroyUnneededIslands() {
        importantIslands.Sort();
        int numberDeleted = 0;
        //remove doubles
        for (int i = importantIslands.Count - 2; i > -1; i--) {
            if (importantIslands[i] == importantIslands[i + 1]) {
                importantIslands.RemoveAt(i + 1);
                numberDeleted++;
            }
        }
        //remove islands
        int islands = vertices.Length;
        for (int i = islands - 1; i > -1; i--) {
            if (importantIslands.Contains(i) == false) {
                float randomNumber = Random.Range(0f, 1.0f);
                if (randomNumber < destructionLevel) {
                    Destroy(islandParent.GetChild(i).gameObject);
                }
            }
        }
    }

    public Vector3 GetBasePosition() {
        //not transform.GetChild(0).position because we may change that in a future commit
        return basePos;
    }
}

class IcoSphere {
    private float radius;
    public List<Vector3> verticesList = new List<Vector3>();

    public struct TriangleIndices {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3) {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    public IcoSphere(float radius, int cycles, ref Vector3[] vertices, ref Vector2[] edges) {
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
        List<TriangleIndices> faces = new List<TriangleIndices>();
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        verticesList.Add(new Vector3(-1f, t, 0f).normalized * radius);
        verticesList.Add(new Vector3(1f, t, 0f).normalized * radius);
        verticesList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
        verticesList.Add(new Vector3(1f, -t, 0f).normalized * radius);

        verticesList.Add(new Vector3(0f, -1f, t).normalized * radius);
        verticesList.Add(new Vector3(0f, 1f, t).normalized * radius);
        verticesList.Add(new Vector3(0f, -1f, -t).normalized * radius);
        verticesList.Add(new Vector3(0f, 1f, -t).normalized * radius);

        verticesList.Add(new Vector3(t, 0f, -1f).normalized * radius);
        verticesList.Add(new Vector3(t, 0f, 1f).normalized * radius);
        verticesList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
        verticesList.Add(new Vector3(-t, 0f, 1f).normalized * radius);

        // 5 faces around point 0
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        // 5 adjacent faces
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        // 5 neighbour faces
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));

        // 5 faces around point 3
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        // refine triangles
        for (int i = 0; i < cycles; i++) {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces) {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2, ref verticesList, ref middlePointIndexCache, radius);
                int b = getMiddlePoint(tri.v2, tri.v3, ref verticesList, ref middlePointIndexCache, radius);
                int c = getMiddlePoint(tri.v3, tri.v1, ref verticesList, ref middlePointIndexCache, radius);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        //at this point the sphere is already generated.
        //more information is created which are needed for modifying the sphere
        //TODO: optimize this part. its horrible
        vertices = verticesList.ToArray();
        List<Vector2> edgesList = new List<Vector2>();
        faces.ForEach(delegate (TriangleIndices tri) {
            Vector2[] triEdges = new Vector2[3];
            triEdges[0] = new Vector2(tri.v1, tri.v2);
            triEdges[1] = new Vector2(tri.v2, tri.v3);
            triEdges[2] = new Vector2(tri.v1, tri.v3);
            for (int i = 0; i < 3; i++) {
                if (triEdges[i].x > triEdges[i].y) {
                    triEdges[i] = new Vector2(triEdges[i].y, triEdges[i].x);
                }
            }

            bool[] contains = { false, false, false };
            edgesList.ForEach(delegate (Vector2 existingEdge) {
                for (int i = 0; i < 3; i++) {
                    if (existingEdge == triEdges[i]) {
                        contains[i] = true;
                    }
                }
            });
            for (int i = 0; i < 3; i++) {
                if (contains[i] == false) {
                    edgesList.Add(triEdges[i]);
                }
            }
        });
        edges = edgesList.ToArray();
    }

    // return index of point in the middle of p1 and p2
    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius) {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret)) {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        // add vertex makes sure point is on unit sphere
        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);

        // store it, return index
        cache.Add(key, i);

        return i;
    }
}