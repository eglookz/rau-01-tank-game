using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static readonly Vector3 EnemySpawnPosition = new Vector3(6f, 3f, -0.1f);
    private static readonly Vector3 EnemyScale = new Vector3(0.25229257f, 0.3374701f, 0.45985118f);

    private bool _gameOver;
    private Health _playerHealth;
    private Health _enemyHealth;
    private Text _hudText;

    private void Start()
    {
        CreateHud();
        SpawnEnemy();
        HookPlayerHealth();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        UpdateHud();
    }

    private void HookPlayerHealth()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        var health = player.GetComponent<Health>();
        if (health == null)
        {
            health = player.AddComponent<Health>();
        }

        _playerHealth = health;
        health.OnDeath += () => EndGame("Поражение\nEscape — заново");
    }

    private void SpawnEnemy()
    {
        var sprite = Resources.Load<Sprite>("tank");

        var enemyObject = new GameObject("Enemy");
        enemyObject.transform.position = EnemySpawnPosition;
        enemyObject.transform.localScale = EnemyScale;
        enemyObject.tag = "Enemy";

        var renderer = enemyObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = new Color(1f, 0.4f, 0.4f);

        enemyObject.AddComponent<CircleCollider2D>().isTrigger = true;

        var health = enemyObject.AddComponent<Health>();
        _enemyHealth = health;
        health.OnDeath += () => EndGame("Победа!\nEscape — заново");

        enemyObject.AddComponent<Enemy>();
    }

    private void EndGame(string message)
    {
        if (_gameOver)
        {
            return;
        }

        _gameOver = true;
        Time.timeScale = 0f;
        ShowMessage(message);
    }

    private void CreateHud()
    {
        var canvasObject = new GameObject("HudCanvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();

        var textObject = new GameObject("HudText");
        textObject.transform.SetParent(canvasObject.transform, false);

        _hudText = textObject.AddComponent<Text>();
        _hudText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _hudText.fontSize = 24;
        _hudText.alignment = TextAnchor.UpperLeft;
        _hudText.color = Color.white;

        var rect = _hudText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(10f, -10f);
        rect.sizeDelta = new Vector2(300f, 80f);
    }

    private void UpdateHud()
    {
        int playerHp = _playerHealth != null ? _playerHealth.Current : 0;
        int enemyHp = _enemyHealth != null ? _enemyHealth.Current : 0;
        _hudText.text = $"Игрок: {playerHp}\nВраг: {enemyHp}";
    }

    private void ShowMessage(string message)
    {
        var canvasObject = new GameObject("EndScreenCanvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();

        var textObject = new GameObject("Message");
        textObject.transform.SetParent(canvasObject.transform, false);

        var text = textObject.AddComponent<Text>();
        text.text = message;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 48;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        var rect = text.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
