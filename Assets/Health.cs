using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    private int _current;
    private bool _isDead;

    public event Action OnDeath;

    public int Current => _current;

    private void Awake()
    {
        _current = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead)
        {
            return;
        }

        _current -= amount;
        if (_current > 0)
        {
            return;
        }

        _isDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
