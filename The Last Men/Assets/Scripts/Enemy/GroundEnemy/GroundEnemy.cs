using UnityEngine;
using System.Collections;
using System;

public class GroundEnemy : Enemy {

    public NavigationGrid navGrid { get; set; }

    private int pathIndex;
    private ArrayList _path;
    public ArrayList path
    {
        get { return _path; }
        set { _path = value; pathIndex = 0; }
    }

    public override void Init()
    {
        base.Init();

        navGrid = transform.parent.GetComponentInChildren<NavigationGrid>();
    }

    void Update()
    {
        // Keep enemy on navplane height
        this.transform.position -= Vector3.Dot(this.transform.position - navGrid.transform.position, navGrid.transform.up) * navGrid.transform.up;
    }

    protected override void Move()
    {
        if (path != null)
        {
            if (pathIndex < (path.Count - 1))
            {
                Vector3 nextNodePos = navGrid.GetNodeWorldPos((NavigationNode)path[pathIndex + 1]);

                this.transform.LookAt(nextNodePos, navGrid.transform.up);
                //this.transform.Translate(this.transform.forward * moveSpeed * Time.deltaTime, Space.World);

                controller.Move(transform.forward * moveSpeed * Time.deltaTime);

                if (Vector3.Distance(this.transform.position, nextNodePos) < 0.1f)
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

    protected override void ChangeState(EnemyState.stateIDs _stateID)
    {
        if (this.state == null || this.state.getID() != _stateID)
        {
            switch (_stateID)
            {
                case EnemyState.stateIDs.Idle:
                    state = new GroundEnemyStateIdle(this);
                    break;
                case EnemyState.stateIDs.Search:
                    state = new GroundEnemyStateSearching(this);
                    break;
                case EnemyState.stateIDs.Attack:
                    state = new GroundEnemyStateAttack(this);
                    break;
                default:
                    Debug.LogError("False enemy state provided");
                    break;
            }
        }
    }
}
