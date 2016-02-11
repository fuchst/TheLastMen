using UnityEngine;

public class Deathzone : MonoBehaviour {
    public LayerMask collectibleLayer;

	/*void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            s_GameManager.Instance.HurtPlayer(9001); //over 9000 damage will surely kill the player instantly ;)
        }
        if(collision.gameObject.CompareTag("Crystal"))
        {
            Debug.Log("Crystal destroyed");
            Destroy(collision.gameObject);
        }
    }*/

    void OnTriggerEnter (Collider other) {
        if(other.CompareTag("Player")) {
            s_GameManager.Instance.HurtPlayer(9001); //over 9000 damage will surely kill the player instantly ;)
        }
        else if(other.gameObject.layer == collectibleLayer.value) {
            Destroy(other.gameObject);
        }
    }
}
