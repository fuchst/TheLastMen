using UnityEngine;
using UnityEngine.UI;

public class s_GUIPopupMessage : MonoBehaviour {

    [SerializeField]protected CanvasGroup canvGroup;
    [SerializeField]protected Image image;
    [SerializeField]protected float duration;
    [SerializeField]protected float upSpeed = 100;
    protected float remainingTime;

    protected RectTransform rectTransform;

    public void Initialize (RectTransform spawnPos, Sprite icon) {
        image.sprite = icon;
        canvGroup.alpha = 1;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.SetParent(spawnPos);
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = 1.5f * Vector3.one;
        remainingTime = duration;
        Destroy(gameObject, duration);
    }
	
	// Update is called once per frame
	void Update () {
        remainingTime -= Time.deltaTime;
        //rectTransform.anchoredPosition += upSpeed * Vector2.up * Time.deltaTime;
        //float adjustedDelta = (1 / duration) * Time.deltaTime;
        //canvGroup.alpha -=  adjustedDelta;
        //rectTransform.localScale -= adjustedDelta * Vector3.one;
        float lerpParameter = 1 - Mathf.Pow((1 - remainingTime / duration), 2);
        canvGroup.alpha = lerpParameter;
        rectTransform.localScale = Mathf.Lerp(0.25f, 1.25f, lerpParameter) * Vector3.one;
        rectTransform.anchoredPosition += (1.5f - lerpParameter) * upSpeed * Vector2.up * Time.deltaTime;

    }
}
