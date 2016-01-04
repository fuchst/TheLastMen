using UnityEngine;
using UnityEngine.UI;

public class s_GUIMain : MonoBehaviour {
	[SerializeField]protected Canvas canvas;
	protected RectTransform canvasRT;

	[SerializeField]protected Image iconPlayerBastion;
    [SerializeField]protected Image iconPlayerBastionDirection;
    [SerializeField]protected Image iconPlayerBastionFrame;
    [SerializeField]protected int screenBorderThreshold = 50;
    [SerializeField]protected float lerpingSpeedFactor = 5.0f;
    [SerializeField]protected Color colorBastionNear;
    [SerializeField]protected Color colorBastionFar;
    [Tooltip("color reaches \"far\" value at distances of distanceBastionFar and above")]
    [SerializeField]protected float distanceBastionFar = 250.0f; //color reaches "far" value at distances of distanceBastionFar and above

    //use radial or straight fill of icon, or bar around icon, or just text values?
    [SerializeField]protected Image iconPlayerHealth;
	[SerializeField]protected Image iconPlayerEnergy;

    [SerializeField]protected Image iconArtifacts1; //ancient thrust
    [SerializeField]protected Image iconArtifacts2; //gravitational freeze

	[SerializeField]protected Image iconRemainingTime;
    [SerializeField]protected Image iconCurrentLayer;


	[SerializeField]protected Text textPlayerHealth;
	[SerializeField]protected Text textPlayerEnergy;

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
        
		if(!bastionTransform){

			bastionTransform = new GameObject("bastion").transform;
			bastionTransform.position = playerTransform.position - playerTransform.up * 0.99f * playerTransform.localScale.y;
            bastionTransform.rotation = playerTransform.rotation;
            bastionTransform.gameObject.AddComponent<SpriteRenderer>();
            bastionTransform.gameObject.GetComponent<SpriteRenderer>().sprite = iconPlayerBastion.sprite;
        }

        min = 0 + screenBorderThreshold;
        maxX = Screen.width - screenBorderThreshold;
        maxY = Screen.height - screenBorderThreshold;
    }
	
	void Update () {
        //If we are in a test level and we dont have a GameManager
        if(s_GameManager.Instance == null)
        {
            return;
        }

		int remainingTime = (int)(s_GameManager.Instance.endTime - Time.time);
		textRemainingTime.text =  remainingTime/60 + ":" + remainingTime%60;

        //TODO: only update when changing?
        textArtifacts1.text = s_GameManager.Instance.artifactCountCur.ToString();
        iconArtifacts1.fillAmount = s_GameManager.Instance.artifactCountCur / s_GameManager.Instance.artifactCountMax;
        textPlayerHealth.text = s_GameManager.Instance.healthpoints.ToString();

        UpdateBastionDirectionIcon();
	}

    //TODO: possibly change color, transparency, size?
    void UpdateBastionDirectionIcon () {
        offscreen = false;

        Vector3 bastionScreenPos = playerCamera.WorldToScreenPoint (bastionTransform.position);
        
        if(bastionScreenPos.z < 0)
        {
            bastionScreenPos *= -1;
            offscreen = true;
        }

        Vector2 originalScreenPos = bastionScreenPos;
        bastionScreenPos.x = Mathf.Clamp(bastionScreenPos.x, min, maxX);
        bastionScreenPos.y = Mathf.Clamp(bastionScreenPos.y, min, maxY);
        if (originalScreenPos != (Vector2) bastionScreenPos)
        {
            offscreen = true;
        }

        Vector2 bastionCanvasPos = ScreenToCanvasSpace(bastionScreenPos, canvasRT.sizeDelta);

        //lerp bastion icon to avoid jittering
        iconPlayerBastion.rectTransform.anchoredPosition = Vector2.Lerp(iconPlayerBastion.rectTransform.anchoredPosition, bastionCanvasPos, lerpingSpeedFactor * Time.deltaTime);

        //show direction arrow
        iconPlayerBastionDirection.enabled = offscreen;
        float angle = -90 + Mathf.Rad2Deg * Mathf.Atan2(x:originalScreenPos.x, y:originalScreenPos.y);
        iconPlayerBastionDirection.rectTransform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //change color based on distance
        float lerpParameter = (playerTransform.position - bastionTransform.position).sqrMagnitude / (distanceBastionFar * distanceBastionFar); 
        iconPlayerBastionFrame.color = Colorx.Slerp(colorBastionNear, colorBastionFar, lerpParameter);
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
