using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    public Transform player;
    public FireGrapplingHook fireGrapplingHook;
    
    private bool flying = true;
    private LineRenderer lineRenderer;
    new private Rigidbody rigidbody;
    

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Update line lenderer
        lineRenderer.SetPosition(0, player.position);
        lineRenderer.SetPosition(1, transform.position);
        float distance = Vector3.Distance(player.position, transform.position);
        lineRenderer.material.mainTextureScale = new Vector2(distance, 1);

        //Check distance to player
        if(distance > fireGrapplingHook.maxRopeLength)
        {
            fireGrapplingHook.RemoveRope();
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (flying == true)
        {
            rigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * fireGrapplingHook.hookSpeed);
        }
    }

    void OnCollisionEnter (Collision col) {
        if (flying == true) {
            flying = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
            fireGrapplingHook.SetRope();
        }
    }
}
