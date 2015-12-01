using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int rngSeed = 1337;
    public LevelVariables[] levelVariables = new LevelVariables[3];

    private Level[] levels = new Level[3];
    private int currentLevel = 0;

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
}
