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
        if (enemy.distanceToPlayer <= enemy.attackRange)
        {
            enemy.transform.LookAt(enemy.playerPosition, enemy.navGrid.transform.up);

            timeSinceAttack += Time.fixedDeltaTime;

            if (timeSinceAttack > enemy.attackSpeed)
            {
                enemy.player.transform.SendMessage("OnHit", enemy.damage);
                timeSinceAttack = 0.0f;
                enemy.GetComponent<Animation>().Play();
            }

            if (enemy.path != null)
            {
                enemy.path.Clear();
            }
        }
        else if(enemy.distanceToPlayer < 2 * enemy.navGrid.stepSize)
        {
            enemy.transform.LookAt(enemy.playerPosition, enemy.navGrid.transform.up);
            enemy.controller.Move(enemy.transform.forward * enemy.moveSpeed * Time.deltaTime);

            if (enemy.path != null)
            {
                enemy.path.Clear();
            }
        }
        else if (enemy.navGrid.GetClosestNode(enemy.playerPosition) == null || enemy.distanceToPlayer > enemy.senseRangeSearching)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Idle);
        }
        else if (enemy.path == null || enemy.path.Count == 0 || enemy.navGrid.GetClosestNode(enemy.playerPosition) != enemy.path[enemy.path.Count - 1])
        {
            int startID = enemy.currentNodeID;
            int endID = enemy.navGrid.GetClosestNode(enemy.playerPosition).GetID();

            enemy.path = enemy.navGrid.findPath(startID, endID, enemy.currentNodeID);
        }     
    }
}
