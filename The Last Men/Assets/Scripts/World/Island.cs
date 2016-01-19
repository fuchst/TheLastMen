using UnityEngine;
using System.Collections.Generic;

public class Island : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnSettings
    {
        public Transform objectParent;
        public GameObject[] trees;
        public GameObject[] bushes;
        public GameObject[] enemies;
        public GameObject[] crystals;
        public float chanceToSpawnATree;
        public float chanceToSpawnABush;
        public int minEnemiesOnIsland;
        public int minCrystalsOnIsland;
        public Vector3 maxSpawnOffSet;
        //NiceToHave: Additional variable for big - small tree distribution
    }

    public SpawnSettings spawnSettings;

    public int priority;

    private IslandNavigation islandNavigation;
    private List<Transform> treeSpawnList = new List<Transform>();
    private List<Transform> bushesSpawnList = new List<Transform>();
    private List<Transform> enemyNCrystalSpawnPosList = new List<Transform>();
	private float fallingSpeed = 0;
    private float extraSpeedFactorOnCollision = 5.0f;
    new private Rigidbody rigidbody;
	
	void Awake(){
		islandNavigation = GetComponentInChildren<IslandNavigation>();
		if (LevelManager.Instance == true) {
			fallingSpeed = LevelManager.Instance.islandFallingSpeed;
		}
		rigidbody = GetComponent<Rigidbody>();

		//Setup spawn references
		Transform spawns = transform.FindChild("Spawns");
		foreach (Transform child in spawns)
		{
			if (child.CompareTag("Spawn: Tree"))
			{
				treeSpawnList.Add(child);
			}
            else if (child.CompareTag("Spawn: Bush"))
            {
                bushesSpawnList.Add(child);
            }
			else if (child.CompareTag("Spawn: Enemy n Crystal"))
			{
				enemyNCrystalSpawnPosList.Add(child);
			}
		}
		if (spawnSettings.enemies.Length == 0)
		{
			spawnSettings.minEnemiesOnIsland = 0;
		}
		else if (spawnSettings.crystals.Length == 0)
		{
			spawnSettings.minCrystalsOnIsland = 0;
		}
		if (spawnSettings.minCrystalsOnIsland + spawnSettings.minEnemiesOnIsland > enemyNCrystalSpawnPosList.Count)
		{
			Debug.LogError("MinCrystalOnIsland + MinEnemiesOnIsland is too high");
			spawnSettings.minCrystalsOnIsland = Mathf.FloorToInt(0.5f * enemyNCrystalSpawnPosList.Count);
			spawnSettings.minEnemiesOnIsland = spawnSettings.minCrystalsOnIsland;
		}
	}

    void Start()
    {
        //Spawn trees
        if (spawnSettings.trees.Length > 0)
        {
            foreach (Transform spawnPos in treeSpawnList)
            {
                if (Random.Range(0.0f, 1.0f) <= spawnSettings.chanceToSpawnATree)
                {
                    int treeType = Random.Range(0, spawnSettings.trees.Length);
                    Vector3 position = spawnPos.position + new Vector3(
                        Random.Range(-spawnSettings.maxSpawnOffSet.x, spawnSettings.maxSpawnOffSet.x),
                        0,
                        Random.Range(-spawnSettings.maxSpawnOffSet.z, spawnSettings.maxSpawnOffSet.z));
                    Quaternion rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                    GameObject tree = Instantiate(spawnSettings.trees[treeType], position, Quaternion.identity) as GameObject;
                    tree.transform.parent = spawnSettings.objectParent;
                    tree.transform.localRotation = rotation;
                }
            }
        }

        if (spawnSettings.bushes.Length > 0)
        {
            foreach (Transform spawnPos in bushesSpawnList)
            {
                if (Random.Range(0.0f, 1.0f) > spawnSettings.chanceToSpawnABush) {
                    continue;
                }
                int bushType = Random.Range(0, spawnSettings.bushes.Length);
                Quaternion rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                GameObject bush = Instantiate(spawnSettings.bushes[bushType], spawnPos.position, Quaternion.identity) as GameObject;
                bush.transform.parent = spawnSettings.objectParent;
                bush.transform.localRotation = rotation;
            }
        }

        //Spawn enemies and crystals
        List<GameObject> spawnBagList = new List<GameObject>();
        int tmpEnemies = spawnSettings.minEnemiesOnIsland;
        int tmpCrystals = spawnSettings.minCrystalsOnIsland;
        int actualSpawnBagSize = Random.Range(tmpEnemies + tmpCrystals, enemyNCrystalSpawnPosList.Count);
        //Minimum spawnbag
        while (tmpEnemies > 0)
        {
            tmpEnemies--;
            spawnBagList.Add(spawnSettings.enemies[Random.Range(0, spawnSettings.enemies.Length)]);
        }
        while (tmpCrystals > 0)
        {
            tmpCrystals--;
            spawnBagList.Add(spawnSettings.crystals[Random.Range(0, spawnSettings.crystals.Length)]);
        }
        //Maximum spawnbag
        while (spawnBagList.Count < actualSpawnBagSize)
        {
            GameObject toAdd = null;
            if (Random.Range(0, 2) == 0)
            {
                if (spawnSettings.enemies.Length != 0)
                {
                    toAdd = spawnSettings.enemies[Random.Range(0, spawnSettings.enemies.Length)];
                }
            }
            else
            {
                toAdd = spawnSettings.crystals[Random.Range(0, spawnSettings.crystals.Length)];
            }
            spawnBagList.Add(toAdd);
        }
        //Spawn enemies and crystals
        Stack<KeyValuePair<GameObject, Vector3>> enemiesWithSpawnPosition = new Stack<KeyValuePair<GameObject, Vector3>>();
        while (spawnBagList.Count > 0)
        {
            int i = Random.Range(0, enemyNCrystalSpawnPosList.Count - 1);
            Vector3 position = enemyNCrystalSpawnPosList[i].position;
            enemyNCrystalSpawnPosList.RemoveAt(i);
            if (spawnBagList[0] != null)
            {
                GameObject nextSpawn = spawnBagList[0];
                if (nextSpawn.tag == "Enemy")
                {
                    enemiesWithSpawnPosition.Push(new KeyValuePair<GameObject, Vector3>(nextSpawn, position));
                }
                else
                {
                    nextSpawn = Instantiate(nextSpawn, position, Quaternion.identity) as GameObject;
                    nextSpawn.transform.parent = spawnSettings.objectParent;
					nextSpawn.transform.localRotation = Quaternion.identity;
                }
            }
            spawnBagList.RemoveAt(0);
        }
        islandNavigation.SpawnEnemies(spawnSettings.objectParent, enemiesWithSpawnPosition);
    }

    void Update()
    {
        rigidbody.MovePosition(transform.position - transform.up * Time.deltaTime * fallingSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        switch (tag)
        {
            case "Island":
                Island islandScript = collision.gameObject.GetComponent<Island>();
                if (islandScript.priority > priority)
                {
                    fallingSpeed *= extraSpeedFactorOnCollision;
                }
                break;
            case "Deathzone":
                //NiceToHave: add fancy explosion effects here
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
