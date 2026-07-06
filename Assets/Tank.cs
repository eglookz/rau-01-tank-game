using UnityEngine;

public class Tank : MonoBehaviour
{
    private const float FireRate = 0.35f;
    private const float BulletSpeed = 8f;
    private const float BulletLifetime = 2f;
    private const float BulletSpawnOffset = 0.5f;

    private Transform _transform;
    private AudioSource _audioSource;
    private Sprite _bulletSprite;
    private float _fireCooldown;

    void Start()
    {
        _transform = GetComponent<Transform>();
        _audioSource = GetComponent<AudioSource>();
        _bulletSprite = Resources.Load<Sprite>("bullet");

        gameObject.tag = "Player";
        if (GetComponent<Collider2D>() == null)
        {
            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }

        if (Object.FindFirstObjectByType<GameManager>() == null)
        {
            new GameObject("GameManager").AddComponent<GameManager>();
        }
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 ud = new Vector3(0, 0.1f, 0);
        Vector3 lr = new Vector3(0.1f, 0, 0);

        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.fixedDeltaTime;
        }

        if (Input.GetKey(KeyCode.Space) && _fireCooldown <= 0f)
        {
            Fire();
        }

        if (Input.GetKey(KeyCode.W))
        {
            _transform.rotation = Quaternion.Euler(0, 0, 90);
            if (_transform.position.y > 4.1f)
            {
                return;
            }
            _transform.position += ud;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _transform.rotation = Quaternion.Euler(0, 0, 270);
            if (_transform.position.y < -4.2f)
            {
                return;
            }
            _transform.position -= ud;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _transform.rotation = Quaternion.Euler(0, 0, 180);
            if (_transform.position.x < -7.8f)
            {
                return;
            }
            _transform.position -= lr;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _transform.rotation = Quaternion.Euler(0, 0, 0);
            if (_transform.position.x > 8.2f)
            {
                return;
            }
            _transform.position += lr;
        }
    }

    private void Fire()
    {
        _fireCooldown = FireRate;
        _audioSource.Play();

        Vector2 direction = _transform.right;
        Vector3 spawnPosition = _transform.position + (Vector3)direction * BulletSpawnOffset;

        Bullet.Spawn(spawnPosition, _transform.rotation, direction, "Player", _bulletSprite, BulletSpeed, BulletLifetime);
    }
}
