using UnityEngine;
using System.Collections;

public class GroundEnemyStateSearching : EnemyState {

    protected GroundEnemy enemy;
    const stateIDs id = stateIDs.Search;

    public GroundEnemyStateSearching(Enemy _enemy)
    {
        //Debug.Log("Search state");
        //_enemy.GetComponent<Renderer>().material = _enemy.materials[1];
        enemy = _enemy as GroundEnemy;
    }

    public override void action()
    {
        float angle = Vector3.Angle(enemy.directionToPlayer, enemy.transform.forward);

        // Change state to Attack if player is in viewfield or close by and on island
        if((angle < enemy.fov || enemy.distanceToPlayer < enemy.senseRangeAttack) && enemy.navGrid.GetClosestNode(enemy.transform.position) != null )
        {
            enemy.ChangeState(stateIDs.Attack);
        }
        else if (enemy.distanceToPlayer > enemy.senseRangeSearching)
        {
            enemy.ChangeState(stateIDs.Idle);
        }
        else if (enemy.path == null || enemy.path.Count == 0)
        {
            int x = Random.Range(0, enemy.navGrid.nodes.Count - 1);

            int startID = enemy.currentNodeID;
            int endID = enemy.navGrid.nodes.Values[x].GetID();

            if (enemy.navGrid.nodes[endID].nodeType == NavigationNode.nodeTypes.Free)
            {
                enemy.path = enemy.navGrid.findPath(startID, endID, enemy.currentNodeID);
            }
        }
    }
}

