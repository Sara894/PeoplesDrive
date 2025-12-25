using System.Collections;
using UnityEngine;

public class DelayedMeshReveal : MonoBehaviour
{
    [Header("Character to Reveal")]
    [Tooltip("The character GameObject to activate after a delay")]
    [SerializeField] private GameObject characterToReveal;

    [Header("Smoke Effect")]
    [Tooltip("Optional: Smoke effect GameObject to activate when character spawns")]
    [SerializeField] private GameObject smokeEffect;

    [Header("Timing Settings")]
    [Tooltip("Delay in seconds before the character appears")]
    [SerializeField] private float revealDelay = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private void OnEnable()
    {
        if (characterToReveal == null)
        {
            Debug.LogWarning($"DelayedMeshReveal on '{gameObject.name}': No character assigned to reveal!");
            return;
        }

        characterToReveal.SetActive(false);

        if (smokeEffect != null)
        {
            smokeEffect.SetActive(true);

            if (debugMode)
            {
                Debug.Log($"DelayedMeshReveal: Activated smoke effect '{smokeEffect.name}'");
            }
        }

        StartCoroutine(RevealCharacterAfterDelay());
    }

    private void OnDisable()
    {
        if (smokeEffect != null)
        {
            smokeEffect.SetActive(false);

            if (debugMode)
            {
                Debug.Log($"DelayedMeshReveal: Deactivated smoke effect '{smokeEffect.name}'");
            }
        }
    }

    private IEnumerator RevealCharacterAfterDelay()
    {
        if (debugMode)
        {
            Debug.Log($"DelayedMeshReveal: Character hidden, revealing in {revealDelay}s");
        }

        yield return new WaitForSeconds(revealDelay);

        if (characterToReveal != null)
        {
            characterToReveal.SetActive(true);

            if (debugMode)
            {
                Debug.Log($"DelayedMeshReveal: Revealed character '{characterToReveal.name}'");
            }
        }
    }

    public void SetRevealDelay(float delay)
    {
        revealDelay = delay;
    }
}
