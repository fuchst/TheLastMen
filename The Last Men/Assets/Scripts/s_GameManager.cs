using UnityEngine;

public class s_GameManager : MonoBehaviour {

    private LevelManager levelManager;
    
    public int artifactCountCur = 0;
    public int artifactCountMax = 10;
    public int healthpoints = 100;

    private static s_GameManager instance;

	public static s_GameManager Instance { get { return instance; } }

	void Awake () {
		if (instance) {
			Destroy(this);
		} else {
			instance = this;
		}

        levelManager = GetComponent<LevelManager>();
    }

    public float roundDuration = 300.0f;
	public float endTime;

	void Start () {
		endTime = Time.time + roundDuration;
        
        levelManager.LoadLevel();
    }
}
