using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GhostAI : MonoBehaviour
{
    #region Public Variables
    [Space, Header("Enums")]
    public AIState currState = AIState.Wander;
    public enum AIState { Wander, Chase, Attack };

    [Space, Header("Enemy Variables")]
    public GameObject player;
    public float idleDelay = 1f;
    public float enemyWanderspeed = 3.5f;
    public float enemyChaseSpeed = 4f;
    public float chaseDistance;
    public float attackDistance;

    [Space, Header("Wander Variables")]
    public float wanderRadius;
    public float maxWanderTimer = 2f;
    #endregion

    #region Private Variables
    [Header("Enemy AI Variables")]
    private NavMeshAgent _ghostAgent;
    private Animator _ghostAnim;
    [SerializeField] private float distanceToPlayer;

    [Header("Wander Variables")]
    private float _currTimer;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _ghostAgent = GetComponent<NavMeshAgent>();
        _ghostAgent.speed = enemyWanderspeed;

        _ghostAnim = GetComponent<Animator>();

        _currTimer = maxWanderTimer;
    }

    void Update()
    {
        // Disance check from player and syncing ghost animation with navmesh agent magnitude;
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        _ghostAnim.SetFloat("Speed", _ghostAgent.velocity.magnitude);

        States();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
    #endregion

    #region My Functions

    #region FSM
    void States()
    {
        switch (currState) // Using Switch Case method for making Finite State Machine;
        {
            case AIState.Wander:
                WanderState();
                //Debug.Log("Wandering");

                if (distanceToPlayer <= chaseDistance) // If Player is within the chase range, Chase;
                    currState = AIState.Chase;
                break;

            case AIState.Chase:
                ChaseState();
                //Debug.Log("Chasing");

                if (distanceToPlayer >= chaseDistance) // If Player is outside the chase range, Wander;
                    currState = AIState.Wander;

                if (distanceToPlayer <= attackDistance) // If Player is within the attack range, Attack;
                    currState = AIState.Attack;
                break;

            case AIState.Attack:
                AttackState();
                //Debug.Log("Attacking");

                if (!player.activeInHierarchy) // If Player is inactive/destroyed, Wander;
                    currState = AIState.Wander;

                if (distanceToPlayer >= attackDistance && player.activeInHierarchy) // If Player is active and outside the attack range, Chase;
                    currState = AIState.Chase;
                break;

            default:
                break;
        }
    }
    #endregion

    #region AI Movements

    #region States
    void WanderState() // Wander Function;
    {
        _ghostAnim.SetBool("isAttacking", false);

        _ghostAgent.speed = enemyWanderspeed;

        _currTimer += Time.deltaTime;
        if (_currTimer >= maxWanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            _ghostAgent.SetDestination(newPos);
            _currTimer = 0;
        }
    }

    void ChaseState() // Chase Function;
    {
        _ghostAnim.SetBool("isAttacking", false);

        _ghostAgent.speed = enemyChaseSpeed;
        _ghostAgent.SetDestination(player.transform.position);
    }

    void AttackState() // Attack Function;
    {
        _ghostAnim.SetBool("isAttacking", true);
        SceneManager.LoadScene("LooseScene");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region AI Checks
    static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        //Sets a random position inside the sphere and that is multiplied with the distance and the center of the sphere;
        Vector3 randomPos = Random.insideUnitSphere * dist;

        //Vector 3 position is returned to the origin parameter;
        randomPos += origin;

        //Bool check if the random position is suitable on the navmesh. If true, then return the hit position;
        NavMesh.SamplePosition(randomPos, out NavMeshHit hit, dist, layermask);
        return hit.position;
    }
    #endregion

    #endregion

    #endregion
}