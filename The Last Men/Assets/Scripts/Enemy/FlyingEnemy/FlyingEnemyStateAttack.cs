using UnityEngine;

public class FlyingEnemyStateAttack : EnemyState {

    protected FlyingEnemy enemy;
    const stateIDs id = stateIDs.Attack;

    public float timeSinceAttack = 10.0f;

    public FlyingEnemyStateAttack(Enemy _enemy)
    {
        enemy = _enemy as FlyingEnemy;
        enemy.target = Vector3.zero;
    }

    public override void action()
    {
        enemy.target = enemy.playerPosition;

        timeSinceAttack += Time.fixedDeltaTime;

        if (enemy.distanceToPlayer < enemy.attackRange)
        {
            if (timeSinceAttack > enemy.attackSpeed)
            {
                enemy.player.transform.SendMessage("OnHit", enemy.damage);
                timeSinceAttack = 0.0f;
            }
        }
        else if (enemy.distanceToPlayer > enemy.maxDistance)
        {
            enemy.ChangeState(stateIDs.Idle);
        }
    }
}
