using UnityEngine;
using UnityEngine.UI;

public enum GUIUpdateEvent {
    Energy,
    Wood,
    Artifact,
    Health,
    Pause,
    Layer,
    All
}

public class s_GUIMain : MonoBehaviour {
    private static s_GUIMain instance;

	public static s_GUIMain Instance { get { return instance; } }

	void Awake () {
		if (instance) {
			Destroy(this);
		} else {
			instance = this;
		}
    }

	[SerializeField]protected Canvas canvas;
	protected RectTransform canvasRT;

    [SerializeField]protected CanvasGroup GUI_Ingame;
    [SerializeField]protected CanvasGroup GUI_PauseMenu;
    [SerializeField]protected CanvasGroup GUI_BastionMenu;
    [SerializeField]protected CanvasGroup GUI_LayerChange;
    [SerializeField]protected CanvasGroup GUI_GameEnd;


	[SerializeField]protected Image iconBastion;
    [SerializeField]protected Image iconBastionEnergy_Main;
    [SerializeField]protected Image iconBastionDirection;
    [SerializeField]protected Image iconBastionFrame;
    [SerializeField]protected RectTransform textBastionDirDisplayParent;
    [SerializeField]protected Text textBastionDirDisplay;
    [SerializeField]protected int screenBorderThreshold = 50;
    [SerializeField]protected float lerpingSpeedFactor = 5.0f;
    [SerializeField]protected Color colorBastionNear;
    [SerializeField]protected Color colorBastionFar;
    [Tooltip("color reaches \"far\" value at distances of distanceBastionFar and above")]
    [SerializeField]protected float distanceBastionFar = 250.0f; //color reaches "far" value at distances of distanceBastionFar and above
    

    [SerializeField]protected Image iconPlayerEnergy_Main;
    [SerializeField]protected Image iconPlayerWood_Main;
    //[SerializeField]protected Image iconPlayerEnergy_Bastion;
    //[SerializeField]protected Image iconPlayerWood_Bastion;
    //[SerializeField]protected Image iconBastionEnergy_Bastion;
    //[SerializeField]protected Image iconBastionWood_Bastion;
    [SerializeField]protected Text textPlayerEnergy_Main;
    [SerializeField]protected Text textPlayerWood_Main;
    [SerializeField]protected Text textPlayerEnergy_Bastion;
    [SerializeField]protected Text textPlayerWood_Bastion;
    [SerializeField]protected Text textBastionEnergy_Bastion;
    [SerializeField]protected Text textBastionWood_Bastion;


    [SerializeField]protected Image iconArtifacts1_Main; //ancient thrust
    [SerializeField]protected Image iconArtifacts2_Main; //gravitational freeze
    [SerializeField]protected ParticleSystem particlesArtifacts1;
    [SerializeField]protected ParticleSystem particlesArtifacts2;
    [SerializeField]protected Image iconArtifacts1_Bastion; //ancient thrust
    [SerializeField]protected Image iconArtifacts2_Bastion; //gravitational freeze
    [SerializeField]protected Text textArtifacts1_Main;
    [SerializeField]protected Text textArtifacts2_Main;
    [SerializeField]protected Text textArtifacts1_Bastion;
    [SerializeField]protected Text textArtifacts2_Bastion;


	[SerializeField]protected Image iconRemainingTime;
    [SerializeField]protected Text textRemainingTime;
    
    [SerializeField]protected Image iconPlayerHealth;
	[SerializeField]protected Text textPlayerHealth;

    [SerializeField]protected Image iconSkillCooldownBar;

    [SerializeField]protected Button buttonPause;

    [SerializeField]protected Button buttonTake1Energy;
    [SerializeField]protected Button buttonTake10Energy;
    [SerializeField]protected Button buttonStore1Energy;
    [SerializeField]protected Button buttonStore10Energy;
    [SerializeField]protected Button buttonTake1Wood;
    [SerializeField]protected Button buttonTake10Wood;
    [SerializeField]protected Button buttonStore1Wood;
    [SerializeField]protected Button buttonStore10Wood;
    [SerializeField]protected Button buttonClimbLayer;

    [SerializeField]protected Text textSurvivorCount;

