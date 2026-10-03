using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [Header("Vuelo e impacto")]
    public float speed = 12f;
    public float impactDamage = 40f;

    [Header("area de impacto")]
    public GameObject fireZonePrefab;

    private GameObject caster;
    private Vector3 targetPosition;
    private bool hasExploded = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Initialize(GameObject shooter, Vector3 destination)
    {
        caster = shooter;
        targetPosition = destination;
    }

    private void FixedUpdate()
    {
        if (hasExploded) return;
        Vector2 newPos = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, targetPosition) <= 0.01f)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;
        if (caster != null && collision.gameObject == caster) return;

        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(impactDamage);//Daño por impacto directo 
        }

        Explode();
    }

    private void Explode()
    {
        hasExploded = true;

        if (fireZonePrefab != null)
        {
            GameObject zone = Instantiate(fireZonePrefab, transform.position, Quaternion.identity);

            FireZone zoneScript = zone.GetComponent<FireZone>();
            if (zoneScript != null) zoneScript.Initialize(caster);
        }

        Destroy(gameObject);
    }
}
