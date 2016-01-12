using UnityEngine;

public class FlyingEnemyStateAttack : EnemyState {

    protected FlyingEnemy enemy;
    const stateIDs id = stateIDs.Attack;

    public float timeSinceAttack = 0;

    public FlyingEnemyStateAttack(Enemy _enemy)
    {
        enemy = _enemy as FlyingEnemy;
        enemy.target = Vector3.zero;
    }

    public override void action()
    {
        enemy.target = enemy.player.transform.position;

        float distance = Vector3.Distance(enemy.player.transform.position, enemy.transform.position);

        if (distance < enemy.attackRange)
        {
            timeSinceAttack += Time.fixedDeltaTime;

            if (timeSinceAttack > enemy.attackSpeed)
            {
                enemy.player.transform.SendMessage("OnHit", enemy.damage);
                timeSinceAttack = 0.0f;
            }
        }
        else if (distance > enemy.maxDistance)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Idle);
        }
    }
}
