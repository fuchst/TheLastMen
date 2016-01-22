using UnityEngine;

public class Deathzone : MonoBehaviour {

	void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            s_GameManager.Instance.HurtPlayer(9001); //over 9000 damage will surely kill the player instantly ;)
        }
        if(collision.gameObject.CompareTag("Crystal"))
        {
            Debug.Log("Crystal destroyed");
            Destroy(collision.gameObject);
        }
    }
}
