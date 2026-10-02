using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private GameObject basicAttackPrefab;
    private IAbility equippedBasicAttack;
    private IAbility equippedSkill1;
    public bool hasInfiniteAmmo { get; set; } = false;

    //Cargar el prefap con el characterbuilder
    public void Initialize(GameObject basicPrefab,GameObject skill1Prefap)
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
    }

    private void OnEnable()
    {
        CombatInputManager.OnBasicAttackStarted += HandleBasicAttackStarted;
        CombatInputManager.OnBasicAttackStay += HandleBasicAttackStay;
        CombatInputManager.OnBasicAttackEnded += HandleBasicAttackEnded;
    }

    private void OnDisable()
    {
        CombatInputManager.OnBasicAttackStarted -= HandleBasicAttackStarted;
        CombatInputManager.OnBasicAttackStay -= HandleBasicAttackStay;
        CombatInputManager.OnBasicAttackEnded -= HandleBasicAttackEnded;
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
}