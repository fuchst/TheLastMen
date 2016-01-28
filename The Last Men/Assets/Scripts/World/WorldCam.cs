using UnityEngine;
using System.Collections;

public class WorldCam : MonoBehaviour {

	[SerializeField] private Vector3 point = Vector3.zero;
	[SerializeField] private Vector3 axis = Vector3.forward;
	[SerializeField] private float angle = 0.2f;

	void Update () {
		transform.RotateAround (point, axis, angle);
	}
}
