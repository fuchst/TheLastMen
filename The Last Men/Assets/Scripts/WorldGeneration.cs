using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WorldGeneration : MonoBehaviour {

    public float radius = 150.0f;
    public int cycles = 3;
    public int randomSeed = 1337;
    public int numberOfArtifacts = 8;
    public float destructionLevel = 0.6f;
    public float heightOffset = 20.0f;

    //island prefab
    private string islandModel = "IslandSimple";

    private SortedDictionary<int, Island> islandDictionary = new SortedDictionary<int, Island>();
    private Transform islandParent;

    public void CreateWorld() {
        Random.seed = randomSeed;
        IcoSphere(radius, cycles, ref islandDictionary);

        CreateIslands();
        CreateBase();
        CreateArtifacts();
        CreateArtifactPaths();
        DestroyUnneededIslands();

        SetIslandHeights();
        CreateIslandsBetweenNodes();
    }

    void Update() {
        foreach (Island island in islandDictionary.Values) {
            foreach (int v in island.neighbors) {
                Island target;
                islandDictionary.TryGetValue(v, out target);
                Debug.DrawLine(island.position, target.position);
            }
        }
    }

    void CreateIslands() {
        islandParent = new GameObject("Islands").transform;
        foreach (KeyValuePair<int, Island> entry in islandDictionary) {
            GameObject newIsland = Instantiate(Resources.Load(islandModel, typeof(GameObject)), entry.Value.position, Quaternion.identity) as GameObject;
            newIsland.name = entry.Key.ToString();
            newIsland.transform.up = newIsland.transform.position;
            newIsland.transform.parent = islandParent;

            entry.Value.linkedGameObject = newIsland;
        }
    }

    void CreateBase() {
        Island baseIsland;
        islandDictionary.TryGetValue(0, out baseIsland);
        baseIsland.islandType = 1; //base
        baseIsland.linkedGameObject.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/BaseSimple", typeof(Material)) as Material;
        foreach (int neighborKey in baseIsland.neighbors) {
            islandDictionary.TryGetValue(neighborKey, out baseIsland);
            baseIsland.islandType = 2; //islands surrounding the base
            baseIsland.linkedGameObject.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/MainPathSimple", typeof(Material)) as Material;
        }
    }

    void CreateArtifacts() {
        for (int i = 0; i < numberOfArtifacts; i++) {
            //grab a random island
            Island artifactIsland = islandDictionary.ElementAt(Random.Range(0, islandDictionary.Count)).Value;
            if (artifactIsland.islandType > 0) {
                //reroll in case the island is already used for something else
                i--;
                continue;
            }
            artifactIsland.islandType = 3;
            artifactIsland.linkedGameObject.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/ArtifactSimple", typeof(Material)) as Material;
        }
    }

    void CreateArtifactPaths() {
        Vector3 origin = islandDictionary[0].position;
        foreach (KeyValuePair<int, Island> item in islandDictionary.Where(item => item.Value.islandType == 3)) {
            Island currentIsland = item.Value;
            while (currentIsland.islandType != 1) {
                int neighborClosestToBase = 0;
                float lastDistance = Vector3.Distance(islandDictionary[currentIsland.neighbors[0]].position, origin);
                for (int i = 1; i < currentIsland.neighbors.Count; i++) {
                    if (lastDistance > Vector3.Distance(islandDictionary[currentIsland.neighbors[i]].position, origin)) {
                        lastDistance = Vector3.Distance(islandDictionary[currentIsland.neighbors[i]].position, origin);
                        neighborClosestToBase = i;
                    }
                }
                islandDictionary[currentIsland.neighbors[neighborClosestToBase]].linkedGameObject.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/MainPathSimple", typeof(Material)) as Material;
                currentIsland.islandType = 2;
                currentIsland = islandDictionary[currentIsland.neighbors[neighborClosestToBase]];
            }
        }
        //base color is propably overpaint so re repaint the base island
        islandDictionary[0].islandType = 1;
        islandDictionary[0].linkedGameObject.GetComponent<MeshRenderer>().material = Resources.Load("SimpleMats/BaseSimple", typeof(Material)) as Material;
    }

    void DestroyUnneededIslands() {
        Stack<int> deleteStack = new Stack<int>();
        foreach (KeyValuePair<int, Island> item in islandDictionary) {
            if (item.Value.islandType == 0) {
                float randomNumber = Random.Range(0f, 1.0f);
                if (randomNumber < destructionLevel) {
                    deleteStack.Push(item.Key);
                }
            }
        }
        while (deleteStack.Count > 0) {
            int key = deleteStack.Pop();
            foreach (int neighbor in islandDictionary[key].neighbors) {
                islandDictionary[neighbor].neighbors.Remove(key);
            }
            Destroy(islandDictionary[key].linkedGameObject);
            islandDictionary.Remove(key);
        }
    }

    void SetIslandHeights() {
        foreach (Island island in islandDictionary.Values) {
            //we dont want to reset height of base or islands surrounding base
            if (island.islandType != 1 && island.neighbors.Contains(0) == false) {
                int offset = Random.Range(-1, 2);
                if (offset != 0) {
                    island.position = island.position + island.position.normalized * (offset * heightOffset);
                    island.linkedGameObject.transform.position = island.position;
                }
            }
        }
    }

    void CreateIslandsBetweenNodes() {
        foreach (KeyValuePair<int, Island> item in islandDictionary) {
            foreach (int neighbor in item.Value.neighbors) {
                if (neighbor > item.Key) {
                    Vector3 newPos = islandDictionary[neighbor].position + 0.5f * (item.Value.position - islandDictionary[neighbor].position);
                    GameObject newIsland = Instantiate(Resources.Load("IslandSmallSimple", typeof(GameObject)), newPos, Quaternion.identity) as GameObject;
                    newIsland.transform.up = newIsland.transform.position;
                    int rand = Random.Range(0, 2);
                    if (rand == 0) {
                        newIsland.transform.position = newIsland.transform.position + newIsland.transform.position.normalized * (1.5f * heightOffset);
                    }
                }
            }
        }
    }

    public Vector3 GetBasePosition() {
        return islandDictionary[0].position;
    }

    struct TriangleIndices {
        public int[] v;

        public TriangleIndices(int v1, int v2, int v3) {
            v = new int[] { v1, v2, v3 };
        }
    }

    private void IcoSphere(float radius, int cycles, ref SortedDictionary<int, Island> islandDictionary) {
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
        List<TriangleIndices> faces = new List<TriangleIndices>();
        List<Vector3> verticesList = new List<Vector3>();

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

        // subdivide
        for (int i = 0; i < cycles; i++) {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces) {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v[0], tri.v[1], ref verticesList, ref middlePointIndexCache, radius);
                int b = getMiddlePoint(tri.v[1], tri.v[2], ref verticesList, ref middlePointIndexCache, radius);
                int c = getMiddlePoint(tri.v[2], tri.v[0], ref verticesList, ref middlePointIndexCache, radius);

                faces2.Add(new TriangleIndices(tri.v[0], a, c));
                faces2.Add(new TriangleIndices(tri.v[1], b, a));
                faces2.Add(new TriangleIndices(tri.v[2], c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        //we dont want to store the sphere in faces.
        //instead we need a graph of nodes with its neighbors
        //lets map faces into a dictionary of islands where each island knows its neigbors
        for (int i = 0; i < faces.Count; i++) {
            for (int j = 0; j < 3; j++) {
                Island island;
                if (islandDictionary.TryGetValue(faces[i].v[j], out island) == false) {
                    island = new Island();
                    island.position = verticesList[faces[i].v[j]];
                    islandDictionary.Add(faces[i].v[j], island);
                }
                for (int k = 0; k < 3; k++) {
                    if (k != j) {
                        if (island.neighbors.Contains(faces[i].v[k]) == false) {
                            island.neighbors.Add(faces[i].v[k]);
                        }
                    }
                }
            }
        }
    }

    //helper method for icosphere subdivision
    private int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius) {
        //first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret)) {
            return ret;
        }

        //not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);
        cache.Add(key, i);
        return i;
    }
}

class Island {
    /*
    islandType
    0: unassigned
    1: base
    2: main path
    3: artifact
    */
    public List<int> neighbors = new List<int>();
    public int islandType = 0;
    public GameObject linkedGameObject;
    public Vector3 position;
}