using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class s_GenerateIslandsSpherical : MonoBehaviour {

	public static s_GenerateIslandsSpherical instance;

	public Transform floatingIslandParent;

	//islands
	public List<GameObject> prefabsRegular = new List<GameObject>();
	public List<GameObject> prefabsSpecial = new List<GameObject>();
	public float radius = 100;
	public int amount = 25;
	public float specialPercentage = 0.05f;
	public Vector2 sizeMinMax = new Vector2(0.25f, 2.5f);

	protected List<GameObject> spawnedIslands = new List<GameObject>();


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
			Vector3 tmpPos = Random.insideUnitSphere * radius;
			Quaternion tmpRot = Quaternion.AngleAxis(Random.value*360, Vector3.up);
			bool selectSpecial = Random.value < specialPercentage;
			GameObject selectedPrefab = (selectSpecial ? prefabsSpecial[Random.Range(0, prefabsSpecial.Count)] : prefabsRegular[Random.Range(0, prefabsRegular.Count)]);
			GameObject tmpObj = Instantiate(selectedPrefab, tmpPos, tmpRot) as GameObject;
			tmpObj.transform.up = tmpPos;
			float scalingFactor = selectSpecial ? Random.Range(1, 2) : Random.Range(sizeMinMax.x, sizeMinMax.y);
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
			GameObject tmpObj = Instantiate(prefabsRegular[Random.Range(0, prefabsRegular.Count)], tmpPos, tmpRot) as GameObject;
			tmpObj.transform.up = tmpPos;
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
