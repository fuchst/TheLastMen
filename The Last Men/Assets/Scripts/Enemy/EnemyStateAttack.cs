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
        enemy.transform.LookAt(playerPos);
        enemy.transform.Translate(enemy.transform.forward * Time.deltaTime);
    }
}
