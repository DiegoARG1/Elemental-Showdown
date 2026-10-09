using System.Collections;
using UnityEngine;

public class DummyAutoShooter : MonoBehaviour
{
    [Header("Configuracion")]
    public GameObject fireballProjectilePrefab;
    public Transform shootPoint;

    [Tooltip("Tiempo en segundos entre cada disparo")]
    public float fireRate = 10f;

    [Tooltip("Que tan lejos viajara la bola antes de explotar")]
    public float shootDistance = 15f;

    private void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            if (fireballProjectilePrefab != null)
            {
                Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
                Vector3 destination = spawnPos + (transform.right * shootDistance);

                GameObject fireball = Instantiate(fireballProjectilePrefab, spawnPos, Quaternion.identity);

                FireballProjectile script = fireball.GetComponent<FireballProjectile>();
                if (script != null)
                {
                    script.Initialize(gameObject, destination);
                }
            }
        }
    }
}