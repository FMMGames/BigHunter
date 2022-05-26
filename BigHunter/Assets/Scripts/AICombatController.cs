using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICombatController : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] NavMeshAgent _navmeshAgent;
    Transform _combatTarget;
    Vector3 _targetDirection;

    float _chargeTimer;
    bool isAttacking;

    private void Update()
    {
        if (_combatTarget == null)
            return;

        if (!isAttacking)
        {
            _chargeTimer += Time.deltaTime;

            if (_chargeTimer >= 1.5f)
            {
                _animator.SetBool("IsMoving", false);

                Attack();
                return;
            }

            _animator.SetBool("IsMoving", true);
            _navmeshAgent.Move(_targetDirection * 0.1f);
        }
    }

    public void SetTarget(Transform target)
    {
        _combatTarget = target;
        _targetDirection = (_combatTarget.position - transform.position).normalized;

        Attack();
    }

    void Attack()
    {
        isAttacking = true;
        Run.After(3f, () =>
        {
            _targetDirection = (_combatTarget.position - transform.position).normalized;
            transform.LookAt(_combatTarget);

            _animator.SetTrigger("Attack");

            Run.After(3f, () => isAttacking = false);
        });

        _animator.SetTrigger("Attack");
        _chargeTimer = 0;
    }
}
