using UnityEngine;
using System;

public class FireConeAbility : MonoBehaviour,IAbility
{
    [Header("Daño")]
    public float damagePerSecond = 20f;

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
        if (currentFuel > 0)
        {
            caster = context.Caster;
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
        if (isFiring)
        {
            currentFuel -= fuelCostPerSecond * Time.deltaTime;//Gasto de gasolina

            if (currentFuel <= 0)//Si no hay combustible no se puedo usar el fuego
            {
                currentFuel = 0;
                StopFiring();
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
        //Detener particulas en un futuro
    }
    private void OnTriggerStay2D(Collider2D collision)//Sistema de daño
    {
        if (caster != null && collision.gameObject == caster) return;//Para no hacer daño a si mismo

        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
