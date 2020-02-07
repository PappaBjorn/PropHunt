using UnityEngine;

public interface IHealth
{
    bool TakeDamage(float amount);
    void Heal(float amount);
}
