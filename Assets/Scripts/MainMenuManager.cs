using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startGameButton;
    [SerializeField] Button exitGameButton;
    [SerializeField] Button yesExitGame;
    [SerializeField] Button noExitGame;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitCreditsButton;
    [SerializeField] Button resetProgressButton;

    [Header("UI Panels")]
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject exitGameUI;
    [SerializeField] GameObject creditsCanvas;
    [SerializeField] GameObject thankYouImage;

    [Header("Input Actions for UI")]
    [SerializeField] InputActionReference cancelAction;
    [SerializeField] InputActionReference navigateAction;
    [SerializeField] InputActionReference submitAction;

    private List<Button> mainMenuButtons;
    private List<Button> exitGameButtons;
    private int selectedButtonIndex = 0;
    private float navigationCooldown = 0.15f;
    private float lastNavigationTime = 0f;

    private float inputLockUntil = 0f;
    private const float inputLockDuration = 0.2f;

    private void Awake()
    {
        mainMenuButtons = new List<Button> { startGameButton, resetProgressButton, creditsButton, exitGameButton };
        exitGameButtons = new List<Button> { yesExitGame, noExitGame };
    }

    private void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;

        navigateAction.action.Enable();
        submitAction.action.Enable();

        startGameButton.onClick.AddListener(LoadSinglePlayer);
        exitGameButton.onClick.AddListener(OpenExitGameCanvas);
        yesExitGame.onClick.AddListener(ShowThankYouAndExit);
        noExitGame.onClick.AddListener(OpenMainMenu);
        creditsButton.onClick.AddListener(OpenCreditsCanvas);
        exitCreditsButton.onClick.AddListener(CloseCreditsCanvas);

        if (resetProgressButton != null)
            resetProgressButton.onClick.AddListener(ResetProgress);
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();

        navigateAction.action.Disable();
        submitAction.action.Disable();

        if (resetProgressButton != null)
            resetProgressButton.onClick.RemoveListener(ResetProgress);
    }

    private void Update()
    {
        if (mainMenuUI.activeSelf)
        {
            HandleMenuNavigation(mainMenuButtons);
        }
        else if (exitGameUI.activeSelf)
        {
            HandleMenuNavigation(exitGameButtons);
        }
        else if (creditsCanvas != null && creditsCanvas.activeSelf)
        {
            HandleCreditsSubmit();
        }
    }

    private void HandleMenuNavigation(List<Button> buttons)
    {
        if (buttons == null || buttons.Count == 0)
            return;

        Vector2 nav = navigateAction.action.ReadValue<Vector2>();
        bool moved = false;

        if (Time.unscaledTime - lastNavigationTime > navigationCooldown)
        {
            if (nav.y > 0.5f)
            {
                selectedButtonIndex = (selectedButtonIndex - 1 + buttons.Count) % buttons.Count;
                moved = true;
            }
            else if (nav.y < -0.5f)
            {
                selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count;
                moved = true;
            }

            if (moved)
            {
                lastNavigationTime = Time.unscaledTime;
                buttons[selectedButtonIndex].Select();
            }
        }

        if (Time.unscaledTime > inputLockUntil && submitAction.action.WasPressedThisFrame())
        {
            buttons[selectedButtonIndex].onClick.Invoke();
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            buttons[selectedButtonIndex].Select();
        }
    }

    private void HandleCreditsSubmit()
    {
        if (Time.unscaledTime > inputLockUntil && submitAction.action.WasPressedThisFrame())
        {
            exitCreditsButton.onClick.Invoke();
        }

        if (EventSystem.current.currentSelectedGameObject != exitCreditsButton.gameObject)
        {
            exitCreditsButton.Select();
        }
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
        SceneManager.LoadScene("Cyber_Truck");
    }

    public void OpenExitGameCanvas()
    {
        mainMenuUI.SetActive(false);
        exitGameUI.SetActive(true);
        creditsCanvas?.SetActive(false);
        selectedButtonIndex = 0;
        if (exitGameButtons.Count > 0 && exitGameButtons[0] != null)
            exitGameButtons[0].Select();
        inputLockUntil = Time.unscaledTime + inputLockDuration;
    }

    public void OpenMainMenu()
    {
        exitGameUI.SetActive(false);
        mainMenuUI.SetActive(true);
        creditsCanvas?.SetActive(false);
        thankYouImage?.SetActive(false);
        selectedButtonIndex = 0;
        mainMenuButtons[selectedButtonIndex].Select();
        inputLockUntil = Time.unscaledTime + inputLockDuration;
    }

    public void OpenCreditsCanvas()
    {
        mainMenuUI.SetActive(false);
        creditsCanvas?.SetActive(true);
        selectedButtonIndex = 0;
        if (exitCreditsButton != null)
            exitCreditsButton.Select();
        inputLockUntil = Time.unscaledTime + inputLockDuration;
    }

    public void CloseCreditsCanvas()
    {
        creditsCanvas?.SetActive(false);
        mainMenuUI.SetActive(true);
        selectedButtonIndex = 0;
        mainMenuButtons[selectedButtonIndex].Select();
        inputLockUntil = Time.unscaledTime + inputLockDuration;
    }

    public void ShowThankYouAndExit()
    {
        exitGameUI.SetActive(false);
        thankYouImage?.SetActive(true);

        Debug.Log("Thank you for playing!");
        Application.Quit();
    }

    public void ResetProgress()
    {
        SaveManager.ResetAllProgress();
    }
}
