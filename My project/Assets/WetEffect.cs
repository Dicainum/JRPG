using System.Collections.Generic;
using UnityEngine;

public class WetEffect : BasicDebuff
{
    private List<BasicDebuff> _buffedDebuffs = new List<BasicDebuff>();

    private float _damageMultiplier = 1.1f;

    protected override void OnAwake()
    {
        base.OnAwake();
        _duration = 2;
    }

    public void ApplyWaterDebuff(TurnUnit target)
    {
        if (_orderController == null)
        {
            _orderController = OrderController.Order ?? FindFirstObjectByType<OrderController>();
        }

        if (_isActive && _debuffTarget == target)
        {
            _turnsLeft = _duration;
            ApplyDebuffEffect();
            return;
        }

        if (_isActive && _debuffTarget != target)
        {
            RemoveDebuffEffect();

            if (_orderController != null)
                _orderController.OnTurnStarted -= TurnStarted;

            Destroy(this);

            WetEffect newWetEff = target.gObject.AddComponent<WetEffect>();
            newWetEff.ApplyWaterDebuff(target);
            return;
        }

        base.ApplyDebuff(target);

        ApplyDebuffEffect();
    }

    protected override void ApplyDebuffEffect()
    {
        var currentDebuffs = gameObject.GetComponents<BasicDebuff>();

        if (currentDebuffs.Length > 0)
        {
            foreach (var debuff in currentDebuffs)
            {
                if (debuff == this) continue;

                if (debuff.isPeriodicDamage)
                {
                    int amplifiedDamage = Mathf.CeilToInt(debuff.basicDamage * _damageMultiplier);

                    if (debuff.DamagePerTurn != amplifiedDamage)
                    {
                        debuff.DamagePerTurn = amplifiedDamage;

                        if (!_buffedDebuffs.Contains(debuff))
                        {
                            _buffedDebuffs.Add(debuff);
                        }

                        Debug.Log($"Wet Effect: Усилил {debuff.GetType().Name}. Урон {debuff.basicDamage} -> {debuff.DamagePerTurn}");
                    }
                }
            }
        }
    }

    protected override void RemoveDebuffEffect()
    {
        if (_buffedDebuffs.Count > 0)
        {
            foreach (var debuff in _buffedDebuffs)
            {
                if (debuff != null)
                {
                    debuff.DamagePerTurn = debuff.basicDamage;
                }
            }
        }
        _buffedDebuffs.Clear();
    }

    protected override void TurnStarted(TurnUnit turnUnit)
    {
        if (!_isActive || _debuffTarget == null) return;

        if (turnUnit == _debuffTarget)
        {
            _turnsLeft--;
            if (_turnsLeft <= 0)
            {
                RemoveDebuffEffect();
                _isActive = false;
                _debuffTarget = null;

                if (_orderController != null)
                {
                    _orderController.OnTurnStarted -= TurnStarted;
                }

                Destroy(this);
                return;
            }
        }
        ApplyDebuffEffect();
    }
}