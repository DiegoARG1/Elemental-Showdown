using UnityEngine;
using System.Collections;

public class WaterDiveAbility : MonoBehaviour, IAbility
{
    [Header("Configuracion")]
    public float diveDuration = 3f;
    public float cooldown = 6f;

    private float currentCooldown = 0f;

    public void OnInputStarted(AbilityContext context)
    {
        if (currentCooldown > 0) return;

        StartCoroutine(DiveRoutine(context.Caster));

        currentCooldown = cooldown;
    }

    public void OnInputStay(AbilityContext context) {}

    public void OnInputEnded(AbilityContext context) {}

    private IEnumerator DiveRoutine(GameObject caster)
    {
        HealthSystem health = caster.GetComponent<HealthSystem>();
        SpriteRenderer sprite = caster.GetComponent<SpriteRenderer>();

        int originalLayer = caster.layer;

        if (health != null) health.isInvincible = true;

        if (sprite != null) sprite.color = new Color(1f, 1f, 1f, 0.5f);

        caster.layer = LayerMask.NameToLayer("Submerged");

        yield return new WaitForSeconds(diveDuration);

        if (health != null) health.isInvincible = false;
        if (sprite != null) sprite.color = Color.white;
        caster.layer = originalLayer;
    }

    private void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }
}
