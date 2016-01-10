using System;
using UnityEngine;

public class FlyingEnemy : Enemy {

    public override void Init()
    {
        base.Init();
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
