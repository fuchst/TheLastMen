using UnityEngine;
using System.Collections;

public class s_HealingPlant : s_Collectible {

	[Range(2, 5)] public int healAmount;

    protected override void Collect () {
        s_GameManager.Instance.HealPlayer(healAmount);
    }
}
