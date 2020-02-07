using UnityEngine;

[System.Serializable]
public class HealthComponent
{
    public float MaxHealth;
    public float Health;
    public float HealthRegain;

    HealthComponent(float maxHealth, float startHealth, float healthRegain = 0f)
    {
        MaxHealth = maxHealth;
        Health = startHealth;
        HealthRegain = healthRegain;
    }

    public bool TakeDamage(float amount)
    {
        Health -= amount;

        Health = Mathf.Clamp(Health, 0, MaxHealth);

        if (Health <= 0)
            return true;

        return false;
    }

    public void AddHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
    }

    public void IncreaseMaxHealth(float amount)
    {
        MaxHealth += amount;
    }

    public void RegainHealthUpdate(float dt)
    {
        if (Health > 0)
        {
            Health += HealthRegain * dt;
            Health = Mathf.Clamp(Health, 0, MaxHealth);
        }
    }

    public void ResetHealth()
    {
        Health = MaxHealth;
    }
}