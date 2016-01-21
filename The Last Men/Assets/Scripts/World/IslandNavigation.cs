using UnityEngine;
using System.Collections.Generic;

public class IslandNavigation : MonoBehaviour {

    //private GameObject enemyPrefab;
    public List<GameObject> enemies;

    private GameObject navGridPrefab;
    public NavigationGrid navGridInstance { get; set; }

    void Awake()
    {
        navGridPrefab = Resources.Load("NavGrid") as GameObject;
        //enemyPrefab = Resources.Load("GroundEnemy") as GameObject;

        navGridInstance = (Instantiate(navGridPrefab, this.transform.position, this.transform.rotation) as GameObject).GetComponent<NavigationGrid>();
        navGridInstance.transform.SetParent(transform);
        navGridInstance.Init();
        
        enemies = new List<GameObject>();
/*
        for(int i = 0; i < 10; i++)
        {
            int rand = Random.Range(0, 100);

            if (rand < 20)
            {
                NavigationNode node = navGridInstance.GetComponent<NavigationGrid>().GetRandomFreeNode();

                if (node != null)
                {
                    GameObject obj = Instantiate(enemyPrefab, navGridInstance.GetNodeWorldPos(node), this.transform.rotation) as GameObject;
                    //obj.GetComponent<GroundEnemy>().Init(navGridInstance);
                    obj.transform.SetParent(transform);
                    enemies.Add(obj);
                }
            }
        }
*/
    }

    public void SpawnEnemies(Transform parent, Stack<KeyValuePair<GameObject, Vector3>> enemiesWithSpawnPositions)
    {
		while (enemiesWithSpawnPositions.Count > 0) {
			KeyValuePair<GameObject,Vector3> enemyWithSpawnPos = enemiesWithSpawnPositions.Pop ();
			GameObject enemy = Instantiate (enemyWithSpawnPos.Key, enemyWithSpawnPos.Value, Quaternion.identity) as GameObject;
			enemy.transform.parent = parent;
			enemy.GetComponent<GroundEnemy>().navGrid = navGridInstance;
		}
    }

    void OnDestroy()
    {
        while(enemies.Count != 0)
        {
            Destroy(enemies[0]);
            enemies.RemoveAt(0);
        }
    }
}
