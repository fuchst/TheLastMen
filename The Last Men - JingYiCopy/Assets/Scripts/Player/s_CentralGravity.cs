using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class s_CentralGravity : MonoBehaviour {
	private Rigidbody rb;
	private float gravityMagnitude;
	public float factor = 1;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		gravityMagnitude = Physics.gravity.magnitude;
	}

	void FixedUpdate () {
		rb.AddForce (-rb.position.normalized * gravityMagnitude * factor, ForceMode.Acceleration);
	}
}
