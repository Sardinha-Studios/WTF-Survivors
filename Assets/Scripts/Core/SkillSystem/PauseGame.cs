using UnityEngine;
using LL;

public class PauseGame : Singleton<PauseGame>
{
    private bool isPaused = false;

    public void TogglePause()
    {
        // Debug.LogError($"Update pause system and delete this script!");

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            // GameManager.Instance.Pause(PauseMethods.PauseMenu);
        }
        else
        {
            Time.timeScale = 1f;
            // GameManager.Instance.Pause(PauseMethods.NoPauseMenu);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
