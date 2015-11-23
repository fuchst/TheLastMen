using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float vel;
    Vector3 direction;
    public Vector3 gravity;

	// Use this for initialization
	void Start () {
        this.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
        direction = this.transform.forward * vel; 
	}
	
	// Update is called once per frame
	void Update () {
        direction += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;
	}
}
