using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    //public Material[] materials;

    public int hpMax = 100;
    [HideInInspector]public int hpCur;
    public float attackRange = 2.0f;
    public int damage = 5;
    public float moveSpeed = 1.0f;
    public float attackInterval = 1.0f;

    public float fov = 30.0f;
    // Change state to searching if smaller
    public float senseRangeSearching = 15.0f;
    // Change state to attack if smaller
    public float senseRangeAttack = 5.0f;

    [SerializeField]protected new Renderer renderer;
    [SerializeField]protected Color colorFullHealth;
    [SerializeField]protected Color colorNoHealth;
    protected HSBColor colorFullHealth_HSB;
    protected HSBColor colorNoHealth_HSB;

    protected bool alive;

    protected new AudioSource audio;
    [SerializeField]protected GameObject attackSoundPrefab;
    [SerializeField]protected GameObject deathEffectPrefab;

    private GameObject _player;
    public GameObject player
    {
        get { return _player; }
        private set { _player = value; }
    }
    private Combat _playerCombat;
    public Combat PlayerCombat
    {
        get { return _playerCombat; }
        private set { _playerCombat = value; }
    }


    // Player specific attributes
    protected Vector3 _playerPosition;
    public Vector3 playerPosition { get { return _playerPosition; } }
    protected Vector3 _enemyToPlayer;
    public Vector3 enemyToPlayer { get { return _enemyToPlayer; } }
    protected Vector3 _directionToPlayer;
    public Vector3 directionToPlayer { get { return _directionToPlayer; } }
    protected float _distanceToPlayer;
    public float distanceToPlayer { get { return _distanceToPlayer; } }

    protected CharacterController _controller;
    public CharacterController controller { get { return _controller; } }

    protected EnemyState state;
    public EnemyState State
    {
        get { return state; }
    }
    
    void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        hpCur = hpMax;
        alive = true;
        colorFullHealth_HSB = HSBColor.FromColor(colorFullHealth);
        colorNoHealth_HSB = HSBColor.FromColor(colorNoHealth);
    }

    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        player = LevelManager.Instance.player;
        PlayerCombat = player.GetComponent<Combat>();
        ChangeState(EnemyState.stateIDs.Idle);
        _controller = GetComponent<CharacterController>();
    }

    void OnEnable () {
        audio = GetComponent<AudioSource>();
        audio.loop = true;
        audio.Play();
    }

    void Update()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate()
    {
        if (!player)
        {
            //player = GameObject.FindGameObjectWithTag("Player");
            player = LevelManager.Instance.player;
            PlayerCombat = player.GetComponent<Combat>();
        }
        else
        {
            _playerPosition = player.transform.position;
            _enemyToPlayer = playerPosition - transform.position;
            //_directionToPlayer = enemyToPlayer.normalized;
            _distanceToPlayer = enemyToPlayer.magnitude;
            _directionToPlayer = enemyToPlayer / _distanceToPlayer; //more efficient, because .normalized computes magnitude again (square root computation)
        }
    }

    protected abstract void Move();

    public void MakeAttackSound () {
        Instantiate(attackSoundPrefab, transform.position, transform.rotation);
    }

    public void OnHit (int dmg) {
        hpCur -= dmg;
        //Debug.Log("I got hit with " + dmg + " damage! I only have " + hpCur + " remaining health now, but once, I was powerful, with " + hpMax + " health...");
        renderer.material.color = HSBColor.Lerp(colorNoHealth_HSB, colorFullHealth_HSB, (float)hpCur / (float)hpMax).ToColor();
        if(alive && hpCur <= 0) {
            alive = false;
            OnDeath();
        }
        else {
            ChangeState(EnemyState.stateIDs.Attack);
        }
    }

    protected virtual void OnDeath() {
        s_GameManager.Instance.enemyKillCount++;
        GameObject loot = s_GameManager.Instance.RetrieveLoot();
        if (loot){
            Instantiate(loot, transform.position, transform.rotation);
        }

        Instantiate(deathEffectPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
	
    public abstract void ChangeState(EnemyState.stateIDs _stateID);
}
