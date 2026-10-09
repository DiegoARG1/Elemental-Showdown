using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    [Header("Jugador")]
    public HealthSystem playerHealth;

    [Header("HUD")]
    public GameObject controlsPanel;
    public Slider healthBar;
    public TextMeshProUGUI livesText;

    [Header("Menu de pausa")]
    public GameObject pauseMenuPanel;
    public GameObject pauseMainButtons;

    [Header("Configuracion")]
    public GameObject settingsPanel;
    public GameObject tabGeneralPage;
    public GameObject tabAudioPage;
    public Toggle vibrationToggle;

    [Header("Game over")]
    public GameObject gameOverPanel;

    private void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMainButtons != null) pauseMainButtons.SetActive(true);
        if (controlsPanel != null) controlsPanel.SetActive(true);

        Time.timeScale = 1f;

        if (ModuleManager.Instance != null && vibrationToggle != null)
        {
            vibrationToggle.isOn = ModuleManager.Instance.isVibrationEnabled;
            vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealth;
            playerHealth.OnLivesChanged += UpdateLives;
            playerHealth.OnDeath += HandleGameOver;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealth;
            playerHealth.OnLivesChanged -= UpdateLives;
            playerHealth.OnDeath -= HandleGameOver;
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void UpdateLives(int currentLives)
    {
        if (livesText != null)
        {
            livesText.text = "  " + currentLives;
        }
    }

    public void HandleGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void TogglePauseMenu()
    {
        bool isPaused = pauseMenuPanel.activeSelf;

        pauseMenuPanel.SetActive(!isPaused);

        if (controlsPanel != null) controlsPanel.SetActive(isPaused);

        if (!isPaused)
        {
            if (pauseMainButtons != null) pauseMainButtons.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        Time.timeScale = isPaused ? 1f : 0f;
    }

    public void OpenSettings()
    {
        if (pauseMainButtons != null) pauseMainButtons.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        OpenTabGeneral();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMainButtons != null) pauseMainButtons.SetActive(true);
    }

    public void OpenTabGeneral()
    {
        if (tabGeneralPage != null) tabGeneralPage.SetActive(true);
        if (tabAudioPage != null) tabAudioPage.SetActive(false);
    }

    public void OpenTabAudio()
    {
        if (tabGeneralPage != null) tabGeneralPage.SetActive(false);
        if (tabAudioPage != null) tabAudioPage.SetActive(true);
    }

    public void OnVibrationToggled(bool isEnabled)
    {
        if (ModuleManager.Instance != null)
        {
            ModuleManager.Instance.SetVibration(isEnabled);
        }
    }

    public void AbandonMatch()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}