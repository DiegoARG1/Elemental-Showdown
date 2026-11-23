using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(SkillManager))]
public class CharacterBuilder : MonoBehaviour   
{
    [Header("CharacterData")]
    [SerializeField] private CharacterData characterData;

    private void Start()
    {
        SkillManager skills = GetComponent<SkillManager>();

        if (!skills.isAI && CharacterSelection.selected != null)
        {
            characterData = CharacterSelection.selected;
        }

        if (characterData == null)
        {
            Debug.LogError($"[{gameObject.name}] Falta asignar el CharacterData en el CharacterBuilder.");
            return;
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null && characterData.animatorController != null)
        {
            anim.runtimeAnimatorController = characterData.animatorController;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && characterData.defaultSprite != null)
        {
            spriteRenderer.sprite = characterData.defaultSprite;
        }

        HealthSystem health = GetComponent<HealthSystem>();
        PlayerController movement = GetComponent<PlayerController>();

        //Inyectar las variables del ScriptableObject
        health.Initialize(characterData.maxHealth, characterData.maxLives);
        movement.Initialize(characterData.moveSpeed);

        if (characterData.basicAttackPrefab != null)
        {
            skills.Initialize(characterData.basicAttackPrefab,characterData.skill1Prefab,characterData.skill2Prefab);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] El CharacterData no tiene un Basic Attack asignado.");
        }
        if (skills != null && !skills.isAI)
        {
            HUDManager hud = FindAnyObjectByType<HUDManager>();
            if (hud != null)
            {
                hud.SetCharacterHUD(characterData);
            }
        }
    }
}
