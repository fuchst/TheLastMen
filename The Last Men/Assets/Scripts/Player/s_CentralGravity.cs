using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class s_CentralGravity : MonoBehaviour {
	private Rigidbody rb;
	private float gravityMagnitude;
	public float factor = 1;
    public float startDelay = 0;

	void Start () {
		rb = GetComponent<Rigidbody> ();
        if (!rb) {
            enabled = false;
        }
        gravityMagnitude = Physics.gravity.magnitude;

        if (startDelay > 0) {
            StartCoroutine(DelayedStart());
        }
	}

    IEnumerator DelayedStart () {
        rb.isKinematic = true;
        yield return new WaitForSeconds(startDelay);
        rb.isKinematic = false;
    }

    void FixedUpdate () {
        rb.AddForce (-rb.position.normalized * gravityMagnitude * factor, ForceMode.Acceleration);
	}
}
