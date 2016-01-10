using UnityEngine;

public class Deathzone : MonoBehaviour {

	void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Application.LoadLevel("MainMenuDUmmy");
        }
    }
}
