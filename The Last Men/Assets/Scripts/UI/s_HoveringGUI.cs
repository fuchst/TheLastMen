using UnityEngine;

public class s_HoveringGUI : MonoBehaviour {

    [SerializeField]protected Canvas canvas;
    [SerializeField]protected CanvasGroup canvasGroup;
    [SerializeField]protected float maxCameraDistanceForDisplay = 15.0f;
    [Range(0.0f, 1.0f)][SerializeField]protected float fadeOutStart = 0.75f;
    [Range(0.0f, 1.0f)][SerializeField]protected float regularOpacity = 1.0f;

    protected RectTransform rectTrans;
    protected Transform player;
    
    public void InitGUI () {
        if (RenderMode.WorldSpace != canvas.renderMode) {
            enabled = false;
        }
        rectTrans = canvas.GetComponent<RectTransform>();
        player = LevelManager.Instance.player.transform;
    }
	
	void Update () {
        if (LevelManager.Instance.gameState == LevelManager.GameState.Playing)
        {
            UpdateAppearance();
        }
	}

    protected void UpdateAppearance () {
        Vector3 dirToCam = rectTrans.position - player.position;
		float distToCam = dirToCam.magnitude;
		
        //TODO: refactor bastion menu open/close state to be in game manager, and check this here!!!
        //maybe only show when in trigger
        if(true && !s_GameManager.Instance.gamePaused && distToCam <= maxCameraDistanceForDisplay){
			canvas.enabled = true;
			//rectTrans.rotation = Quaternion.LookRotation(dirToCam);
            rectTrans.rotation = Quaternion.LookRotation(player.forward, player.up);
			
			if(distToCam <= maxCameraDistanceForDisplay * fadeOutStart){
                //set original opacity
                canvasGroup.alpha = regularOpacity;
			}
			else{
				//fade opacity
				float fadedAlpha = Mathf.Lerp(0, regularOpacity, Mathf.InverseLerp(maxCameraDistanceForDisplay, maxCameraDistanceForDisplay * fadeOutStart, distToCam));
                canvasGroup.alpha = fadedAlpha;
			}
		}
		else{
			canvas.enabled = false;
		}
	}
}
