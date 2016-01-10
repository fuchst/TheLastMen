using UnityEngine;
using System.Collections;

public class Bullethole : MonoBehaviour {

    public float ttl = 10.0f;
    private float startttl;
    private Vector3 startScale;
    	
    void Start ()
    {
        startttl = ttl;
        startScale = transform.localScale;
    }

	// Update is called once per frame
	void Update ()
    {
        ttl -= Time.deltaTime;

        transform.localScale = startScale * (ttl / startttl);

        if (ttl < 0)
            Destroy(gameObject);
	}
}
