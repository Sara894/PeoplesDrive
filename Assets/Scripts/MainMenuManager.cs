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
    [SerializeField] Button toggleMusicButton;
    [SerializeField] Button yesExitGame;
    [SerializeField] Button noExitGame;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitCreditsButton;

    [Header("UI Panels")]
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject exitGameUI;
    [SerializeField] GameObject creditsCanvas;
    [SerializeField] GameObject thankYouImage;

    [Header("Safety Screen")]
    [SerializeField] GameObject safetyCanvas;
    [SerializeField] float safetyScreenDuration = 3f;

    [Header("Input Actions for UI")]
    public InputActionReference cancelAction;

    private bool isMusicOn = true;

    private void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;

        startGameButton.onClick.AddListener(LoadSinglePlayer);
        exitGameButton.onClick.AddListener(OpenExitGameCanvas);
        yesExitGame.onClick.AddListener(ShowThankYouAndExit);
        noExitGame.onClick.AddListener(OpenMainMenu);
        toggleMusicButton.onClick.AddListener(ToggleMusic);
        creditsButton.onClick.AddListener(OpenCreditsCanvas);
        exitCreditsButton.onClick.AddListener(CloseCreditsCanvas);
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {

        if (exitGameUI.activeSelf)
        {
            OpenMainMenu();
            return;
        }

        if (creditsCanvas != null && creditsCanvas.activeSelf)
        {
            CloseCreditsCanvas();
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
        mainMenuUI.SetActive(false);
        exitGameUI.SetActive(false);
        creditsCanvas?.SetActive(false);

        if (safetyCanvas != null)
        {
            safetyCanvas.SetActive(true);
            yield return new WaitForSeconds(safetyScreenDuration);
            safetyCanvas.SetActive(false);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync("Cyber_Truck");
        operation.allowSceneActivation = true;

        yield return new WaitUntil(() => operation.isDone);
    }

    public void OpenExitGameCanvas()
    {
        mainMenuUI.SetActive(false);
        exitGameUI.SetActive(true);
        creditsCanvas?.SetActive(false);
    }

    public void OpenMainMenu()
    {
        exitGameUI.SetActive(false);
        mainMenuUI.SetActive(true);
        creditsCanvas?.SetActive(false);
        thankYouImage?.SetActive(false);
    }

    public void OpenCreditsCanvas()
    {
        mainMenuUI.SetActive(false);
        creditsCanvas?.SetActive(true);
    }

    public void CloseCreditsCanvas()
    {
        creditsCanvas?.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    private void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        // AudioListener.pause = !isMusicOn;
        Debug.Log("Music is now " + (isMusicOn ? "ON" : "OFF"));
    }

    public void ShowThankYouAndExit()
    {
        exitGameUI.SetActive(false);
        thankYouImage?.SetActive(true);
        Debug.Log("Thank you for playing!");

        Application.Quit();
        Debug.Log("Game Closed");
    }
}
