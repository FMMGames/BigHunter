using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] HealthManager _boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCombatController>() && !other.GetComponent<PlayerCombatController>().isAggro)
        {
            _boss.GetComponent<AICombatController>().SetTarget(other.transform);
            other.GetComponent<PlayerCombatController>().AcquireTarget(_boss);
        }
    }
}
