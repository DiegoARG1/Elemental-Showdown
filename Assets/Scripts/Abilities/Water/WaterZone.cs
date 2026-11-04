using UnityEngine;
using System.Collections.Generic;

public class WaterZone : MonoBehaviour
{
    [Header("Configuracion de Ralentizacion")]
    [Range(0.1f, 0.9f)]
    public float slowMultiplier = 0.5f;
    public float duration = 5f;

    private GameObject caster;

    private Dictionary<PlayerController, float> slowedEnemies = new Dictionary<PlayerController, float>();

    public void Initialize(GameObject creator)
    {
        caster = creator;
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (caster != null && collision.gameObject == caster) return;

        PlayerController targetMovement = collision.GetComponent<PlayerController>();

        if (targetMovement != null && !slowedEnemies.ContainsKey(targetMovement))
        {
            float originalSpeed = targetMovement.moveSpeed;
            slowedEnemies.Add(targetMovement, originalSpeed);

            targetMovement.moveSpeed = originalSpeed * slowMultiplier;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController targetMovement = collision.GetComponent<PlayerController>();

        if (targetMovement != null && slowedEnemies.ContainsKey(targetMovement))
        {
            targetMovement.moveSpeed = slowedEnemies[targetMovement];
            slowedEnemies.Remove(targetMovement);
        }
    }

    private void OnDestroy()
    {
        foreach (var enemy in slowedEnemies)
        {
            if (enemy.Key != null)
            {
                enemy.Key.moveSpeed = enemy.Value;
            }
        }
    }
}