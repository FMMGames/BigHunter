using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public delegate void NPCDeath(HealthManager victim);
    public event NPCDeath OnDeath;

    public delegate void NPCDamage(HealthManager aggressor);
    public event NPCDamage OnDamage;

    [Header("Health Settings")]
    public float _maxHealth;
    float _regenerationRate;
    float _regenerationCooldown;

    public float _currentHealth;

    [Header("VFX")]
    [SerializeField] GameObject _hitFX;

    [HideInInspector] public Animator _animator;

    public bool IsDead;
    float _regenerationTimer;
    float _lerpedHealth;

    #region Unity

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void Update()
    {

    }

    #endregion

    #region Health Management

    // handles the timer for last time avatar was attacked and health regeneration
    void HealthRegeneration()
    {
        if (_regenerationTimer > 0)
            _regenerationTimer -= Time.deltaTime;
        else if (_regenerationTimer < 0)
            _regenerationTimer = 0;
        else
        {
            if (_currentHealth < _maxHealth)
            {
                _currentHealth += _regenerationRate * Time.deltaTime;
            }
            else if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;
        }
    }

    // main method for changing health value
    void ChangeHealthPoints(float _changeAmount)
    {
        _currentHealth += _changeAmount;
        _currentHealth = Mathf.CeilToInt(_currentHealth);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Death();
        }
        else if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    // method for depleting health
    public void Damage(float _amount, HealthManager aggressor)
    {
        if (_hitFX != null)
        {
            GameObject fx = Instantiate(_hitFX, transform.position, _hitFX.transform.rotation);
            fx.transform.LookAt(new Vector3(aggressor.transform.position.x, fx.transform.position.y, aggressor.transform.position.z));
        }

        OnDamage?.Invoke(aggressor);

        _regenerationTimer = _regenerationCooldown;

        ChangeHealthPoints(-Mathf.CeilToInt(Mathf.Abs(_amount)));
    }

    // method for replenishing health
    public void Heal(float _amount)
    {
        ChangeHealthPoints(_amount);
    }

    // on health zeroed, updates visuals and disables collider to avoid being detected by enemies while dead
    void Death()
    {
        if (!IsDead)
        {
            IsDead = true;

            GetComponent<Collider>().enabled = false;

            OnDeath?.Invoke(this);
        }
    }

    // resets status, visuals and takes player back to a set point
    public void Respawn()
    {
        IsDead = false;

        GetComponent<Collider>().enabled = true;

        Heal(_maxHealth);
    }

    #endregion
}
