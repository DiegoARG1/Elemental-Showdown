using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(PlayerController))]
public class CharacterBuilder : MonoBehaviour   
{
    [Header("CharacterData")]
    [SerializeField] private CharacterData characterData;

    private void Awake()
    {
        if (characterData == null)
        {
            Debug.LogError($"[{gameObject.name}] Falta asignar el CharacterData en el CharacterBuilder.");
            return;
        }

        HealthSystem health = GetComponent<HealthSystem>();
        PlayerController movement = GetComponent<PlayerController>();

        //Inyectar las variables del ScriptableObject
        health.Initialize(characterData.maxHealth, characterData.maxLives);
        movement.Initialize(characterData.moveSpeed);

        // (En el futuro, aqui tambien inyectaremos los datos al SkillManager)
    }
}
