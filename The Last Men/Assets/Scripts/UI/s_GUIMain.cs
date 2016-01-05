using UnityEngine;
using UnityEngine.UI;

public class s_GUIMain : MonoBehaviour {
	[SerializeField]protected Canvas canvas;
	protected RectTransform canvasRT;

	[SerializeField]protected Image iconBastion;
    [SerializeField]protected Image iconBastionEnergy;
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
    
    
    [SerializeField]protected Image iconPlayerHealth;
	[SerializeField]protected Image iconPlayerEnergy;
    [SerializeField]protected Image iconPlayerWood;

    [SerializeField]protected Image iconArtifacts1; //ancient thrust
    [SerializeField]protected Image iconArtifacts2; //gravitational freeze

	[SerializeField]protected Image iconRemainingTime;
    [SerializeField]protected Image iconCurrentLayer;

    [SerializeField]protected Image iconSkillCooldownBar;
    [SerializeField]protected Button buttonPause;


	[SerializeField]protected Text textPlayerHealth;
	[SerializeField]protected Text textPlayerEnergy;
    [SerializeField]protected Text textPlayerWood;

    [SerializeField]protected Text textArtifacts1;
    [SerializeField]protected Text textArtifacts2;

	[SerializeField]protected Text textRemainingTime;
    [SerializeField]protected Text textCurrentLayer;


    //get from game manager later?
    [SerializeField]protected Transform bastionTransform;
	[SerializeField]protected Transform playerTransform;
	[SerializeField]protected Camera playerCamera;

    
    //for bastion indicator
    protected bool offscreen = false;
    protected int min, maxX, maxY;
    

	// Use this for initialization
	void Start () {
		canvasRT = canvas.GetComponent<RectTransform> ();

        if (!playerTransform) {

        }

        if (!playerCamera) {

        }

		if(!bastionTransform){

			bastionTransform = new GameObject("bastion").transform;
			bastionTransform.position = playerTransform.position - playerTransform.up * 0.99f * playerTransform.localScale.y;
            bastionTransform.rotation = playerTransform.rotation;
            bastionTransform.gameObject.AddComponent<SpriteRenderer>();
            bastionTransform.gameObject.GetComponent<SpriteRenderer>().sprite = iconBastion.sprite;
        }

        min = 0 + screenBorderThreshold;
        maxX = Screen.width - screenBorderThreshold;
        maxY = Screen.height - screenBorderThreshold;

        buttonPause.onClick.AddListener(() => { s_GameManager.Instance.SwitchGamePaused(); });
    }
	
	// Update is called once per frame
	void Update () {
        s_GameManager game = s_GameManager.Instance;

        int remainingTime = (int)(game.endTime - Time.time);
		textRemainingTime.text =  remainingTime/60 + ":" + remainingTime%60;
        iconRemainingTime.fillAmount = (float)remainingTime / game.roundDuration;

        //TODO: only update when changing?
        textArtifacts1.text = game.artifact1CountCur.ToString();
        iconArtifacts1.fillAmount = game.artifact1CountCur / game.artifactCountMax;
        textArtifacts2.text = game.artifact2CountCur.ToString();
        iconArtifacts2.fillAmount = game.artifact2CountCur / game.artifactCountMax;

        textPlayerHealth.text = game.healthpointsCur.ToString();
        iconPlayerHealth.fillAmount = (float)game.healthpointsCur / (float)game.healthpointsMax;

        //TODO: put in proper references!! 
        iconSkillCooldownBar.fillAmount = (0.1f * Time.time) % 1.0f;
        iconPlayerWood.fillAmount = 0.4f;
        iconPlayerEnergy.fillAmount = 0.75f;
        iconBastionEnergy.fillAmount = Mathf.PingPong(0.25f * Time.time, 1.0f);
        

        UpdateBastionDirectionIcon();
	}

