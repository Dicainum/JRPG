using System.Collections.Generic;
using UnityEngine;

public class InfinteDarkness : BasicSkill
{
    [Header("Infinite Darkness Settings")]
    [SerializeField] private float _hpCostPercentage = 0.4f;
    [SerializeField] private int _attackBuffDuration = 2;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    public override bool CanUse()
    {
        TurnUnit caster = GetCurrentUnit();
        if (caster == null) return false;

        bool hpCondition = caster.stats.health > (caster.stats.maxHealth * 0.5f);
        
        var stacksBuff = caster.gObject.GetComponent<DarkAndLightStacksBuff>();
        if (stacksBuff != null)
        {
            int lightStacks = stacksBuff.GetStackCount(DarkAndLightStacksBuff.StackType.Light);
            return lightStacks >= 3 && hpCondition;
        }

        return false;
    }

    public override void TryCast()
    {
        if (_inCooldown) return;

        if (CanUse())
        {
            if (_skillTargetSystem != null)
            {
                _skillTargetSystem.SetCurrentSkill(this);
                _skillTargetSystem.TargetSelected += OnTargetSelected;
                _skillTargetSystem.TargetCanceled += OnTargetCanceled;
                _skillTargetSystem.StartTargeting();
            }
            else
            {
                OnTargetSelected(null);
            }
        }
    }

    private void OnTargetSelected(TurnUnit target)
    {
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }

        TurnUnit caster = GetCurrentUnit();
        if (caster == null) return;

        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }

        int hpCost = Mathf.RoundToInt(caster.stats.maxHealth * _hpCostPercentage);
        caster.stats.TakeDamage(hpCost);

        var stacksBuff = caster.gObject.GetComponent<DarkAndLightStacksBuff>();
        if (stacksBuff != null)
        {
            stacksBuff.RemoveAllStacksOfType(DarkAndLightStacksBuff.StackType.Light);
        }

        List<TurnUnit> targets = GetOpponents(caster);
        foreach (var t in targets)
        {
            if (!t.IsAlive) continue;
            t.stats.TakeDamage(_damage);
        }

        var atkBuff = caster.gObject.GetComponent<AttackBuff>();
        if (atkBuff == null)
        {
            atkBuff = caster.gObject.AddComponent<AttackBuff>();
        }
        atkBuff.ApplyBuff(caster);

        UseAction();
        StartCooldown();
    }

    private void OnTargetCanceled()
    {
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
        }
    }

    private List<TurnUnit> GetOpponents(TurnUnit caster)
    {
        List<TurnUnit> opponents = new List<TurnUnit>();
        bool casterIsPlayer = caster.gObject.CompareTag("Player");

        if (_myOrderController != null && _myOrderController.units != null)
        {
            foreach (var unit in _myOrderController.units)
            {
                if (!unit.IsAlive || unit == caster) continue;

                bool unitIsPlayer = unit.gObject.CompareTag("Player");
                
                if (casterIsPlayer != unitIsPlayer)
                {
                    opponents.Add(unit);
                }
            }
        }
        return opponents;
    }
}
