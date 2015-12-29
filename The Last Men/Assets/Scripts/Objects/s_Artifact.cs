using UnityEngine;

public class s_Artifact : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		if (other.tag.Equals ("Player")) {
			//other.GetComponent<s_Player>().AddArtifact();
			s_GameManager.Instance.artifactCountCur++;
			GetComponent<Collider>().enabled = false;
			foreach(MeshRenderer r in GetComponentsInChildren<MeshRenderer>()){
				r.enabled = false;
			}
			Destroy(gameObject, 2.0f);
		}
	}
}
