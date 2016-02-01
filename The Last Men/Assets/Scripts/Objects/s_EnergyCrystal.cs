using UnityEngine;
using System.Collections.Generic;

public class s_EnergyCrystal : s_Collectible {

	public float energyLootMin = 3;
    public float energyLootMax = 7;
    protected float energyLootCur;
    protected new AudioSource audio;
    [SerializeField]
    protected List<AudioClip> collectSounds;

    void Awake () {
        autoDestroyOnCollect = false;
        energyLootCur = RandomFromDistribution.RandomRangeNormalDistribution(energyLootMin, energyLootMax, RandomFromDistribution.ConfidenceLevel_e._99);
        audio = GetComponent<AudioSource>();

    }

    protected override void Collect () {
        s_GameManager game = s_GameManager.Instance;
        if(game.energyPlayer_Cur + energyLootMin <= game.energyPlayer_Max) {
            game.energyPlayer_Cur = Mathf.Min(game.energyPlayer_Cur + energyLootCur, game.energyPlayer_Max);
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
            s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Energy);
            audio.clip = collectSounds[Random.Range(0, collectSounds.Count)];
            audio.Play();
            DestroyCollectible();
        }
    }
}
