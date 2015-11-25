using UnityEngine;
using System.Collections;

public class EnemyStateIdle : EnemyState {

    const stateIDs id = stateIDs.Idle;

    public EnemyStateIdle(Enemy _enemy) 
        : base(_enemy)
    {
        Debug.Log("Idle state");
        _enemy.GetComponent<Renderer>().material = _enemy.materials[0];
    }

    public override void action()
    {

    }
}
