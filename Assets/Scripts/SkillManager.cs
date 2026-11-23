using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    [Header("Configuracion IA")]
    public bool isAI = false;

    public bool hasInfiniteAmmo { get; set; } = false;
    private GameObject basicAttackPrefab;
    private IAbility equippedBasicAttack;
    private IAbility equippedSkill1;
    private IAbility equippedSkill2;

    private Animator anim;
    private PlayerController playerController;
    private bool isBasicAttacking = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    //Cargar el prefap con el characterbuilder
    public void Initialize(GameObject basicPrefab, GameObject skill1Prefap, GameObject skill2Prefap)
    {
        basicAttackPrefab = basicPrefab;

        if (basicAttackPrefab != null)//Creamos el arma como hija del jugador
        {
            GameObject weaponInstance = Instantiate(basicAttackPrefab, transform.position, Quaternion.identity, transform);
            equippedBasicAttack = weaponInstance.GetComponent<IAbility>();

            if (equippedBasicAttack == null)
            {
                Debug.LogError($"[SkillManager] El prefab {basicAttackPrefab.name} no tiene un script que implemente IAbility.");
            }
        }
        if (skill1Prefap != null)
        {
            GameObject skill1Instance = Instantiate(skill1Prefap, transform.position, Quaternion.identity, transform);
            equippedSkill1 = skill1Instance.GetComponent<IAbility>();
        }
        if (skill2Prefap != null)
        {
            GameObject skill2Instance = Instantiate(skill2Prefap, transform.position, Quaternion.identity, transform);
            equippedSkill2 = skill2Instance.GetComponent<IAbility>();
        }
    }

    private void OnEnable()
    {
        if (!isAI)
        {
            CombatInputManager.OnBasicAttackStarted += HandleBasicAttackStarted;
            CombatInputManager.OnBasicAttackStay += HandleBasicAttackStay;
            CombatInputManager.OnBasicAttackEnded += HandleBasicAttackEnded;

            CombatInputManager.OnSkill2Started += OnSkill2Started;
            CombatInputManager.OnSkill2Stay += OnSkill2Stay;
            CombatInputManager.OnSkill2Ended += OnSkill2Ended;
        }
    }

    private void OnDisable()
    {
        if (!isAI)
        {
            CombatInputManager.OnBasicAttackStarted -= HandleBasicAttackStarted;
            CombatInputManager.OnBasicAttackStay -= HandleBasicAttackStay;
            CombatInputManager.OnBasicAttackEnded -= HandleBasicAttackEnded;

            CombatInputManager.OnSkill2Started -= OnSkill2Started;
            CombatInputManager.OnSkill2Stay -= OnSkill2Stay;
            CombatInputManager.OnSkill2Ended -= OnSkill2Ended;
        }
    }

    private void HandleBasicAttackStarted(Vector2 direction)
    {
        if (equippedBasicAttack != null)
        {
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = direction };
            equippedBasicAttack.OnInputStarted(context);
            SetBasicAttackAnim(true);
            if (playerController != null) playerController.SetAimDirection(direction);
        }
    }

    private void HandleBasicAttackStay(Vector2 direction)
    {
        if (equippedBasicAttack != null)
        {
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = direction };
            equippedBasicAttack.OnInputStay(context);
            if (playerController != null) playerController.SetAimDirection(direction);
        }
    }

    private void HandleBasicAttackEnded()
    {
        if (equippedBasicAttack != null)
        {
            // Al soltar el dedo ya no hay direccion, mandamos Vector2.zero por defecto
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = Vector2.zero };
            equippedBasicAttack.OnInputEnded(context);
            SetBasicAttackAnim(false);
            if (playerController != null) playerController.ClearAimDirection();
        }
    }

    public void CastSkill1()
    {
        if (equippedSkill1 != null)
        {
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = Vector2.zero };//no necesita direccion
            equippedSkill1.OnInputStarted(context);
        }
    }

    public void OnSkill2Started()
    {
        if (equippedSkill2 != null)
        {
            equippedSkill2.OnInputStarted(new AbilityContext { Caster = gameObject, TargetDirection = Vector2.zero });
        }
    }

    public void OnSkill2Stay(Vector2 dragDirection)
    {
        if (equippedSkill2 != null)
        {
            equippedSkill2.OnInputStay(new AbilityContext { Caster = gameObject, TargetDirection = dragDirection });
        }
    }

    public void OnSkill2Ended()
    {
        if (equippedSkill2 != null)
        {
            equippedSkill2.OnInputEnded(new AbilityContext { Caster = gameObject, TargetDirection = Vector2.zero });
            StartCoroutine(BriefSkillAttackAnim(0.3f));
        }
    }

    //public void ExecuteAIAttack(Vector2 direction)
    //{
    //    if (equippedBasicAttack != null)
    //    {
    //        AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = direction };
    //        equippedBasicAttack.OnInputStarted(context);
    //        equippedBasicAttack.OnInputEnded(context);
    //    }
    //}

    public void StartAIBasicAttack(Vector2 direction)
    {
        if (equippedBasicAttack != null)
        {
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = direction };
            equippedBasicAttack.OnInputStarted(context);
            SetBasicAttackAnim(true);
            if (playerController != null) playerController.SetAimDirection(direction);
        }
    }

    public void StayAIBasicAttack(Vector2 direction)
    {
        if (equippedBasicAttack != null)
        {
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = direction };
            equippedBasicAttack.OnInputStay(context);
            if (playerController != null) playerController.SetAimDirection(direction);
        }
    }

    public void StopAIBasicAttack()
    {
        if (equippedBasicAttack != null)
        {
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = Vector2.zero };
            equippedBasicAttack.OnInputEnded(context);
            SetBasicAttackAnim(false);
            if (playerController != null) playerController.ClearAimDirection();
        }
    }
    private void SetBasicAttackAnim(bool state)
    {
        isBasicAttacking = state;
        if (anim != null) anim.SetBool("IsAttacking", state);
    }

    private IEnumerator BriefSkillAttackAnim(float duration)
    {
        if (anim != null) anim.SetBool("IsAttacking", true);
        yield return new WaitForSeconds(duration);
        if (anim != null && !isBasicAttacking) anim.SetBool("IsAttacking", false);
    }
}