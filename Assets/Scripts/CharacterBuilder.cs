using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(SkillManager))]
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
        SkillManager skills = GetComponent<SkillManager>();
        if (characterData.basicAttackPrefab != null)
        {
            skills.Initialize(characterData.basicAttackPrefab,characterData.skill1Prefab);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] El CharacterData no tiene un Basic Attack asignado.");
        }
    }
}
