using UnityEngine;

public class Bastion : MonoBehaviour
{
    public GameObject triggerObject;

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
