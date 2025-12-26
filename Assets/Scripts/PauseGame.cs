using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button pauseButton;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference pauseAction;

    private bool isPaused;

    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPause;

        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(BackToMainMenu);
        pauseButton.onClick.AddListener(PauseGame);
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPause;
        pauseAction.action.Disable();

        resumeButton.onClick.RemoveListener(ResumeGame);
        mainMenuButton.onClick.RemoveListener(BackToMainMenu);
        pauseButton.onClick.RemoveListener(PauseGame);
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        pauseCanvas.SetActive(true);
        isPaused = true;

        // Set the first selected button so keyboard/controller navigation works
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        pauseCanvas.SetActive(false);
        isPaused = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}