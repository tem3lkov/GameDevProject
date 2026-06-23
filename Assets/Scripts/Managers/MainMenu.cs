using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newRunButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        continueButton.interactable = SaveManager.Instance.SaveExists();
    }

    public void NewGame()
    {
        GameManager.Instance.PendingStartMode = StartMode.NewGame;
        SceneManager.LoadScene("SampleScene");
    }

    public void ContinueGame()
    {
        GameManager.Instance.PendingStartMode = StartMode.Continue;
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
