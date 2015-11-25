using UnityEngine;
using System.Collections;

public class BS : MonoBehaviour {

    public GameObject island;

    private float timer = 0;

	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer < 2)
        {
            Vector3 pos = Random.insideUnitSphere * 100.0f;
            Quaternion rot = Random.rotation;
            Instantiate(island, pos, rot);
        }
	}
}
