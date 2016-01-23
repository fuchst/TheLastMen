using UnityEngine;

public class Bastion : MonoBehaviour
{
    public GameObject triggerObject;

    //[SerializeField] private float rebaseSpeed = 5.0f;
    //private new Rigidbody rigidbody;
    private Vector3 newPosition;

    //void Awake()
    //{
    //    rigidbody = GetComponent<Rigidbody>();
    //}

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

    public void RebaseBastion(Vector3 newPosition)
    {
        this.newPosition = newPosition;
        LevelManager.Instance.player.transform.SetParent(transform,true);
        transform.position = this.newPosition;
        LevelManager.Instance.player.transform.parent = null;
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
