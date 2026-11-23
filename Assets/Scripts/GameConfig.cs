using UnityEngine;

public class GameConfig : MonoBehaviour
{
    private static GameConfig instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}