using UnityEngine;
using System.Collections;

public class EnemySense : MonoBehaviour {

    // Check if player is inside sensing range
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Trigger entered");
            this.transform.parent.SendMessage("ChangeState", EnemyState.stateIDs.Search);
        }
    }

    // Register player leaving sensing range
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Trigger exited");
            this.transform.parent.SendMessage("ChangeState", EnemyState.stateIDs.Idle);
        }
    }
}

