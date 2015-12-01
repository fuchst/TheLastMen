using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject UIInventory;
    public GameObject player;
    LevelManager levelManager;

    Camera worldCam;    //Used for a future feature#

    string playerPrefabPath = "Player";

    void Awake()
    {
        if (UIInventory == null)
        {
            UIInventory = GameObject.Find("UIInventory");
            if(UIInventory == null)
            {
                Debug.LogError("No UIInventory found");
            }
        }
        levelManager = GetComponent<LevelManager>();
        worldCam = Camera.main;
    }

    void Start()
    {
        //Create Game World
        levelManager.CreateLevel();

        //Setup Player
        if (player == null)
        {
            player = Resources.Load(playerPrefabPath, typeof(GameObject)) as GameObject;
        }
        levelManager.StartLevel(player);
        player.GetComponent<Inventory>().SetUIInventory(UIInventory);
        Destroy(worldCam.gameObject);
    }
}
