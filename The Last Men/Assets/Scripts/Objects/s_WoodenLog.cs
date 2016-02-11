using UnityEngine;
using System.Collections.Generic;

public class s_WoodenLog : s_Collectible {
    
    void Awake () {
        autoDestroyOnCollect = false;
        audio = GetComponent<AudioSource>();
    }

	protected override void Collect () {
        if(s_GameManager.Instance.woodPlayer_Cur < s_GameManager.Instance.WoodPlayerMax) {
            s_GameManager.Instance.woodPlayer_Cur++;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Wood);
            s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Wood);
            if (collectSounds.Count > 0) {
                audio.pitch = 0.75f;
                audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
                audio.Play();
            }
            DestroyCollectible();
        }
    }
}
