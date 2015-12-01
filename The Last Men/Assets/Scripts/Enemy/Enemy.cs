using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public Material[] materials;
    public int hp;

    public GameObject player;
    public NavigationGrid island;
    public float fov;

    private EnemyState state;

    public ArrayList path { get; set; }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ChangeState(EnemyState.stateIDs.Idle);
    }

    void FixedUpdate()
    {
        state.action();
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
        if(path != null)
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);

            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(island.GetNodeWorldPos((NavigationNode)path[i]), island.GetNodeWorldPos((NavigationNode)path[i + 1]));
            }
        }       
    }
}
