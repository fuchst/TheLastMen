using UnityEngine;
using System;

public class FlyingEnemyStateIdle : EnemyState {

    protected FlyingEnemy enemy;
    const stateIDs id = stateIDs.Search;

    public FlyingEnemyStateIdle(Enemy _enemy)
    {
        //Debug.Log("Search state");
        //_enemy.GetComponent<Renderer>().material = _enemy.materials[1];
        enemy = _enemy as FlyingEnemy;
    }

    public override void action()
    {
        // Check if the player is close by
        if (Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.senseRange)
        {
            //enemy.SendMessage("ChangeState", EnemyState.stateIDs.Search);
        }
        else if (enemy.target == Vector3.zero)
        {
            GameObject[] islands = GameObject.FindGameObjectsWithTag("Island");
            int rand = UnityEngine.Random.Range(0, islands.Length - 1);
            enemy.target = islands[rand].transform.position + islands[rand].transform.up * 10.0f;
        }
        else if (Vector3.Distance(enemy.transform.position, enemy.target) < 0.1f)
        {
            enemy.target = Vector3.zero;
        }
    }
}
