using UnityEngine;
using System.Collections;

public class Bullet_backup : MonoBehaviour {

    public float gravity { get; set; }

    public float destDistMax = 1000.0f;
    public float destDistMin = 10.0f;
    public int damage;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 gravityDir = -this.transform.position.normalized;

        this.GetComponent<Rigidbody>().AddForce(gravityDir * gravity, ForceMode.Acceleration);

        if(!(transform.position.magnitude < destDistMax && transform.position.magnitude > destDistMin))
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Hit" + other.tag);

        if(other.tag == "Enemy")
        {
            other.transform.SendMessage("OnHit", damage);
        }

        Destroy(this.gameObject);
    }
}