	[SerializeField]protected Image iconCurrentLayer;
    [SerializeField]protected Text textCurrentLayer;
    [SerializeField]protected Text textClimbLayer;

    //get from game manager later?
    //[SerializeField]protected Transform bastionTransform;
    public Transform bastionTransform;
    [SerializeField]protected Transform playerTransform;
	[SerializeField]protected Camera playerCamera;

    [SerializeField]protected Image pauseScreenOverlay;
    [SerializeField]protected Button buttonContinue;
    [SerializeField]protected Button buttonRestart;
    [SerializeField]protected Button buttonQuit;
    //[SerializeField]protected Button buttonSeed;
    [SerializeField]protected InputField seedPause;
    [SerializeField]protected InputField seedGameEnd;


    //for bastion indicator
    protected bool offscreen = false;
    protected int min, maxX, maxY;

    //shortcut
    protected s_GameManager game;
    

	// Use this for initialization
	void Start () {
        //If we are in a test level and we dont have a GameManager
        if(s_GameManager.Instance == null) {
            enabled = false;
        }
        else {
            game = s_GameManager.Instance;
        }

		canvasRT = canvas.GetComponent<RectTransform> ();

        if (!playerTransform) {
            playerTransform = LevelManager.Instance.player.transform;
        }

        if (!playerCamera) {
            playerCamera = playerTransform.GetChild(0).GetComponent<Camera>();
        }

		/*if(!bastionTransform){
			bastionTransform = new GameObject("bastion").transform;
			bastionTransform.position = playerTransform.position - playerTransform.up * 0.99f * playerTransform.localScale.y;
            bastionTransform.rotation = playerTransform.rotation;
            bastionTransform.gameObject.AddComponent<SpriteRenderer>();
            bastionTransform.gameObject.GetComponent<SpriteRenderer>().sprite = iconBastion.sprite;
        }*/

        min = 0 + screenBorderThreshold;
        maxX = Screen.width - screenBorderThreshold;
        maxY = Screen.height - screenBorderThreshold;


        buttonPause.onClick.AddListener(() => { s_GameManager.Instance.SetGamePaused(true); });
        buttonContinue.onClick.AddListener(() => { s_GameManager.Instance.SetGamePaused(false); });
        buttonRestart.onClick.AddListener(() => { Application.LoadLevel(Application.loadedLevel); });

        buttonTake1Wood.onClick.AddListener(() => { s_GameManager.Instance.TakeWood(1); });
        buttonTake10Wood.onClick.AddListener(() => { s_GameManager.Instance.TakeWood(10); });
        buttonStore1Wood.onClick.AddListener(() => { s_GameManager.Instance.StoreWood(1); });
        buttonStore10Wood.onClick.AddListener(() => { s_GameManager.Instance.StoreWood(10); });
        buttonTake1Energy.onClick.AddListener(() => { s_GameManager.Instance.TakeEnergy(1); });
        buttonTake10Energy.onClick.AddListener(() => { s_GameManager.Instance.TakeEnergy(10); });
        buttonStore1Energy.onClick.AddListener(() => { s_GameManager.Instance.StoreEnergy(1); });
        buttonStore10Energy.onClick.AddListener(() => { s_GameManager.Instance.StoreEnergy(10); });

        buttonClimbLayer.onClick.AddListener(() => { s_GameManager.Instance.ClimbLayer(); });
        
    }
	
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            game.SwitchGamePaused();
        }
        //if(game.playerInBastion && Input.GetAxis("Inventory") != 0) {
        if (game.playerInBastion && Input.GetButtonDown("Inventory")) {
            GUI_BastionMenu.gameObject.SetActive(!GUI_BastionMenu.gameObject.activeSelf);
            UpdateCursor();
        }

        if (!game.gamePaused) {
            
            #region UpdatePerFrame
            int remainingTime = (int)Mathf.Max(0, game.endTime - Time.time);
		    textRemainingTime.text =  remainingTime/60 + ":" + (remainingTime%60).ToString("00");
            iconRemainingTime.fillAmount = (float)remainingTime / game.roundDuration;
            if (bastionTransform != null)   //JingYi: quickfix to avoid wall of errors after the bastion is destroyed by the black whole and the player is still on a higher island
            {
                UpdateBastionDirectionIcon();
            }
            #endregion
            
            #region TODO: put in proper references!! 
            //iconSkillCooldownBar.fillAmount = (0.1f * Time.time) % 1.0f;
            #endregion

        }
        if (game.gamePaused) {
            //show seed in pause menu
        }

	}
    
    protected void UpdateCursor () {
        bool cursorNeeded = game.gamePaused || GUI_BastionMenu.gameObject.activeSelf;
        Cursor.visible = cursorNeeded;
        Cursor.lockState = cursorNeeded ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void UpdateGUI (GUIUpdateEvent updateType = GUIUpdateEvent.All) {
        if (!game) {
            game = s_GameManager.Instance;
        }
        switch (updateType) {
            case GUIUpdateEvent.Energy:
                UpdateEnergyState();
                break;
            case GUIUpdateEvent.Wood:
                UpdateWoodState();
                break;
            case GUIUpdateEvent.Artifact:
                UpdateArtifactState();
                break;
            case GUIUpdateEvent.Health:
                UpdateHealthState();
                break;
            case GUIUpdateEvent.Pause:
                UpdatePauseState();
                break;
            case GUIUpdateEvent.Layer:
                UpdateLayerState();
                break;
            case GUIUpdateEvent.All:
                UpdateEnergyState();
                UpdateWoodState();
                UpdateArtifactState();
                UpdateHealthState();
                UpdatePauseState();
                UpdateLayerState();
                break;
        }
    }

    protected void UpdateEnergyState () {
        iconPlayerEnergy_Main.fillAmount = game.energyPlayer_Cur / game.energyPlayer_Max;
        textPlayerEnergy_Bastion.text = textPlayerEnergy_Main.text = game.energyPlayer_Cur.ToString("0.0");

        iconBastionEnergy_Main.fillAmount = game.energyBastion_Cur / game.energyBastion_Max;
        textBastionEnergy_Bastion.text = game.energyBastion_Cur.ToString("0.0");

        buttonClimbLayer.interactable = game.energyBastion_Cur >= game.energyCostClimbLayer;
        textClimbLayer.text = game.energyCostClimbLayer.ToString("0");
    }

    protected void UpdateWoodState () {
        iconPlayerWood_Main.fillAmount = game.woodPlayer_Cur / game.woodPlayer_Max;
        textPlayerWood_Bastion.text = textPlayerWood_Main.text = game.woodPlayer_Cur.ToString("0");

        textBastionWood_Bastion.text = game.woodBastion_Cur.ToString("0");
    }

    protected void UpdateArtifactState () {
        bool artifacts1Incomplete = game.artifact1CountCur < game.artifactCountMax;
        textArtifacts1_Bastion.text = textArtifacts1_Main.text = artifacts1Incomplete ? game.artifact1CountCur.ToString() : "";
        iconArtifacts1_Bastion.fillAmount = iconArtifacts1_Main.fillAmount = game.artifact1CountCur / (float)game.artifactCountMax;
        particlesArtifacts1.enableEmission = !artifacts1Incomplete;

        bool artifacts2Incomplete = game.artifact2CountCur < game.artifactCountMax;
        textArtifacts2_Bastion.text = textArtifacts2_Main.text = artifacts2Incomplete ? game.artifact2CountCur.ToString() : "";
        iconArtifacts2_Bastion.fillAmount = iconArtifacts2_Main.fillAmount = game.artifact2CountCur / (float)game.artifactCountMax;
        particlesArtifacts2.enableEmission = !artifacts2Incomplete;
    }

    protected void UpdateHealthState () {
        textPlayerHealth.text = game.healthpointsCur.ToString();
        iconPlayerHealth.fillAmount = (float)game.healthpointsCur / (float)game.healthpointsMax;
        textSurvivorCount.text = game.survivorsCur.ToString();
    }

    protected void UpdatePauseState () {
        bool paused = game.gamePaused;
        GUI_Ingame.gameObject.SetActive(!paused);
        //GUI_Ingame.interactable = !paused;
        GUI_PauseMenu.gameObject.SetActive(paused);
        //GUI_PauseMenu.interactable = paused;
        pauseScreenOverlay.CrossFadeAlpha(paused ? 1.0f : 0.0f, 0.5f, true);
        UpdateCursor();
    }

    protected void UpdateLayerState () {
        textCurrentLayer.text = "Layer " + (LevelManager.Instance.CurLvl + 1);
        seedPause.text = LevelManager.Instance.RngSeed.ToString();
    }

    //TODO: possibly change color, transparency, size?
    protected void UpdateBastionDirectionIcon () {
        offscreen = false;

        //get screen position of bastion

        Vector3 bastionScreenPos = playerCamera.WorldToScreenPoint (bastionTransform.position);

        Vector3 testDir = (bastionTransform.position - playerCamera.transform.position).normalized;
        testDir = Vector3.ProjectOnPlane(testDir, playerCamera.transform.forward);
        Debug.DrawRay(playerCamera.transform.position, testDir * 1000, Color.magenta);

        //if it is behind us, flip position
        if (bastionScreenPos.z < 0) {
            bastionScreenPos *= -1;
            offscreen = true;
        }

        //save original screen space position for later, and clamp it to fit on screen
        Vector2 originalScreenPos = bastionScreenPos;
        bastionScreenPos.x = Mathf.Clamp(bastionScreenPos.x, min, maxX);
        bastionScreenPos.y = Mathf.Clamp(bastionScreenPos.y, min, maxY);

        if (originalScreenPos != (Vector2) bastionScreenPos) {
            offscreen = true;
        }

        //convert to coordinate space of canvas
        Vector2 bastionCanvasPos = ScreenToCanvasSpace(bastionScreenPos, canvasRT.sizeDelta);

        //lerp bastion icon to avoid jittering
        iconBastion.rectTransform.anchoredPosition = Vector2.Lerp(iconBastion.rectTransform.anchoredPosition, bastionCanvasPos, lerpingSpeedFactor * Time.deltaTime);

        //change color of icon frame based on distance
        float distanceToBastion = (playerTransform.position - bastionTransform.position).magnitude;
        float lerpParameter = distanceToBastion / distanceBastionFar;
        iconBastionFrame.color = Colorx.Slerp(colorBastionNear, colorBastionFar, lerpParameter);
        
        //show direction arrow
        iconBastionDirection.enabled = offscreen;

        if(offscreen) {
            float angle = 0;
            //if x is within screen boundaries, show arrow up/down
            if (bastionScreenPos.x == originalScreenPos.x) {
                angle = -90 + 90 * Mathf.Sign(originalScreenPos.y - min);
            }
            //if y is within screen boundaries, show arrow left/right
            else if (bastionScreenPos.y == originalScreenPos.y) {
                angle = -180 + 90 * Mathf.Sign(originalScreenPos.x - min);
            }
            else {
                angle = -90 + Mathf.Rad2Deg * Mathf.Atan2(x: originalScreenPos.x, y: originalScreenPos.y);
            }
            iconBastionDirection.rectTransform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //update distance text
            textBastionDirDisplay.text = distanceToBastion.ToString("0") + "m";
            textBastionDirDisplayParent.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            textBastionDirDisplay.rectTransform.localRotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            textBastionDirDisplay.rectTransform.anchoredPosition = (-3 * textBastionDirDisplay.text.Length) * Vector3.up * (Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (angle))));
        }
        else {
            //update distance text
            textBastionDirDisplay.text = distanceToBastion.ToString("0") + "m";
            textBastionDirDisplayParent.localRotation = Quaternion.identity;
            textBastionDirDisplay.rectTransform.localRotation = Quaternion.identity;
            textBastionDirDisplay.rectTransform.anchoredPosition = Vector3.zero;
        }
    }

    Vector2 ViewportToCanvasSpace (Vector3 posViewport, Vector2 canvasSizeDelta) {
        return new Vector2((posViewport.x - 0.5f) * canvasSizeDelta.x, (posViewport.y - 0.5f) * canvasSizeDelta.y);
    }

    Vector2 ScreenToCanvasSpace (Vector3 posScreen, Vector2 canvasSizeDelta) {
        posScreen.x *= 1.0f/Screen.width;
        posScreen.y *= 1.0f/Screen.height;
        return new Vector2((posScreen.x - 0.5f) * canvasSizeDelta.x, (posScreen.y - 0.5f) * canvasSizeDelta.y);
    }
}
