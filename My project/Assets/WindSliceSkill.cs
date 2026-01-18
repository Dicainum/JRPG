using System.Collections.Generic;
using UnityEngine;

public class WindSliceSkill : BasicSkill
{
    private const int BaseDamage = 20;
    private TurnUnit _currentTarget;
    private WindBoost _windBoost;
    private List<TurnUnit> _damagedTargets = new List<TurnUnit>();
    private TurnUnit _myCurrentUnit;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Wind Slice";
        skillDescription = "Наносит малый ветряной урон цели. Каждое применение увеличивает урон последующих применений на 5%.";
        _damage = BaseDamage;
        
        _windBoost = GetComponent<WindBoost>();
        if (_windBoost == null)
        {
            _windBoost = gameObject.AddComponent<WindBoost>();
        }
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
            Debug.Log("WindSlice is on cooldown");
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
            Debug.Log("No actions left");
            return;
        }
        Debug.Log("wind slice");
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
        ApplyWindSlice();
    }

    private void ApplyWindSlice()
    {
        Debug.Log("Wind Slice applied");
        TurnUnit currentUnit = GetCurrentUnit();
        if (currentUnit == null || _currentTarget == null)
            return;

        float damageMultiplier = _windBoost != null ? _windBoost.GetDamageMultiplier() : 1f;
        int finalDamage = Mathf.RoundToInt(BaseDamage * damageMultiplier);

        _currentTarget.stats.TakeDamage(finalDamage);
        _damagedTargets.Clear();
        _damagedTargets.Add(_currentTarget);

        if (_windBoost != null)
        {
            _windBoost.ApplyBuff(currentUnit);
        }

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
            _skillTargetSystem.StopTargeting();
        }

        skillUsed?.Invoke(currentUnit);
        
        _inCooldown = true;
        _turnsLeft = _cooldownTime;
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }

        UseAction();
    } 
}
