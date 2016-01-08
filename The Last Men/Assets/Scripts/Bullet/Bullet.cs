using UnityEngine;

public class Bullet : MonoBehaviour {

    public float gravity { get; set; }
    public float damage { get; set; }

    public float destDistMax = 1000.0f;
    public float destDistMin = 10.0f;

    public float ttl = 15.0f;

    public GameObject bulletHole;

    // Update is called once per frame
    void FixedUpdate () {
        Vector3 gravityDir = -transform.position.normalized;

        GetComponent<Rigidbody>().AddForce(gravityDir * gravity, ForceMode.Acceleration);

        ttl -= Time.deltaTime;

        if(ttl <= 0.0f)
        {
            Destroy(gameObject);
        }

        if(!(transform.position.magnitude < destDistMax && transform.position.magnitude > destDistMin))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        // Debug.Log("Hit" + other.tag);

        if(coll.transform.tag != "Bullet")
        {
            if(coll.transform.tag == "Enemy")
            {
                coll.transform.SendMessage("OnHit", damage);
            }
            else if(coll.transform.tag == "Island")
            {
                GameObject bullet = Instantiate(bulletHole, coll.contacts[0].point + coll.contacts[0].normal * 0.05f, Quaternion.FromToRotation(Vector3.up, coll.contacts[0].normal)) as GameObject;
                bullet.transform.SetParent(coll.transform);
            }

            Destroy(gameObject);
        }
    }
}
