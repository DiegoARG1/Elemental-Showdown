using UnityEngine;
using System.Collections;
public class InfiniteFuelBuff : MonoBehaviour, IAbility
{
    [Header("Configuración del Buff")]
    public float duration = 5f;
    public float cooldown = 10f;
    [SerializeField] private float currentCooldown = 0f;
    private SkillManager mySkillManager;

    public void OnInputStarted(AbilityContext context)//Al tocar el boton
    {
        mySkillManager = context.Caster.GetComponent<SkillManager>();

        if (currentCooldown > 0)
        {
            Debug.Log($"[Buff] Habilidad en recarga. Faltan {currentCooldown:F1} segundos.");
            return;
        }

        if (mySkillManager != null)
        {
            mySkillManager.hasInfiniteAmmo = true;//Se enciende la municion infinita
            currentCooldown = cooldown;
            //Aqui se pueden poner mas efectos de la habilidad

            StartCoroutine(BuffDurationRoutine());//Iniciamos el tiempo para apagarlo
        }
    }

    // Se que esto esta mal pero por tiempo sorry:(
    public void OnInputStay(AbilityContext context) { }
    public void OnInputEnded(AbilityContext context) { }

    private void Update()
    {
        // Si el cooldown es mayor a 0, le restamos el tiempo que va pasando
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;

            // Cuando termine, lo clavamos exactamente en 0 para evitar numeros negativos
            if (currentCooldown <= 0)
            {
                currentCooldown = 0;
            }
        }
    }
    private IEnumerator BuffDurationRoutine()
    {
        yield return new WaitForSeconds(duration);//El codigo se pausu durante X seg

        //Al pasar el tiempo,lo apagamos
        if (mySkillManager != null)
        {
            mySkillManager.hasInfiniteAmmo = false;
        }
    }
}
