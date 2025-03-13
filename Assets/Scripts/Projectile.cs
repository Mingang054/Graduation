using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed;
    private float damage;
    private float penetration;
    private Faction faction; // 🔹 피아 식별을 위한 Faction 변수

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetForEnable(Faction faction, float speed, float damage, float penetration, float colliderSize)
    {
        this.faction = faction; // 🔹 탄환의 소속 설정
        this.speed = speed;
        this.damage = damage;
        this.penetration = penetration;

        // 🔹 콜라이더 크기 설정
        Collider2D collider = GetComponent<Collider2D>();
        if (collider is BoxCollider2D boxCollider)
        {
            boxCollider.size = new Vector2(colliderSize, colliderSize);
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            circleCollider.radius = colliderSize / 2f;
        }
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        Debug.Log(faction);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🔹 충돌한 대상이 DamageableEntity인지 확인
        DamageableEntity target = other.GetComponent<DamageableEntity>();

        if (target != null && target.faction != faction) // 🔹 Faction이 다르면 데미지 적용
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position); // 충돌 지점
            Vector2 hitNormal = (transform.position - other.transform.position).normalized; // 충돌 방향

            target.OnHitDamage(damage, penetration, hitPoint, hitNormal);

            gameObject.SetActive(false);
            ProjectilePoolManager.Instance.ReturnProjectile(gameObject);
        }
        else if (other.CompareTag("Wall")) // 🔹 벽(Wall)과 충돌한 경우에만 삭제
        {
            gameObject.SetActive(false);
            ProjectilePoolManager.Instance.ReturnProjectile(gameObject);
        }
    }

}
