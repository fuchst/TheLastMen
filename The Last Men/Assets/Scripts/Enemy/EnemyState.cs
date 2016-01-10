using UnityEngine;
using System.Collections;

public abstract class EnemyState {
    
    const stateIDs id = stateIDs.Base;

    public enum stateIDs
    {
        Base,
        Idle,
        Search,
        Attack
    };

    public stateIDs getID()
    {
        return id;
    }

    public abstract void action();
}
