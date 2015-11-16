using UnityEngine;
using System.Collections;

public class s_SimpleJumpRun : MonoBehaviour {

	protected Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Vector3 newPos = rb.position + Time.deltaTime * new Vector3 ();
		//rb.MovePosition (newPos);
		Vector3 dir = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
		rb.AddForce (100*(10 * Time.fixedDeltaTime * dir + transform.up * (Input.GetKeyDown(KeyCode.Space) ? 1 : 0)), ForceMode.Force);
	}
}
