using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
    [SerializeField] private  GameObject pauseOverlay;
    [SerializeField] private Light2D globalLight;
    private GameState stateBeforePause;
    
    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (GameManager.Instance.currentState != GameState.gamePaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    public void Pause()
    {
        globalLight.intensity = 0.2f;
        pauseOverlay.SetActive(true);
        stateBeforePause = GameManager.Instance.currentState;
        GameManager.Instance.ChangeState(GameState.gamePaused);
    }
    public void Unpause()
    {
        globalLight.intensity = 0.5f;
        pauseOverlay.SetActive(false);
        GameManager.Instance.ChangeState(stateBeforePause);
    }
    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
