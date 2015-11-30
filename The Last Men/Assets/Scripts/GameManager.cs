using UnityEngine;

public class GameManager : MonoBehaviour
{
    LevelManager levelManager;
    GameObject player;
    Camera worldCam;    //Used for a future feature
    string playerPrefabPath = "Player";

    void Awake()
    {
        levelManager = GetComponent<LevelManager>();
        worldCam = Camera.main;
        player = Resources.Load(playerPrefabPath, typeof(GameObject)) as GameObject;
    }

    void Start()
    {
        levelManager.CreateLevel();
        levelManager.StartLevel(player);
        Destroy(worldCam.gameObject);
    }
}
