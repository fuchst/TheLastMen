using UnityEngine;

public class Bastion : MonoBehaviour
{
    private new Rigidbody rigidbody;
    public GameObject triggerObject;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start ()
    {
        s_GUIMain.Instance.bastionTransform = triggerObject.transform;
    }

    /*public void PlayerLanded()
    {
        if (Mathf.Max(s_GameManager.Instance.artifact1CountCur, s_GameManager.Instance.artifact2CountCur) == s_GameManager.Instance.artifactCountMax)
        {
            LevelManager.Instance.AdvanceLevel();
        }
    }*/

    void Update()
    {
        rigidbody.MovePosition(transform.position - transform.up * Time.deltaTime * LevelManager.Instance.islandFallingSpeed);
    }

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")) {
            s_GameManager.Instance.playerInBastion = true;
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.CompareTag("Player")) {
            s_GameManager.Instance.playerInBastion = false;
        }
    }
}
