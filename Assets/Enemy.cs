using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const float MoveSpeed = 1.5f;
    private const float StopDistance = 3f;
    private const float FireRate = 1.2f;
    private const float BulletSpeed = 6f;
    private const float BulletLifetime = 2f;
    private const float BulletSpawnOffset = 0.5f;

    private Transform _player;
    private Sprite _bulletSprite;
    private float _fireCooldown;

    private void Start()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        _player = playerObject != null ? playerObject.transform : null;

        _bulletSprite = Resources.Load<Sprite>("bullet");
        _fireCooldown = FireRate;
    }

    private void FixedUpdate()
    {
        if (_player == null)
        {
            return;
        }

        Vector2 toPlayer = _player.position - transform.position;
        Vector2 direction;
        float rotationZ;

        if (Mathf.Abs(toPlayer.x) > Mathf.Abs(toPlayer.y))
        {
            direction = toPlayer.x > 0 ? Vector2.right : Vector2.left;
            rotationZ = toPlayer.x > 0 ? 0f : 180f;
        }
        else
        {
            direction = toPlayer.y > 0 ? Vector2.up : Vector2.down;
            rotationZ = toPlayer.y > 0 ? 90f : 270f;
        }

        transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        if (toPlayer.magnitude > StopDistance)
        {
            transform.position += (Vector3)(direction * MoveSpeed * Time.fixedDeltaTime);
        }

        _fireCooldown -= Time.fixedDeltaTime;
        if (_fireCooldown <= 0f)
        {
            _fireCooldown = FireRate;
            Fire(direction);
        }
    }

    private void Fire(Vector2 direction)
    {
        Vector3 spawnPosition = transform.position + (Vector3)direction * BulletSpawnOffset;
        Bullet.Spawn(spawnPosition, transform.rotation, direction, "Enemy", _bulletSprite, BulletSpeed, BulletLifetime);
    }
}
