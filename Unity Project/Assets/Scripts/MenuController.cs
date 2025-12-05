using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;


    void Start()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameData.menuOpen) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameData.menuOpen = true;
    }

    public void Resume()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked; // adjust to your input setup
        Cursor.visible = false;
        GameData.menuOpen = false;
    } 

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
        // this allows for replayability, if you go to the menu then start the game it should all act the same now.
        GameData.playerHealth = GameData.MAX_PLAYER_HEALTH;
        GameData.playerMana = GameData.MAX_PLAYER_MANA;
        GameData.numEnemies = 15;
    }

    public void QuitGame()
    { //don't want this functionality for demo day tomorrow
        //Application.Quit();
    }
}
