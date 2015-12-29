using UnityEngine;

public class BoundsTest : MonoBehaviour {
    
	void Start () {
        Debug.Log(GetComponent<MeshRenderer>().bounds.size);
	}
}
