using UnityEngine;
using System.Collections;

public class s_EnergyCrystal : s_Collectible {

	public float energyLootMin = 3;
    public float energyLootMax = 7;
    protected float energyLootCur;

    void Awake () {
        autoDestroyOnCollect = false;
        energyLootCur = RandomFromDistribution.RandomRangeNormalDistribution(energyLootMin, energyLootMax, RandomFromDistribution.ConfidenceLevel_e._99);
    }

    protected override void Collect () {
        s_GameManager game = s_GameManager.Instance;
        if(game.energyPlayer_Cur + energyLootMin <= game.energyPlayer_Max) {
            game.energyPlayer_Cur = Mathf.Min(game.energyPlayer_Cur + energyLootCur, game.energyPlayer_Max);
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
            DestroyCollectible();
        }
    }
}
