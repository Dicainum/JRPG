using System.Collections.Generic;
using UnityEngine;

public class WindForceSkill : BasicSkill
{
    private const int _baseBoost = 20;
    private TurnUnit _currentTarget;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Wind Force";
        skillDescription = "Увеличивает скорость союзника на $_boost$.";
        _boost = _baseBoost;
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
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
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
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.SetCurrentSkill(this);
        }
        Debug.Log("Wind Force");
        Cast();
    }

    protected override void Cast()
    {
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected += OnTargetSelected;
            _skillTargetSystem.TargetCanceled += OnTargetCanceled;
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

    private void OnTargetCanceled()
    {
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
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
        ApplyWindForce();
    }

    private void ApplyWindForce()
    {
        Debug.Log("SpeedBoostApplied");
        TurnUnit currentUnit = GetCurrentUnit();
        if (currentUnit == null || _currentTarget == null)
            return;

        _currentTarget.stats.ChangeSpeed(_boost);
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }

        UseAction();
    }
}