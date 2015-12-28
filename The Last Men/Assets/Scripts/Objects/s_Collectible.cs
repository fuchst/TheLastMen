using UnityEngine;
using System.Collections;

public abstract class s_Collectible : MonoBehaviour {

    public float destructionDelay = 2.0f;

	void OnTriggerEnter (Collider other) {
        if (other.tag.Equals("Player")){
            #if UNITY_EDITOR
                Debug.Log(this.GetType() + " on " + gameObject.name + "was collected.");
            #endif
            Collect();
            DestroyCollectible();
        }
    }

    protected virtual void Collect () {
        //do the fance stuff here
    }

    protected void DestroyCollectible () {
        foreach(MeshRenderer r in GetComponentsInChildren<MeshRenderer>()){
			r.enabled = false;
		}
		Destroy(gameObject, destructionDelay);
    }
}
