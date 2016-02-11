using UnityEngine;
using System.Collections.Generic;

public class Bastion : MonoBehaviour
{
    private static Bastion instance;

	public static Bastion Instance { get { return instance; } }

	void Awake () {
		if (instance) {
			Destroy(this);
		} else {
			instance = this;
		}
    }

    [SerializeField]protected GameObject triggerObject;
    [SerializeField]protected GameObject survivorPrefab;
    [SerializeField]protected Transform survivorSpawnPositionParent;
    protected List<Transform> survivorSpawnPositions;
    protected List<GameObject> survivors;
    protected s_GameManager game;

    //[SerializeField] private float rebaseSpeed = 5.0f;
    //private new Rigidbody rigidbody;
    protected Vector3 newPosition;

    //void Awake()
    //{
    //    rigidbody = GetComponent<Rigidbody>();
    //}

    void Start () {
        s_GUIMain.Instance.bastionTransform = triggerObject.transform;
        game = s_GameManager.Instance;
        game.SetPlayerInBastion(true);

        survivorSpawnPositions = new List<Transform>(10);
        foreach (Transform spawnPos in survivorSpawnPositionParent) {
            survivorSpawnPositions.Add(spawnPos);
        }

        survivors = new List<GameObject>(10);
        UpdateSurvivors();
    }

    public void UpdateSurvivors () {
        if (survivors.Count != game.survivorsCur - 1) {
            int newSurvivorAmount = Mathf.Min(survivorSpawnPositions.Count, s_GameManager.Instance.survivorsCur - 1);

            for (int i = survivors.Count; i < newSurvivorAmount; i++) {
                GameObject newSurvivor = Instantiate(survivorPrefab, survivorSpawnPositions[i].position, survivorSpawnPositions[i].rotation) as GameObject;
                newSurvivor.transform.SetParent(survivorSpawnPositionParent);
                survivors.Add(newSurvivor);
            }
            
            while (survivors.Count > Mathf.Max(0, game.survivorsCur - 1)) {
                GameObject oldSurvivor = survivors[survivors.Count - 1];
                survivors.Remove(oldSurvivor);
                Destroy(oldSurvivor);
            }
        }
    }

    /*public void PlayerLanded()
    {
        if (Mathf.Max(s_GameManager.Instance.artifact1CountCur, s_GameManager.Instance.artifact2CountCur) == s_GameManager.Instance.artifactCountMax)
        {
            LevelManager.Instance.AdvanceLevel();
        }
    }*/

    public void RebaseBastion(Vector3 newPosition)
    {
        this.newPosition = newPosition;
        LevelManager.Instance.player.transform.SetParent(transform, true);
        transform.position = this.newPosition;
        LevelManager.Instance.player.transform.parent = null;
    }

    void OnTriggerEnter (Collider other) {
        if(LevelManager.Instance.gameState == LevelManager.GameState.Loading)
        {
            return;
        }
        if (other.CompareTag("Player")) {
            s_GameManager.Instance.SetPlayerInBastion(true);
        }
        else if(other.CompareTag("Enemy")) {
            other.GetComponent<Enemy>().OnHit(9001);
        }
    }

    void OnTriggerExit (Collider other) {
        if (LevelManager.Instance.gameState == LevelManager.GameState.Loading)
        {
            return;
        }

        if (other.CompareTag("Player")) {
            s_GameManager.Instance.SetPlayerInBastion(false);
        }
    }
}
