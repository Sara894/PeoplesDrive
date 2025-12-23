using System.Collections;
using UnityEngine;

public class DelayedMeshReveal : MonoBehaviour
{
    [Header("Smoke Effect")]
    [Tooltip("Optional: Smoke effect GameObject to activate when this character spawns")]
    [SerializeField] private GameObject smokeEffect;

    [Header("Timing Settings")]
    [Tooltip("Delay in seconds before the mesh appears")]
    [SerializeField] private float revealDelay = 0.5f;

    [Header("Mesh Settings")]
    [Tooltip("If true, finds all child MeshRenderers. If false, only uses MeshRenderers on this GameObject.")]
    [SerializeField] private bool includeChildren = true;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private MeshRenderer[] meshRenderers;

    private void Awake()
    {
        if (includeChildren)
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        }
        else
        {
            meshRenderers = GetComponents<MeshRenderer>();
        }

        if (meshRenderers.Length == 0)
        {
            Debug.LogWarning($"DelayedMeshReveal on '{gameObject.name}': No MeshRenderers found!");
        }
    }

    private void OnEnable()
    {
        if (smokeEffect != null)
        {
            smokeEffect.SetActive(true);

            if (debugMode)
            {
                Debug.Log($"DelayedMeshReveal: Activated smoke effect '{smokeEffect.name}'");
            }
        }
        
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        StartCoroutine(RevealMeshAfterDelay());
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

    private IEnumerator RevealMeshAfterDelay()
    {
        if (debugMode)
        {
            Debug.Log($"DelayedMeshReveal: Hiding {meshRenderers.Length} mesh(es), revealing in {revealDelay}s");
        }

        yield return new WaitForSeconds(revealDelay);

        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = true;
            }
        }

        if (debugMode)
        {
            Debug.Log($"DelayedMeshReveal: Revealed {meshRenderers.Length} mesh(es)");
        }
    }

    public void SetRevealDelay(float delay)
    {
        revealDelay = delay;
    }
}
