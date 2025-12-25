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
    [SerializeField] private InputActionReference navigateAction;
    [SerializeField] private InputActionReference submitAction;

    private Button[] buttons;
    private int currentIndex = 0;
    private bool isPaused;

    private void Awake()
    {
        buttons = new Button[]
        {
            resumeButton,
            mainMenuButton
        };
    }

    private void OnEnable()
    {
        pauseAction.action.Enable();
        navigateAction.action.Enable();
        submitAction.action.Enable();

        pauseAction.action.performed += OnPause;
        navigateAction.action.performed += OnNavigate;
        submitAction.action.performed += OnSubmit;

        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(BackToMainMenu);
        pauseButton.onClick.AddListener(PauseGame);
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPause;
        navigateAction.action.performed -= OnNavigate;
        submitAction.action.performed -= OnSubmit;

        pauseAction.action.Disable();
        navigateAction.action.Disable();
        submitAction.action.Disable();

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

        currentIndex = 0;
        SelectButton(currentIndex);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        pauseCanvas.SetActive(false);
        isPaused = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!isPaused)
            return;

        float y = ctx.ReadValue<Vector2>().y;

        if (Mathf.Abs(y) < 0.5f)
            return;

        if (y < 0)
            currentIndex++;
        else
            currentIndex--;

        currentIndex = Mathf.Clamp(currentIndex, 0, buttons.Length - 1);

        SelectButton(currentIndex);
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!isPaused)
            return;

        buttons[currentIndex].onClick.Invoke();
    }

    private void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
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
