using UnityEngine;
using System.Collections;

public class BastionDoorTrigger : MonoBehaviour {

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Enemy")) {
            //getting killed by crazy flying enemies within your own base feels terribly creepy... so we will kill them first! 
            other.GetComponent<Enemy>().OnHit(9001);
        }
    }
}
