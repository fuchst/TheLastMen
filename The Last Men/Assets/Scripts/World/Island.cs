using UnityEngine;

public class Island : MonoBehaviour {

    public int priority;
    new private Rigidbody rigidbody;
    
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

	void Update () {
        rigidbody.MovePosition(transform.position - transform.up * Time.deltaTime * LevelManager.Instance.islandFallingSpeed);
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
