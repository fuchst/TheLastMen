using UnityEngine;

public class DestroyTimer : MonoBehaviour {

	public float timerLengthInSeconds = 5;

	void Start () {
		Destroy(gameObject, timerLengthInSeconds);
	}
}
