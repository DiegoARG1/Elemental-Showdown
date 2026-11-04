using UnityEngine;

public class WaterBombAbility : MonoBehaviour, IAbility
{
    [Header("Configuracion")]
    public GameObject waterBombPrefab;
    public float maxRange = 6f;
    public float cooldown = 4f;

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

        Vector3 dragVector = new Vector3(context.TargetDirection.x, context.TargetDirection.y, 0);
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

        if (waterBombPrefab != null)
        {
            GameObject bomb = Instantiate(waterBombPrefab, caster.transform.position, Quaternion.identity);

            WaterBombProjectile script = bomb.GetComponent<WaterBombProjectile>();
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