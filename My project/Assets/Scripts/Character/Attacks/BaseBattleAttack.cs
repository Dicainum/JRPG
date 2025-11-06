using System;
using UnityEngine;

public class BaseBattleAttack : BattleAttack
{
    private SkillTargetSystem _skillTargetSystem;
    
    private void OnEnable()
    {
       _skillTargetSystem.TargetSelected += ApplyAttack;
    }

    private void OnDisable()
    { 
        _skillTargetSystem.TargetSelected -= ApplyAttack;
    }
    
    protected override void OnAwake()
    {
        base.OnAwake();
        _skillTargetSystem = FindFirstObjectByType<SkillTargetSystem>();
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
            target.stats.TakeDamage(_damage);
            Debug.Log("Attack applied");
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
