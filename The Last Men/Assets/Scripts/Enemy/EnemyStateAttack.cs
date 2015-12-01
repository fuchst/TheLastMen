using UnityEngine;
using System.Collections;

public class EnemyStateAttack : EnemyState {

    const stateIDs id = stateIDs.Attack;

    ArrayList path;

    public EnemyStateAttack(Enemy _enemy) 
        : base(_enemy)
    {
        Debug.Log("Attack state");
        _enemy.GetComponent<Renderer>().material = _enemy.materials[2];
    }

    public override void action()
    {
        if(path == null)
        {
            path = enemy.island.findPath(enemy.island.GetClosestNode(enemy.transform.position), enemy.island.GetClosestNode(enemy.player.transform.position));
            foreach(NavigationNode n in path)
            {
                Debug.Log(n.GetGridIndices().x + " " + n.GetGridIndices().y);
            }
        }

        Vector3 playerPos = enemy.player.transform.position;
        enemy.transform.LookAt(playerPos);
        enemy.transform.Translate(enemy.transform.forward * Time.deltaTime);
    }
}
