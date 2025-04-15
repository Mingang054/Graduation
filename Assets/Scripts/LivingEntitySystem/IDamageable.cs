
using UnityEngine;

public interface IDamageable
{
    public void OnHitDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal, Faction projectileFaction);
}