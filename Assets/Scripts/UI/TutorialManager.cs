using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialMenuController tutorialMenuController;

    private void Start()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.inputEvents.onTutorialTogglePressed += OnTutorialTogglePressed;
            Debug.Log("TutorialManager: Subscribed to tutorial toggle input");
        }
        else
        {
            Debug.LogError("TutorialManager: GameEventsManager.instance is null!");
        }

        if (tutorialMenuController == null)
        {
            Debug.LogError("TutorialManager: TutorialMenuController not assigned!");
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.inputEvents.onTutorialTogglePressed -= OnTutorialTogglePressed;
        }
    }

    private void OnTutorialTogglePressed()
    {
        if (tutorialMenuController == null)
        {
            Debug.LogError("TutorialManager: Cannot toggle tutorial - TutorialMenuController not assigned!");
            return;
        }

        Debug.Log("TutorialManager: Tutorial toggle pressed!");

        InputEventContext currentContext = GameEventsManager.instance.inputEvents.inputEventContext;

        if (currentContext == InputEventContext.DIALOGUE)
        {
            Debug.Log("TutorialManager: Cannot open Tutorial while in dialogue");
            return;
        }

        if (currentContext == InputEventContext.QUEST_LOG)
        {
            Debug.Log("TutorialManager: Cannot open Tutorial while Quest Log is open");
            return;
        }

        if (tutorialMenuController.gameObject.activeInHierarchy)
        {
            Debug.Log("TutorialManager: Closing tutorial");
            tutorialMenuController.CloseTutorial();
        }
        else
        {
            Debug.Log("TutorialManager: Opening tutorial");
            tutorialMenuController.OpenTutorial();
        }
    }
}
