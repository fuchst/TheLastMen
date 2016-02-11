using UnityEngine;
using System.Collections.Generic;

public class s_EnergyCrystal : s_Collectible {

	public float energyLootMin = 3;
    public float energyLootMax = 7;
    protected float energyLootCur;

    void Awake () {
        autoDestroyOnCollect = false;
        energyLootCur = RandomFromDistribution.RandomRangeNormalDistribution(energyLootMin, energyLootMax, RandomFromDistribution.ConfidenceLevel_e._99);
        transform.localScale *= 1 + 0.15f * (energyLootCur - (energyLootMin + energyLootMax)/2);
        audio = GetComponent<AudioSource>();
        //audio.volume = 0.5f;
    }

    protected override void Collect () {
        s_GameManager game = s_GameManager.Instance;
        s_GameManager.UpgradeSettings.UpgradeObject harvest = game.upgradeSettings.upgrades[s_GameManager.UpgradeSettings.UpgradeTypes.ResourceHarvesting];
        energyLootCur *= (1 + harvest.progress_cur * harvest.stepSize);
        float maxEnergy = game.EnergyPlayerMax;
        if(game.energyPlayer_Cur + energyLootMin <= maxEnergy) {
            game.energyPlayer_Cur = Mathf.Min(game.energyPlayer_Cur + energyLootCur, maxEnergy);
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
            s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Energy);
            if (collectSounds.Count > 0) {
                audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
                audio.Play();
            }
            DestroyCollectible();
        }
    }
}
