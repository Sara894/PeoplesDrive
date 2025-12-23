using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenuController : MonoBehaviour
{
    [Header("Slide Management")]
    [SerializeField] private List<GameObject> slides = new List<GameObject>();
    
    [Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button exitButton;

    [Header("Settings")]
    [SerializeField] private bool loopSlides = true;
    [SerializeField] private bool debugMode = false;

    private int currentSlideIndex = 0;

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        ValidateSlides();
    }

    private void OnEnable()
    {
        if (slides.Count > 0)
        {
            if (currentSlideIndex < 0)
            {
                currentSlideIndex = 0;
            }
            
            ShowSlide(currentSlideIndex);
            UpdateButtonStates();

            if (GameEventsManager.instance != null)
            {
                GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.TUTORIAL);
                GameEventsManager.instance.playerEvents.DisablePlayerMovement();
            }

            if (debugMode)
            {
                Debug.Log($"TutorialMenuController: Opened tutorial, showing slide {currentSlideIndex}");
            }
        }
    }

    private void OnDisable()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);
            GameEventsManager.instance.playerEvents.EnablePlayerMovement();
        }

        if (debugMode)
        {
            Debug.Log("TutorialMenuController: Closed tutorial");
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(OnNextButtonClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }

    private void ValidateSlides()
    {
        if (slides.Count == 0)
        {
            Debug.LogWarning("TutorialMenuController: No slides assigned! Please assign slide GameObjects in the Inspector.");
            return;
        }

        slides.RemoveAll(slide => slide == null);

        if (slides.Count == 0)
        {
            Debug.LogError("TutorialMenuController: All slides are null! Cannot display tutorial.");
        }
    }

    private void ShowSlide(int index)
    {
        if (slides.Count == 0)
        {
            return;
        }

        for (int i = 0; i < slides.Count; i++)
        {
            if (slides[i] != null)
            {
                slides[i].SetActive(i == index);
            }
        }

        if (debugMode)
        {
            Debug.Log($"TutorialMenuController: Showing slide {index + 1}/{slides.Count}");
        }
    }

    private void UpdateButtonStates()
    {
        if (slides.Count <= 1)
        {
            if (nextButton != null) nextButton.interactable = false;
            if (backButton != null) backButton.interactable = false;
            return;
        }

        if (!loopSlides)
        {
            if (backButton != null)
            {
                backButton.interactable = currentSlideIndex > 0;
            }

            if (nextButton != null)
            {
                nextButton.interactable = currentSlideIndex < slides.Count - 1;
            }
        }
        else
        {
            if (backButton != null) backButton.interactable = true;
            if (nextButton != null) nextButton.interactable = true;
        }
    }

    private void OnNextButtonClicked()
    {
        if (slides.Count == 0)
        {
            return;
        }

        currentSlideIndex++;

        if (currentSlideIndex >= slides.Count)
        {
            if (loopSlides)
            {
                currentSlideIndex = 0;
            }
            else
            {
                currentSlideIndex = slides.Count - 1;
            }
        }

        ShowSlide(currentSlideIndex);
        UpdateButtonStates();
    }

    private void OnBackButtonClicked()
    {
        if (slides.Count == 0)
        {
            return;
        }

        currentSlideIndex--;

        if (currentSlideIndex < 0)
        {
            if (loopSlides)
            {
                currentSlideIndex = slides.Count - 1;
            }
            else
            {
                currentSlideIndex = 0;
            }
        }

        ShowSlide(currentSlideIndex);
        UpdateButtonStates();
    }

    private void OnExitButtonClicked()
    {
        gameObject.SetActive(false);

        if (debugMode)
        {
            Debug.Log("TutorialMenuController: Exit button clicked, closing tutorial");
        }
    }

    public void OpenTutorial()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    public void GoToSlide(int index)
    {
        if (index < 0 || index >= slides.Count)
        {
            Debug.LogWarning($"TutorialMenuController: Slide index {index} out of range (0-{slides.Count - 1})");
            return;
        }

        currentSlideIndex = index;
        ShowSlide(currentSlideIndex);
        UpdateButtonStates();
    }
}
