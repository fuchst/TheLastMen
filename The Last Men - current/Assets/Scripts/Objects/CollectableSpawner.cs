using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectableSpawner : MonoBehaviour {

    public GameObject[] CollectablePrefabs;
    public List<Transform> spawnLocations;
    
    void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "EnergySpawn")
            {
                spawnLocations.Add(child.transform);
            }
        }
    }
    
    // Use this for initialization
	void Start () {
        SpawnEnergy();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SpawnEnergy()
    {
        for(int i=0;i < spawnLocations.Count; i++)
        {
            float val = Random.value;
            if(val >= 0.5)
            {
                
                int pIdx = Random.Range(0, CollectablePrefabs.Length - 1);
                GameObject go = Object.Instantiate(CollectablePrefabs[pIdx], new Vector3(0,0,0), Quaternion.identity) as GameObject;
                go.transform.parent = spawnLocations[i];
                go.transform.localPosition = new Vector3(0, 0, 0);
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = new Vector3(10, 10, 10);
            }
        }
    }
}
