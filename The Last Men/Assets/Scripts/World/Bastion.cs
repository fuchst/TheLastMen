using UnityEngine;

public class Bastion : MonoBehaviour
{
    private new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void PlayerLanded()
    {
        if (Mathf.Max(s_GameManager.Instance.artifact1CountCur, s_GameManager.Instance.artifact2CountCur) == s_GameManager.Instance.artifactCountMax)
        {
            LevelManager.Instance.AdvanceLevel();
        }
    }

    void Update()
    {
        rigidbody.MovePosition(transform.position - transform.up * Time.deltaTime * LevelManager.Instance.islandFallingSpeed);
    }
}
