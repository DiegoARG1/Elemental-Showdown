using UnityEngine;
using System;
using System.Collections.Generic;

public class FireConeAbility : MonoBehaviour,IAbility
{
    [Header("Daño")]
    public float damagePerTick = 10f;
    public float tickInterval = 0.5f;

    [Header("Combustible")]
    public float maxFuel = 100f;
    public float fuelCostPerSecond = 35f; // Gasto por segundo
    public float fuelRegenRate = 15f;     // Recuperacion por segundo sin disparar

    public event Action<float, float> OnFuelChanged;//Evento para la barra de combustible

    [SerializeField] private float currentFuel;
    private bool isFiring = false;

    private GameObject caster;
    private Collider2D coneCollider;
    private SpriteRenderer spriteVisual;
    private SkillManager mySkillManager;

    private List<IDamageable> enemiesInside = new List<IDamageable>();
    private float currentTickTimer = 0f;
    private void Awake()
    {
        currentFuel = maxFuel;
        coneCollider = GetComponent<Collider2D>();
        spriteVisual = GetComponentInChildren<SpriteRenderer>();
        coneCollider.enabled = false;//El arma nace apagada
        if (spriteVisual != null) spriteVisual.enabled = false;//Apagado visual
    }

    public void OnInputStarted(AbilityContext context)//Cuando tocas la pantalla
    {
        caster = context.Caster;
        mySkillManager = caster.GetComponent<SkillManager>();
        bool isInfinite = mySkillManager != null && mySkillManager.hasInfiniteAmmo;//evaluamos si la skill 1 esta encendida
        if (currentFuel > 0 || isInfinite)
        {
            isFiring = true;
            coneCollider.enabled = true;
            if (spriteVisual != null) spriteVisual.enabled = true;

            UpdateRotation(context.TargetDirection);
            //Aqui se puede agregar el sistema de particulas
        }
    }

    public void OnInputStay(AbilityContext context)//Mientras mueves el dedo
    {
        if (isFiring)
        {
            UpdateRotation(context.TargetDirection);
        }
    }

    public void OnInputEnded(AbilityContext context)//Sueltas el dedo
    {
        StopFiring();
    }

    private void UpdateRotation(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isInfinite = mySkillManager != null && mySkillManager.hasInfiniteAmmo;

        if (isFiring)
        {
            if (!isInfinite)
            {
                currentFuel -= fuelCostPerSecond * Time.deltaTime;

                if (currentFuel <= 0)//Si no hay combustible no se puedo usar el fuego
                {
                    currentFuel = 0;
                    StopFiring();
                }
            }
            if (isFiring && enemiesInside.Count > 0)
            {
                currentTickTimer -= Time.deltaTime;
                if (currentTickTimer <= 0f)
                {
                    for (int i = enemiesInside.Count - 1; i >= 0; i--)
                    {
                        IDamageable target = enemiesInside[i];
                        if (target != null && target as MonoBehaviour != null)
                        {
                            target.TakeDamage(damagePerTick);
                        }
                        else
                        {
                            enemiesInside.RemoveAt(i);
                        }
                    }
                    currentTickTimer = tickInterval;
                }
            }
        }
        else
        {
            //Regeneracion pasiva de combustible
            if (currentFuel < maxFuel)
            {
                currentFuel += fuelRegenRate * Time.deltaTime;
                if (currentFuel > maxFuel) currentFuel = maxFuel; // Poner el tope
            }
        }

        OnFuelChanged?.Invoke(currentFuel, maxFuel);
    }
    private void StopFiring()
    {
        isFiring = false;
        coneCollider.enabled = false;
        if (spriteVisual != null) spriteVisual.enabled = false;
        enemiesInside.Clear();
        currentTickTimer = 0f;
        //Detener particulas en un futuro
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (caster != null && collision.gameObject == caster) return;

        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null && !enemiesInside.Contains(target))
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
