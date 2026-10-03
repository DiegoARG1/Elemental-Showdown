using UnityEngine;

public class FireballAbility : MonoBehaviour, IAbility
{
    [Header("Configuracion")]
    public GameObject fireballPrefab;
    public float maxRange = 4f;
    public float cooldown = 3f;

    [Header("Visuales")]
    public GameObject indicatorVisual;

    private float currentCooldown = 0f;
    private GameObject caster;
    private Vector3 currentTargetPosition;

    private void Awake()
    {
        if (indicatorVisual != null) indicatorVisual.SetActive(false);
    }

    public void OnInputStarted(AbilityContext context)
    {
        if (currentCooldown > 0) return;

        caster = context.Caster;
        currentTargetPosition = caster.transform.position;

        if (indicatorVisual != null) indicatorVisual.SetActive(true);
    }

    public void OnInputStay(AbilityContext context)
    {
        if (currentCooldown > 0 || caster == null) return;

        // context.TargetDirection ahora mide de 0 a 1 gracias a nuestro cambio en el Input
        Vector3 dragVector = new Vector3(context.TargetDirection.x, context.TargetDirection.y, 0);

        // Coordenada exacta: Pies del jugador + (Vector del dedo * Rango Maximo)
        currentTargetPosition = caster.transform.position + (dragVector * maxRange);

        if (indicatorVisual != null)
        {
            indicatorVisual.transform.position = currentTargetPosition;
        }
    }

    public void OnInputEnded(AbilityContext context)
    {
        if (currentCooldown > 0 || caster == null) return;

        if (indicatorVisual != null) indicatorVisual.SetActive(false);

        if (fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, caster.transform.position, Quaternion.identity);

            FireballProjectile script = fireball.GetComponent<FireballProjectile>();
            if (script != null)
            {
                script.Initialize(caster, currentTargetPosition);
            }
        }

        currentCooldown = cooldown;
    }

    private void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }
}
