using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandNavigation : MonoBehaviour {

    private GameObject enemyPrefab;
    public List<GameObject> enemies;

    private GameObject navGridPrefab;
    public NavigationGrid navGridInstance { get; set; }

    void Awake()
    {
        navGridPrefab = Resources.Load("NavGrid") as GameObject;
        enemyPrefab = Resources.Load("Enemy") as GameObject;
    }

	// Use this for initialization
	void Start () {
        navGridInstance = (Instantiate(navGridPrefab, this.transform.position, this.transform.rotation) as GameObject).GetComponent<NavigationGrid>();
        navGridInstance.transform.parent = this.transform;
        navGridInstance.Init();
        
        enemies = new List<GameObject>();

        int rand = Random.Range(0, 100);

        if(rand < 20)
        {
            NavigationNode node = navGridInstance.GetComponent<NavigationGrid>().GetRandomFreeNode();

            if(node != null)
            {
                GameObject obj = Instantiate(enemyPrefab, navGridInstance.GetNodeWorldPos(node), this.transform.rotation) as GameObject;
                obj.transform.parent = this.transform;
                enemies.Add(obj);
            }        
        }
	}
}
