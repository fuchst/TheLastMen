using UnityEngine;

public class EnemyStateAttack : EnemyState {

    const stateIDs id = stateIDs.Attack;

    float timeSinceAttack = 0;

    public EnemyStateAttack(Enemy _enemy) 
        : base(_enemy)
    {
        //Debug.Log("Attack state");
        _enemy.GetComponent<Renderer>().material = _enemy.materials[2];
    }

    public override void action()
    {
        Vector3 playerPos = enemy.player.transform.position;

        float dist = Vector3.Distance(playerPos, enemy.transform.position);

        if (dist <= enemy.attackRange)
        {
            enemy.transform.LookAt(playerPos, enemy.navGrid.transform.up);

            timeSinceAttack += Time.fixedDeltaTime;

            if(timeSinceAttack > enemy.attackSpeed)
            {
                enemy.player.transform.SendMessage("OnHit", enemy.damage);
                timeSinceAttack = 0.0f;
            }
            
            enemy.path.Clear();
        }
        else if ( dist > enemy.senseRange )
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Idle);
        }
        else if (enemy.path == null || enemy.path.Count == 0 || enemy.navGrid.GetClosestNode(playerPos) != enemy.path[enemy.path.Count - 1])
        {
            enemy.path = enemy.navGrid.findPath(enemy.navGrid.GetClosestNode(enemy.transform.position), enemy.navGrid.GetClosestNode(playerPos));
        }     
    }
}
