using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] GameObject _hitFX;

    [SerializeField] int _damageValue;

    public HealthManager _emitter;

    // receives the damage value from the AI controller
    public void SetupDamage(int _damage, HealthManager source)
    {
        _damageValue = _damage;

        _emitter = source;
    }

    public int DamageValue(Vector3 where)
    {
        if (_hitFX)
        {
            GameObject fx = Instantiate(_hitFX, where, _hitFX.transform.rotation);
        }

        return _damageValue;
    }
}
