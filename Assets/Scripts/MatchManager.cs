using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Puntos de aparicion")]
    public Transform p1SpawnPoint;
    public Transform p2SpawnPoint;

    [Header("Personajes en combate")]
    public GameObject player1;
    public GameObject player2;

    [Header("Pantallas finales (UI)")]
    public GameObject victoryScreen;
    public GameObject gameOverScreen;

    private HealthSystem p1Health;
    private HealthSystem p2Health;

    private void Start()
    {
        if (player1 != null && player2 != null)
        {
            SetupMatch(player1, player2);
        }
    }

    public void SetupMatch(GameObject p1, GameObject p2)
    {
        player1 = p1;
        player2 = p2;

        p1Health = player1.GetComponent<HealthSystem>();
        p2Health = player2.GetComponent<HealthSystem>();

        if (p1Health != null)
        {
            p1Health.OnLivesChanged += ResetRound;
            p1Health.OnDeath += TriggerGameOver;
        }
        if (p2Health != null)
        {
            p2Health.OnLivesChanged += ResetRound;
            p2Health.OnDeath += TriggerVictory;
        }

        ResetPositions();
    }

    private void OnDisable()
    {
        if (p1Health != null)
        {
            p1Health.OnLivesChanged -= ResetRound;
            p1Health.OnDeath -= TriggerGameOver;
        }
        if (p2Health != null)
        {
            p2Health.OnLivesChanged -= ResetRound;
            p2Health.OnDeath -= TriggerVictory;
        }
    }

    private void ResetRound(int currentLives)
    {
        if (currentLives <= 0) return;

        if (p1Health != null) p1Health.ResetHealth();
        if (p2Health != null) p2Health.ResetHealth();

        ResetPositions();
        CleanArena();
    }

    private void TriggerVictory()
    {
        if (victoryScreen != null) victoryScreen.SetActive(true);
        StopCombat();
    }

    private void TriggerGameOver()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        StopCombat();
    }

    private void StopCombat()
    {
        if (player1 != null)
        {
            var p1Controller = player1.GetComponent<PlayerController>();
            if (p1Controller != null) p1Controller.enabled = false;
        }

        if (player2 != null)
        {
            var p2AI = player2.GetComponent<EnemyAIController>();
            if (p2AI != null) p2AI.enabled = false;

            var p2Controller = player2.GetComponent<PlayerController>();
            if (p2Controller != null) p2Controller.enabled = false;
        }
    }

    private void ResetPositions()
    {
        if (player1 != null && p1SpawnPoint != null) player1.transform.position = p1SpawnPoint.position;
        if (player2 != null && p2SpawnPoint != null) player2.transform.position = p2SpawnPoint.position;
    }

    private void CleanArena()
    {
        foreach (var fireball in FindObjectsByType<FireballProjectile>()) Destroy(fireball.gameObject);
        foreach (var fireZone in FindObjectsByType<FireZone>()) Destroy(fireZone.gameObject);
        foreach (var bomb in FindObjectsByType<WaterBombProjectile>()) Destroy(bomb.gameObject);
        foreach (var waterZone in FindObjectsByType<WaterZone>()) Destroy(waterZone.gameObject);
    }
}