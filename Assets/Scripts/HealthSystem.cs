using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    // Eventos para desacoplar la UI y otros sistemas
    public event Action<float, float> OnHealthChanged; // Manda vida actual y vida maxima
    public event Action<int> OnLivesChanged;           // Manda las vidas restantes
    public event Action OnDeath;                       // Grita cuando se acaban las vidas

    private float maxHealth;
    private float currentHealth;
    private int currentLives;

    public void Initialize(float maxHealthData, int maxLivesData)
    {
        maxHealth = maxHealthData;
        currentHealth = maxHealth;
        currentLives = maxLivesData;

        // Avisamos a la UI los valores iniciales
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnLivesChanged?.Invoke(currentLives);
    }
    public void TakeDamage(float amount)
    {
        // Si ya estamos muertos, ignoramos el daño extra
        if (currentLives <= 0) return;

        currentHealth -= amount;

        // Evitamos que la vida baje de 0
        currentHealth = Mathf.Max(currentHealth, 0);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            LoseLife();
        }
    }
    private void LoseLife()
    {
        currentLives--;
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives > 0)
        {
            // Revivir (Restaurar vida para la siguiente ronda)
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        else
        {
            Die();
        }
    }
    private void Die()
    {
        OnDeath?.Invoke();
    }
}
