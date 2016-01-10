using UnityEngine;
using System.Collections;

public class GroundEnemyStateIdle : EnemyState {

    protected GroundEnemy enemy;
    const stateIDs id = stateIDs.Idle;

    public GroundEnemyStateIdle(Enemy _enemy)
    {
        //Debug.Log("Idle state");
        //_enemy.GetComponent<Renderer>().material = _enemy.materials[0];
        enemy = _enemy as GroundEnemy;
    }

    public override void action()
    {
        // Check if the player is close by and clear the path
        if(Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.senseRange)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Search);

            if(enemy.path != null)
            {
                enemy.path.Clear();
            } 
        }
        else if (enemy.path == null || enemy.path.Count == 0)
        {
            int x = Random.Range(0, enemy.navGrid.nodes.Count - 1);

            NavigationNode start = enemy.navGrid.GetClosestNode(enemy.transform.position);
            NavigationNode end = enemy.navGrid.nodes.Values[x];

            if (end.nodeType == NavigationNode.nodeTypes.Free)
            {
                enemy.path = enemy.navGrid.findPath(start, end);
            }
        }
    }
}
