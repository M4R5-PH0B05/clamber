using UnityEngine;

public class MainMenuManagerScript : MonoBehaviour
{
    private GameObject m_levelSelectPanel;
    private GameObject m_mainMenuPanel;
    private void Awake()
    {
        m_levelSelectPanel = GameObject.Find("LevelSelectPanel");
        m_mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (m_levelSelectPanel != null)
        {
            m_levelSelectPanel.SetActive(false);
        }
    }

    public void OpenLevelSelectMenu()
    {
        m_mainMenuPanel.SetActive(false);
        m_levelSelectPanel.SetActive(true);
    }

    public void CloseLevelSelectMenu()
    {
        m_mainMenuPanel.SetActive(true);
        m_levelSelectPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Will close application on build");
        Application.Quit();
    }

}
