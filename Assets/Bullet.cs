using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const int Damage = 1;
    private const float BulletScale = 0.12f;

    private Vector2 _direction;
    private float _speed;
    private string _ownerTag;

    public static void Spawn(Vector3 position, Quaternion rotation, Vector2 direction, string ownerTag, Sprite sprite, float speed, float lifetime)
    {
        var bulletObject = new GameObject("bullet");
        bulletObject.transform.position = position;
        bulletObject.transform.rotation = rotation;
        bulletObject.transform.localScale = Vector3.one * BulletScale;

        var renderer = bulletObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        var rigidbody = bulletObject.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        rigidbody.gravityScale = 0f;

        var collider = bulletObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        var bullet = bulletObject.AddComponent<Bullet>();
        bullet.Init(direction, speed, lifetime, ownerTag);
    }

    private void Init(Vector2 direction, float speed, float lifetime, string ownerTag)
    {
        _direction = direction.normalized;
        _speed = speed;
        _ownerTag = ownerTag;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(_direction * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_ownerTag))
        {
            return;
        }

        var health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(Damage);
        }

        ImpactEffect.Spawn(transform.position);
        Destroy(gameObject);
    }
}
