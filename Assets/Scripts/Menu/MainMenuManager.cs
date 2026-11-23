using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("SelectorPersonaje");
    }

    public void OpenSettings()
    {
        Debug.Log("Abriendo configuración de audio...");
    }

    public void QuitGame()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();//Solo funciona en celular
    }
}
