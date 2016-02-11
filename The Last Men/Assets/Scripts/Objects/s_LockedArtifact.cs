using UnityEngine;
using System.Collections;

public class s_LockedArtifact : s_Artifact {

    protected bool unlocked = false;

	// Use this for initialization
	void Awake () {
        autoDestroyOnCollect = false;
        audio = GetComponent<AudioSource>();
    }
	
    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")){
            if (s_GameManager.Instance.amountOfKeys > 0) {
                s_GameManager.Instance.amountOfKeys--;
                s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Key);
                unlocked = true;
            }
            //#if UNITY_EDITOR
            //    Debug.Log(this.GetType() + " on " + gameObject.name + " was collected.");
            //#endif
            Collect();
            if (autoDestroyOnCollect) {
                DestroyCollectible();
            }
        }
    }

	protected override void Collect () {
        if (unlocked) {
            if(collectSounds.Count > 0) {
                audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
                audio.Play();
            }
            artifactType = Random.Range(1, 3); //3 is exclusive to the range for int!
            if (artifactType == 1) {
                s_GameManager.Instance.artifact1CountCur++;
                s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Artifact1);
            }
            else if (artifactType == 2) {
                s_GameManager.Instance.artifact2CountCur++;
                s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Artifact2);
            }
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Artifact);
            DestroyCollectible();
        }
    }
}
