using UnityEngine;
using System.Collections;

public class PlaceFlyingEnemy : MonoBehaviour {

    GameObject prefab;

    void Awake()
    {
        prefab = Resources.Load("FlyingEnemy") as GameObject;
    }

	// Use this for initialization
	void Start ()
    {
        Vector3 spawnpoint = transform.position + transform.up * 10.0f;
        Instantiate(prefab, spawnpoint , transform.rotation);
	}
}
