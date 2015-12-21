using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    private enum IslandType { None, Bastion, Path, Artifact, Grappling, Small };

    public float baseIslandSize = 10f;
    public float radius = 150.0f;
    public int cycles = 3;
    public int randomSeed = 1337;
    public int artifactCount = 8;
    public float destructionLevel = 0.6f;
    public float layerHeightOffset = 200.0f;
    public float grapplingIslandExtraheight;

    private SortedDictionary<int, Island> islandDictionary = new SortedDictionary<int, Island>();
    private Transform islandParent;

    public void CreateWorld()
    {
        Random.seed = randomSeed;
        IcoSphere(radius, cycles, ref islandDictionary);
        islandParent = new GameObject("Islands").transform;
        SetupBastion();
        SetupArtifacts();
        MarkArtifactPaths();
        //DestroyUnneededIslands();
        SetupLayers();
        SetupGrapplingIslands();
        InstantiateIslands();
    }

    void Update()
    {
        if (LevelManager.Instance.showPaths == true)
        {
            foreach (Island island in islandDictionary.Values)
            {
                foreach (int key in island.neighbors)
                {
                    Island target = islandDictionary[key];
                    Debug.DrawLine(island.position, target.position);
                }
            }
        }
    }

    private void SetupBastion()
    {
        Island island = islandDictionary[0];
        island.islandType = IslandType.Bastion;
        island.layer = 0;

        //mark islands surrounding the base
        foreach (int key in island.neighbors)
        {
            Island neighbor = islandDictionary[key];
            neighbor.islandType = IslandType.Path;
            neighbor.layer = 0;
        }
    }

    private void SetupArtifacts()
    {
        for (int i = 0; i < artifactCount; i++)
        {
            Island artifactIsland = islandDictionary.ElementAt(Random.Range(0, islandDictionary.Count)).Value;  //grab a random island;
            //reroll in case the island is already used for something else
            if (artifactIsland.islandType != IslandType.None)
            {
                i--;
                continue;
            }
            artifactIsland.islandType = IslandType.Artifact;
            artifactIsland.layer = 1;
        }
    }

    void MarkArtifactPaths()
    {
        Vector3 origin = islandDictionary[0].position;
        foreach (KeyValuePair<int, Island> item in islandDictionary.Where(item => item.Value.islandType == IslandType.Artifact))
        {
            Island currentIsland = item.Value;
            while (currentIsland.islandType != IslandType.Bastion)
            {
                int neighborClosestToBase = 0;
                float lastDistance = Vector3.Distance(islandDictionary[currentIsland.neighbors[0]].position, origin);
                for (int i = 1; i < currentIsland.neighbors.Count; i++)
                {
                    if (lastDistance > Vector3.Distance(islandDictionary[currentIsland.neighbors[i]].position, origin))
                    {
                        lastDistance = Vector3.Distance(islandDictionary[currentIsland.neighbors[i]].position, origin);
                        neighborClosestToBase = i;
                    }
                }
                if (currentIsland.islandType == IslandType.None)
                {
                    currentIsland.islandType = IslandType.Path;
                }
                currentIsland = islandDictionary[currentIsland.neighbors[neighborClosestToBase]];
            }
        }
    }

    //void DestroyUnneededIslands()
    //{
    //    Stack<int> deleteStack = new Stack<int>();
    //    foreach (KeyValuePair<int, Island> item in islandDictionary)
    //    {
    //        if (item.Value.islandType == 0)
    //        {
    //            float randomNumber = Random.Range(0f, 1.0f);
    //            if (randomNumber < destructionLevel)
    //            {
    //                deleteStack.Push(item.Key);
    //            }
    //        }
    //    }
    //    while (deleteStack.Count > 0)
    //    {
    //        int key = deleteStack.Pop();
    //        foreach (int neighbor in islandDictionary[key].neighbors)
    //        {
    //            islandDictionary[neighbor].neighbors.Remove(key);
    //        }
    //        Destroy(islandDictionary[key].linkedGameObject);
    //        islandDictionary.Remove(key);
    //    }
    //}

    void SetupLayers()
    {
        foreach (Island island in islandDictionary.Values)
        {
            //we dont want to reset height of base or islands surrounding base
            if (island.islandType != IslandType.Bastion && island.neighbors.Contains(0) == false)
            {
                island.layer = (short)Random.Range(-1, 2);
                if (island.layer != 0)
                {
                    island.position += island.position.normalized * island.layer * layerHeightOffset;
                }
            }
        }
    }

    void SetupGrapplingIslands()
    {
        Stack<KeyValuePair<int, Island>> newIslandStack = new Stack<KeyValuePair<int, Island>>();
        foreach (KeyValuePair<int, Island> item in islandDictionary)
        {
            for (int i = 0; i < item.Value.neighbors.Count; i++)
            {
                if (item.Value.neighbors[i] > item.Key)
                {
                    Island island = item.Value;
                    Island neighbor = islandDictionary[item.Value.neighbors[i]];
                    Island[] grapplingIslands = new Island[2];

                    for (int j = 0; j < grapplingIslands.Length; j++)
                    {
                        grapplingIslands[j] = new Island();

                        //Set grappling island between big islands
                        Vector3 start = island.position + (neighbor.position - island.position).normalized * baseIslandSize;
                        Vector3 end = neighbor.position + (island.position - neighbor.position).normalized * baseIslandSize;
                        Vector3 newPos = (end + 0.333f * (j + 1) * (start - end)).normalized * radius;

                        //Set correct height of grappling islands
                        if (island.layer != neighbor.layer)
                        {
                            newPos = newPos.normalized * neighbor.position.magnitude + newPos.normalized * (island.position.magnitude - neighbor.position.magnitude) * 0.333f * (j + 1);
                        }
                        else
                        {
                            newPos += newPos.normalized * island.layer * layerHeightOffset;
                        }

                        grapplingIslands[j].position = newPos;
                        grapplingIslands[j].islandType = IslandType.Small;
                        string name = item.Key.ToString() + "99" + j.ToString() + "99" + island.neighbors[i].ToString();
                        newIslandStack.Push(new KeyValuePair<int, Island>(int.Parse(name), grapplingIslands[j]));
                    }

                    if (island.layer == neighbor.layer)
                    {
                        int x = Random.Range(0, 3);
                        if (x < 2)
                        {
                            grapplingIslands[x].position += grapplingIslands[x].position.normalized * grapplingIslandExtraheight;
                        }
                    }
                }
            }
        }
        while (newIslandStack.Count > 0)
        {
            KeyValuePair<int, Island> item = newIslandStack.Pop();
            islandDictionary.Add(item.Key, item.Value);
        }
    }

    private void InstantiateIslands()
    {
        foreach (KeyValuePair<int, Island> item in islandDictionary)
        {
            GameObject islandGameObject;
            switch (item.Value.islandType)
            {
                case IslandType.Bastion:
                    islandGameObject = Instantiate(LevelManager.Instance.islandBastion, item.Value.position, Quaternion.identity) as GameObject;
                    break;
                case IslandType.Artifact:
                    islandGameObject = Instantiate(LevelManager.Instance.islandArtifact, item.Value.position, Quaternion.identity) as GameObject;
                    break;
                case IslandType.Grappling:
                case IslandType.Small:
                    islandGameObject = Instantiate(LevelManager.Instance.islandSmall, item.Value.position, Quaternion.identity) as GameObject;
                    break;
                default:
                    if (Random.Range(0, 12) == 0)
                    {
                        islandGameObject = Instantiate(LevelManager.Instance.bigIslands[1], item.Value.position, Quaternion.identity) as GameObject;
                    }
                    else
                    {
                        islandGameObject = Instantiate(LevelManager.Instance.bigIslands[0], item.Value.position, Quaternion.identity) as GameObject;
                    }
                    break;
            }
            islandGameObject.name = item.Key.ToString() + item.Value.islandType.ToString();
            islandGameObject.transform.up = islandGameObject.transform.position;
            islandGameObject.transform.parent = islandParent;
        }
    }

    public Vector3 GetBasePosition()
    {
        Island bastion = islandDictionary[0];
        if (bastion.islandType != IslandType.Bastion)
        {
            Debug.LogError("Base island not found. Player spawn position may be wrong");
        }
        return islandDictionary[0].position + islandDictionary[0].position.normalized * 10;
    }

    struct TriangleIndices
    {
        public int[] v;

        public TriangleIndices(int v1, int v2, int v3)
        {
            v = new int[] { v1, v2, v3 };
        }
    }

    private void IcoSphere(float radius, int cycles, ref SortedDictionary<int, Island> islandDictionary)
    {
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
        for (int i = 0; i < cycles; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = GetMiddlePoint(tri.v[0], tri.v[1], ref verticesList, ref middlePointIndexCache, radius);
                int b = GetMiddlePoint(tri.v[1], tri.v[2], ref verticesList, ref middlePointIndexCache, radius);
                int c = GetMiddlePoint(tri.v[2], tri.v[0], ref verticesList, ref middlePointIndexCache, radius);

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
        for (int i = 0; i < faces.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Island island;
                if (islandDictionary.TryGetValue(faces[i].v[j], out island) == false)
                {
                    island = new Island();
                    island.position = verticesList[faces[i].v[j]];
                    islandDictionary.Add(faces[i].v[j], island);
                }
                for (int k = 0; k < 3; k++)
                {
                    if (k != j)
                    {
                        if (island.neighbors.Contains(faces[i].v[k]) == false)
                        {
                            island.neighbors.Add(faces[i].v[k]);
                        }
                    }
                }
            }
        }
    }

    //helper method for icosphere subdivision
    private int GetMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
    {
        //first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
        {
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

    private class Island
    {
        public IslandType islandType;
        public List<int> neighbors = new List<int>();
        public short layer;
        public Vector3 position;

    }

    public void DestroyLevel()
    {
        Destroy(islandParent.gameObject);
    }
}

