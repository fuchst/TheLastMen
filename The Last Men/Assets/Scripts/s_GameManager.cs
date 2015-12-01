using UnityEngine;
using System.Collections;

public class s_GameManager : MonoBehaviour {

	private static s_GameManager instance;

	public static s_GameManager Instance { get { return instance; } }

	void Awake () {
		if (instance) {
			Destroy(this);
		} else {
			instance = this;
		}
	}

	public float roundDuration = 300.0f;
	public float endTime;

	void Start () {
		endTime = Time.time + roundDuration;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
