using System;
using UnityEngine;

public partial class PlayerController
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public event Action<int, int> HealthChanged;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;
    public float HealthRatio => maxHealth <= 0 ? 0f : (float)currentHealth / maxHealth;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        SetCurrentHealth(currentHealth - amount);
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        SetCurrentHealth(currentHealth + amount);
    }

    public void RestoreHealth()
    {
        SetCurrentHealth(maxHealth);
    }

    private void SetCurrentHealth(int value)
    {
        int clampedValue = Mathf.Clamp(value, 0, maxHealth);

        if (currentHealth == clampedValue)
        {
            return;
        }

        currentHealth = clampedValue;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
