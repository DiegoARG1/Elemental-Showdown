using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "ElementalShowdown/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Estadisticas Base")]
    public string characterName;
    public int maxLives = 3;
    public float maxHealth = 100f;
    public float moveSpeed = 5f;

    [Header("Habilidades (Prefabs)")]
    public GameObject basicAttackPrefab;
    public GameObject ability1Prefab;
    public GameObject ability2Prefab;
}
