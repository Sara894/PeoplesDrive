using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    private Image healthFillImage;
    private VehicleHealth vehicleHealth;

    private void Start()
    {
        healthFillImage = GetComponent<Image>();
        
        if (healthFillImage == null)
        {
            Debug.LogError("HealthManager: No Image component found!");
            return;
        }

        vehicleHealth = FindObjectOfType<VehicleHealth>();
        
        if (vehicleHealth == null)
        {
            Debug.LogError("HealthManager: No VehicleHealth found in scene!");
            return;
        }

        UpdateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthFillImage != null && vehicleHealth != null)
        {
            float fillAmount = Mathf.Clamp01((float)vehicleHealth.currentHealth / 100f);
            healthFillImage.fillAmount = fillAmount;
        }
    }
}
