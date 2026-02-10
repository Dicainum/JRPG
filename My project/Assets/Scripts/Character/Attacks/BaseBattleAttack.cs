using System;
using DG.Tweening;
using UnityEngine;

public class BaseBattleAttack : BattleAttack
{
    private SkillTargetSystem _skillTargetSystem;
    private CharacterAnimationController _animationController;

    protected override void OnAwake()
    {
        base.OnAwake();
        _animationController = GetComponentInChildren<CharacterAnimationController>();
        _skillTargetSystem = FindFirstObjectByType<SkillTargetSystem>();
        _skillTargetSystem.TargetSelected += ApplyAttack;
    }
    protected override void OnDisable()
    { 
        base.OnDisable();
        _skillTargetSystem.TargetSelected -= ApplyAttack;
    }
    
    public override void StartAttacking()
    {
        base.StartAttacking();
        if (_animationController != null)
        {
            _animationController.StartAimingBasicAttack();
        }
        _skillTargetSystem.StartTargeting();
    }

    protected override void ApplyAttack(TurnUnit target)
    {
        if (!attackApplied)
        {
            //MoveToAttackPosition(target.stats.index);
            StartAttackAnimation();
            baseAttackTarget = target;
        }
    }

    public void ApplyDamage()
    {
        CalculateDamage();
        baseAttackTarget.stats.TakeDamage(_damage);
        Debug.Log("Attack applied by " + gameObject.name + " with " + _damage + " damage");
        base.ApplyAttack(baseAttackTarget);
        EndBaseAttack();
    }

    public void EndBaseAttack()
    {
        baseAttackTarget = null;
        _skillTargetSystem.StopTargeting();
        attackApplied = true;
    }

    public void MoveBackToPosition()
    {
        transform.DOMove(_currentUnitPosition.position, timeToMoveBack)
            .SetEase(Ease.Linear);
    }
}
