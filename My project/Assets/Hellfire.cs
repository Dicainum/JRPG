using System.Collections.Generic;
using UnityEngine;

public class Hellfire : BasicSkill
{
    [Header("Hellfire Settings")]
    [SerializeField] private int _burnDamage = 10;
    [SerializeField] private int _hellfireDamage = 30;
    [SerializeField] private float _critMultiplier = 1.5f;

    protected override void OnAwake()
    {
        base.OnAwake();
        skillName = "Hellfire";
        skillDescription = "Deals fire damage. If Frozen, crits and hits all.";
        _damage = _hellfireDamage;
    }

    public override void TryCast()
    {
        if (_inCooldown) return;

        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected += OnTargetSelected;
            _skillTargetSystem.TargetCanceled += OnTargetCanceled;
            _skillTargetSystem.StartTargeting();
        }
    }

    private void OnTargetSelected(TurnUnit target)
    {
        if (target == null || !target.IsAlive) return;

        TurnUnit caster = GetCurrentUnit();
        if (caster == null) return;

        ExecuteHellfire(caster, target);

        UseAction();
        StartCooldown();
        
        if (_skillTargetSystem != null)
        {
            _skillTargetSystem.TargetSelected -= OnTargetSelected;
            _skillTargetSystem.TargetCanceled -= OnTargetCanceled;
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

    private void ExecuteHellfire(TurnUnit caster, TurnUnit target)
    {
        var freezing = target.gObject.GetComponent<FreezingDebuff>();
        bool isFrozen = freezing != null; 

        List<TurnUnit> targets = new List<TurnUnit>();

        if (isFrozen)
        {
            targets = GetOpponents(caster);
        }
        else
        {
            targets.Add(target);
        }

        foreach (var t in targets)
        {
            if (!t.IsAlive) continue;

            int dmg = _hellfireDamage;
            if (isFrozen)
            {
                dmg = Mathf.RoundToInt(dmg * _critMultiplier);
            }

            t.stats.TakeDamage(dmg);
            
            var burn = t.gObject.GetComponent<BurnDebuff>();
            if (burn == null)
            {
                burn = t.gObject.AddComponent<BurnDebuff>();
            }
            burn.ApplyBurnDebuff(t);
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
