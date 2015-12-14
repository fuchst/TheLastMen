using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance { get { return instance; } }

    public int rngSeed = 1337;
    public LevelVariables[] levelVariables = new LevelVariables[3];

    private Level[] levels = new Level[3];
    private int currentLevel = 0;

    public GameObject islandBasic;
    public GameObject islandBastion;
    public GameObject islandArtifact;
    public GameObject islandGrappling;

    void Awake()
    {
        if (instance)
            Destroy(this);
        else
            instance = this;

        //if prefab references are not set
        if(islandBasic == null)
            islandBasic = Resources.Load("IslandSimple", typeof(GameObject)) as GameObject;
        if (islandBastion == null)
            islandBastion = Resources.Load("IslandSimple", typeof(GameObject)) as GameObject;
        if (islandArtifact == null)
            islandArtifact = Resources.Load("IslandSimple", typeof(GameObject)) as GameObject;
        if (islandGrappling == null)
            islandGrappling = Resources.Load("IslandSimple", typeof(GameObject)) as GameObject;
    }

    public void CreateLevel()
    {
        levels[currentLevel] = gameObject.AddComponent<Level>() as Level;
        levels[currentLevel].randomSeed = rngSeed;
        levels[currentLevel].radius = levelVariables[currentLevel].radius;
        levels[currentLevel].cycles = levelVariables[currentLevel].cycles;
        levels[currentLevel].destructionLevel = levelVariables[currentLevel].destructionLevel;
        levels[currentLevel].numberOfArtifacts = levelVariables[currentLevel].numberOfArtifacts;
        levels[currentLevel].heightOffset = levelVariables[currentLevel].heightOffset;
        levels[currentLevel].grapplingIslandExtraheight = levelVariables[currentLevel].grapplingIslandExtraHeight;
        levels[currentLevel].CreateWorld();
        levels[currentLevel].ColorizeIslands();
        s_GameManager.Instance.artifactCountMax = levelVariables[currentLevel].numberOfArtifacts;
        int nextLevel = currentLevel + 1;

        if (nextLevel < levels.Length)
        {
            //Create the next layer for visuals only
            levels[nextLevel] = gameObject.AddComponent<Level>() as Level;
            levels[nextLevel].randomSeed = rngSeed;
            levels[nextLevel].radius = levelVariables[nextLevel].radius;
            levels[nextLevel].cycles = levelVariables[nextLevel].cycles;
            levels[nextLevel].destructionLevel = levelVariables[nextLevel].destructionLevel;
            levels[nextLevel].numberOfArtifacts = levelVariables[nextLevel].numberOfArtifacts;
            levels[nextLevel].heightOffset = levelVariables[nextLevel].heightOffset;
            levels[nextLevel].grapplingIslandExtraheight = levelVariables[nextLevel].grapplingIslandExtraHeight;
            levels[nextLevel].PseudoIsland();
            levels[nextLevel].CreateWorld();
            levels[nextLevel].DarkenIslands();
        }
    }

    public void StartLevel(GameObject player)
    {
        Vector3 spawnPos = levels[currentLevel].GetBasePosition();
        spawnPos += spawnPos.normalized;
        Instantiate(player, spawnPos, Quaternion.identity);
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

    }
}
