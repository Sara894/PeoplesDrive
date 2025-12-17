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
    [SerializeField] Button settings;
    [SerializeField] Button yesExitGame;
    [SerializeField] Button noExitGame;
    [SerializeField] Button closeButtonSettings;

    [Header("UI Panels")]
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject exitGameUI;
    [SerializeField] GameObject loadingScreenUI;

    [Header("Input Actions for UI")]
    public InputActionReference cancelAction;

    private void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;
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

        if (settingsUI.activeSelf)
        {
            OpenMainMenu();
            return;
        }

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
        settingsUI.SetActive(false);
        exitGameUI.SetActive(false);

        yield return new WaitForSeconds(2f);

        AsyncOperation operation = SceneManager.LoadSceneAsync("ControlsPopUp");
        operation.allowSceneActivation = true;

        yield return new WaitUntil(() => operation.isDone);
    }

    public void OpenExitGameCanvas()
    {
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(false);
        exitGameUI.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsUI.SetActive(true);
        mainMenuUI.SetActive(false);
        exitGameUI.SetActive(false);
    }

    public void OpenMainMenu()
    {
        exitGameUI.SetActive(false);
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
