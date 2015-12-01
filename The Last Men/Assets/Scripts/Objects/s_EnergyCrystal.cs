using UnityEngine;
using System.Collections;

public class s_EnergyCrystal : MonoBehaviour {

	public Vector2 energyLootRange = new Vector2(3.0f, 7.0f);

	void OnTriggerEnter (Collider other) {
		if(other.tag.Equals("Player")){

			//other.GetComponent<s_Player>().AddEnergy(energyLoot);
			Debug.Log("Energy collected.");
			GetComponent<Collider>().enabled = false;
			foreach(MeshRenderer r in GetComponentsInChildren<MeshRenderer>()){
				r.enabled = false;
			}
			Destroy(gameObject, 2.0f);
		}
	}
}
