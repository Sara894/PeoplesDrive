using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHealth : MonoBehaviour
{
    [Header("Health Configuration")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int startingHealth = 100;

    [Header("Collision Damage")]
    [SerializeField] private float minVelocityForDamage = 5f;
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private float collisionCooldown = 0.5f;

    public int currentHealth { get; private set; }
    public bool isHealthChallengeActive { get; private set; }

    private float lastCollisionTime;
    private Rigidbody rb;

    private void Awake()
    {
        currentHealth = startingHealth;
        rb = GetComponent<Rigidbody>();
        isHealthChallengeActive = false;
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    public void EnableHealthChallenge()
    {
        isHealthChallengeActive = true;
        Debug.Log("Vehicle Health Challenge Active!");
    }

    public void DisableHealthChallenge()
    {
        isHealthChallengeActive = false;
        Debug.Log("Vehicle Health Challenge Disabled");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHealthChallengeActive)
            return;

        if (Time.time - lastCollisionTime < collisionCooldown)
            return;

        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity >= minVelocityForDamage)
        {
            int damage = Mathf.RoundToInt((impactVelocity - minVelocityForDamage) * damageMultiplier);
            TakeDamage(damage);
            lastCollisionTime = Time.time;
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthUI();

        Debug.Log($"Vehicle took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            VehicleDestroyed();
        }
    }

    public void RepairVehicle(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthUI();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        Debug.Log($"Vehicle Health: {currentHealth}/{maxHealth}");
    }

    private void VehicleDestroyed()
    {
        Debug.LogWarning("Vehicle destroyed! Quest failed.");
        isHealthChallengeActive = false;
    }
}
