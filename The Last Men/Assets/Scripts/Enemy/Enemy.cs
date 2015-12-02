using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public Material[] materials;

    public int hp = 100;
    public float attackRange = 2.0f;
    public int damage = 5;
    public float speed = 1.0f;

    private GameObject _player;
    public GameObject player
    {
        get { return _player; }
        private set { _player = value; }
    }

    public NavigationGrid navGrid { get; set; }
    public float fov = 20.0f;
    public float senseRange = 15.0f;

    private EnemyState state;

    private int pathIndex;
    private ArrayList _path;
    public ArrayList path {
        get { return _path; }
        set { _path = value; pathIndex = 0; }
    }

    public void Init(NavigationGrid _navGrid)
    {
        navGrid = _navGrid;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //navGrid = transform.parent.GetComponentInChildren<NavigationGrid>();
        ChangeState(EnemyState.stateIDs.Idle);
    }

    void FixedUpdate()
    {
        state.action();
        Move();
    }

    void Move()
    {
        if (path != null)
        {
            if ( pathIndex < (path.Count - 1) )
            {
                Vector3 nextNodePos = navGrid.GetNodeWorldPos((NavigationNode)path[pathIndex + 1]);

                this.transform.LookAt(nextNodePos, navGrid.transform.up);
                this.transform.Translate(this.transform.forward * speed * Time.deltaTime, Space.World);

                if(Vector3.Distance(this.transform.position, nextNodePos) < 0.1f)
                {
                    pathIndex++;
                }
            }
            else
            {
                path.Clear();
            }
        }
    }

    void OnHit(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void ChangeState(EnemyState.stateIDs _stateID)
    {
        if(this.state == null || this.state.getID() != _stateID)
        {
            switch(_stateID)
            {
                case EnemyState.stateIDs.Idle:
                    state = new EnemyStateIdle(this);
                    break;
                case EnemyState.stateIDs.Search:
                    state = new EnemyStateSearching(this);
                    break;
                case EnemyState.stateIDs.Attack:
                    state = new EnemyStateAttack(this);
                    break;
                default:
                    Debug.LogError("False enemy state provided");
                    break;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Camera.current.name == "MainCamera")
        {
            if (path != null)
            {
                Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(navGrid.GetNodeWorldPos((NavigationNode)path[i]), navGrid.GetNodeWorldPos((NavigationNode)path[i + 1]));
                }
            }
        }      
    }
}
