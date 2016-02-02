using UnityEngine;

public class PlaceFlyingEnemy : MonoBehaviour {

    public static Transform flyingEnemyParent;
    public static int maxFlyingEnemies = 25;
    public static int currentFlyingEnemeis = 0;
    GameObject prefab;

    //void Awake()
    //{
    //    prefab = Resources.Load("FlyingEnemy") as GameObject;
    //}
    
	void Start ()
    {
        if (currentFlyingEnemeis < maxFlyingEnemies)
        {
            Vector3 spawnpoint = transform.position + transform.up * 10.0f;
            GameObject flyingEnemy = Instantiate(Resources.Load("FlyingEnemy") as GameObject, spawnpoint, transform.rotation) as GameObject;
            flyingEnemy.transform.parent = flyingEnemyParent;
            currentFlyingEnemeis++;
        }
	}
}
