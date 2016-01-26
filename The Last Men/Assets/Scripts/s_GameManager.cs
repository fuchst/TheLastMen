using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class s_GameManager : MonoBehaviour {

    private LevelManager levelManager;
    //private Camera worldCam;

    public int artifact1CountCur = 0;
    public int artifact2CountCur = 0;
    public int artifactCountMax = 10;

    public float energyPlayer_Cur = 10;
    public float energyPlayer_Max = 100;
    public float energyBastion_Cur = 0;
    public float energyBastion_Max = 100;

    public float woodPlayer_Cur = 0;
    public float woodBastion_Cur = 0;
    public float woodPlayer_Max = 10;
    public float woodBastion_Max = 10;
    
    public int healthpointsCur = 100;
    public int healthpointsPrev = 100;
    public int healthpointsMax = 100;
    public int healthRegenerationRateRegular = 0;
    public int healthRegenerationRateBastion = 10;
    public int survivorsCur = 3;

    public float energyCostClimbLayer = 15;

    public bool gamePaused = false;
    protected bool bastionMenu = false;
    protected bool playerInBastion = false;

    public GameObject[] lootTable = new GameObject[1];

    private static s_GameManager instance;

	public static s_GameManager Instance { get { return instance; } }
    
    public bool PlayerInBastion {
        get { return playerInBastion; }
    }

    public bool BastionMenu {
        get { return bastionMenu; }
    }

    public void ToggleBastionMenu () {
        bastionMenu = !bastionMenu;
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.BastionMenu);
    }

    public void SetPlayerInBastion (bool inBastion) {
        if(inBastion == playerInBastion) {
            return;
        }
        playerInBastion = inBastion;
        bastionMenu &= playerInBastion;
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.BastionMenu);
    }

    protected IEnumerator HealPlayer () {
        while (enabled) {
            yield return new WaitForSeconds(1.0f);
            if (playerInBastion) {
                HealPlayer(healthRegenerationRateBastion);
            }
            else {
                HealPlayer(healthRegenerationRateRegular);
            }
        }
    }

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
        ResetLevelClock();
        
        levelManager.LoadLevel();
        InitializePlayerStats();
        StartCoroutine(HealPlayer());
    }

    void Update () {
        if (Time.time > endTime) {
            EndGame();
        }
        healthpointsPrev = healthpointsCur;
    }

    protected void InitializePlayerStats () {
        healthpointsCur = healthpointsMax;
        energyPlayer_Cur = 10;
        woodPlayer_Cur = 0;
    }

    public void ToggleGamePaused () {
        SetGamePaused(!gamePaused);
    }

    public void SetGamePaused (bool paused) {
        gamePaused = paused;
        Time.timeScale = gamePaused ? 0 : 1;
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Pause);
        //Debug.Log("The game is " + (gamePaused ? "" : "un") + "paused now."); //JingYi: Pause/Unpause is working perfectly so we don't need this anymore I guess
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

    public void ClimbLayer () {
        if (energyBastion_Cur < energyCostClimbLayer)
            return;
        energyBastion_Cur -= energyCostClimbLayer;
        Debug.Log("Advancing to next level");
        ResetLevelClock();
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
        LevelManager.Instance.AdvanceLevel();
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Layer);
    }

    protected void ResetLevelClock () {
        endTime = Time.time + roundDuration;
    }

    public void ConsumeEnergy (float amount) {
        amount = Mathf.Clamp(amount, 0, energyPlayer_Cur);
        if (amount > 0) {
            energyPlayer_Cur -= amount;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
        }
    }

    public void HealPlayer(int amount) {
        if (amount <= 0)
            return;
        healthpointsCur = Mathf.Min(healthpointsCur + amount, healthpointsMax);
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Health);
    }

    public void HurtPlayer (int damage) {
        if (damage <= 0)
            return;
        healthpointsCur -= damage;
        if (healthpointsCur <= 0) {
            healthpointsCur = 0;
            KillPlayer();
        }
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Health);
    }

    protected void KillPlayer () {
        survivorsCur--;
        if(survivorsCur <= 0) {
            survivorsCur = 0;
            EndGame();
        }
        else {
            levelManager.player.GetComponent<FireGrapplingHook>().RemoveRope();
            //TODO: reset upgrades
            //reset some stuff in controller? 
            //levelManager.player.GetComponent<RigidbodyFirstPersonControllerSpherical>();
            levelManager.player.transform.position = levelManager.GetPlayerSpawnPos();
            InitializePlayerStats();
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Wood);
        }
        //Debug.Log("Player died");
        
    }

    protected void EndGame () {
        //TODO: more elaborate things!
        Application.LoadLevel(0);
    }
}
