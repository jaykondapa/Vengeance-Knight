using UnityEngine;

/// <summary>
/// Enemy AI - Patrols back and forth, detects player, chases and attacks.
/// Attach this to every enemy GameObject.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    public float patrolSpeed = 2f;

    [Header("Detection Settings")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;

    private Transform player;
    private PlayerHealth playerHealth;
    private Animator animator;
    private EnemyHealth enemyHealth;

    private Vector3 startPosition;
    private Vector3 patrolTarget;
    private float attackTimer = 0f;

    private enum EnemyState { Patrol, Chase, Attack, Dead }
    private EnemyState currentState = EnemyState.Patrol;

    private bool movingRight = true;

    // Safely track which parameters exist
    private bool hasSpeedParam = false;
    private bool hasAttackParam = false;

    void Start()
    {
        startPosition = transform.position;
        patrolTarget  = startPosition + Vector3.right * patrolDistance;

        animator    = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        // Check which animation parameters exist safely
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == "Speed")  hasSpeedParam  = true;
                if (param.name == "Attack") hasAttackParam = true;
            }
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player       = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            currentState = EnemyState.Dead;
            SetSpeed(0f);
            return;
        }

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);

            if (dist <= attackRange)
                currentState = EnemyState.Attack;
            else if (dist <= detectionRange)
                currentState = EnemyState.Chase;
            else
                currentState = EnemyState.Patrol;
        }

        switch (currentState)
        {
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Chase:  Chase();  break;
            case EnemyState.Attack: Attack(); break;
        }
    }

    void Patrol()
    {
        SetSpeed(patrolSpeed);

        transform.position = Vector3.MoveTowards(
            transform.position, patrolTarget, patrolSpeed * Time.deltaTime);

        if (movingRight)
            transform.rotation = Quaternion.LookRotation(Vector3.right);
        else
            transform.rotation = Quaternion.LookRotation(Vector3.left);

        if (Vector3.Distance(transform.position, patrolTarget) < 0.2f)
        {
            movingRight = !movingRight;
            patrolTarget = movingRight
                ? startPosition + Vector3.right * patrolDistance
                : startPosition - Vector3.right * patrolDistance;
        }
    }

    void Chase()
    {
        SetSpeed(chaseSpeed);

        transform.position = Vector3.MoveTowards(
            transform.position, player.position, chaseSpeed * Time.deltaTime);

        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void Attack()
    {
        SetSpeed(0f);

        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;

            if (hasAttackParam)
                animator?.SetTrigger("Attack");

            PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
            bool wasBlocked = playerCombat != null &&
                              playerCombat.TryBlock(attackDamage, false, transform);

            if (!wasBlocked)
                playerHealth?.TakeDamage(attackDamage);
        }
    }

    void SetSpeed(float speed)
    {
        if (hasSpeedParam)
            animator?.SetFloat("Speed", speed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawLine(
            start - Vector3.right * patrolDistance,
            start + Vector3.right * patrolDistance);
    }
}