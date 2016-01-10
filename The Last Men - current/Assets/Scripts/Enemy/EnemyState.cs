using UnityEngine;
using System.Collections;

public abstract class EnemyState {

    protected Enemy enemy;
    const stateIDs id = stateIDs.Base;

    public enum stateIDs
    {
        Base,
        Idle,
        Search,
        Attack
    };

    protected EnemyState(Enemy _enemy)
    {
        enemy = _enemy;
    }

    public stateIDs getID()
    {
        return id;
    }

    public abstract void action();
}
