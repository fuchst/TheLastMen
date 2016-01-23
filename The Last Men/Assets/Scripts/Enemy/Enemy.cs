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

    protected CharacterController controller;

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
        controller = GetComponent<CharacterController>();
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
        Instantiate(s_GameManager.Instance.lootTable[idx], transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

    protected abstract void ChangeState(EnemyState.stateIDs _stateID);
}
