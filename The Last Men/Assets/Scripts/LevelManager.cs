using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get { return instance; } }
    public int rngSeed = 1337;
    public GameObject player;
    public LevelVariables[] levelVariables = new LevelVariables[3];

    public bool showPaths = false;

    public GameObject[] bigIslands;
    public GameObject islandBasic;
    public GameObject islandBastion;
    public GameObject islandArtifact;
    public GameObject islandGrappling;
    public GameObject islandSmall;

    private static LevelManager instance;
    private Camera worldCam;
    private Level[] levels = new Level[4];
    private int currentLevel = 0;

    void Awake()
    {
        if (instance)
            Destroy(this);
        else
            instance = this;

        worldCam = Camera.main;
    }

    public void LoadLevel()
    {
        CreateLevel();
        if (worldCam != null)
        {
            Destroy(worldCam.gameObject);
        }
        if (currentLevel == 0)
        {
            StartLevel();
        }
    }

    void CreateLevel()
    {
        levels[currentLevel] = gameObject.AddComponent<Level>() as Level;
        levels[currentLevel].randomSeed = rngSeed;
        levels[currentLevel].radius = levelVariables[currentLevel].radius;
        levels[currentLevel].cycles = levelVariables[currentLevel].cycles;
        levels[currentLevel].destructionLevel = levelVariables[currentLevel].destructionLevel;
        levels[currentLevel].artifactCount = levelVariables[currentLevel].numberOfArtifacts;
        levels[currentLevel].layerHeightOffset = levelVariables[currentLevel].heightOffset;
        levels[currentLevel].grapplingIslandExtraheight = levelVariables[currentLevel].grapplingIslandExtraHeight;
        levels[currentLevel].CreateWorld();
        //levels[currentLevel].ColorizeIslands();
        s_GameManager.Instance.artifactCountMax = levelVariables[currentLevel].numberOfArtifacts;

        //int nextLevel = currentLevel + 1;
        //if (nextLevel < levels.Length)
        //{
        //    //Create the next layer for visuals only
        //    levels[nextLevel] = gameObject.AddComponent<Level>() as Level;
        //    levels[nextLevel].randomSeed = rngSeed;
        //    levels[nextLevel].radius = levelVariables[nextLevel].radius;
        //    levels[nextLevel].cycles = levelVariables[nextLevel].cycles;
        //    levels[nextLevel].destructionLevel = levelVariables[nextLevel].destructionLevel;
        //    levels[nextLevel].numberOfArtifacts = levelVariables[nextLevel].numberOfArtifacts;
        //    levels[nextLevel].heightOffset = levelVariables[nextLevel].heightOffset;
        //    levels[nextLevel].grapplingIslandExtraheight = levelVariables[nextLevel].grapplingIslandExtraHeight;
        //    levels[nextLevel].CreateWorld();
        //}
    }

    public void StartLevel()
    {
        Vector3 spawnPos = levels[currentLevel].GetBasePosition();
        spawnPos += spawnPos.normalized;
        if (currentLevel == 0)
        {
            player = Instantiate(player, spawnPos, Quaternion.identity) as GameObject;

        }
        else
        {
            player.transform.position = spawnPos;
        }
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

    public void AdvanceLevel()
    {
        levels[currentLevel].DestroyLevel();
        Destroy(levels[currentLevel]);
        if (currentLevel < levels.Length)
        {
            levels[currentLevel + 1].DestroyLevel();
            Destroy(levels[currentLevel + 1]);
            currentLevel++;
            LoadLevel();
        }
    }
}
