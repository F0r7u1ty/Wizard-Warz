using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("StartScene"); // change name as needed
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
