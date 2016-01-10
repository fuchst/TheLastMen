using UnityEngine;

public class GroundEnemyStateAttack : EnemyState {

    protected GroundEnemy enemy;
    const stateIDs id = stateIDs.Attack;

    public float timeSinceAttack = 0;

    public GroundEnemyStateAttack(Enemy _enemy)
    {
        //Debug.Log("Attack state");
        //_enemy.GetComponent<Renderer>().material = _enemy.materials[2];
        enemy = _enemy as GroundEnemy;
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
                enemy.GetComponent<Animation>().Play();
            }
            
            enemy.path.Clear();
        }
        else if ( dist > enemy.senseRange || enemy.navGrid.GetClosestNode(enemy.transform.position)== null)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Idle);
        }
        else if (enemy.path == null || enemy.path.Count == 0 || enemy.navGrid.GetClosestNode(playerPos) != enemy.path[enemy.path.Count - 1])
        {
            enemy.path = enemy.navGrid.findPath(enemy.navGrid.GetClosestNode(enemy.transform.position), enemy.navGrid.GetClosestNode(playerPos));
        }     
    }
}
