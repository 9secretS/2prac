using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float chaseDistance = 10f;
    public float attackDistance = 1.5f;
    public float attackCooldown = 1f;
    public int damage = 10;

    public float normalSpeed = 3.5f;
    public float fastSpeed = 7f;

    private NavMeshAgent agent;
    private float nextAttackTime;

    int fastArea;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        fastArea = NavMesh.GetAreaFromName("FastArea");
    }

    void Update()
    {
        if (target == null) return;

        CheckFastArea();

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= chaseDistance)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.ResetPath();
        }

        if (dist <= attackDistance && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            var pc = target.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage(damage);
                Debug.Log("Enemy hit player!");
            }
        }
    }

    void CheckFastArea()
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            if ((hit.mask & (1 << fastArea)) != 0)
                agent.speed = fastSpeed;
            else
                agent.speed = normalSpeed;
        }
    }
}