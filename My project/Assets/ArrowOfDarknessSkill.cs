using UnityEngine;

public class ArrowOfDarknessSkill : BasicSkill
{
    private const int BaseDamage = 45;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Dark Arrow";
        skillDescription = "Deals medium Dark damage to a single target.";
        _damage = BaseDamage;
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

        Cast();
    }

    protected override void Cast()
    {
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected += OnTargetSelected;
            _skillTargetSystem.TargetCanceled += OnTargetCanceled;
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

        target.stats.TakeDamage(_damage);

        var buff = GetComponent<DarkAndLightStacksBuff>();
        if (buff != null)
        {
            buff.AddStacks(2, currentUnit);
        }

        UseAction();
        StartCooldown();

        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
            _skillTargetSystem.StopTargeting();
        }
    }
}
