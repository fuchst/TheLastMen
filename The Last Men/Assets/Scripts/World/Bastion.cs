using UnityEngine;

public class Bastion : MonoBehaviour
{
    public void PlayerLanded()
    {
        if (s_GameManager.Instance.artifactCountCur == s_GameManager.Instance.artifactCountMax)
        {
            LevelManager.Instance.AdvanceLevel();
        }
    }
}
