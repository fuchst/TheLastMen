using UnityEngine;

public class s_GameManager : MonoBehaviour {

    private LevelManager levelManager;
    //private Camera worldCam;

    public int artifact1CountCur = 0;
    public int artifact2CountCur = 0;

    public int artifactCountMax = 10;
    public float energyCur = 0;
    public float energyMax = 10;
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
