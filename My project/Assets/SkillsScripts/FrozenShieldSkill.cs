using System.Collections.Generic;
using UnityEngine;

//TODO: Исправить ошибку, когда щит можно было наложить на мертвого союзника и получить бесконечный "ход"
public class FrozenShieldSkill : BasicSkill
{
    private List<TurnUnit> _damagedTargets = new List<TurnUnit>();
    private const int _baseShield = 20;
    private TurnUnit _currentTarget;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Frozen Shield";
        skillDescription = "Дарует щит союзнику или себе на $_boost$. Накладывает эффект 'Разрез ветра'.";
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
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }
    }
    public override void TryCast()
    {
        if (_inCooldown)
        {
            Debug.Log($"{skillName} is on cooldown.");
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
            _skillTargetSystem.StartTargetingAlly();
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
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }

        UseAction();
        StartCooldown();
    }
}
