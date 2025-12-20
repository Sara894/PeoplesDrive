using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startGameButton;
    [SerializeField] Button exitGameButton;
    [SerializeField] Button toggleMusicButton; // Replacing settings
    [SerializeField] Button yesExitGame;
    [SerializeField] Button noExitGame;

    [Header("UI Panels")]
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject exitGameUI;
    [SerializeField] GameObject loadingScreenUI;

    [Header("Safety Screen")]
    [SerializeField] GameObject safetyCanvas; // assign in inspector
    [SerializeField] float safetyScreenDuration = 3f; // 3-4 seconds

    [Header("Input Actions for UI")]
    public InputActionReference cancelAction;

    private bool isMusicOn = true;

    private void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;

        // Add listeners
        startGameButton.onClick.AddListener(LoadSinglePlayer);
        exitGameButton.onClick.AddListener(OpenExitGameCanvas);
        yesExitGame.onClick.AddListener(ExitGame);
        noExitGame.onClick.AddListener(OpenMainMenu);
        toggleMusicButton.onClick.AddListener(ToggleMusic);
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (loadingScreenUI.activeSelf)
            return;

        if (exitGameUI.activeSelf)
        {
            OpenMainMenu();
            return;
        }

        if (mainMenuUI.activeSelf)
        {
            OpenExitGameCanvas();
            return;
        }
    }

    public void LoadSinglePlayer()
    {
        StartCoroutine(LoadSinglePlayerCoroutine());
    }

    IEnumerator LoadSinglePlayerCoroutine()
    {
        loadingScreenUI.SetActive(true);
        mainMenuUI.SetActive(false);
        exitGameUI.SetActive(false);

        // Show safety screen
        if (safetyCanvas != null)
        {
            safetyCanvas.SetActive(true);
            yield return new WaitForSeconds(safetyScreenDuration);
            safetyCanvas.SetActive(false);
        }

        // Load Cyber_Truck scene
        AsyncOperation operation = SceneManager.LoadSceneAsync("Cyber_Truck");
        operation.allowSceneActivation = true;

        yield return new WaitUntil(() => operation.isDone);
    }


    public void OpenExitGameCanvas()
    {
        mainMenuUI.SetActive(false);
        exitGameUI.SetActive(true);
    }

    public void OpenMainMenu()
    {
        exitGameUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    private void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        // Here you can add your actual music toggle logic, e.g.:
        // AudioListener.pause = !isMusicOn;
        Debug.Log("Music is now " + (isMusicOn ? "ON" : "OFF"));
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}
