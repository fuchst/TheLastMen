using UnityEngine;

public class GameManager : MonoBehaviour {
	
	public GameObject player;
	WorldGeneration gameWorld;
    Camera worldCam;

    void Awake() {
        worldCam = Camera.main;
        gameWorld = GetComponent<WorldGeneration>();
        if (!player) {
			player = Resources.Load ("Player", typeof(GameObject)) as GameObject;
		}
    }

    void Start() {
        gameWorld.CreateWorld();
        Vector3 spawnPos = gameWorld.GetBasePosition();
        spawnPos += spawnPos.normalized;
        Instantiate(player, spawnPos, Quaternion.identity);
        Destroy(worldCam.gameObject);
    }
}
