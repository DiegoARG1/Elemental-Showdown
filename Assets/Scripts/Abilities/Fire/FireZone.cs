using UnityEngine;
using System.Collections.Generic;

public class FireZone : MonoBehaviour
{
    [Header("Configuracion del daño")]
    public float damagePerTick = 5f;
    public float tickInterval = 1f;  // Cada cuantos segundos ocurre un tick (1 = cada segundo)
    public float duration = 5f;

    private GameObject caster;
    private List<IDamageable> enemiesInside = new List<IDamageable>();
    private float currentTickTimer;

    public void Initialize(GameObject creator)
    {
        caster = creator;
        currentTickTimer = 0f;
        Destroy(gameObject, duration);
    }
    private void Update()
    {
        if (enemiesInside.Count == 0) return;
        currentTickTimer -= Time.deltaTime;
        if (currentTickTimer <= 0f)
        {
            // Usamos un loop inverso por si el enemigo MUERE mientras lo quemamos
            // Si usamos un foreach normal y el enemigo se destruye (Destroy), Unity nos lanzara un error.
            for (int i = enemiesInside.Count - 1; i >= 0; i--)
            {
                IDamageable target = enemiesInside[i];

                // Verificamos que el enemigo siga existiendo en la escena
                if (target != null && target as MonoBehaviour != null)
                {
                    target.TakeDamage(damagePerTick);
                }
                else
                {
                    enemiesInside.RemoveAt(i);//Si el enemigo ya no esta se remueve
                }
            }
            currentTickTimer = tickInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (caster != null && collision.gameObject == caster) return;

        IDamageable target = collision.GetComponent<IDamageable>();

        if (target != null && !enemiesInside.Contains(target)) //es destructible y no esta en la lista?
        {
            enemiesInside.Add(target);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();

        if (target != null && enemiesInside.Contains(target))
        {
            enemiesInside.Remove(target);
        }
    }
}
