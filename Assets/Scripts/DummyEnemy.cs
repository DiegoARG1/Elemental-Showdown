using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (sprite != null) sprite.color = Color.red;
        Invoke(nameof(ResetColor), 0.1f);

        Debug.Log($"[Dummy] Recibiendo {amount} de daño. Vida restante: {Mathf.Round(health)}");

        if (health <= 0)
        {
            Debug.Log("[Dummy] Destruido por el fuego.");
            Destroy(gameObject);
        }
    }

    private void ResetColor()
    {
        if (sprite != null) sprite.color = Color.white;
    }
}
