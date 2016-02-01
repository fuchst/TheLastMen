using UnityEngine;
using System.Collections.Generic;

public class s_WoodenLog : s_Collectible {
    protected new AudioSource audio;
    [SerializeField]protected List<AudioClip> collectSounds;

    void Awake () {
        autoDestroyOnCollect = false;
        audio = GetComponent<AudioSource>();
        audio.pitch = 0.75f;
    }

	protected override void Collect () {
        if(s_GameManager.Instance.woodPlayer_Cur < s_GameManager.Instance.woodPlayer_Max) {
            s_GameManager.Instance.woodPlayer_Cur++;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Wood);
            s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Wood);
            if (collectSounds.Count > 0) {
                audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
                audio.Play();
            }
            DestroyCollectible();
        }
    }
}
