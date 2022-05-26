using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] HealthManager _healthManager;
    [SerializeField] Animator _animator;

    [SerializeField] Weapon _currentWeapon;

    [SerializeField] GameObject _currentIdleVersion;
    [SerializeField] GameObject _currentActiveVersion;

    [SerializeField] Transform _idleVersionSpot;
    [SerializeField] Transform _activeVersionSpot;

    public  HealthManager _currentCombatTarget;


    public bool isAggro;

    float _attackTimer;

    private void Start()
    {
        SetupWeapon();
    }

    private void Update()
    {
        AutoAttack();
    }

    void SetupWeapon()
    {
        if (_currentActiveVersion)
            Destroy(_currentActiveVersion);

        if (_currentIdleVersion)
            Destroy(_currentIdleVersion);

        if(_currentWeapon.weaponType == WeaponType.Melee)
        {
            _currentActiveVersion = Instantiate(_currentWeapon.weaponObject, _activeVersionSpot.position, _activeVersionSpot.rotation);
            _currentActiveVersion.transform.SetParent(_activeVersionSpot);

            _currentActiveVersion.SetActive(false);
        }

        _currentIdleVersion = Instantiate(_currentWeapon.idleVersion, _idleVersionSpot.position, _idleVersionSpot.rotation);
        _currentIdleVersion.transform.SetParent(_idleVersionSpot);

        _animator.SetInteger("WeaponType", (int)_currentWeapon.weaponType);
        _attackTimer = _currentWeapon.attackRate;
    }

    #region Combat

    void AutoAttack()
    {
        if (!isAggro)
            return;

        if (_attackTimer > 0)
            _attackTimer -= Time.deltaTime;
        else if (_attackTimer < 0)
            _attackTimer = 0;

        if(_attackTimer == 0)
        {
            _animator.SetTrigger("Attack");
            _attackTimer = _currentWeapon.attackRate;
        }
    }

    #endregion

    #region Targeting Control

    public void ShiftTarget(HealthManager _newTarget)
    {
        AcquireTarget(_newTarget);
    }

    public void LoseTarget()
    {
        _currentCombatTarget = null;
        SetAggro(false);
    }

    public void AcquireTarget(HealthManager _newTarget)
    {
        HealthManager prevTarget = _currentCombatTarget;
        _currentCombatTarget = _newTarget;

        if (_currentCombatTarget)
            SetAggro(true);
        else
            SetAggro(false);
    }

    void SetAggro(bool state)
    {
        isAggro = state;

        _animator.SetBool("IsAggro", isAggro);        
    }

    #endregion

    #region Animation Events

    public void Reload()
    {
        _currentActiveVersion = Instantiate(_currentWeapon.weaponObject, _activeVersionSpot.position, _activeVersionSpot.rotation);
        _currentActiveVersion.transform.SetParent(_activeVersionSpot);
    }

    public void Shoot()
    {
        _currentActiveVersion.transform.parent = null;
        _currentActiveVersion.GetComponent<ProjectileController>().Setup(_currentCombatTarget.transform);
        _currentActiveVersion.GetComponent<DamageSource>().SetupDamage(_currentWeapon.weaponDamage, _healthManager);
    }

    public void PutAway()
    {
        if (_currentActiveVersion)
        {
            if (_currentWeapon.weaponType == WeaponType.Melee)
                _currentActiveVersion.SetActive(false);
            else
                Destroy(_currentActiveVersion);
        }
    }

    #endregion
}
