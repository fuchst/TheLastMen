using UnityEngine;

public class FlyingEnemy : Enemy {

    public Vector3 target { get; set; }

    // Distance after which the enemy stops following the player
    public float maxDistance = 50.0f;

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        state.action();
        Move();
    }

    protected override void Move()
    {
        GetComponent<Animation>().Play();

        // Get vector to target
        Vector3 toTarget = target - transform.position;

        // Get direction to target
        //Vector3 direction = toTarget.normalized;

        // Get tangent of circle with radius of transform.position
        //Vector3 tangent = Vector3.Cross(Vector3.Cross(-direction, transform.position), transform.position).normalized;
        Vector3 tangent = Vector3.Cross(Vector3.Cross(-toTarget, transform.position), transform.position).normalized;

        // Height difference of target and transform.position
        float diffHeight = transform.position.magnitude - target.magnitude;

        // Angle between both vectors
        float angle = Mathf.Abs(Vector3.Angle(target, transform.position));

        // Get distance on circle
        float distance = 2.0f * transform.position.magnitude * Mathf.PI * angle / 180.0f;

        // Calculate move direction
        Vector3 moveDirection = (tangent - diffHeight / distance * transform.up).normalized;

        // Alter flight path incase of obstacles
        moveDirection = evade(moveDirection);

        transform.LookAt(transform.position + moveDirection, transform.position.normalized);
        _controller.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    private Vector3 evade(Vector3 currentDirection)
    {
        return currentDirection;
    }

    public override void ChangeState(EnemyState.stateIDs _stateID)
    {
        if (this.state == null || this.state.getID() != _stateID)
        {
            switch (_stateID)
            {
                case EnemyState.stateIDs.Idle:
                    state = new FlyingEnemyStateIdle(this);
                    break;
                case EnemyState.stateIDs.Attack:
                    state = new FlyingEnemyStateAttack(this);
                    break;
                default:
                    Debug.LogError("False enemy state provided");
                    break;
            }
        }
    }
}
