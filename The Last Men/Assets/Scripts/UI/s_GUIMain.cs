using UnityEngine;
using UnityEngine.UI;

public class s_GUIMain : MonoBehaviour {
	[SerializeField]protected Canvas canvas;
	protected RectTransform canvasRT;
	[SerializeField]protected Image iconPlayerBastion;

	//use radial or straight fill of icon, or bar around icon, or just text values?
	[SerializeField]protected Image iconPlayerHealth;
	[SerializeField]protected Image iconPlayerEnergy;
	[SerializeField]protected Image iconPlayerArtifacts;

	[SerializeField]protected Image iconRemainingTime;


	[SerializeField]protected Text textPlayerHealth;
	[SerializeField]protected Text textPlayerEnergy;
	[SerializeField]protected Text textPlayerArtifacts;

	[SerializeField]protected Text textRemainingTime;

	//get from game manager later?
	[SerializeField]protected Transform bastionTransform;
	[SerializeField]protected Transform playerTransform;
	[SerializeField]protected Camera playerCamera;

	// Use this for initialization
	void Start () {
		canvasRT = canvas.GetComponent<RectTransform> ();
		if(!bastionTransform){

			bastionTransform = new GameObject("bastion").transform;
			bastionTransform.position = playerTransform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		int remainingTime = (int)(s_GameManager.Instance.endTime - Time.time);
		textRemainingTime.text =  remainingTime/60 + ":" + remainingTime%60;
        textPlayerArtifacts.text = s_GameManager.Instance.artifactCount.ToString();
        textPlayerHealth.text = s_GameManager.Instance.healthpoints.ToString();

		Vector3 bastionViewpointPos = playerCamera.WorldToViewportPoint (bastionTransform.position);

		//if bastion is visible from camera
		if (bastionViewpointPos.z > 0 &&
			bastionViewpointPos.x > 0 && bastionViewpointPos.x < 1 &&
			bastionViewpointPos.y > 0 && bastionViewpointPos.y < 1) {

			iconPlayerBastion.enabled = true;
			//iconPlayerBastion.rectTransform.localPosition = bastionViewpointPos;

			Vector2 bastionScreenPos = new Vector2((bastionViewpointPos.x - 0.5f) * canvasRT.sizeDelta.x, 
			                                       (bastionViewpointPos.y - 0.5f) * canvasRT.sizeDelta.y);

			//lerp to avoid jittering
			iconPlayerBastion.rectTransform.anchoredPosition = Vector2.Lerp(iconPlayerBastion.rectTransform.anchoredPosition, bastionScreenPos, 0.1f);
		}
		//...
		else {
			iconPlayerBastion.enabled = false;
		}
	}
}
