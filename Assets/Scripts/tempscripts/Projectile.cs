using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed;
    private float damage;
    private float penetration;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetForEnable(float speed, float damage, float penetration, float colliderSize)
    {
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
            ProjectilePoolManager.Instance.ReturnProjectile(gameObject);
        }
    }
}
