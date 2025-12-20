using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class EventSystemInputFixer : MonoBehaviour
{
    private void Awake()
    {
        EventSystem eventSystem = GetComponent<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("EventSystemInputFixer: No EventSystem component found!");
            return;
        }

        StandaloneInputModule oldInputModule = GetComponent<StandaloneInputModule>();
        if (oldInputModule != null)
        {
            Debug.Log("EventSystemInputFixer: Removing old StandaloneInputModule and adding InputSystemUIInputModule");
            Destroy(oldInputModule);

            InputSystemUIInputModule newInputModule = gameObject.AddComponent<InputSystemUIInputModule>();
            Debug.Log("EventSystemInputFixer: Successfully switched to new Input System UI module!");
        }
        else
        {
            InputSystemUIInputModule existingModule = GetComponent<InputSystemUIInputModule>();
            if (existingModule == null)
            {
                gameObject.AddComponent<InputSystemUIInputModule>();
                Debug.Log("EventSystemInputFixer: Added InputSystemUIInputModule");
            }
            else
            {
                Debug.Log("EventSystemInputFixer: InputSystemUIInputModule already present");
            }
        }
    }
}
