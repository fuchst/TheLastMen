using UnityEngine;

public class s_WoodenLog : s_Collectible {
    
    void Awake () {
        autoDestroyOnCollect = false;
    }

	protected override void Collect () {
        if(s_GameManager.Instance.woodPlayer_Cur < s_GameManager.Instance.woodPlayer_Max) {
            s_GameManager.Instance.woodPlayer_Cur++;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Wood);
            DestroyCollectible();
        }
    }
}
