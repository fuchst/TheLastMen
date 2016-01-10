using UnityEngine;
using System.Collections;

public class EnemyStateSearching : EnemyState {

    const stateIDs id = stateIDs.Search;

    public EnemyStateSearching(Enemy _enemy) 
        : base(_enemy)
    {
        Debug.Log("Search state");
        _enemy.GetComponent<Renderer>().material = _enemy.materials[1];
    }

    public override void action()
    {
        Vector3 playerPos = enemy.player.transform.position;
        Vector3 dir = (playerPos - this.enemy.transform.position).normalized;
        float dist = Vector3.Distance(playerPos, enemy.transform.position);

        float angle = Vector3.Angle(dir, enemy.transform.forward);

        if(angle < enemy.fov)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Attack);
        }
        else if (dist > enemy.senseRange)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Idle);
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

