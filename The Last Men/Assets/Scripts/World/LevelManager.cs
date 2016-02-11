using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum GameState { Loading, Playing };

    [HideInInspector] public GameState gameState = GameState.Loading;
    public GameObject playerPrefab;
    public bool showPaths;
    [HideInInspector] public GameObject bastion;
    [HideInInspector] public GameObject player;
    public IslandPrefabs islandPrefabs;
    public LevelVariables[] levelVariables = new LevelVariables[3];

    [HideInInspector] public Transform flyingEnemyParent;
    [HideInInspector] public Transform islandParent;
    [HideInInspector] public Vector3 playerSpawnPos;

    public float createLevelCoroutineCounter = 2.0f;

    [SerializeField] private bool menuIsInSameLevel;
    [SerializeField] private int rngSeed = 1337;
    [SerializeField] private float maxFallingSpeed = 0.2f;
    [SerializeField] private int maxFlyingEnemiesPerLevel = 25;
    [SerializeField] private GameObject blackHole;
    [SerializeField] private GameObject levelParticles;
    private float islandFallingSpeed = 2.0f;
    private static LevelManager instance;

    //[HideInInspector] public Camera worldCam;
    public Camera worldCam;
    [HideInInspector] public Camera playerCam;

    private Level[] levels;
    private int currentLevel = 0;
    
    void Awake()
    {
        if (instance) { Destroy(this); }
        else { instance = this; }

        //Check prefabs
        bool prefabsSet = true;
        if (islandPrefabs.Bastion == null || islandPrefabs.AritfactIsland == null)
        {
            prefabsSet = false;
        }
        foreach (GameObject go in islandPrefabs.BigIslands)
        {
            if (go == null)
            {
                prefabsSet = false;
                break;
            }
        }
        foreach (GameObject go in islandPrefabs.SmallIslands)
        {
            if (go == null)
            {
                prefabsSet = false;
                break;
            }
        }
        if (prefabsSet == false)
        {
            Debug.LogError("At least one prefab is not linked to LevelManager");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        if (!flyingEnemyParent)
        {
            flyingEnemyParent = new GameObject("FlyingEnemyParent").transform;
        }
        if (!islandParent)
        {
            islandParent = new GameObject("IslandParent").transform;
        }

        //Gather island bounds
        GameObject tmp;
        islandPrefabs.bigIslandWidths = new float[islandPrefabs.BigIslands.Length];
        for (int i = 0; i < islandPrefabs.bigIslandWidths.Length; i++)
        {
            tmp = Instantiate(islandPrefabs.BigIslands[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            islandPrefabs.bigIslandWidths[i] = tmp.GetComponentInChildren<Collider>().bounds.extents.x;
            tmp.SetActive(false);
            Destroy(tmp);
        }
        islandPrefabs.smallIslandWidths = new float[islandPrefabs.SmallIslands.Length];
        for (int i = 0; i < islandPrefabs.smallIslandWidths.Length; i++)
        {
            tmp = Instantiate(islandPrefabs.SmallIslands[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            islandPrefabs.smallIslandWidths[i] = tmp.GetComponentInChildren<Collider>().bounds.extents.x;
            tmp.SetActive(false);
            Destroy(tmp);
        }

        tmp = Instantiate(islandPrefabs.Bastion, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        islandPrefabs.bastionWidth = tmp.GetComponentInChildren<Collider>().bounds.extents.x;
        tmp.SetActive(false);
        Destroy(tmp);
        tmp = Instantiate(islandPrefabs.AritfactIsland, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        islandPrefabs.artifactIslandWidth = tmp.GetComponentInChildren<Collider>().bounds.extents.x;
        tmp.SetActive(false);
        Destroy(tmp);

        if (worldCam == null)
        {
            worldCam = Camera.main;
        }
        PlaceFlyingEnemy.flyingEnemyParent = flyingEnemyParent;
        levels = new Level[levelVariables.Length];

        if (s_GameManager.Instance.useRandomSeed) {
            Random.seed = (int)System.DateTime.UtcNow.Ticks;
        }
        else {
            Random.seed = rngSeed;
        }    

        //Init camera rot
        worldCam.gameObject.transform.position = new Vector3(levelVariables[currentLevel].radius * 2.0f, 0, 0);
        worldCam.gameObject.transform.LookAt(Vector3.zero);
    }

    public void LoadLevel()
    {
        levels[currentLevel] = gameObject.AddComponent<Level>() as Level;
        levels[currentLevel].Radius = levelVariables[currentLevel].radius;
        levels[currentLevel].Cycles = levelVariables[currentLevel].cycles;
        levels[currentLevel].DestructionLevel = levelVariables[currentLevel].destructionLevel;
        levels[currentLevel].ArtifactCount = levelVariables[currentLevel].numberOfArtifacts;
        levels[currentLevel].LayerHeightOffset = levelVariables[currentLevel].heightOffset;
        levels[currentLevel].grapplingIslandExtraheight = levelVariables[currentLevel].grapplingIslandExtraHeight;

        if (menuIsInSameLevel == false)
        {
            levels[currentLevel].CreateLevel();
        }
        else
        {
            levels[currentLevel].CreateLevelInstant();
        }
    }

    public void StartLevel()
    {
        //s_GameManager.Instance.artifactCountMax = levelVariables[currentLevel].numberOfArtifacts;

        //Change to player cam
        worldCam.enabled = false;

        //First time inits
        if(currentLevel == 0)
        {
            playerCam = player.transform.FindChild("MainCamera").GetComponent<Camera>();
            s_GUIMain.Instance.InitGUI();
            bastion.GetComponentInChildren<s_HoveringGUI>().InitGUI();
            player.GetComponent<FireGrapplingHook>().Init();
        }
        else
        {
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Layer);
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.BastionMenu);
        }
        player.SetActive(true);
        playerCam.enabled = true;

        s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Layer);
        
        gameState = GameState.Playing;
        s_GameManager.Instance.LevelLoaded();

        if (levelParticles) {
            Instantiate(levelParticles, Vector3.zero, Quaternion.identity);
        }

        //if (worldCam != null) {
        //    Destroy(worldCam.gameObject);
        //}

        //    s_GUIMain.Instance.InitGUI();
        //    player.transform.FindChild("MainCamera").GetComponent<Camera>().enabled = true;

        //if (currentLevel == 0)
        //{
        //    s_GUIMain.Instance.InitGUI();
        //    player.transform.FindChild("MainCamera").GetComponent<Camera>().enabled = true;
        //}
        //else
        //{
        //    player.transform.position = GetPlayerSpawnPos();
        //    player.SetActive(true);
        //}
        //bastion.GetComponentInChildren<s_HoveringGUI>().InitGUI();
        //gameState = GameState.Playing;
    }

    //public Vector3 GetPlayerSpawnPos()
    //{
    //    Vector3 spawnPos = levels[currentLevel].GetBasePosition();
    //    spawnPos += spawnPos.normalized;
    //    return spawnPos;
    //}

    [System.Serializable]
    public struct LevelVariables
    {
        public float radius;
        public int cycles;
        public int numberOfArtifacts;
        public float destructionLevel;
        public float heightOffset;
        public float grapplingIslandExtraHeight;
    }

    [System.Serializable]
    public class IslandPrefabs
    {
        public GameObject[] BigIslands;
        public float[] bigIslandWidths;
        public GameObject[] SmallIslands;
        public float[] smallIslandWidths;
        public GameObject Bastion;
        public float bastionWidth;
        public GameObject AritfactIsland;
        public float artifactIslandWidth;
        public GameObject[] UniqueArtifacts;
        public GameObject[] UniqueSmallIslands;
    }

    public Vector3 UpdatePlayerSpawnPos () {
        Vector3 spawnPos = bastion.transform.FindChild("Spawn").transform.position;
        spawnPos += spawnPos.normalized;
        playerSpawnPos = spawnPos;
        return spawnPos;
    }

    public void RestartLevel()
    {
        gameState = GameState.Loading;
        Application.LoadLevel(Application.loadedLevel);
        s_GameManager.Instance.SetGamePaused(false);
    }

    public void AdvanceLevel()
    {
        //If go to menu instead
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
        //Application.LoadLevel(0);

        gameState = GameState.Loading;
        playerCam.enabled = false;
		player.SetActive(false);

        currentLevel++;
        worldCam.gameObject.transform.position = new Vector3(levelVariables[currentLevel].radius * 2.0f, 0, 0);
        worldCam.gameObject.transform.LookAt(Vector3.zero);
        worldCam.enabled = true;

        Destroy(flyingEnemyParent.gameObject);
        flyingEnemyParent = (new GameObject("FlyingEnemyParent") as GameObject).transform;
        PlaceFlyingEnemy.currentFlyingEnemies = 0; //reset enemy count after enemies are destroyed!
        PlaceFlyingEnemy.maxFlyingEnemies = maxFlyingEnemiesPerLevel + 5 * currentLevel; //slightly increase limit in later layers to account for bigger level size

        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in islandParent)
        {
            children.Add(child.gameObject);

        }
        children.RemoveAt(0);   //we dont want to destroy our bastion
        children.ForEach(child => Destroy(child));

        Destroy(levels[currentLevel]);

        if (currentLevel < levels.Length)
        {
            LoadLevel();
        }

        //make this only when level is done loading
        //s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Layer);

        ////Destroy the levels script/component
        //Destroy(levels[currentLevel]);
    }

    //Get,Set Methods
    public static LevelManager Instance { get { return instance; } }
    public int CurLvl { get { return currentLevel; } }
    public float MaxFallingSpeed { get { return maxFallingSpeed; } }
    public float IslandFallingSpeed { get { return islandFallingSpeed; } set { islandFallingSpeed = value; } }
    public int RngSeed { get { return rngSeed; } }
    public GameObject BlackHole { get { return blackHole; } }
}
