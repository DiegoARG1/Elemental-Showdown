using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    public static ModuleManager Instance;
    public bool isVibrationEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//Evita que se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Vibrate()
    {
        if (isVibrationEnabled)
        {
            Handheld.Vibrate();
        }
    }

    public void SetVibration(bool isEnabled)
    {
        isVibrationEnabled = isEnabled;
    }
}
