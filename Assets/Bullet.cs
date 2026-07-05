using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 _direction;
    private float _speed;

    public void Init(Vector2 direction, float speed, float lifetime)
    {
        _direction = direction.normalized;
        _speed = speed;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(_direction * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }

        Destroy(gameObject);
    }
}
