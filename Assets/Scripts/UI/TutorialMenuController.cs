using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TutorialMenuController : MonoBehaviour
{
    [Header("Slide Management")]
    [SerializeField] private List<GameObject> slides = new();

    [Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button exitButton;

    [Header("Input (UI Action Map)")]
    [SerializeField] private InputActionReference navigateAction; // UI/Navigate
    [SerializeField] private InputActionReference cancelAction;   // UI/Cancel

    [Header("Settings")]
    [SerializeField] private bool loopSlides = true;
    [SerializeField] private bool debugMode = false;

    private int currentSlideIndex;
    private float lastInputTime;
    private const float inputCooldown = 0.25f;

    private void Awake()
    {
        nextButton?.onClick.AddListener(OnNext);
        backButton?.onClick.AddListener(OnBack);
        exitButton?.onClick.AddListener(OnExit);

        ValidateSlides();
    }

    private void OnEnable()
    {
        navigateAction.action.Enable();
        cancelAction.action.Enable();

        navigateAction.action.performed += OnNavigate;
        cancelAction.action.performed += OnCancel;

        ShowSlide(currentSlideIndex);
        UpdateButtonStates();

        GameEventsManager.instance?.inputEvents
            .ChangeInputEventContext(InputEventContext.TUTORIAL);
        GameEventsManager.instance?.playerEvents.DisablePlayerMovement();

        if (debugMode)
            Debug.Log("Tutorial opened");
    }

    private void OnDisable()
    {
        navigateAction.action.performed -= OnNavigate;
        cancelAction.action.performed -= OnCancel;

        navigateAction.action.Disable();
        cancelAction.action.Disable();

        GameEventsManager.instance?.inputEvents
            .ChangeInputEventContext(InputEventContext.DEFAULT);
        GameEventsManager.instance?.playerEvents.EnablePlayerMovement();

        if (debugMode)
            Debug.Log("Tutorial closed");
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (Time.unscaledTime - lastInputTime < inputCooldown)
            return;

        float x = ctx.ReadValue<Vector2>().x;

        if (x > 0.5f)
            OnNext();
        else if (x < -0.5f)
            OnBack();

        lastInputTime = Time.unscaledTime;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        OnExit();
    }

    private void OnNext()
    {
        if (slides.Count == 0) return;

        currentSlideIndex++;
        if (currentSlideIndex >= slides.Count)
            currentSlideIndex = loopSlides ? 0 : slides.Count - 1;

        ShowSlide(currentSlideIndex);
        UpdateButtonStates();
    }

    private void OnBack()
    {
        if (slides.Count == 0) return;

        currentSlideIndex--;
        if (currentSlideIndex < 0)
            currentSlideIndex = loopSlides ? slides.Count - 1 : 0;

        ShowSlide(currentSlideIndex);
        UpdateButtonStates();
    }

    private void OnExit()
    {
        gameObject.SetActive(false);
    }


    private void ShowSlide(int index)
    {
        for (int i = 0; i < slides.Count; i++)
            slides[i]?.SetActive(i == index);

        if (debugMode)
            Debug.Log($"Slide {index + 1}/{slides.Count}");
    }

    private void UpdateButtonStates()
    {
        if (slides.Count <= 1)
        {
            nextButton.interactable = false;
            backButton.interactable = false;
            return;
        }

        if (!loopSlides)
        {
            backButton.interactable = currentSlideIndex > 0;
            nextButton.interactable = currentSlideIndex < slides.Count - 1;
        }
        else
        {
            backButton.interactable = true;
            nextButton.interactable = true;
        }
    }

    private void ValidateSlides()
    {
        slides.RemoveAll(s => s == null);

        if (slides.Count == 0)
            Debug.LogError("Tutorial has no slides!");
    }

    public void OpenTutorial()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
