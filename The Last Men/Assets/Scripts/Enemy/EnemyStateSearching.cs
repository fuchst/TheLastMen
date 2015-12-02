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

        float angle = Vector3.Angle(dir, enemy.transform.forward);

        if(angle < enemy.fov)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Attack);
        }
        else if (enemy.path == null || enemy.path.Count == 0)
        {
            int x = Random.Range(0, NavigationGrid.sizeX - 1);
            int y = Random.Range(0, NavigationGrid.sizeY - 1);

            NavigationNode start = enemy.island.GetClosestNode(enemy.transform.position);
            NavigationNode end = enemy.island.GetNodeAtIndices(x, y);

            if (end.GetNodeType() == NavigationNode.nodeTypes.Free)
            {
                enemy.path = enemy.island.findPath(start, end);
            }
        }
    }
}

