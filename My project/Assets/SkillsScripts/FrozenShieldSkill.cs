using System.Collections.Generic;
using UnityEngine;

//TODO: Исправить ошибку, когда щит можно было наложить на мертвого союзника и получить бесконечный "ход"
public class FrozenShieldSkill : BasicSkill
{
    private List<TurnUnit> _damagedTargets = new List<TurnUnit>();
    private const int _baseShield = 20;
    private TurnUnit _currentTarget;
    [SerializeField] private GameObject _sheildVFX;

    protected override void OnAwake()
    {
        base.OnAwake();
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
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }
        ApplyFrozenShield();
        
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }

        UseAction();
    }

    private void ApplyFrozenShield()
    {
        Debug.Log("ShieldApplied");
        TurnUnit currentUnit = GetCurrentUnit();
        if (currentUnit == null || _currentTarget == null)
            return;
        
        FrozenShieldBuff frozenShieldBuff = _currentTarget.gObject.GetComponent<FrozenShieldBuff>();
        if (frozenShieldBuff == null)
        {
            frozenShieldBuff = _currentTarget.gObject.AddComponent<FrozenShieldBuff>();
            frozenShieldBuff.Initialize(_boost, _sheildVFX);
            frozenShieldBuff.ApplyBuff(_currentTarget);
        }
        else
        {
            frozenShieldBuff.Initialize(_boost, _sheildVFX);
            frozenShieldBuff.ApplyBuff(_currentTarget);
        }
        StartCooldown();
    }
}
