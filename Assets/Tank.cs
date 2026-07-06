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
    private Camera _camera;
    private float _boundsMargin;
    private float _fireCooldown;

    void Start()
    {
        _transform = GetComponent<Transform>();
        _audioSource = GetComponent<AudioSource>();
        _bulletSprite = Resources.Load<Sprite>("bullet");
        _camera = Camera.main;

        // Same margin works for every facing: rotating in 90-degree steps always
        // puts the sprite's original width along whichever axis the tank is moving on.
        _boundsMargin = GetComponent<SpriteRenderer>().sprite.bounds.extents.x * _transform.localScale.x;

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

        if (Object.FindFirstObjectByType<MainMenu>() == null)
        {
            new GameObject("MainMenu").AddComponent<MainMenu>();
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

        float halfHeight = _camera.orthographicSize;
        float halfWidth = halfHeight * _camera.aspect;
        Vector3 camPos = _camera.transform.position;

        float maxY = camPos.y + halfHeight - _boundsMargin;
        float minY = camPos.y - halfHeight + _boundsMargin;
        float maxX = camPos.x + halfWidth - _boundsMargin;
        float minX = camPos.x - halfWidth + _boundsMargin;

        if (Input.GetKey(KeyCode.W))
        {
            _transform.rotation = Quaternion.Euler(0, 0, 90);
            if (_transform.position.y > maxY)
            {
                return;
            }
            _transform.position += ud;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _transform.rotation = Quaternion.Euler(0, 0, 270);
            if (_transform.position.y < minY)
            {
                return;
            }
            _transform.position -= ud;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _transform.rotation = Quaternion.Euler(0, 0, 180);
            if (_transform.position.x < minX)
            {
                return;
            }
            _transform.position -= lr;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _transform.rotation = Quaternion.Euler(0, 0, 0);
            if (_transform.position.x > maxX)
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
