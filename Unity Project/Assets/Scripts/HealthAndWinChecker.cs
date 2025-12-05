using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthChecker : MonoBehaviour
{
    void Update()
    {
        if (GameData.playerHealth <= 0)
        {
            SceneManager.LoadScene("LossScene");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (GameData.numEnemies <= 0)
        {
            SceneManager.LoadScene("WinScene");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameData.numEnemies = 15;
        }
    }
}