using UnityEngine;
using System.Collections;

public class s_Key : s_Collectible {

	protected override void Collect () {
        s_GameManager.Instance.amountOfKeys++;
        s_GUIMain.Instance.SpawnPopupMessage(GUIPopupMessage.Key);
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Key);
    }
}
