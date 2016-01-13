using UnityEngine;
using System.Collections;

public class s_EnergyCrystal : s_Collectible {

	public float energyLootMin = 3;
    public float energyLootMax = 7;
    protected float energyLootCur;

    void Awake () {
        energyLootCur = RandomFromDistribution.RandomRangeNormalDistribution(energyLootMin, energyLootMax, RandomFromDistribution.ConfidenceLevel_e._99);
    }

    protected override void Collect () {
        s_GameManager.Instance.energyPlayer_Cur = Mathf.Clamp(s_GameManager.Instance.energyPlayer_Cur + energyLootCur, 0, s_GameManager.Instance.energyPlayer_Max);
    }
}
