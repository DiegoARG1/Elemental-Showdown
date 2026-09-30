using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private GameObject basicAttackPrefab;
    private bool hasInfiniteAmmo = false;

    //Cargar el prefap con el characterbuilder
    public void Initialize(GameObject basicPrefab)
    {
        basicAttackPrefab = basicPrefab;
    }

    private void OnEnable()
    {
        CombatInputManager.OnBasicAttackReleased += CastBasicAttack;//suscripcion al evento de ataque
    }

    private void OnDisable()
    {
        CombatInputManager.OnBasicAttackReleased -= CastBasicAttack;
    }

    private void CastBasicAttack(Vector2 direction)
    {
        if (basicAttackPrefab == null) return;

        //Instanciamos el Prefab del hechizo en la posicion del jugador
        GameObject spellInstance = Instantiate(basicAttackPrefab, transform.position, Quaternion.identity);

        //Buscamos la interfaz ability en el objeto
        IAbility ability = spellInstance.GetComponent<IAbility>();

        if (ability != null)
        {
            AbilityContext context = new AbilityContext// asignamos el contexto de este objeto
            {
                Caster = this.gameObject,
                TargetDirection = direction
            };

            // ejecutamos la habilidad
            ability.Execute(context);
        }
        else
        {
            //Por si se nos olvida poner la interfaz en el prefap
            Debug.LogError($"[SkillManager] El prefab {basicAttackPrefab.name} no implementa la interfaz IAbility.");
            Destroy(spellInstance);
        }
    }

    public void SetInfiniteAmmo(bool state)
    {
        hasInfiniteAmmo = state;
    }
}