using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class s_IslandGravity : MonoBehaviour {

	public float gravityRange = 5;
	public float gravityConstant = 1;

	protected Rigidbody rb;
	//protected List<GameObject> gravityAffectedObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		gravityRange *= transform.localScale.x;
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		if (rb) {
			Gizmos.DrawSphere (rb.worldCenterOfMass, 1);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (rb.worldCenterOfMass, gravityRange);
		}
	}

	void FixedUpdate () {
		Collider[] objectsInRange = Physics.OverlapSphere (rb.worldCenterOfMass, gravityRange, LayerMask.GetMask(new string[]{"IslandGravityAffected"}));
		foreach(Collider c in objectsInRange){
			if(!c.attachedRigidbody){
				continue;
			}
			Vector3 dir = transform.position - c.transform.position;
			c.attachedRigidbody.AddForce(Time.fixedDeltaTime * gravityConstant * rb.mass * dir.normalized/dir.sqrMagnitude, ForceMode.Acceleration);
		}
	}
}
