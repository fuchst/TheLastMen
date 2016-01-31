using UnityEngine;
using System.Collections;

public class s_HealingPlant : s_Collectible {

	[Range(2, 5)] public int healAmount;

    void Awake () {
        autoDestroyOnCollect = false;
    }

    protected override void Collect () {
        if(s_GameManager.Instance.healthpointsCur < s_GameManager.Instance.healthpointsMax) {
            s_GameManager.Instance.HealPlayer(healAmount);
            s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Healing);
            if (collectParticleEffect) {
                Instantiate(collectParticleEffect, transform.position, transform.rotation);
            }
            DestroyCollectible();
        }
    }
}
