﻿using UnityEngine;
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
        Vector3 playerPos = enemy.player.transform.position;
        Vector3 dir = (playerPos - this.enemy.transform.position).normalized;
        float dist = Vector3.Distance(playerPos, enemy.transform.position);

        float angle = Vector3.Angle(dir, enemy.transform.forward);

        // Change state to Attack if player is in viewfield and on island
        if(angle < enemy.fov && enemy.navGrid.GetClosestNode(enemy.transform.position) != null)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Attack);
        }
        else if (dist > enemy.senseRange)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Idle);
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
