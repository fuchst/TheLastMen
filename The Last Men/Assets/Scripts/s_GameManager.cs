using UnityEngine;

public class s_GameManager : MonoBehaviour {

    private LevelManager levelManager;
    //private Camera worldCam;

    public int artifact1CountCur = 0;
    public int artifact2CountCur = 0;
    public int artifactCountMax = 10;

    public float energyPlayer_Cur = 0;
    public float energyPlayer_Max = 100;
    public float energyBastion_Cur = 0;
    public float energyBastion_Max = 100;

    public float woodPlayer_Cur = 0;
    public float woodBastion_Cur = 0;
    public float woodPlayer_Max = 10;
    public float woodBastion_Max = 10;
    
    public int healthpointsCur = 100;
    public int healthpointsMax = 100;

    public float energyCostClimbLayer = 15;

    public bool gamePaused = false;
    public bool playerInBastion = false;

    private static s_GameManager instance;

	public static s_GameManager Instance { get { return instance; } }

	void Awake () {
		if (instance) {
			Destroy(this);
		} else {
			instance = this;
		}

        levelManager = GetComponent<LevelManager>();
    }

    public float roundDuration = 300.0f;
	public float endTime;

	void Start () {
		endTime = Time.time + roundDuration;
        
        levelManager.LoadLevel();
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.All);
    }

    public void SwitchGamePaused () {
        SetGamePaused(!gamePaused);
    }

    public void SetGamePaused (bool paused) {
        gamePaused = paused;
        Time.timeScale = gamePaused ? 0 : 1;
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Pause);
        Debug.Log("The game is " + (gamePaused ? "" : "un") + "paused now.");
    }

    public void StoreEnergy (float amount) {
        amount = Mathf.Min(Mathf.Min(energyPlayer_Cur, amount), energyBastion_Max - energyBastion_Cur);
        if(amount > 0) {
            energyPlayer_Cur -= amount;
            energyBastion_Cur += amount;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
        }
    }

    public void TakeEnergy (float amount) {
        amount = Mathf.Min(Mathf.Min(energyBastion_Cur, amount), energyPlayer_Max - energyPlayer_Cur);
        if(amount > 0) {
            energyPlayer_Cur += amount;
            energyBastion_Cur -= amount;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
        }
    }

    public void StoreWood (float amount) {
        amount = Mathf.Min(Mathf.Min(woodPlayer_Cur, amount), woodBastion_Max - woodBastion_Cur);
        if(amount > 0) {
            woodPlayer_Cur -= amount;
            woodBastion_Cur += amount;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Wood);
        }
    }

    public void TakeWood (float amount) {
        amount = Mathf.Min(Mathf.Min(woodBastion_Cur, amount), woodPlayer_Max - woodPlayer_Cur);
        if(amount > 0) {
            woodPlayer_Cur += amount;
            woodBastion_Cur -= amount;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Wood);
        }
    }
}
