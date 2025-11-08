using System;
using UnityEngine;

public class BaseBattleAttack : BattleAttack
{
    private SkillTargetSystem _skillTargetSystem;
    protected override void OnAwake()
    {
        base.OnAwake();
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
        _skillTargetSystem.StartTargeting();
    }

    protected override void ApplyAttack(TurnUnit target)
    {
        if (!attackApplied)
        {
            baseAttackTarget = target;
            CalculateDamage();
            target.stats.TakeDamage(_damage);
            Debug.Log("Attack applied by " + gameObject.name + " with " + _damage + " damage");
            base.ApplyAttack(target);
            EndBaseAttack();
        }
    }

    public void EndBaseAttack()
    {
        baseAttackTarget = null;
        _skillTargetSystem.StopTargeting();
        attackApplied = true;
    }
}
