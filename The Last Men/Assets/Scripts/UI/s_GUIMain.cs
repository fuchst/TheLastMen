using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GUIUpdateEvent {
    Energy,
    Wood,
    Artifact,
    Health,
    Tool,
    Pause,
    BastionMenu,
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

    [SerializeField]protected CanvasGroup GUI_Ingame, GUI_PauseMenu, GUI_Controls, GUI_BastionMenu, GUI_LayerChange, GUI_GameEnd;


	[SerializeField]protected Image iconBastion, iconBastionEnergy_Main, iconBastionDirection, iconBastionFrame;
    [SerializeField]protected RectTransform textBastionDirDisplayParent;
    [SerializeField]protected Text textBastionDirDisplay;
    [SerializeField]protected int screenBorderThreshold = 50;
    [SerializeField]protected float lerpingSpeedFactor = 5.0f;
    [SerializeField]protected Color colorBastionNear, colorBastionFar;
    [Tooltip("color reaches \"far\" value at distances of distanceBastionFar and above")]
    [SerializeField]protected float distanceBastionFar = 250.0f; //color reaches "far" value at distances of distanceBastionFar and above
    

    [SerializeField]protected Image iconPlayerEnergy_Main, iconPlayerWood_Main;
    [SerializeField]protected Text textPlayerEnergy_Main, textPlayerWood_Main;
    [SerializeField]protected Text textPlayerEnergy_Bastion, textPlayerWood_Bastion;
    [SerializeField]protected Text textBastionEnergy_Bastion, textBastionWood_Bastion;

    //artifact type 1 = ancient thrust, artifact type 2 = gravitational freeze
    [SerializeField]protected Image iconArtifacts1_Main, iconArtifacts2_Main;
    [SerializeField]protected ParticleSystem particlesArtifacts1, particlesArtifacts2;
    [SerializeField]protected Image iconArtifacts1_Bastion, iconArtifacts2_Bastion;
    [SerializeField]protected Text textArtifacts1_Main, textArtifacts2_Main;
    [SerializeField]protected Text textArtifacts1_Bastion, textArtifacts2_Bastion;


	[SerializeField]protected Image iconRemainingTime;
    [SerializeField]protected Text textRemainingTime;
    [SerializeField]protected Image iconPlayerHealth;
	[SerializeField]protected Text textPlayerHealth;
    [SerializeField]protected Image damageScreenOverlay;
    [SerializeField]protected Color damageOverlayColorActive;
    [SerializeField]protected Color damageOverlayColorRegular;


    [SerializeField]protected Text currentToolText, currentToolDescription;
    [SerializeField]protected Image selectedPistol, selectedShotgun;
    [SerializeField]protected Image iconRope, iconPistol, iconShotgun;

    [SerializeField]protected Image iconSkillCooldownBar;

    [SerializeField]protected Image pauseScreenOverlay;
    [SerializeField]protected Button buttonPause;
    [SerializeField]protected Button buttonContinue, buttonControls, buttonRestart, buttonQuit;
    [SerializeField]protected Button buttonReturn;
    
    [SerializeField]protected Button buttonTake1Energy, buttonTake10Energy, buttonStore1Energy, buttonStore10Energy;
    [SerializeField]protected Button buttonTake1Wood, buttonTake10Wood, buttonStore1Wood, buttonStore10Wood;
    [SerializeField]protected Button buttonClimbLayer;
    [SerializeField]protected Button buttonCloseBastionMenu;

    [SerializeField]protected Text textSurvivorCount;

	[SerializeField]protected Image iconCurrentLayer;
    [SerializeField]protected Text textCurrentLayer;
    [SerializeField]protected Text textClimbLayer;

    public Transform bastionTransform;
    [SerializeField]protected Transform playerTransform;
	[SerializeField]protected Camera playerCamera;

    //[SerializeField]protected Button buttonSeed;
    [SerializeField]protected InputField seedPause;
    [SerializeField]protected InputField seedGameEnd;


    //for bastion indicator
    protected bool offscreen = false;
    protected int min, maxX, maxY;

    //protected int fadingIn = 0;

    //shortcut
    protected s_GameManager game;
    protected FireGrapplingHook hook;
    protected Combat combat;

	// Use this for initialization
	public void InitGUI () {
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

        hook = playerTransform.GetComponent<FireGrapplingHook>();
        combat = playerTransform.GetComponent<Combat>();

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
        buttonControls.onClick.AddListener(() => { s_GUIMain.Instance.GUI_Controls.gameObject.SetActive(true); });
        buttonReturn.onClick.AddListener(() => { s_GUIMain.Instance.GUI_Controls.gameObject.SetActive(false); });
        buttonRestart.onClick.AddListener(() => { Application.LoadLevel(Application.loadedLevel); s_GameManager.Instance.SetGamePaused(false); });
        buttonQuit.onClick.AddListener(() => { Application.LoadLevel(0); });
        buttonCloseBastionMenu.onClick.AddListener(() => { s_GameManager.Instance.ToggleBastionMenu(); });

        buttonTake1Wood.onClick.AddListener(() => { s_GameManager.Instance.TakeWood(1); });
        buttonTake10Wood.onClick.AddListener(() => { s_GameManager.Instance.TakeWood(10); });
        buttonStore1Wood.onClick.AddListener(() => { s_GameManager.Instance.StoreWood(1); });
        buttonStore10Wood.onClick.AddListener(() => { s_GameManager.Instance.StoreWood(10); });
        buttonTake1Energy.onClick.AddListener(() => { s_GameManager.Instance.TakeEnergy(1); });
        buttonTake10Energy.onClick.AddListener(() => { s_GameManager.Instance.TakeEnergy(10); });
        buttonStore1Energy.onClick.AddListener(() => { s_GameManager.Instance.StoreEnergy(1); });
        buttonStore10Energy.onClick.AddListener(() => { s_GameManager.Instance.StoreEnergy(10); });

        buttonClimbLayer.onClick.AddListener(() => { s_GameManager.Instance.ClimbLayer(); });
        
        damageScreenOverlay.CrossFadeColor(damageOverlayColorRegular, 0.0f, true, true);
    }
	
	void Update () {
		if (LevelManager.Instance.GetGameState == LevelManager.GameState.Playing) {
			UpdateGUI ();
			if (Input.GetKeyDown (KeyCode.Escape)) {
				game.ToggleGamePaused ();
			}
			if (game.PlayerInBastion && Input.GetButtonDown ("Inventory")) {
				game.ToggleBastionMenu ();
			}

			if (!game.gamePaused) {
            
				//UpdatePerFrame
				int remainingTime = (int)Mathf.Max (0, game.endTime - Time.time);
				textRemainingTime.text = remainingTime / 60 + ":" + (remainingTime % 60).ToString ("00");
				iconRemainingTime.fillAmount = (float)remainingTime / game.roundDuration;
				if (bastionTransform != null) { //JingYi: quickfix to avoid wall of errors after the bastion is destroyed by the black whole and the player is still on a higher island
					UpdateBastionDirectionIcon ();
				}
            
				#region TODO: put in proper references!! 
				//iconSkillCooldownBar.fillAmount = (0.1f * Time.time) % 1.0f;
				#endregion

			}
			if (game.gamePaused) {
				//show seed in pause menu
			}
			//come on, would be the coolest cheat mode ever! when you enter it, it displays "God mode entered" on the screen and Thor occasionally throws/drops his hammer onto your current position, and you have to dodge it, in return for unlimited health (for mortal enemies) and time
			//stealth bushes 
			//ruin island with survivor and chests; keys as rare drop from enemies
			//add small, floating/moving rocks to world generation? (more atmospheric)
			//rotating sun
			//finish survivor display!
		}
    }

    protected void UpdateCursor () {
        bool cursorNeeded = game.gamePaused || game.BastionMenu;
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
            case GUIUpdateEvent.Tool:
                UpdateToolState();
                break;
            case GUIUpdateEvent.Pause:
                UpdatePauseState();
                break;
            case GUIUpdateEvent.BastionMenu:
                UpdateBastionMenuState();
                break;
            case GUIUpdateEvent.Layer:
                UpdateLayerState();
                break;
            case GUIUpdateEvent.All:
                UpdateEnergyState();
                UpdateWoodState();
                UpdateArtifactState();
                UpdateHealthState();
                UpdateToolState();
                UpdatePauseState();
                UpdateBastionMenuState();
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
        if(game.healthpointsPrev > game.healthpointsCur) {
            //StartCoroutine(FadeDamageOverlay());
            damageScreenOverlay.CrossFadeColor(damageOverlayColorActive, 0.0f, false, true);
            damageScreenOverlay.CrossFadeColor(damageOverlayColorRegular, 1.0f, false, true);
        }
    }

    protected void UpdateToolState () {
        //refresh text
        currentToolText.text = hook.Hooked ? "Rope" : combat.GetCurrentWeaponName();
        currentToolDescription.text = hook.Hooked ? hook.ToString() : combat.GetCurrentWeaponDescription();
        bool pistol = combat.ActiveWeaponIndex == 0 ? true : false;
        //highlight active weapon
        selectedPistol.enabled = pistol;
        selectedShotgun.enabled = !pistol;
        //switch between tool icons
        if (hook.Hooked) {
            iconRope.enabled = true;
            iconPistol.enabled = iconShotgun.enabled = false;
        }
        else {
            iconRope.enabled = false;
            iconPistol.enabled = pistol;
            iconShotgun.enabled = !pistol;
        }
    }

    protected void UpdatePauseState () {
        bool paused = game.gamePaused;
        //GUI_Ingame.gameObject.SetActive(!paused);
        GUI_Ingame.interactable = !paused;
        GUI_Ingame.alpha = paused ? 0f : 1f;
        GUI_PauseMenu.gameObject.SetActive(paused);
        //GUI_PauseMenu.interactable = paused;
        pauseScreenOverlay.CrossFadeAlpha(paused ? 1.0f : 0.0f, 0.5f, true);
        if (!paused) { GUI_Controls.gameObject.SetActive(false); }
        UpdateCursor();
    }

    protected void UpdateBastionMenuState () {
        bool menu = game.BastionMenu;
        GUI_Ingame.gameObject.SetActive(!menu);
        GUI_BastionMenu.gameObject.SetActive(menu);
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
        //Debug.DrawRay(playerCamera.transform.position, testDir * 1000, Color.magenta);

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

    /*protected IEnumerator FadeDamageOverlay () {
        //fadingIn++;
        damageScreenOverlay.CrossFadeColor(damageOverlayColorActive, 0.0f, false, true);
        //yield return new WaitForSeconds(0.1f);
        //damageScreenOverlay.color = damageOverlayColorActive;
        //fadingIn--;
        yield return new WaitForEndOfFrame();
        damageScreenOverlay.CrossFadeColor(damageOverlayColorRegular, 1.0f, false, true);
        //if(fadingIn <= 0) {
        //    yield return new WaitForSeconds(1.0f);
        //    damageScreenOverlay.color = damageOverlayColorRegular;
        //}
    }*/
}
