﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class s_GenerateIslands : MonoBehaviour {

	public static s_GenerateIslands instance;

	public Transform floatingIslandParent;

	//islands
	public List<GameObject> prefabs = new List<GameObject>();
	public Vector2 heightMinMax = new Vector2(-2, 10);
	public float radius = 100;
	public int amount = 25;
	public Vector2 sizeMinMax = new Vector2(0.25f, 2.5f);

	protected List<GameObject> spawnedIslands = new List<GameObject>();

	//grid


	void Awake () {
		if (!instance) {
			instance = this;
		} else {
			Destroy(this);
		}
	}

	void Start () {
		floatingIslandParent = floatingIslandParent ? floatingIslandParent : transform;
		InitializeGrid ();
		SpawnIslands ();
	}

	void InitializeGrid () {

	}

	void SpawnIslands () {
		for (int i = 0; i < amount; i++) {
			Vector3 tmpPos = new Vector3(Random.Range(-radius, radius), Random.Range(heightMinMax.x, heightMinMax.y), Random.Range(-radius, radius));
			Quaternion tmpRot = Quaternion.AngleAxis(Random.value*360, Vector3.up);
			GameObject tmpObj = Instantiate(prefabs[Random.Range(0, prefabs.Count)], tmpPos, tmpRot) as GameObject;
			float scalingFactor = Random.Range(sizeMinMax.x, sizeMinMax.y);
			tmpObj.transform.localScale *= scalingFactor;
			tmpObj.GetComponent<Rigidbody>().mass = 1000 * Mathf.Pow(scalingFactor, 3);
			tmpObj.transform.SetParent(floatingIslandParent);
			tmpObj.name += " " + i;
			spawnedIslands.Add(tmpObj);
		}
	}

	public void MergeIslands (GameObject island1, GameObject island2) {
		if (island1 && island2 && island1.activeSelf && island2.activeSelf) {
			Rigidbody rb1, rb2, rb3;
			rb1 = island1.GetComponent<Rigidbody> ();
			rb2 = island2.GetComponent<Rigidbody> ();
			island1.SetActive (false);
			island2.SetActive (false);
			
			Vector3 tmpPos = 0.5f*(island1.transform.position + island2.transform.position);
			Quaternion tmpRot = Quaternion.AngleAxis(Random.value*360, Vector3.up);
			GameObject tmpObj = Instantiate(prefabs[Random.Range(0, prefabs.Count)], tmpPos, tmpRot) as GameObject;
			rb3 = tmpObj.GetComponent<Rigidbody> ();
			rb3.mass = rb1.mass + rb2.mass;
			tmpObj.transform.localScale *= Mathf.Pow(rb3.mass/1000f, 1f/3f);
			tmpObj.transform.SetParent(floatingIslandParent);
			tmpObj.name = island1.name + " + " + island2.name;
			spawnedIslands.Add(tmpObj);

			spawnedIslands.Remove (island1);
			spawnedIslands.Remove (island2);
			Destroy (island1);
			Destroy (island2);

			Debug.Log ("merged");
		}

	}
}