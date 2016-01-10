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
    public float senseRange = 15.0f;

    private GameObject _player;
    public GameObject player
    {
        get { return _player; }
        private set { _player = value; }
    }

    protected CharacterController controller;

    protected EnemyState state;

    public virtual void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ChangeState(EnemyState.stateIDs.Idle);
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
		if (!player) {
			player = GameObject.FindGameObjectWithTag ("Player");
		} else {
			state.action();
			Move ();
		}
    }

    protected abstract void Move();

    void OnHit(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    protected abstract void ChangeState(EnemyState.stateIDs _stateID);
}
