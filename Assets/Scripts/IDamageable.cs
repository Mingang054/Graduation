
using UnityEngine;

public interface IDamageable
{
    //피격 시 데미지 입기
    void OnDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal);
}