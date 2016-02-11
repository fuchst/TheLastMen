using UnityEngine;
using System.Collections.Generic;

public abstract class s_Collectible : MonoBehaviour {

    public bool autoDestroyOnCollect = true;
    public float destructionDelay = 2.0f;
    public GameObject collectParticleEffect;
    protected new AudioSource audio;
    [SerializeField]protected List<AudioClip> collectSounds;

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")){
            //#if UNITY_EDITOR
            //    Debug.Log(this.GetType() + " on " + gameObject.name + " was collected.");
            //#endif
            Collect();
            if (autoDestroyOnCollect) {
                DestroyCollectible();
            }
        }
    }

    protected virtual void Collect () {
        //do the fancy stuff here
    }

    protected void DestroyCollectible () {
		GetComponent<Collider>().enabled = false;
        foreach(MeshRenderer r in GetComponentsInChildren<MeshRenderer>()){
			r.enabled = false;
		}
		Destroy(gameObject, destructionDelay);
    }
}
