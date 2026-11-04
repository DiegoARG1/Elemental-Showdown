using UnityEngine;

[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(PlayerController))]
public class EnemyAIController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform targetPlayer;
    private SkillManager skillManager;
    private PlayerController playerController;

    [Header("Comportamiento")]
    public float attackRange = 7f;
    public float safeDistance = 3f;
    public float attackCooldown = 2.5f;

    private float nextAttackTime;

    private void Start()
    {
        skillManager = GetComponent<SkillManager>();
        playerController = GetComponent<PlayerController>();

        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) targetPlayer = playerObj.transform;
        }
    }

    private void Update()
    {
        if (targetPlayer == null) return;

        float distance = Vector2.Distance(transform.position, targetPlayer.position);
        ManageMovement(distance);
        ManageCombat(distance);
    }

    private void ManageMovement(float distance)
    {
        Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized;

        if (distance > attackRange)
        {
            // Perseguir
            playerController.SetMoveInput(directionToPlayer);
        }
        else if (distance < safeDistance)
        {
            // Retroceder
            playerController.SetMoveInput(-directionToPlayer);
        }
        else
        {
            // Quedarse quieto y disparar
            playerController.SetMoveInput(Vector2.zero);
        }
    }

    private void ManageCombat(float distance)
    {
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized;
            skillManager.ExecuteAIAttack(directionToPlayer);
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}