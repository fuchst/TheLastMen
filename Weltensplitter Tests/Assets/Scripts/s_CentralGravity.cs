using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class s_CentralGravity : MonoBehaviour {
	private Rigidbody rb;
	private float gravityMagnitude;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		gravityMagnitude = Physics.gravity.magnitude;
	}

	void FixedUpdate () {
		rb.AddForce (-rb.position.normalized * gravityMagnitude, ForceMode.Acceleration);
	}
}
