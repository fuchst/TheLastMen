using UnityEngine;
using System.Collections;

public class s_SetStartVel : MonoBehaviour {

	public Vector3 initialVelocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody> ().velocity = initialVelocity;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
