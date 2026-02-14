using UnityEngine;

public class Glares : BasicSkill
{
    [Header("Glares Settings")]
    [SerializeField] private int _damagePerHit = 8;
    [SerializeField] private int _hitCount = 5;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Glares";
        skillDescription = $"Наносит светом {_hitCount} ударов малого урона. Если стаков тьмы больше, чем света, преобразует все стаки тьмы в свет.";
        _damage = _damagePerHit * _hitCount; 
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CleanupEvents();
    }

    public override void TryCast()
    {
        if (_inCooldown) return;

        TurnUnit caster = GetCurrentUnit();
        if (caster == null || caster.stats.actions <= 0) return;

        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.SetCurrentSkill(this);
            _skillTargetSystem.TargetSelected += OnTargetSelected;
            _skillTargetSystem.TargetCanceled += OnTargetCanceled;
            _skillTargetSystem.StartTargeting();
        }
        else
        {
            Debug.LogWarning($"[{skillName}] SkillTargetSystem not found!");
        }
    }

    private void OnTargetSelected(TurnUnit target)
    {
        if (target == null || !target.IsAlive) return;

        TurnUnit caster = GetCurrentUnit();
        if (caster == null) return;

        HandleStackConversion(caster);

        ApplyMultiHitDamage(target);

        UseAction();
        StartCooldown();
        
        CleanupEvents();
    }

    private void OnTargetCanceled()
    {
        CleanupEvents();
    }

    private void CleanupEvents()
    {
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }
    }

    private void HandleStackConversion(TurnUnit caster)
    {
        if (caster.gObject.TryGetComponent<DarkAndLightStacksBuff>(out var stacksBuff))
        {
            int darkStacks = stacksBuff.GetStackCount(DarkAndLightStacksBuff.StackType.Dark);
            int lightStacks = stacksBuff.GetStackCount(DarkAndLightStacksBuff.StackType.Light);

            if (darkStacks > lightStacks)
            {
                stacksBuff.RemoveAllStacksOfType(DarkAndLightStacksBuff.StackType.Dark);
                var previousForm = stacksBuff.CurrentForm;
                
                stacksBuff.CurrentForm = DarkAndLightStacksBuff.Form.Light;
                stacksBuff.AddStacks(darkStacks, caster);
                
                stacksBuff.CurrentForm = previousForm;

                Debug.Log($"[{skillName}] Converted {darkStacks} Dark stacks to Light stacks.");
            }
        }
    }

    private void ApplyMultiHitDamage(TurnUnit target)
    {
        for (int i = 0; i < _hitCount; i++)
        {
            if (!target.IsAlive) break;
            target.stats.TakeDamage(_damagePerHit);
        }
    }
}