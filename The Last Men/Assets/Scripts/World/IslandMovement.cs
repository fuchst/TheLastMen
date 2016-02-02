using UnityEngine;

public class IslandMovement : MonoBehaviour
{
    public float fallingSpeed = 0;
    public int priority = 0;
    private float extraSpeedFactorOnCollision = 5.0f;
    new private Rigidbody rigidbody;

    void Awake()
    {
        if (LevelManager.Instance == true)
        {
            fallingSpeed = LevelManager.Instance.IslandFallingSpeed;
        }
        rigidbody = GetComponentInParent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (LevelManager.Instance.gameState == LevelManager.GameState.Playing)
        {
            rigidbody.MovePosition(transform.position - transform.up * Time.deltaTime * fallingSpeed);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("IslandCollider"))
        {
            string tag = collider.gameObject.tag;
            switch (tag)
            {
                case "IslandCollision":
                    //Debug.Log(this.gameObject.name + "Collided with" + collider.gameObject.transform.parent.gameObject.name);
                    IslandMovement islandMovement = collider.transform.parent.GetComponent<IslandMovement>();
                    if (islandMovement != null && islandMovement.priority >= priority)
                    {
                        fallingSpeed *= extraSpeedFactorOnCollision;
                    }
                    break;
                case "Deathzone":
                    //NiceToHave: add fancy explosion effects here
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
