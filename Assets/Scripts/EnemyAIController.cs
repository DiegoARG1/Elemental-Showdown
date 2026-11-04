using UnityEngine;

[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(PlayerController))]
public class EnemyAIController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform targetPlayer;
    private SkillManager skillManager;
    private PlayerController playerController;
    private WaterStreamAbility waterWeapon;

    [Header("Rangos de vision")]
    public float skill2Range = 7f;
    public float basicAttackRange = 2.4f;
    public float safeDistance = 1.5f;

    [Header("Movimiento")]
    public float changeStrafeInterval = 2f;
    private float nextStrafeTime;
    private int strafeDirection = 1;

    [Header("Evasion de obstaculos")]
    public LayerMask obstacleMask;
    public float obstacleCheckDistance = 1.2f;

    [Header("Tiempos de ataque")]
    public float basicAttackCooldown = 1.5f;
    public float skill2Cooldown = 6f;

    private float nextBasicAttackTime;
    private float nextSkill2Time;

    private bool isFiringBasic = false;
    private bool isReloading = false;

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

        if (waterWeapon == null)
        {
            waterWeapon = GetComponentInChildren<WaterStreamAbility>(true);
        }

        float distance = Vector2.Distance(transform.position, targetPlayer.position);
        ManageMovement(distance);
        ManageCombat(distance);
    }

    private void ManageMovement(float distance)
    {
        Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized;

        if (waterWeapon != null)
        {
            if (waterWeapon.currentFuel <= 0f) isReloading = true;
            if (waterWeapon.currentFuel >= waterWeapon.maxFuel * 0.8f) isReloading = false;
        }

        if (isReloading)
        {
            playerController.SetMoveInput(AvoidObstacles(-directionToPlayer));
            return;
        }

        if (Time.time >= nextStrafeTime)
        {
            strafeDirection = Random.Range(0, 2) == 0 ? 1 : -1;
            nextStrafeTime = Time.time + Random.Range(changeStrafeInterval * 0.5f, changeStrafeInterval * 1.5f);
        }

        Vector2 strafeVector = new Vector2(-directionToPlayer.y, directionToPlayer.x) * strafeDirection;

        if (distance > basicAttackRange)
        {
            Vector2 desiredMove = (directionToPlayer + strafeVector * 0.5f).normalized;
            playerController.SetMoveInput(AvoidObstacles(desiredMove));
        }
        else if (distance < safeDistance)
        {
            playerController.SetMoveInput(AvoidObstacles(-directionToPlayer));
            skillManager.CastSkill1();
        }
        else
        {
            playerController.SetMoveInput(AvoidObstacles(strafeVector));
        }
    }

    private Vector2 AvoidObstacles(Vector2 desiredDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, desiredDirection, obstacleCheckDistance, obstacleMask);

        if (hit.collider != null)
        {
            Vector2 perp = new Vector2(-desiredDirection.y, desiredDirection.x);
            return (desiredDirection + perp).normalized;
        }

        return desiredDirection;
    }

    private void ManageCombat(float distance)
    {
        Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized;

        if (isReloading)
        {
            if (isFiringBasic)
            {
                skillManager.StopAIBasicAttack();
                isFiringBasic = false;
            }
            return;
        }

        if (distance <= skill2Range && distance > basicAttackRange && Time.time >= nextSkill2Time)
        {
            if (isFiringBasic)
            {
                skillManager.StopAIBasicAttack();
                isFiringBasic = false;
            }

            skillManager.OnSkill2Started();
            skillManager.OnSkill2Stay(directionToPlayer);
            skillManager.OnSkill2Ended();

            nextSkill2Time = Time.time + skill2Cooldown;
        }
        else if (distance <= basicAttackRange)
        {
            if (!isFiringBasic)
            {
                skillManager.StartAIBasicAttack(directionToPlayer);
                isFiringBasic = true;
            }
            else
            {
                skillManager.StayAIBasicAttack(directionToPlayer);
            }
        }
        else
        {
            if (isFiringBasic)
            {
                skillManager.StopAIBasicAttack();
                isFiringBasic = false;
            }
        }
    }
}