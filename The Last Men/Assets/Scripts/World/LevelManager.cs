using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] protected GameObject playerPrefab;
    public GameObject bastion;
    public GameObject player;
    public IslandPrefabs islandPrefabs;
    public LevelVariables[] levelVariables = new LevelVariables[3];
    public Transform flyingEnemyParent;
    public Transform islandParent;
	[HideInInspector] public bool levelLoaded = false;
    
    [SerializeField] private int rngSeed = 1337;
    [SerializeField] private bool showPaths = false;
    [SerializeField] private float maxFallingSpeed = 0.2f;
    [SerializeField] private float islandFallingSpeed = 2.0f;
    [SerializeField] private GameObject blackHole;
    private static LevelManager instance;
    
	public Camera worldCam;
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
        if (!flyingEnemyParent) {
            flyingEnemyParent = new GameObject("FlyingEnemyParent").transform;
        }
        if (!islandParent) {
            islandParent = new GameObject("IslandParent").transform;
        }

        //Gather island bounds
		GameObject tmp;
		islandPrefabs.bigIslandWidths = new float[islandPrefabs.BigIslands.Length];
		for (int i = 0; i< islandPrefabs.bigIslandWidths.Length; i++) {
			tmp = Instantiate(islandPrefabs.BigIslands[i], new Vector3(0,0,0), Quaternion.identity) as GameObject;
			islandPrefabs.bigIslandWidths[i] = tmp.GetComponentInChildren<Collider>().bounds.extents.x;
			Destroy(tmp);
		}
		islandPrefabs.smallIslandWidths = new float[islandPrefabs.SmallIslands.Length];
		for (int i = 0; i< islandPrefabs.smallIslandWidths.Length; i++) {
			tmp = Instantiate(islandPrefabs.SmallIslands[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			islandPrefabs.smallIslandWidths[i] = tmp.GetComponentInChildren<Collider>().bounds.extents.x;
			Destroy(tmp);
		}

		tmp = Instantiate(islandPrefabs.Bastion, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		islandPrefabs.bastionWidth = tmp.GetComponentInChildren<Collider>().bounds.extents.x;
		Destroy(tmp);
		tmp = Instantiate(islandPrefabs.AritfactIsland, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		islandPrefabs.artifactIslandWidth = tmp.GetComponentInChildren<Collider>().bounds.extents.x;
		Destroy(tmp);

		if (worldCam == null) {
			worldCam = Camera.main;
		}
        PlaceFlyingEnemy.flyingEnemyParent = flyingEnemyParent;
        levels = new Level[levelVariables.Length];
        Random.seed = rngSeed;
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

		levels [currentLevel].InitLevelCreation ();

        //levels[currentLevel].CreateLevel();

        //s_GameManager.Instance.artifactCountMax = levelVariables[currentLevel].numberOfArtifacts;

        //if (worldCam != null){ Destroy(worldCam.gameObject); }
       // StartLevel();
    }

    public void StartLevel()
    {
		s_GUIMain.Instance.InitGUI ();
        if (currentLevel == 0)
        {
            player = Instantiate(playerPrefab, GetPlayerSpawnPos(), Quaternion.identity) as GameObject;

        }
        else
        {
            //player.transform.position = GetPlayerSpawnPos();
            //player.SetActive(true);
        }
		levelLoaded = true;
    }

    public Vector3 GetPlayerSpawnPos () {
        Vector3 spawnPos = levels[currentLevel].GetBasePosition();
        spawnPos += spawnPos.normalized;
        return spawnPos;
    }

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
    }

    public void AdvanceLevel()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Application.LoadLevel(0);
        
		/*
		player.SetActive(false);

        //Destroy all islands except the bastion
        while (islandParent.childCount > 1)
        {
            Destroy(islandParent.GetChild(1));
        }
        
        //Destroy the levels script/component
        Destroy(levels[currentLevel]);

        if (currentLevel < levels.Length)
        {
            //levels[currentLevel + 1].DestroyLevel();
            //Destroy(levels[currentLevel + 1]);
            currentLevel++;

            
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Layer);
            LoadLevel();
        }
		*/
    }

    //Get,Set Methods
    public static LevelManager Instance { get { return instance; } }
    public int CurLvl { get { return currentLevel; } }
    public float MaxFallingSpeed { get { return maxFallingSpeed; } }
    public float IslandFallingSpeed { get { return islandFallingSpeed; } set { islandFallingSpeed = value; } }
    public bool ShowPaths { get { return showPaths; } }
    public int RngSeed { get { return rngSeed; } }
    public GameObject BlackHole { get { return blackHole; } }
}
