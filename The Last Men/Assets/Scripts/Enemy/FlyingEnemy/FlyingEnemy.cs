using System;
using UnityEngine;

public class FlyingEnemy : Enemy {

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void Move()
    {
        throw new NotImplementedException();
    }

    protected override void ChangeState(EnemyState.stateIDs _stateID)
    {
        throw new NotImplementedException();
    }
}
