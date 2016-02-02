using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class s_GameManager : MonoBehaviour {

    private LevelManager levelManager;
    //private Camera worldCam;

    [System.Serializable]
    public class UpgradeSettings {
        public enum UpgradeTypes {
            PistolDamage,
            ShotgunBullets,
            ArmourValue,
            InventoryCapacity,
            ResourceHarvesting, 
            JetpackSpeed,
            RopeLength,
            AirGliding
        }

        [System.Serializable]
        public class UpgradeObject {
            public UpgradeTypes type;
            public float stepSize;
            public bool relative;
            public int cost_wood;
            public float cost_energy;
            public string name;
            public string description;
            public int progress_cur;
            public int progress_max;
            
            public UpgradeObject (UpgradeTypes type, float stepSize, bool relative, int cost_wood, float cost_energy, string name, string description, int progress_max = 3, int progress_cur = 0) {
                this.type = type;
                this.stepSize = stepSize;
                this.relative = relative;
                this.cost_wood = cost_wood;
                this.cost_energy = cost_energy;
                this.name = name;
                this.description = description;
                this.progress_cur = progress_cur;
                this.progress_max = progress_max;
            }
        }

        public Dictionary<UpgradeTypes, UpgradeObject> upgrades;

        public void InitializeUpgrades () {
            upgrades = new Dictionary<UpgradeTypes, UpgradeObject>(8);

            upgrades.Add(UpgradeTypes.PistolDamage,
                new UpgradeObject(UpgradeTypes.PistolDamage, 2.5f, false, 2, 20.0f, "Pistol Damage", "Reinforce your pistol for more powerful shots.\n\n(+2.5 damage per shot)"));
            upgrades.Add(UpgradeTypes.ShotgunBullets,
                new UpgradeObject(UpgradeTypes.ShotgunBullets, 2, false, 3, 15.0f, "Shotgun Bullets", "Augment the shotgun barrel for additional ammo.\n\n(+2 bullets per shot)"));
            upgrades.Add(UpgradeTypes.ArmourValue,
                new UpgradeObject(UpgradeTypes.ArmourValue, 0.2f, false, 5, 7.5f, "Armour Value", "Craft some makeshift armour for yourself.\n\n(+10% damage protection)"));
            upgrades.Add(UpgradeTypes.InventoryCapacity,
                new UpgradeObject(UpgradeTypes.InventoryCapacity, 0.2f, true, 3, 5.0f, "Inventory Size", "Construct and mount a bigger backpack.\n\n(+20% inventory size for wood/energy)"));
            upgrades.Add(UpgradeTypes.ResourceHarvesting,
                new UpgradeObject(UpgradeTypes.ResourceHarvesting, 0.15f, true, 1, 10.0f, "Resource Harvests", "Enhance your harvesting equipment and skills.\n\n(+15% more resource yields)"));
            upgrades.Add(UpgradeTypes.JetpackSpeed,
                new UpgradeObject(UpgradeTypes.JetpackSpeed, 0.2f, true, 1, 15.0f, "Jetpack Speed", "Boost up your jetpack and the sky is no limit.\n\n(+20% higher max speed)"));
            upgrades.Add(UpgradeTypes.RopeLength,
                new UpgradeObject(UpgradeTypes.RopeLength, 7.5f, false, 5, 0.0f, "Rope Length", "Extend your grappling hook's rope.\n\n(+7.5m rope length)"));
            upgrades.Add(UpgradeTypes.AirGliding,
                new UpgradeObject(UpgradeTypes.AirGliding, 0.5f, true, 2, 5.0f, "Air Gliding", "Improve the aerodynamics of your gear, for better air gliding.\n\n(+50% gliding movement)"));
        }

        public void ResetUpgrades() {
            foreach (KeyValuePair<UpgradeTypes, UpgradeObject> element in upgrades) {
                element.Value.progress_cur = 0;
            }
        }

        #region Outdated
        /*
        public int pistolDamage_max = 3;
        public int pistolDamage_cur = 0;
        public float pistolDamage_stepSize = 2.5f;
        public int pistolDamage_wood = 2;
        public float pistolDamage_energy = 20.0f;
        public string pistolDamage_Name = "Pistol Damage";
        public string pistolDamage_description = "Reinforce your pistol for more powerful shots. (+2.5 damage per shot)";

        public int shotgunBullets_max = 3;
        public int shotgunBullets_cur = 0;
        public int shotgunBullets_stepSize = 2;
        public int shotgunBullets_wood = 3;
        public float shotgunBullets_energy = 15.0f;
        public string shotgunBullets_name = "Shotgun Bullets";
        public string shotgunBullets_description = "Augment the shotgun barrel for additional ammo. (+2 bullets per shot)";

        public int armourValue_max = 3;
        public int armourValue_cur = 0;
        public float armourValue_stepSize = 0.1f;
        public int armourValue_wood = 5;
        public float armourValue_energy = 7.5f;
        public string armourValue_name = "Armour Value";
        public string armourValue_description = "Craft some makeshift armour for yourself. (+10% damage protection)";

        public int inventoryCapacity_max = 3;
        public int inventoryCapacity_cur = 0;
        public float inventoryCapacity_stepFactor = 0.2f;
        public int inventoryCapacity_wood = 3;
        public float inventoryCapacity_energy = 5.0f;
        public string inventoryCapacity_name = "Inventory Size";
        public string inventoryCapacity_description = "Construct and mount a bigger backpack. (+20% inventory size for wood/energy)";

        public int resourceHarvest_max = 3;
        public int resourceHarvest_cur = 0;
        public float resourceHarvest_stepFactor = 0.15f;
        public int resourceHarvest_wood = 1;
        public float resourceHarvest_energy = 10.0f;
        public string resourceHarvest_name = "Resource Harvests";
        public string resourceHarvest_description = "Enhance your harvesting equipment and skills. (+15% more resource yields)";

        public int jetpackSpeed_max = 3;
        public int jetpackSpeed_cur = 0;
        public float jetpackSpeed_stepFactor = 0.2f;
        public int jetpackSpeed_wood = 1;
        public float jetpackSpeed_energy = 15.0f;
        public string jetpackSpeed_name = "Jetpack Speed";
        public string jetpackSpeed_description = "Boost up your jetpack and the sky is no limit! (+20% higher max speed)";

        public int ropeLength_max = 3;
        public int ropeLength_cur = 0;
        public float ropeLength_stepSize = 7.5f;
        public int ropeLength_wood = 5;
        public float ropeLength_energy = 0.0f;
        public string ropeLength_name = "Rope Length";
        public string ropeLength_description = "Extend your grappling hook's rope. (+7.5m rope length)";

        public int airGliding_max = 3;
        public int airGliding_cur = 0;
        public float airGliding_stepFactor = 0.5f;
        public int airGliding_wood = 2;
        public float airGliding_energy = 5.0f;
        public string airGliding_name = "Air Gliding";
        public string airGliding_description = "Improve the aerodynamics of your gear, for better air gliding. (+50% gliding movement)";
        */
        #endregion
    }

    public UpgradeSettings upgradeSettings = new UpgradeSettings();

    public int artifact1CountCur = 0;
    public int artifact2CountCur = 0;
    public int artifactCountMax = 10;

    public float energyPlayer_Cur = 10;
    [SerializeField]protected float energyPlayer_Max = 100;
    public float energyBastion_Cur = 0;
    public float energyBastion_Max = 100;
    public float EnergyPlayerMax { get { return energyPlayer_Max * (1 + inventoryCapacity.progress_cur * inventoryCapacity.stepSize); } }
    
    public float woodPlayer_Cur = 0;
    public float woodBastion_Cur = 0;
    [SerializeField]protected float woodPlayer_Max = 10;
    public float woodBastion_Max = 10;
    public float WoodPlayerMax { get { return woodPlayer_Max * (1 + inventoryCapacity.progress_cur * inventoryCapacity.stepSize); } }

    public int healthpointsCur = 100;
    public int healthpointsPrev = 100;
    public int healthpointsMax = 100;
    public int healthRegenerationRateRegular = 0;
    public int healthRegenerationRateBastion = 10;
    public int survivorsCur = 3;
    public int amountOfKeys = 0;
    protected UpgradeSettings.UpgradeObject playerArmour;
    protected UpgradeSettings.UpgradeObject inventoryCapacity;

    public float energyCostClimbLayer = 15;

    public bool gamePaused = false;
    protected bool bastionMenu = false;
    protected bool playerInBastion = false;

    public GameObject[] lootTableRegular = new GameObject[1];
    public GameObject[] lootTableRare = new GameObject[1];
    [Range(0.0f, 1.0f)]public float rareLootChance = 0.1f;

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

    private void HideAllMenus()
    {
        bastionMenu = false;
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

        upgradeSettings.InitializeUpgrades();
    }
    
    public bool CheckUpgradeAvailability (UpgradeSettings.UpgradeTypes upgradeType) {
        UpgradeSettings.UpgradeObject upgradeObj = upgradeSettings.upgrades[upgradeType];
        return upgradeObj.progress_max > upgradeObj.progress_cur && energyBastion_Cur > upgradeObj.cost_energy && woodBastion_Cur > upgradeObj.cost_wood;
    }

    public void BuyUpgrade (UpgradeSettings.UpgradeTypes upgradeType) {
        if (CheckUpgradeAvailability(upgradeType)) {
            UpgradeSettings.UpgradeObject upgradeObj = upgradeSettings.upgrades[upgradeType];
            upgradeObj.progress_cur++;
            energyBastion_Cur -= upgradeObj.cost_energy;
            woodBastion_Cur -= upgradeObj.cost_wood;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Wood);
        }
    }

    public float roundDuration = 300.0f;
	public float endTime;

	void Start () {
        ResetLevelClock();
        
        levelManager.LoadLevel();

        playerArmour = upgradeSettings.upgrades[UpgradeSettings.UpgradeTypes.ArmourValue];
        inventoryCapacity = upgradeSettings.upgrades[UpgradeSettings.UpgradeTypes.InventoryCapacity];
    }

    public void LevelLoaded()
    {
        ResetLevelClock();
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
        amount = Mathf.Min(Mathf.Min(energyBastion_Cur, amount), EnergyPlayerMax - energyPlayer_Cur);
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
        amount = Mathf.Min(Mathf.Min(woodBastion_Cur, amount), WoodPlayerMax - woodPlayer_Cur);
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
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
        ResetLevelClock();

        bastionMenu = false;
        s_GUIMain.Instance.HideAllMenus();

        LevelManager.Instance.AdvanceLevel();
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
        healthpointsCur -= damage * (int)(1 - playerArmour.progress_cur * playerArmour.stepSize);
        if (healthpointsCur <= 0) {
            healthpointsCur = 0;
            KillPlayer();
        }
        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Health);
    }

    protected void KillPlayer () {
        survivorsCur--;
        upgradeSettings.ResetUpgrades();
        if(survivorsCur <= 0) {
            survivorsCur = 0;
            EndGame();
        }
        else {
            levelManager.player.GetComponent<FireGrapplingHook>().RemoveRope();
            //TODO: reset upgrades
            //reset some stuff in controller? 
            //levelManager.player.GetComponent<RigidbodyFirstPersonControllerSpherical>();
            levelManager.UpdatePlayerSpawnPos();
            levelManager.player.transform.position = levelManager.UpdatePlayerSpawnPos();
            InitializePlayerStats();
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Energy);
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Wood);
        }
        //Debug.Log("Player died");
        
    }

    public GameObject RetrieveLoot () {
        if ( Random.value < rareLootChance) {
            //rare loot
            return lootTableRare[Random.Range(0, lootTableRare.Length)];
        }
        else {
            //regular loot
            return lootTableRegular[Random.Range(0, lootTableRegular.Length)];
        }
    }

    protected void EndGame () {
        //TODO: more elaborate things!
        Application.LoadLevel(0);
    }
}
