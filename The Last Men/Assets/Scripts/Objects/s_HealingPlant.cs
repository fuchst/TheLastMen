using UnityEngine;
using System.Collections;

public class s_HealingPlant : s_Collectible {

	[Range(2, 5)] public int healAmount;

    void Awake () {
        autoDestroyOnCollect = false;
        audio = GetComponent<AudioSource>();
    }

    protected override void Collect () {
        if(s_GameManager.Instance.healthpointsCur < s_GameManager.Instance.healthpointsMax) {
            s_GameManager.Instance.HealPlayer(healAmount);
            s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Healing);
            if (collectParticleEffect) {
                Instantiate(collectParticleEffect, transform.position, transform.rotation);
            }
            if (collectSounds.Count > 0) {
                audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
                audio.Play();
            }
            DestroyCollectible();
        }
    }
}
