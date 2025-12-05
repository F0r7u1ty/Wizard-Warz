using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthChecker : MonoBehaviour
{
    void Update()
    {
        if (GameData.playerHealth <= 0)
        {
            SceneManager.LoadScene("LossScene");
        }
    }
}