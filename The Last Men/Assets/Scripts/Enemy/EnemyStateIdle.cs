using UnityEngine;
using System.Collections;

public class EnemyStateIdle : EnemyState {

    const stateIDs id = stateIDs.Idle;

    public EnemyStateIdle(Enemy _enemy) 
        : base(_enemy)
    {
        Debug.Log("Idle state");
        _enemy.GetComponent<Renderer>().material = _enemy.materials[0];
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
            int x = Random.Range(0, enemy.navGrid.sizeX - 1);
            int y = Random.Range(0, enemy.navGrid.sizeY - 1);

            NavigationNode start = enemy.navGrid.GetClosestNode(enemy.transform.position);
            NavigationNode end = enemy.navGrid.GetNodeAtIndices(x, y);

            if (end.nodeType == NavigationNode.nodeTypes.Free)
            {
                enemy.path = enemy.navGrid.findPath(start, end);
            }
        }
    }
}
