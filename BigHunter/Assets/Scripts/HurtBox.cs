using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField] int _damageMultiplier;

    [SerializeField] HealthManager _healthManager;

    void Damage(DamageSource damageSource)
    {
        _healthManager.Damage(damageSource.DamageValue(damageSource.transform.position)* _damageMultiplier, damageSource._emitter);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DamageSource>())
        {
            DamageSource damageSource = other.GetComponent<DamageSource>();

            if(!_healthManager.IsDead && damageSource._emitter != _healthManager)
                Damage(damageSource);
        }
    }
}
