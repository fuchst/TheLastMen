using UnityEngine;

public class IslandMovement : MonoBehaviour
{
    public float fallingSpeed = 0;
    public int priority = 0;
    [SerializeField]protected Collider islandTriggerCollider;
    [SerializeField]protected GameObject destructionEffectPrefab;
    private float extraSpeedFactorOnCollision = 25.0f;
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
            switch (collider.tag) {
                case "IslandCollision":
                    //Debug.Log(this.gameObject.name + "Collided with" + collider.gameObject.transform.parent.gameObject.name);
                    IslandMovement islandMovement = collider.transform.parent.GetComponent<IslandMovement>();
                    if (islandMovement != null && islandMovement.priority >= priority)
                    {
                        //fallingSpeed *= extraSpeedFactorOnCollision;
                        rigidbody.isKinematic = false;
                        gameObject.AddComponent<s_CentralGravity>().factor = 0.5f;
                        /*foreach (Transform child in transform) {
                            Collider[] cols = child.GetComponents<Collider>();
                            foreach (Collider col in cols) {
                                col.enabled = false;
                            }
                        }

                        islandTriggerCollider.enabled = true;*/
                        islandTriggerCollider.tag = "Untagged";

                        //Debug.Log("ddd");
                        //Debug.Break();
                        Instantiate(destructionEffectPrefab, transform.position, transform.rotation);

                        enabled = false;
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
