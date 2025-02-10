
using UnityEngine;

public interface IDamageable
{
    void OnDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal);
}