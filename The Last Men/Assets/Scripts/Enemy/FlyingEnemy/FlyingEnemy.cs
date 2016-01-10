using UnityEngine;

public class FlyingEnemy : Enemy {

    public Vector3 target;

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void Move()
    {
        GetComponent<Animation>().Play();

        transform.LookAt(target, transform.position.normalized);
        controller.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    protected override void ChangeState(EnemyState.stateIDs _stateID)
    {
        if (this.state == null || this.state.getID() != _stateID)
        {
            switch (_stateID)
            {
                case EnemyState.stateIDs.Idle:
                    state = new FlyingEnemyStateIdle(this);
                    break;
                default:
                    Debug.LogError("False enemy state provided");
                    break;
            }
        }
    }
}
