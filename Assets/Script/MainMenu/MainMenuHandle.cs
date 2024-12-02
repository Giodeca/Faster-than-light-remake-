using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandle : MonoBehaviour
{
    [SerializeField] private GameObject panelSettings;

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }


    public void OpenPanel()
    {
        if (!panelSettings.activeSelf)
        {
            panelSettings.SetActive(true);
        }
        else { panelSettings.SetActive(false); }
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
