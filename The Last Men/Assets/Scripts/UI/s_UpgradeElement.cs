using UnityEngine;
using UnityEngine.UI;

public class s_UpgradeElement : MonoBehaviour {

    [SerializeField]protected RectTransform rect;
    [SerializeField]protected Toggle toggle;
    [SerializeField]protected Text text;

    protected RectTransform rectTransform;

    public void Initialize (RectTransform parent, ToggleGroup toggleGroup, s_GameManager.UpgradeSettings.UpgradeTypes type, string title) {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.SetParent(parent);
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener((value) => { if (value) { s_GUIMain.Instance.SwitchSelectedUpgrade(type); } });
        text.text = title;
    }
}
