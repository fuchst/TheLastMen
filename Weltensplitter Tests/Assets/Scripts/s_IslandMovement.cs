using UnityEngine;
using System.Collections;

public class s_IslandMovement : MonoBehaviour {

	public float movementAmplitudeY = 1;
	public float movementPeriodLength = 10;
	public float angularAlignSpeedDegree = 2;

	protected Rigidbody rb;
	protected Vector3 posStart;
	protected Vector3 posCur;

	protected Vector3 posTop;
	protected Vector3 posBottom;
	protected Vector3 localUp;
	protected Quaternion originalRot;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		posStart = rb.position;
		originalRot = rb.rotation;
		localUp = transform.up;
		posTop = posStart + 0.5f * movementAmplitudeY * localUp;
		posBottom = posStart - 0.5f * movementAmplitudeY * localUp;
		movementPeriodLength *= Random.Range (0.75f, 1.5f);
	}

	void FixedUpdate () {
		//float factor = (0.5f - Mathf.PingPong (0.5f + 2*Time.time / movementPeriodLength, 1.0f)); //ping-pongs between 0.5 and -0.5
		//posCur = posStart + Vector3.up * movementAmplitudeY * factor;
		posCur = Vector3.Lerp (posTop, posBottom, 0.5f* (1 + Mathf.Sin(2*Mathf.PI/movementPeriodLength * Time.time)));
		rb.MovePosition (posCur);
		//Quaternion newRot = Quaternion.Slerp (rb.rotation, Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0), 0.2f*Time.fixedDeltaTime);
		Quaternion newRot = Quaternion.Slerp (rb.rotation, originalRot, 0.2f*Time.fixedDeltaTime);
		rb.MoveRotation (newRot);
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		if (rb) {
			Gizmos.DrawSphere (posStart, 1);
			Gizmos.DrawSphere (posCur, 1);
		}
	}

	void OnCollisionEnter (Collision col) {
		if(col.gameObject.tag.Equals("Island")){
			if(s_GenerateIslands.instance){
				s_GenerateIslands.instance.MergeIslands(gameObject, col.gameObject);
			}
			else if(s_GenerateIslandsSpherical.instance){
				s_GenerateIslandsSpherical.instance.MergeIslands(gameObject, col.gameObject);
			}
		}
	}

}
