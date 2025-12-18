using System.Collections.Generic;
using UnityEngine;

//TODO: проверить логику работы когда враги будут дамажить и написать логику накладывания "охлаждения"
public class FrozenShieldSkill : BasicSkill
{
    private List<TurnUnit> _damagedTargets = new List<TurnUnit>();
    private const int _baseShield = 20;
    private TurnUnit _currentTarget;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Frozen Shield";
        skillDescription = "Вешает на союзника щит на $_boost$. Накладывает дебафф 'Охлаждение'.";
        _boost = _baseShield;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
        }
    }
    public override void TryCast()
    {
        if (_inCooldown)
        {
            return;
        }

        TurnUnit currentUnit = GetCurrentUnit();
        if (currentUnit == null)
        {
            Debug.LogWarning("Current unit not found");
            return;
        }

        if (currentUnit.stats.actions <= 0)
        {
            return;
        }
        Debug.Log("Wind Force");
        Cast();
    }

    protected override void Cast()
    {
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected += OnTargetSelected;
        }
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.StartTargetingAlly(); //
        }
        else
        {
            Debug.LogError("SkillTargetSystem not found");
        }
    }

    private void OnTargetSelected(TurnUnit target)
    {
        if (target == null || !target.IsAlive)
            return;

        TurnUnit currentUnit = GetCurrentUnit();
        if (currentUnit == null || currentUnit.stats.actions <= 0)
            return;

        _currentTarget = target;
        ApplyFrozenShield();
    }

    private void ApplyFrozenShield()
    {
        Debug.Log("ShieldApplied");
        TurnUnit currentUnit = GetCurrentUnit();
        if (currentUnit == null || _currentTarget == null)
            return;

        _currentTarget.stats.ChangeShield(_boost);

        foreach (var damagedTarget in _damagedTargets)
        {
            if (damagedTarget != null && damagedTarget.IsAlive)
            {
                WindCut windCut = damagedTarget.gObject.GetComponent<WindCut>();
                if (windCut == null)
                {
                    windCut = damagedTarget.gObject.AddComponent<WindCut>();
                }
                windCut.ApplyDebuff(damagedTarget, currentUnit);
            }
        }

        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
        }

        UseAction();
    }
}
