using UnityEngine;
using System.Collections;

public class SunLight : MonoBehaviour {
    public new Light light;

	void Start () {
        if (!light) {
            light = GetComponent<Light>();
        }
        transform.LookAt(Vector3.zero);
	}

    void OnDrawGizmos () {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, 100 * transform.forward);
        //Gizmos.DrawWireSphere(transform.position, 10);
    }
}
