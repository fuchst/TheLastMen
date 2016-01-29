using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class Level : MonoBehaviour
{
    public int Cycles { get; set; }
    public float Radius { get; set; }
    public int ArtifactCount { get; set; }
    public float DestructionLevel { get; set; }
    public float LayerHeightOffset { get; set; }

    [HideInInspector]
    public float grapplingIslandExtraheight;

    private enum IslandType { None, Bastion, Path, Artifact, Grappling, Small };
    private SortedDictionary<int, Island> islandDictionary = new SortedDictionary<int, Island>();
    private SortedDictionary<string, Island> grapplingIslandDictionary = new SortedDictionary<string, Island>();
    private int maxDistanceToBastion = 0;
    private bool creatingLevel = false;
    
    public void CreateLevel()
    {
        //Init camera rot
        LevelManager.Instance.worldCam.gameObject.transform.position = new Vector3(Radius * 2.0f, 0, 0);
        LevelManager.Instance.worldCam.gameObject.transform.LookAt(Vector3.zero);
        creatingLevel = true;
        StartCoroutine(CreateLevelOverTime());
        //IcoSphere(Radius, Cycles, ref islandDictionary);
        //SetupBastionAndDistances();
        //SetupArtifacts();
        //MarkArtifactPaths();
        //DestroyUnneededIslands();
        //SetupLayers();
        //SetupGrapplingIslands();
        //SyncFallingSpeedWithTimer();
        //InstantiateIslands();
    }

    IEnumerator CreateLevelOverTime()
    {
        yield return new WaitForSeconds(LevelManager.Instance.createLevelCoroutineCounter);
        LevelManager.Instance.showPaths = true;
        IcoSphere(Radius, Cycles, ref islandDictionary);
        SetupBastionAndDistances();
        SetupArtifacts();
        MarkArtifactPaths();

        yield return new WaitForSeconds(LevelManager.Instance.createLevelCoroutineCounter);
        DestroyUnneededIslands();

        yield return new WaitForSeconds(LevelManager.Instance.createLevelCoroutineCounter);
        SetupLayers();
        SetupGrapplingIslands();
        SyncFallingSpeedWithTimer();
        InstantiateBastionAndPlayer();

        yield return new WaitForSeconds(LevelManager.Instance.createLevelCoroutineCounter);
        InstantiateIslands();

        //LevelManager.Instance.StartLevel();
    }

    IEnumerator WaitSeconds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    void Update()
    {
        if (LevelManager.Instance.showPaths == true || creatingLevel == true)
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

    private void SetupBastionAndDistances()
    {
        Island island = islandDictionary[0];
        island.islandType = IslandType.Bastion;
        island.layer = 0;
        island.distanceToBastion = 0;

        //setup island distances to base
        Stack<Island> visitNext = new Stack<Island>();
        visitNext.Push(island);
        SetDistances(0, visitNext);

        //mark islands surrounding the base
        foreach (int key in island.neighbors)
        {
            Island neighbor = islandDictionary[key];
            neighbor.islandType = IslandType.Path;
            neighbor.layer = 0;
        }
    }

    private void SetDistances(int currentDistancce, Stack<Island> islands)
    {
        Stack<Island> visitNext = new Stack<Island>();
        while (islands.Count != 0)
        {
            Island island = islands.Pop();
            for (int i = 0; i < island.neighbors.Count; i++)
            {
                Island neighbor = islandDictionary[island.neighbors[i]];
                if (neighbor.distanceToBastion == -1)
                {
                    neighbor.distanceToBastion = currentDistancce + 1;
                    visitNext.Push(neighbor);
                }
            }
        }
        if (currentDistancce > maxDistanceToBastion)
        {
            maxDistanceToBastion = currentDistancce;
        }
        if (visitNext.Count > 0)
        {
            SetDistances(currentDistancce + 1, visitNext);
        }
    }

    private void SetupArtifacts()
    {
        for (int i = 0; i < ArtifactCount; i++)
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

    void DestroyUnneededIslands()
    {
        Stack<int> removeStack = new Stack<int>();
        foreach (KeyValuePair<int, Island> item in islandDictionary)
        {
            if (item.Value.islandType == 0)
            {
                float randomNumber = Random.Range(0f, 1.0f);
                if (randomNumber < DestructionLevel)
                {
                    removeStack.Push(item.Key);
                }
            }
        }
        while (removeStack.Count > 0)
        {
            int key = removeStack.Pop();
            foreach (int neighbor in islandDictionary[key].neighbors)
            {
                islandDictionary[neighbor].neighbors.Remove(key);
            }
            islandDictionary.Remove(key);
        }
    }

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
                    island.position += island.position.normalized * island.layer * LayerHeightOffset;
                }
            }
        }
    }

    void SetupGrapplingIslands()
    {
        Stack<KeyValuePair<string, Island>> newIslandStack = new Stack<KeyValuePair<string, Island>>();

        foreach (KeyValuePair<int, Island> item in islandDictionary)
        {
            for (int i = 0; i < item.Value.neighbors.Count; i++)
            {
                if (item.Value.neighbors[i] > item.Key)
                {
                    Island startIsland = item.Value;
                    Island endIsland = islandDictionary[item.Value.neighbors[i]];

                    int numberOfSmallIslands;
                    int distanceToBastion;
                    if (Mathf.Min(startIsland.distanceToBastion, endIsland.distanceToBastion) == 0)
                    {
                        distanceToBastion = 0;
                    }
                    else
                    {
                        distanceToBastion = Mathf.Max(startIsland.distanceToBastion, endIsland.distanceToBastion);
                    }

                    if (distanceToBastion <= 1)
                    {
                        numberOfSmallIslands = 3;   //Grappling islands close to bastion
                    }
                    else if (distanceToBastion == maxDistanceToBastion)
                    {
                        numberOfSmallIslands = 1;   //Island furthest away from bastion
                    }
                    else
                    {
                        numberOfSmallIslands = 2;   //All other islands
                    }
                    Island[] smallIslands = new Island[numberOfSmallIslands];

                    float startIslandWidth = LevelManager.Instance.islandPrefabs.bigIslandWidths[startIsland.prefabArrayPosition];
                    float endIslandWidth = LevelManager.Instance.islandPrefabs.bigIslandWidths[endIsland.prefabArrayPosition];

                    Vector3 start = startIsland.position + (endIsland.position - startIsland.position).normalized * startIslandWidth;
                    Vector3 end = endIsland.position + (startIsland.position - endIsland.position).normalized * endIslandWidth;
                    Vector3 direction = start - end;
                    float islandOffset = 1.0f / (smallIslands.Length + 1);

                    for (int j = 0; j < smallIslands.Length; j++)
                    {
                        Vector3 newPos = (end + islandOffset * (j + 1) * direction).normalized * Radius;

                        //Set correct height of grappling islands
                        if (startIsland.layer != endIsland.layer)
                        {
                            newPos = newPos.normalized * endIsland.position.magnitude + newPos.normalized * (startIsland.position.magnitude - endIsland.position.magnitude) * islandOffset * (j + 1);
                        }
                        else
                        {
                            newPos += newPos.normalized * startIsland.layer * LayerHeightOffset;
                        }

                        smallIslands[j] = new Island();
                        smallIslands[j].position = newPos;
                        smallIslands[j].islandType = IslandType.Grappling;
                        string name = item.Key.ToString() + " to " + startIsland.neighbors[i].ToString() + " " + j.ToString();
                        newIslandStack.Push(new KeyValuePair<string, Island>(name, smallIslands[j]));
                    }

                    if (startIsland.layer == endIsland.layer)
                    {
                        int x = Random.Range(0, numberOfSmallIslands);
                        smallIslands[x].position += smallIslands[x].position.normalized * grapplingIslandExtraheight;
                        smallIslands[x].islandType = IslandType.Small;
                    }
                }
            }
        }
        while (newIslandStack.Count > 0)
        {
            KeyValuePair<string, Island> item = newIslandStack.Pop();
            grapplingIslandDictionary.Add(item.Key, item.Value);
        }
    }

    private void SyncFallingSpeedWithTimer()
    {
        //Gather all the variables we will need
        Vector3 bastionPosition = islandDictionary[0].position;
        Vector3 blackHolePosition = LevelManager.Instance.BlackHole.transform.position;
        float blackHoleRadius = LevelManager.Instance.BlackHole.transform.localScale.x;
        float bastionRadius = LevelManager.Instance.islandPrefabs.bastionWidth; //Width is not correct but nobody got time for that
        float roundTime = s_GameManager.Instance.roundDuration;

        //Calc new fallingspeed
        float distance = Vector3.Distance(bastionPosition, blackHolePosition);
        distance -= bastionRadius + blackHoleRadius;
        float newFallingSpeed = distance / roundTime;

        //If new falling speed is too fast make it smaller and make black hole bigger
        if (newFallingSpeed > LevelManager.Instance.MaxFallingSpeed)
        {
            newFallingSpeed = LevelManager.Instance.MaxFallingSpeed;
            float newDistanceToBlackHole = newFallingSpeed * roundTime;
            float newRadius = blackHoleRadius + distance - newDistanceToBlackHole;
            Vector3 newScale = new Vector3(newRadius, newRadius, newRadius);
            LevelManager.Instance.BlackHole.transform.localScale = newScale;
        }
        LevelManager.Instance.IslandFallingSpeed = newFallingSpeed;
    }

    private void InstantiateBastionAndPlayer()
    {
        if (LevelManager.Instance.bastion == null)
        {
            //Bastion
            GameObject bastion = Instantiate(LevelManager.Instance.islandPrefabs.Bastion, islandDictionary[0].position, Quaternion.identity) as GameObject;
            LevelManager.Instance.bastion = bastion;
            bastion.name = "Bastion";
            bastion.transform.up = bastion.transform.position;
            bastion.transform.parent = LevelManager.Instance.islandParent;
            
            //Player
            Vector3 spawnPos = LevelManager.Instance.bastion.transform.FindChild("Spawn").transform.position;
            spawnPos += spawnPos.normalized;
            LevelManager.Instance.playerSpawnPos = spawnPos;
            LevelManager.Instance.player = Instantiate(LevelManager.Instance.playerPrefab, spawnPos, Quaternion.identity) as GameObject;
        }
        else //Bastion already exists
        {
            Bastion bastionScript = LevelManager.Instance.bastion.GetComponent<Bastion>();
            bastionScript.RebaseBastion(islandDictionary[0].position);
        }
    }

    private void InstantiateIslands()
    {
        LevelManager.IslandPrefabs islandPrefabs = LevelManager.Instance.islandPrefabs;
        int bigIsland = 0;
        foreach (KeyValuePair<int, Island> item in islandDictionary)
        {
            GameObject islandGameObject;
            switch (item.Value.islandType)
            {
                case IslandType.Bastion:
                    islandGameObject = null;
                    break;
                case IslandType.Artifact:
                    islandGameObject = Instantiate(islandPrefabs.AritfactIsland, item.Value.position, Quaternion.identity) as GameObject;
                    break;
                default:
                    islandGameObject = Instantiate(islandPrefabs.BigIslands[bigIsland], item.Value.position, Quaternion.identity) as GameObject;
                    bigIsland++;
                    bigIsland %= islandPrefabs.BigIslands.Length;
                    break;
            }
            if (islandGameObject != null)
            {
                islandGameObject.name = item.Key.ToString() + item.Value.islandType.ToString();
                islandGameObject.transform.up = islandGameObject.transform.position;
                islandGameObject.transform.parent = LevelManager.Instance.islandParent;
            }
        }
        int smallIsland = 0;
        foreach (KeyValuePair<string, Island> item in grapplingIslandDictionary)
        {
            GameObject islandGameObject;
            switch (item.Value.islandType)
            {
                case IslandType.Grappling:
                case IslandType.Small:
                //default:
                //    if (Random.Range(0, 12) == 0)
                //    {
                //        islandGameObject = Instantiate(LevelManager.Instance.bigIslands[1], item.Value.position, Quaternion.identity) as GameObject;
                //    }
                //    else
                //    {
                //        islandGameObject = Instantiate(LevelManager.Instance.bigIslands[0], item.Value.position, Quaternion.identity) as GameObject;
                //    }
                //    break;
                default:
                    islandGameObject = Instantiate(islandPrefabs.SmallIslands[smallIsland], item.Value.position, Quaternion.identity) as GameObject;
                    smallIsland++;
                    smallIsland %= islandPrefabs.SmallIslands.Length;
                    break;
            }
            islandGameObject.name = item.Key + item.Value.islandType.ToString();
            islandGameObject.transform.up = islandGameObject.transform.position;
            islandGameObject.transform.parent = LevelManager.Instance.islandParent;
        }
    }

    //public Vector3 GetBasePosition()
    //{
    //    return LevelManager.Instance.bastion.transform.FindChild("Spawn").transform.position;
    //}

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
                    island.islandType = IslandType.None;
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
        public int prefabArrayPosition = 0;
        public List<int> neighbors = new List<int>();
        public short layer;
        public Vector3 position;
        public int distanceToBastion = -1;
    }
}

