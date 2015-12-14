using UnityEngine;

public class Bastion : MonoBehaviour
{

    public void PlayerLanded()
    {
        if (s_GameManager.Instance.artifactCountCur == s_GameManager.Instance.artifactCountMax)
        {
            Debug.Log("Advance to next Level");
            LevelManager.Instance.AdvanceLevel();
        }
        else
        {
            Debug.Log("Not enough artifacts");
        }
    }
}