    //TODO: possibly change color, transparency, size?
    void UpdateBastionDirectionIcon () {
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

    /*
    void UpdateBastionDirectionIconOld () {
        Vector3 bastionViewportPos = playerCamera.WorldToViewportPoint (bastionTransform.position);

        float min, max, rad, screenBorderThresholdF;
        screenBorderThresholdF = screenBorderThreshold * 0.01f; //initially was 0.05f * screen
        min = 0 + screenBorderThresholdF;
        max = 1 - screenBorderThresholdF;
        rad = 0.5f - screenBorderThresholdF;

        //if bastion is visible from camera
        if (bastionViewportPos.z > 0 &&
			bastionViewportPos.x > min && bastionViewportPos.x < max &&
			bastionViewportPos.y > min && bastionViewportPos.y < max) {

			iconPlayerBastion.gameObject.SetActive(true);
            //iconPlayerBastion.rectTransform.localPosition = bastionViewportPos;

            Vector2 bastionScreenPos = ViewportToCanvasSpace(bastionViewportPos, canvasRT.sizeDelta);

			//lerp to avoid jittering
			iconPlayerBastion.rectTransform.anchoredPosition = Vector2.Lerp(iconPlayerBastion.rectTransform.anchoredPosition, bastionScreenPos, lerpingSpeedFactor * Time.deltaTime);
		}
		//if bastion is somewhere not behind the player
		else if (bastionViewportPos.z > 0) {

            iconPlayerBastion.gameObject.SetActive(true);

            bastionViewportPos.x = Mathf.Clamp(bastionViewportPos.x, min, max);
            bastionViewportPos.y = Mathf.Clamp(bastionViewportPos.y, min, max);

            //float distX = Mathf.Abs(0.5f - bastionViewportPos.x);
            //float distY = Mathf.Abs(0.5f - bastionViewportPos.y);
            //if (Mathf.Max(distX, distY) > rad) { 
                // out of screen 
            //}

            //float offsetX = bastionViewportPos.x - 0.5f;
            //float offsetY = bastionViewportPos.y - 0.5f;
            //float maxAbsOffset = Mathf.Max(Mathf.Abs(offsetX), Mathf.Abs(offsetY));
            //bastionViewportPos *= rad/maxAbsOffset;
            //if (maxAbsOffset == Mathf.Abs(offsetX)){
            //    bastionViewportPos *= rad/offsetX;
            //}
            //else {
            //    bastionViewportPos *= rad/offsetY;
            //}
            //float maxAbsOffset = Mathf.Max(Mathf.Abs(offsetX), Mathf.Abs(offsetY));
            //bastionViewportPos.x *= maxAbsOffset * Mathf.Sign(offsetX);
            //bastionViewportPos.y *= maxAbsOffset * Mathf.Sign(offsetY);
            //bastionViewportPos *= rad/Mathf.Max(offsetX, offsetY);

            Vector2 bastionScreenPos = ViewportToCanvasSpace(bastionViewportPos, canvasRT.sizeDelta);

            //lerp to avoid jittering
            iconPlayerBastion.rectTransform.anchoredPosition = Vector2.Lerp(iconPlayerBastion.rectTransform.anchoredPosition, bastionScreenPos, lerpingSpeedFactor * Time.deltaTime);
        }
        else {
            iconPlayerBastion.gameObject.SetActive(false);
        }
    }
    */

    Vector2 ViewportToCanvasSpace (Vector3 posViewport, Vector2 canvasSizeDelta) {
        return new Vector2((posViewport.x - 0.5f) * canvasSizeDelta.x, (posViewport.y - 0.5f) * canvasSizeDelta.y);
    }

    Vector2 ScreenToCanvasSpace (Vector3 posScreen, Vector2 canvasSizeDelta) {
        posScreen.x *= 1.0f/Screen.width;
        posScreen.y *= 1.0f/Screen.height;
        return new Vector2((posScreen.x - 0.5f) * canvasSizeDelta.x, (posScreen.y - 0.5f) * canvasSizeDelta.y);
    }
}
