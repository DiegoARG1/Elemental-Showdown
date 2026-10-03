using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private GameObject basicAttackPrefab;
    private IAbility equippedBasicAttack;
    private IAbility equippedSkill1;
    private IAbility equippedSkill2;
    public bool hasInfiniteAmmo { get; set; } = false;

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
        CombatInputManager.OnBasicAttackStarted += HandleBasicAttackStarted;
        CombatInputManager.OnBasicAttackStay += HandleBasicAttackStay;
        CombatInputManager.OnBasicAttackEnded += HandleBasicAttackEnded;

        CombatInputManager.OnSkill2Started += OnSkill2Started;
        CombatInputManager.OnSkill2Stay += OnSkill2Stay;
        CombatInputManager.OnSkill2Ended += OnSkill2Ended;
    }

    private void OnDisable()
    {
        CombatInputManager.OnBasicAttackStarted -= HandleBasicAttackStarted;
        CombatInputManager.OnBasicAttackStay -= HandleBasicAttackStay;
        CombatInputManager.OnBasicAttackEnded -= HandleBasicAttackEnded;

        CombatInputManager.OnSkill2Started -= OnSkill2Started;
        CombatInputManager.OnSkill2Stay -= OnSkill2Stay;
        CombatInputManager.OnSkill2Ended -= OnSkill2Ended;
    }

    private void HandleBasicAttackStarted(Vector2 direction)
    {
        if (equippedBasicAttack != null)
        {
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = direction };
            equippedBasicAttack.OnInputStarted(context);
        }
    }

    private void HandleBasicAttackStay(Vector2 direction)
    {
        if (equippedBasicAttack != null)
        {
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = direction };
            equippedBasicAttack.OnInputStay(context);
        }
    }

    private void HandleBasicAttackEnded()
    {
        if (equippedBasicAttack != null)
        {
            // Al soltar el dedo ya no hay direccion, mandamos Vector2.zero por defecto
            AbilityContext context = new AbilityContext { Caster = gameObject, TargetDirection = Vector2.zero };
            equippedBasicAttack.OnInputEnded(context);
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
        }
    }
}