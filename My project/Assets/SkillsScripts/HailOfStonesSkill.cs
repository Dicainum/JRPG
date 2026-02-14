using System.Collections.Generic;
using UnityEngine;

public class HailOfStonesSkill : BasicSkill
{
    private const int _baseDamage = 35;
    private TurnUnit _currentTarget;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Heil of Stones";
        skillDescription = "Наносит $_damage$ физического урона по одной цели.";
        _damage = _baseDamage;
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
        Debug.Log("HailOfStones");
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
            _skillTargetSystem.StartTargeting(); //
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
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }
        ApplyHailOfStones();
    }
    private void ApplyHailOfStones()
    {
        Debug.Log("HailApplied");
        TurnUnit currentUnit = GetCurrentUnit();
        if (currentUnit == null || _currentTarget == null)
            return;

        _currentTarget.stats.TakeDamage(_damage);
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }

        UseAction();
    }
}
