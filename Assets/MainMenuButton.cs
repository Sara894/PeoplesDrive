using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    [SerializeField] InputActionReference submitAction;

    void OnEnable()
    {
        if (submitAction != null)
            submitAction.action.performed += OnSubmit;
        if (submitAction != null)
            submitAction.action.Enable();
    }

    void OnDisable()
    {
        if (submitAction != null)
            submitAction.action.performed -= OnSubmit;
        if (submitAction != null)
            submitAction.action.Disable();
    }

    void Start()
    {
        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (mainMenuButton != null && mainMenuButton.interactable)
        {
            mainMenuButton.onClick.Invoke();
        }
    }
}
