using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private GameObject basicAttackPrefab;
    private IAbility equippedBasicAttack;
    private bool hasInfiniteAmmo = false;

    //Cargar el prefap con el characterbuilder
    public void Initialize(GameObject basicPrefab)
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

    public void SetInfiniteAmmo(bool state)
    {
        hasInfiniteAmmo = state;
    }
}