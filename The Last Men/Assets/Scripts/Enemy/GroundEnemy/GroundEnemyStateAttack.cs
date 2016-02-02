using UnityEngine;

public class GroundEnemyStateAttack : EnemyState {

    protected GroundEnemy enemy;
    const stateIDs id = stateIDs.Attack;

    public float timeSinceAttack = 10.0f;

    public GroundEnemyStateAttack(Enemy _enemy)
    {
        //Debug.Log("Attack state");
        //_enemy.GetComponent<Renderer>().material = _enemy.materials[2];
        enemy = _enemy as GroundEnemy;
    }

    public override void action()
    {
        timeSinceAttack += Time.fixedDeltaTime;

        if (enemy.distanceToPlayer <= enemy.attackRange)
        {
            enemy.transform.LookAt(enemy.playerPosition, enemy.navGrid.transform.up);
           
            if (timeSinceAttack > enemy.attackSpeed)
            {
                //enemy.player.transform.SendMessage("OnHit", enemy.damage);
                enemy.PlayerCombat.OnHit(enemy.damage);
                timeSinceAttack = 0.0f;
                enemy.GetComponent<Animation>().Play();
            }

            if (enemy.path != null)
            {
                enemy.path.Clear();
            }
        }

        if (enemy.distanceToPlayer < 2 * enemy.navGrid.stepSize && enemy.distanceToPlayer > enemy.attackRange * 0.9f)
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
            enemy.ChangeState(stateIDs.Idle);
        }
        else if (enemy.path == null || enemy.path.Count == 0 || enemy.navGrid.GetClosestNode(enemy.playerPosition) != enemy.path[enemy.path.Count - 1])
        {
            int startID = enemy.currentNodeID;
            int endID = enemy.navGrid.GetClosestNode(enemy.playerPosition).GetID();

            enemy.path = enemy.navGrid.findPath(startID, endID, enemy.currentNodeID);
        }     
    }
}
