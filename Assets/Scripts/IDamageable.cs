
using UnityEngine;

public interface IDamageable
{
    //�ǰ� �� ������ �Ա�
    void OnDamage(float damage, float penetration, Vector2 hitPoint, Vector2 hitNormal);
}