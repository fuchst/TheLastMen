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
        else if ( dist > enemy.senseRange )
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Idle);
        }
        else if (enemy.path == null || enemy.navGrid.GetClosestNode(playerPos) != enemy.path[enemy.path.Count - 1])
        {
            enemy.path = enemy.navGrid.findPath(enemy.navGrid.GetClosestNode(enemy.transform.position), enemy.navGrid.GetClosestNode(playerPos));
        }     
    }
}
