using UnityEngine;
using System;

public class InputEvents
{
    
    public InputEventContext inputEventContext { get; private set; } = InputEventContext.DEFAULT;

    public void ChangeInputEventContext(InputEventContext newContext) 
    {
        Debug.Log($"<color=lime>InputEvents: Changing context from {this.inputEventContext} to {newContext}</color>");
        this.inputEventContext = newContext;
    }

    public event Action<Vector2> onMovePressed;
    public void MovePressed(Vector2 moveDir) 
    {
        if (onMovePressed != null) 
        {
            onMovePressed(moveDir);
        }
    }

    public event Action<InputEventContext> onSubmitPressed;
    public void SubmitPressed()
    {
        if (onSubmitPressed != null) 
        {
            onSubmitPressed(this.inputEventContext);
        }
    }

    public event Action onQuestLogTogglePressed;
    public void QuestLogTogglePressed()
    {
        if (onQuestLogTogglePressed != null) 
        {
            onQuestLogTogglePressed();
        }
    }

    public event Action onTutorialTogglePressed;
    public void TutorialTogglePressed()
    {
        if (onTutorialTogglePressed != null) 
        {
            onTutorialTogglePressed();
        }
    }
}
