using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    public Transform player;
    public FireGrapplingHook fireGrapplingHook;

    private Transform rope;
    private Material ropeMaterial;
    private bool flying = true;
    new private Rigidbody rigidbody;

    void Awake()
    {
        rope = transform.GetChild(0);
        ropeMaterial = rope.GetComponent<MeshRenderer>().material;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        //Check distance to player
        if(distance > fireGrapplingHook.hookLength)
        {
            fireGrapplingHook.Unfire();
            Destroy(gameObject);
        }

        //Translate and scale rope
        Vector3 startPos = player.position + player.right * 0.3f;
        rope.position = transform.position + 0.5f * (startPos - transform.position);
        rope.localScale = new Vector3(rope.localScale.x, distance, rope.localScale.z);
        rope.up = (startPos - transform.position).normalized;
        ropeMaterial.mainTextureScale = new Vector2(1.0f, distance * 2.0f);
    }

    void FixedUpdate()
    {
        if (flying == true)
        {
            rigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * fireGrapplingHook.hookSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (flying == true)
        {
            flying = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
            transform.parent = other.transform;
            player.GetComponent<FireGrapplingHook>().SetRope();
        }
    }
}
