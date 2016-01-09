using UnityEngine;
using System.Collections;

public class CollectableSpawner : MonoBehaviour {

    public GameObject[] CollectablePrefabs;
    public Vector3[] spawnLocations;
    
    // Use this for initialization
	void Start () {
        SpawnEnergy();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SpawnEnergy()
    {
        for(int i=0;i < spawnLocations.Length; i++)
        {
            float val = Random.value;
            if(val >= 0.5)
            {
                
                int pIdx = Random.Range(0, CollectablePrefabs.Length - 1);
                GameObject go = Object.Instantiate(CollectablePrefabs[pIdx], new Vector3(0,0,0), Quaternion.identity) as GameObject;
                go.transform.parent = transform;
                go.transform.localPosition = spawnLocations[i];
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = new Vector3(10, 10, 10);
            }
        }
    }
}
