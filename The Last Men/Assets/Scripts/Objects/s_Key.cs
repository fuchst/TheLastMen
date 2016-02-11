using UnityEngine;
using System.Collections;

public class s_Key : s_Collectible {

    void Awake () {
        audio = GetComponent<AudioSource>();
    }

	protected override void Collect () {
        if(collectSounds.Count > 0) {
            audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
            audio.Play();
        }
        s_GameManager.Instance.amountOfKeys++;
        s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Key);
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Key);
    }
}
