using UnityEngine;
using UnityEngine.UI;

public class s_DisplayDistanceTest : MonoBehaviour {

    public Transform otherObject;
    public Text distanceText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        distanceText.text = Vector3.Distance(transform.position, otherObject.position).ToString("0.00");
	}
}
