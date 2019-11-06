using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject GameGUI;

    public bool music;

    void Start()
    {
        if (pausePanel != null && GameGUI != null)
        {
            pausePanel.SetActive(false);
            GameGUI.SetActive(true);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        music = true;
    }

    public void TogglePause()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
        GameGUI.SetActive(!GameGUI.activeSelf);
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = Cursor.lockState == CursorLockMode.None;
    }

    public void Game()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void ToggleMusic()
    {
        music = !music;
    }
}