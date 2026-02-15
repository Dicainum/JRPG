using System.Collections.Generic;
using UnityEngine;

public class WaterFlowSkill : BasicSkill
{
    [SerializeField] private int _baseDamage = 40;
    private TurnUnit _currentTarget;
    private List<TurnUnit> _damagedTargets = new List<TurnUnit>();

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Water Flow";
        skillDescription = "Наносит $_damage$ водяного урона по одной цели и накладывает эффект 'Мокрый'.";
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
        Debug.Log("WaterFlow");
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
            _skillTargetSystem.StartTargeting();
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
        ApplyWaterFlow();
    }

    private void ApplyWaterFlow()
    {
        Debug.Log("Water Applied");
        TurnUnit currentUnit = GetCurrentUnit();
        if (currentUnit == null || _currentTarget == null)
            return;

        _currentTarget.stats.TakeDamage(_damage);
        _damagedTargets.Clear();
        _damagedTargets.Add(_currentTarget);

        foreach (var damagedTarget in _damagedTargets)
        {
            if (damagedTarget != null && damagedTarget.IsAlive)
            {
                WetEffect waterEffect = damagedTarget.gObject.GetComponent<WetEffect>();
                if (waterEffect == null)
                {
                    waterEffect = damagedTarget.gObject.AddComponent<WetEffect>();
                }
                waterEffect.ApplyWaterDebuff(damagedTarget);
                //
                //FreezingDebuff freeze = damagedTarget.gObject.GetComponent<FreezingDebuff>();
                //if (freeze == null)
                //{
                //    freeze = damagedTarget.gObject.AddComponent<FreezingDebuff>();
                //}
                //freeze.ApplyFreezeDebuff(damagedTarget);
            }
        }

        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.StopTargeting();
        }

        skillUsed?.Invoke(currentUnit);

        _turnsLeft = _cooldownTime;
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }

        UseAction();
        StartCooldown();
    }
}