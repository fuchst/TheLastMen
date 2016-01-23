using UnityEngine;

// In this state the enemy is flying in circles above an island 
// or randomly changes its current island
public class FlyingEnemyStateIdle : EnemyState {

    protected FlyingEnemy enemy;
    const stateIDs id = stateIDs.Idle;

    private float probabilityOfChange = 0.1f;
    private const float timeToNextChange = 20.0f;
    private float timeSinceLastChange = 0.0f;
    
    private GameObject island;

    // Sign that selects rotation around circle
    private int rotateDirection = 1;
    private const float minCircleRadius = 10.0f;
    private const float maxCircleRadius = 15.0f;
    private float circleRadius;
    private Vector3 circleCenter = Vector3.zero;

    public FlyingEnemyStateIdle(Enemy _enemy)
    {
        enemy = _enemy as FlyingEnemy;
        enemy.target = Vector3.zero;
    }

    public override void action()
    {
        timeSinceLastChange += Time.fixedDeltaTime;

        if (Vector3.Distance(enemy.transform.position, enemy.target) < 5.0f)
        {
            enemy.target = Vector3.zero;
        }

        // Check if the player is close by
        if (enemy.distanceToPlayer < enemy.senseRangeSearching)
        {
            enemy.SendMessage("ChangeState", EnemyState.stateIDs.Attack);
        }
        else if (enemy.target == Vector3.zero)
        {
            Vector3 direction = (enemy.transform.position - circleCenter).normalized;

            // TODO: Time is ignored?!
            // search new island
            if ((timeSinceLastChange > timeToNextChange && Random.Range(0.0f, 1.0f) < probabilityOfChange) || circleCenter == Vector3.zero || island == null)
            {
                timeSinceLastChange = 0.0f;

                GameObject[] islands = GameObject.FindGameObjectsWithTag("Island");
                int rand = UnityEngine.Random.Range(0, islands.Length - 1);
                island = islands[rand];
                circleCenter = island.transform.position + island.transform.up * 15.0f;
                rotateDirection = (Random.Range(0, 1) == 0) ? -1 : 1;
                circleRadius = Random.Range(minCircleRadius, maxCircleRadius);

                // Get targetpoint on circle
                enemy.target = circleCenter + direction - Vector3.Dot(direction, island.transform.up) * island.transform.up * circleRadius;
            } 
            // fly in circles
            else
            {
                enemy.target = enemy.transform.position + rotateDirection * Vector3.Cross(direction, island.transform.up).normalized;
            }
        }
    }
}
