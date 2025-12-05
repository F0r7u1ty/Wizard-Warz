using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class StartController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI messageText;   // Assign in Inspector

    // Called when clicking the Start button
    public void StartGame()
    {
        GameData.playerHealth = GameData.MAX_PLAYER_HEALTH;
        GameData.playerMana = GameData.MAX_PLAYER_MANA;
        SceneManager.LoadScene("gameplay");
        GameData.numEnemies = 15;
    }

    // Called when clicking the Quit button
    public void QuitGame()
    {
        // For now: show message instead of quitting
        StartCoroutine(ShowTempMessage());

        // Actual quit code outside of gameday:
        // Application.Quit();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("StartScene");
        GameData.numEnemies = 15;
    }

    private IEnumerator ShowTempMessage()
    {
        if (messageText != null)
        {
            messageText.text = "Nice try, but no leaving allowed!";
            yield return new WaitForSeconds(1.5f);
            messageText.text = "";
        }
    }
}