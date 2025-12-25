using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Input Keys")]
    [SerializeField] private KeyCode submitKey = KeyCode.E;
    [SerializeField] private KeyCode questLogKey = KeyCode.Q;
    [SerializeField] private KeyCode tutorialToggleKey = KeyCode.T;

    private void Update()
    {
        HandleMovementInput();
        HandleSubmitInput();
        HandleQuestLogInput();
        HandleTutorialToggleInput();
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 moveDir = new Vector2(horizontal, vertical);

        if (moveDir != Vector2.zero)
        {
            GameEventsManager.instance.inputEvents.MovePressed(moveDir);
        }
    }

    private void HandleSubmitInput()
    {
        if (Input.GetKeyDown(submitKey) || Input.GetKeyDown(KeyCode.Return))
        {
            GameEventsManager.instance.inputEvents.SubmitPressed();
        }
    }

    private void HandleQuestLogInput()
    {
        if (Input.GetKeyDown(questLogKey))
        {
            GameEventsManager.instance.inputEvents.QuestLogTogglePressed();
        }
    }

    private void HandleTutorialToggleInput()
    {
        if (Input.GetKeyDown(tutorialToggleKey))
        {
            Debug.Log($"InputManager: Tutorial toggle key '{tutorialToggleKey}' pressed!");
            GameEventsManager.instance.inputEvents.TutorialTogglePressed();
        }
    }
}
