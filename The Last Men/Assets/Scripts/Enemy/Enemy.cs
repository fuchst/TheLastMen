using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

    //public Material[] materials;

    public int hp = 100;
    public float attackRange = 2.0f;
    public int damage = 5;
    public float moveSpeed = 1.0f;
    public float attackSpeed = 1.0f;

    public float fov = 20.0f;
    // Change state to searching if smaller
    public float senseRangeSearching = 15.0f;
    // Change state to attack if smaller
    public float senseRangeAttack = 5.0f;

    private GameObject _player;
    public GameObject player
    {
        get { return _player; }
        private set { _player = value; }
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

    }

    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ChangeState(EnemyState.stateIDs.Idle);
        _controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            _playerPosition = player.transform.position;
            _enemyToPlayer = _playerPosition - transform.position;
            _directionToPlayer = enemyToPlayer.normalized;
            _distanceToPlayer = enemyToPlayer.magnitude;

            state.action();
            Move();
        }
    }

    protected abstract void Move();

    public void OnHit(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        int idx = Random.Range(0, s_GameManager.Instance.lootTable.Length);
        //GameObject loot = Instantiate(s_GameManager.Instance.lootTable[idx], transform.position, transform.rotation) as GameObject;
        Destroy(this.gameObject);
    }

    public abstract void ChangeState(EnemyState.stateIDs _stateID);
}
