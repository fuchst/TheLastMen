using UnityEngine;

public class FlyingEnemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Animation>().Play();

        this.transform.Translate(-transform.forward * 6.0f * Time.deltaTime);
	}
}
