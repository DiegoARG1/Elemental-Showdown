using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
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
