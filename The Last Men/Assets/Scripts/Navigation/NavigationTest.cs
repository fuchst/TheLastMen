using UnityEngine;
using System.Collections;

public class NavigationTest : MonoBehaviour {

    public NavigationGrid grid;
	
	// Update is called once per frame
	void Update () {
        grid.GetClosestNode(this.transform.position);
	}
}
