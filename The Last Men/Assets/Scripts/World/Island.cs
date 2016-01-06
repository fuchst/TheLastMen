using UnityEngine;

public class Island : MonoBehaviour {

    public int priority;

    private float fallingSpeed;
    new private Rigidbody rigidbody;
    
    void Awake()
    {
        fallingSpeed = LevelManager.Instance.islandFallingSpeed;
        rigidbody = GetComponent<Rigidbody>();
    }

	void Update () {
        rigidbody.MovePosition(transform.position - transform.up * Time.deltaTime * fallingSpeed);
	}

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        switch (tag)
        {
            case "Island":
                Island islandScript = collision.gameObject.GetComponent<Island>();
                if(islandScript.priority > priority)
                {
                    fallingSpeed *= 5.0f;
                }
                break;
            case "Deathzone":
                //Feature: add fancy explosion effects here
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
