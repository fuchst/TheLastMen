using UnityEngine;

public class s_Hook : MonoBehaviour
{
    public Vector3 direction;
    private Rigidbody rb;
    private GameObject go;
    public SpringJoint spring;
    private float speed;
    public bool flying;
    public Rigidbody parentRB;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        flying = true;
    }

    void Update()
    {
        // Debug.Log("hookFlying: " + flying);
        if (flying)
        {
            float way = speed * Time.deltaTime;
            transform.forward = direction;
            transform.Translate(Vector3.forward * way);
        }
        //else
        //{
        //    Vector3 v1 = transform.position;
        //    Vector3 v2 = parentRB.transform.position;
        //    //Debug.Log("Distance: " +  Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.x - v2.x), 2)) + 5);
        //}
        // lr.SetPosition(0, transform.position);
        // lr.SetPosition(1, parentRB.transform.position);
        //pendulum.anchor = transform.position;
        //if (CrossPlatformInputManager.GetButtonDown("Hook") && !flying) {
        //    // Destroy(spring);
        //    // lr.enabled = false;
        //}

    }

    /*void FixedUpdate () {
        if (flying)
        {
            rb.transform.Translate(direction * Time.deltaTime*10);
            Debug.Log("flying forward");
        }
	}*/

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag.Equals("Island"))
        {
            if (flying)
            {
                flying = false;
                //Debug.Log("Hit something");
                rb.velocity = new Vector3(0, 0, 0);
                rb.angularVelocity = new Vector3(0, 0, 0);
                transform.parent = col.transform;
                rb.isKinematic = true;
                // spring.spring = 10;
                //spawn Spring joint between collision point and parentRB (player)
                // SpringJoint spring = Instantiate(new SpringJoint(), transform.position, transform.rotation) as SpringJoint;
                //  spring.gameObject = GetComponent<GameObject>();
                // SpringJoint spring = go.AddComponent<SpringJoint>();
                /* spring.connectedBody = parentRB;
                 spring.anchor = new Vector3(0, 0, 0); //transform.position;
                 /*pendulum.connectedBody = parentRB;
                 pendulum.anchor = new Vector3(0,0,0);
                 pendulum.axis = rb.transform.right;*/
                /*Vector3 v1 = transform.position;
                Vector3 v2 = parentRB.transform.position;
                spring.maxDistance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.x - v2.x), 2)) - 5;*/
                // spring.minDistance = 0F;
                // spring.
                //render line
                // lr.enabled = true;
                // Debug.Log("Position:" + transform.position);
                // Destroy(rb);
            }
        }
    }

    public float Speed
    {
        set
        {
            speed = value;
        }
    }
}
