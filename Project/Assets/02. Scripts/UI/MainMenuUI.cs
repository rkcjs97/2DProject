using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionPanel;
    
    [Header("Scene Name")]
    [SerializeField] private string sceneName = "Map"; 
    
    
    // === Button ===

    public void OnClickNewGame()
    {
        Debug.Log("New Game");
        //TODO: Change InGame
    }

    public void OnClickContinue()
    {
        Debug.Log("Continue");
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickOptions()
    {
        Debug.Log("Options");
        mainMenuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }
    
    public void OnClickQuit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void OnClickBack()
    {
        optionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    
}
