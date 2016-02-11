using UnityEngine;

public class PlaceFlyingEnemy : MonoBehaviour {

    public static Transform flyingEnemyParent;
    public static int maxFlyingEnemies = 25;
    public static int currentFlyingEnemies = 0;
    //GameObject prefab;

    //void Awake()
    //{
    //    prefab = Resources.Load("FlyingEnemy") as GameObject;
    //}

    void OnLevelWasLoaded () {
        //Debug.Log(currentFlyingEnemies);
        currentFlyingEnemies = 0;
    }

    void Start () {
        if (currentFlyingEnemies < maxFlyingEnemies) {
            Vector3 spawnpoint = transform.position + transform.up * 10.0f;
            GameObject flyingEnemy = Instantiate(Resources.Load("FlyingEnemy") as GameObject, spawnpoint, transform.rotation) as GameObject;
            flyingEnemy.transform.parent = flyingEnemyParent;
            currentFlyingEnemies++;
        }
	}
}
