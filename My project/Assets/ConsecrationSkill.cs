using System.Collections.Generic;
using UnityEngine;

public class ConsecrationSkill : BasicSkill
{
    [SerializeField] private int _pureDamage = 20;
    [SerializeField] private float _markDamagePercent = 0.5f;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Consecration";
        skillDescription = "Requires 3 Dark Stacks. Consumes stacks to deal Pure damage to all enemies and apply a Mark. Mark deals Light damage when hit by ally.";
        _damage = _pureDamage;
    }

    public override bool CanUse()
    {
        TurnUnit caster = GetCurrentUnit();
        if (caster == null) return false;

        var stacksBuff = caster.gObject.GetComponent<DarkAndLightStacksBuff>();
        if (stacksBuff != null)
        {
            int darkStacks = stacksBuff.GetStackCount(DarkAndLightStacksBuff.StackType.Dark);
            return darkStacks >= 3;
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
                _skillTargetSystem.TargetSelected += OnTargetSelected;
                _skillTargetSystem.TargetCanceled += OnTargetCanceled;
                _skillTargetSystem.StartTargeting();
            }
            else
            {
                OnTargetSelected(null);
            }
        }
        else
        {
            Debug.Log("Not enough Dark Stacks!");
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

        var stacksBuff = caster.gObject.GetComponent<DarkAndLightStacksBuff>();
        if (stacksBuff != null)
        {
            stacksBuff.RemoveAllStacksOfType(DarkAndLightStacksBuff.StackType.Dark);
        }

        List<TurnUnit> targets = GetOpponents(caster);
        
        if (targets.Count == 0)
        {
            Debug.LogWarning("ConsecrationSkill: No opponents found!");
        }

        foreach (var t in targets)
        {
            if (!t.IsAlive) continue;

            t.stats.TakeDamage(_pureDamage);

            var mark = t.gObject.GetComponent<ConsecrationMarkDebuff>();
            if (mark == null)
            {
                mark = t.gObject.AddComponent<ConsecrationMarkDebuff>();
            }
            
            mark.ApplyMark(t, caster, _markDamagePercent);
        }

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
        
        bool casterIsEnemy = false;
        if (caster.stats != null)
        {
            casterIsEnemy = caster.stats.isEnemy;
        }

        if (_myOrderController != null && _myOrderController.units != null)
        {
            foreach (var unit in _myOrderController.units)
            {
                if (!unit.IsAlive || unit == caster) continue;

                bool unitIsEnemy = false;
                if (unit.stats != null)
                {
                    unitIsEnemy = unit.stats.isEnemy;
                }
                
                if (casterIsEnemy != unitIsEnemy)
                {
                    opponents.Add(unit);
                }
            }
        }
        return opponents;
    }
}
