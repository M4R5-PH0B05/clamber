using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManagerScript : MonoBehaviour
{
    private GameObject m_pauseMenu;
    private bool m_isPaused;
    private GameObject m_CustomSceneManager;
    private GameObject m_settingsPanel;
    private GameObject m_PauseMenuPanel;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        m_pauseMenu = GameObject.Find("PauseMenu");
        m_settingsPanel = GameObject.Find("SettingsPanel");
        if (m_settingsPanel != null)
        {
            m_settingsPanel.SetActive(false);
        }
        if (m_pauseMenu != null)
        {
            m_PauseMenuPanel = GameObject.Find("PauseMenuPanel");
            DontDestroyOnLoad(m_pauseMenu);
            m_pauseMenu.SetActive(false);
        }
        m_CustomSceneManager = GameObject.Find("CustomSceneManager");
    }

    /// <summary>
    /// pauses or resumes game appropriately
    /// </summary>
    public void HandlePauseInput()
    {
        if (m_isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    /// <summary>
    /// Resume Time + close Pause Menu
    /// </summary>
    public void Resume()//resumes game playing and stops pause menu showing
    {
        m_isPaused = false;
        Time.timeScale = 1f;
        m_pauseMenu.SetActive(false);
    }

    /// <summary>
    /// Pause time + open Pause menu
    /// </summary>
    public void Pause()//pauses game and loads pause menu
    {
        m_isPaused = true;
        Time.timeScale = 0f;
        m_pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Opens settings Panel
    /// </summary>
    public void OpenSettings()
    {
        m_PauseMenuPanel.SetActive(false);
        m_settingsPanel.SetActive(true);
    }

    /// <summary>
    /// closes settings panel + goes back into main pause menu
    /// </summary>
    public void CloseSettings()
    {
        m_PauseMenuPanel.SetActive(true);
        m_settingsPanel.SetActive(false);
    }

    /// <summary>
    /// loads level select Screen
    /// </summary>
    public void QuitToMenu()
    {
        m_CustomSceneManager.GetComponent<CustomSceneManagerScript>().StartSwapSceneCoroutine("Level Select" ,new Vector3(0,0,0));
        Resume();
    }

    /// <summary>
    /// Closes application
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("will exit when project is built ");
        Application.Quit();
    }

}
