using UnityEngine;
using System.Collections.Generic;

public class IslandNavigation : MonoBehaviour {

    private GameObject enemyPrefab;
    public List<GameObject> enemies;

    private GameObject navGridPrefab;
    public NavigationGrid navGridInstance { get; set; }

    void Awake()
    {
        navGridPrefab = Resources.Load("NavGrid") as GameObject;
        enemyPrefab = Resources.Load("GroundEnemy") as GameObject;
    }

	// Use this for initialization
	void Start () {
        navGridInstance = (Instantiate(navGridPrefab, this.transform.position, this.transform.rotation) as GameObject).GetComponent<NavigationGrid>();
        navGridInstance.transform.SetParent(transform);
        navGridInstance.Init();
        
        enemies = new List<GameObject>();

        int rand = Random.Range(0, 100);

        if(rand < 100)
        {
            NavigationNode node = navGridInstance.GetComponent<NavigationGrid>().GetRandomFreeNode();

            if(node != null)
            {
                GameObject obj = Instantiate(enemyPrefab, navGridInstance.GetNodeWorldPos(node), this.transform.rotation) as GameObject;
                //obj.GetComponent<GroundEnemy>().Init(navGridInstance);
                obj.transform.SetParent(transform);
                enemies.Add(obj);
            }        
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
