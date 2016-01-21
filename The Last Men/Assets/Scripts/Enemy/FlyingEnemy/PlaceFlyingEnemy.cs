using UnityEngine;

public class PlaceFlyingEnemy : MonoBehaviour {

    public static Transform flyingEnemyParent;
    GameObject prefab;

    void Awake()
    {
        prefab = Resources.Load("FlyingEnemy") as GameObject;
    }

	// Use this for initialization
	void Start ()
    {
        Vector3 spawnpoint = transform.position + transform.up * 10.0f;
        GameObject flyingEnemy = Instantiate(prefab, spawnpoint, transform.rotation) as GameObject;
        flyingEnemy.transform.parent = flyingEnemyParent;
	}
}
