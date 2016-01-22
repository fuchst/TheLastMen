using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get { return instance; } }
    public int CurLvl { get { return currentLevel; } }
    public float MaxFallingSpeed { get { return maxFallingSpeed; } }
    public float IslandFallingSpeed { get { return islandFallingSpeed; } set { islandFallingSpeed = value; } }
    public bool ShowPaths { get { return showPaths; } }
    public int RngSeed { get { return rngSeed; } }
    public GameObject BlackHole { get { return blackHole; } }
    
    [SerializeField] protected GameObject playerPrefab;
    public GameObject player;
    public IslandPrefabs islandPrefabs;
    public LevelVariables[] levelVariables = new LevelVariables[3];
    [HideInInspector] public GameObject bastion;
    public Transform flyingEnemyParent;
    
    private static LevelManager instance;
    [SerializeField] private int rngSeed = 1337;
    [SerializeField] private bool showPaths = false;
    [SerializeField] private float maxFallingSpeed = 0.2f;
    [SerializeField] private float islandFallingSpeed = 2.0f;
    [SerializeField] private GameObject blackHole;
    private Camera worldCam;
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

        worldCam = Camera.main;
        PlaceFlyingEnemy.flyingEnemyParent = flyingEnemyParent;
        levels = new Level[levelVariables.Length];
    }

    public void LoadLevel()
    {
        levels[currentLevel] = gameObject.AddComponent<Level>() as Level;
        levels[currentLevel].randomSeed = rngSeed;
        levels[currentLevel].radius = levelVariables[currentLevel].radius;
        levels[currentLevel].cycles = levelVariables[currentLevel].cycles;
        levels[currentLevel].destructionLevel = levelVariables[currentLevel].destructionLevel;
        levels[currentLevel].artifactCount = levelVariables[currentLevel].numberOfArtifacts;
        levels[currentLevel].layerHeightOffset = levelVariables[currentLevel].heightOffset;
        levels[currentLevel].grapplingIslandExtraheight = levelVariables[currentLevel].grapplingIslandExtraHeight;
        levels[currentLevel].CreateLevel();
        s_GameManager.Instance.artifactCountMax = levelVariables[currentLevel].numberOfArtifacts;

        if (worldCam != null)
        {
            Destroy(worldCam.gameObject);
        }
        bastion = levels[currentLevel].islandParent.GetChild(0).gameObject;
        StartLevel();
    }

    public void StartLevel()
    {
        if (currentLevel == 0)
        {
            player = Instantiate(playerPrefab, GetPlayerSpawnPos(), Quaternion.identity) as GameObject;
        }
        else
        {
            player.transform.position = GetPlayerSpawnPos();
            player.SetActive(true);
        }
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
        player.SetActive(false);
        levels[currentLevel].DestroyLevel();
        Destroy(levels[currentLevel]);
        if (currentLevel < levels.Length)
        {
            //levels[currentLevel + 1].DestroyLevel();
            //Destroy(levels[currentLevel + 1]);
            currentLevel++;
            s_GUIMain.Instance.UpdateGUI(GUIUpdateEvent.Layer);
            LoadLevel();
        }
    }
}
