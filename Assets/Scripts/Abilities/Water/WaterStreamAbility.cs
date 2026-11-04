using UnityEngine;
using System;
using System.Collections.Generic;

public class WaterStreamAbility : MonoBehaviour, IAbility
{
    [Header("Daño")]
    public float damagePerTick = 10f;
    public float tickInterval = 0.5f;

    [Header("Combustible")]
    public float maxFuel = 100f;
    public float fuelCostPerSecond = 30f;
    public float fuelRegenRate = 20f;
    public float currentFuel { get; private set; }

    public event Action<float, float> OnFuelChanged;

    private bool isFiring = false;
    private GameObject caster;
    private BoxCollider2D streamCollider;
    private SpriteRenderer spriteVisual;
    private SkillManager mySkillManager;

    private List<IDamageable> enemiesInside = new List<IDamageable>();
    private float currentTickTimer = 0f;

    private void Awake()
    {
        currentFuel = maxFuel;
        streamCollider = GetComponentInChildren<BoxCollider2D>();
        spriteVisual = GetComponentInChildren<SpriteRenderer>();

        if (streamCollider != null) streamCollider.enabled = false;
        if (spriteVisual != null) spriteVisual.enabled = false;
    }

    public void OnInputStarted(AbilityContext context)
    {
        mySkillManager = context.Caster.GetComponent<SkillManager>();
        bool isInfinite = mySkillManager != null && mySkillManager.hasInfiniteAmmo;

        if (currentFuel > 0 || isInfinite)
        {
            caster = context.Caster;
            isFiring = true;

            if (streamCollider != null) streamCollider.enabled = true;
            if (spriteVisual != null) spriteVisual.enabled = true;

            UpdateRotation(context.TargetDirection);
        }
    }

    public void OnInputStay(AbilityContext context)
    {
        if (isFiring) UpdateRotation(context.TargetDirection);
    }

    public void OnInputEnded(AbilityContext context)
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

    void Update()
    {
        bool isInfinite = mySkillManager != null && mySkillManager.hasInfiniteAmmo;

        if (isFiring)
        {
            if (!isInfinite)
            {
                currentFuel -= fuelCostPerSecond * Time.deltaTime;
                if (currentFuel <= 0)
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
            if (currentFuel < maxFuel)
            {
                currentFuel += fuelRegenRate * Time.deltaTime;
                if (currentFuel > maxFuel) currentFuel = maxFuel;
            }
        }

        OnFuelChanged?.Invoke(currentFuel, maxFuel);
    }

    private void StopFiring()
    {
        isFiring = false;
        if (streamCollider != null) streamCollider.enabled = false;
        if (spriteVisual != null) spriteVisual.enabled = false;
        enemiesInside.Clear();
        currentTickTimer = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (caster != null && collision.gameObject == caster) return;
        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null && !enemiesInside.Contains(target)) enemiesInside.Add(target);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null && enemiesInside.Contains(target)) enemiesInside.Remove(target);
    }
}