using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public HealthSystem playerHealth;
    public Transform dummy;

    private Vector3 playerStartPos;
    private Vector3 dummyStartPos;

    private void Awake()
    {
        if (player != null) playerStartPos = player.position;
        if (dummy != null) dummyStartPos = dummy.position;
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnLivesChanged += ResetRound;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnLivesChanged -= ResetRound;
        }
    }

    private void ResetRound(int currentLives)
    {
        if (currentLives <= 0) return;

        if (player != null) player.position = playerStartPos;
        if (dummy != null)
        {
            dummy.position = dummyStartPos;
            DummyEnemy dummyScript = dummy.GetComponent<DummyEnemy>();
            if (dummyScript != null) dummyScript.health = 100f;
        }

        foreach (var fireball in FindObjectsByType<FireballProjectile>())
        {
            Destroy(fireball.gameObject);
        }

        foreach (var zone in FindObjectsByType<FireZone>())
        {
            Destroy(zone.gameObject);
        }
    }
}