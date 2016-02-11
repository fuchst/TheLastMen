using UnityEngine;
using System.Collections.Generic;

public class IslandNavigation : MonoBehaviour {

    //private GameObject enemyPrefab;
    public List<GameObject> enemies;

    private GameObject navGridPrefab;
    public NavigationGrid navGridInstance { get; set; }

    private bool enemiesActive = true;
    private static float minDistance = Mathf.Pow(75.0f, 2);

    void Awake()
    {
        navGridPrefab = Resources.Load("NavGrid") as GameObject;
        //enemyPrefab = Resources.Load("GroundEnemy") as GameObject;

        navGridInstance = (Instantiate(navGridPrefab, this.transform.position, this.transform.rotation) as GameObject).GetComponent<NavigationGrid>();
        navGridInstance.transform.SetParent(transform);
        //Debug.Log(enemiesActive);
              
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

    void Update()
    {
        //if (enemiesActive) {
        //    Debug.Log(enemiesActive);
        //}
        GameObject player = LevelManager.Instance.player;
        float distanceToPlayer = (transform.position - player.transform.position).sqrMagnitude;

        if ((distanceToPlayer <= minDistance) != enemiesActive) {
            enemiesActive = !enemiesActive;

            //remove all null entries before traversing the enemy list
            enemies.RemoveAll(element => null == element);

            foreach (GameObject e in enemies) {
                e.SetActive(enemiesActive);
            }
        }

        /*if (distanceToPlayer > minDistance && enemiesActive) {
            foreach (GameObject e in enemies) {
                e.SetActive(false);
            }
            enemiesActive = false;
        }
        else if(distanceToPlayer <= minDistance && !enemiesActive) {
            foreach (GameObject e in enemies) {
                e.SetActive(true);
            }
            enemiesActive = true;
        }*/
    }

    public void SpawnEnemies(Transform parent, Stack<KeyValuePair<GameObject, Vector3>> enemiesWithSpawnPositions)
    {
		while (enemiesWithSpawnPositions.Count > 0) {
			KeyValuePair<GameObject,Vector3> enemyWithSpawnPos = enemiesWithSpawnPositions.Pop ();
			GameObject enemy = Instantiate (enemyWithSpawnPos.Key, enemyWithSpawnPos.Value, Quaternion.identity) as GameObject;
			enemy.transform.parent = parent;
			enemy.GetComponent<GroundEnemy>().navGrid = navGridInstance;
            enemies.Add(enemy);
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
