using UnityEngine;
using System.Collections;

public class EnemyStateAttack : EnemyState {

    const stateIDs id = stateIDs.Attack;

    public EnemyStateAttack(Enemy _enemy) 
        : base(_enemy)
    {
        Debug.Log("Attack state");
        _enemy.GetComponent<Renderer>().material = _enemy.materials[2];
    }

    public override void action()
    {
        Vector3 playerPos = enemy.player.transform.position;

        float dist = Vector3.Distance(playerPos, enemy.transform.position);

        if (dist <= enemy.attackRange)
        {
            enemy.path.Clear();
            // TODO: damage
        }
        else if (enemy.path == null || enemy.island.GetClosestNode(playerPos) != enemy.path[enemy.path.Count - 1])
        {
            enemy.path = enemy.island.findPath(enemy.island.GetClosestNode(enemy.transform.position), enemy.island.GetClosestNode(playerPos));
        }     
    }
}
