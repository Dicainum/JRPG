using System.Collections.Generic;
using UnityEngine;

public class WindCut : BasicDebuff
{
    private TurnUnit _caster;

    protected override void OnAwake()
    {
        DamagePerTurn = 5;
        basicDamage = DamagePerTurn;
        base.OnAwake();
        _duration = 2;
        isPeriodicDamage = true;
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    public void ApplyDebuff(TurnUnit target, TurnUnit caster)
    {
        _caster = caster;
        
        if (_orderController == null)
        {
            _orderController = OrderController.Order ?? FindFirstObjectByType<OrderController>();
        }
        
        if (_isActive && _debuffTarget == target)
        {
            _turnsLeft = _duration;
            return;
        }
        
        if (_isActive && _debuffTarget != target)
        {
            RemoveDebuffEffect();
            if (_orderController != null)
            {
                _orderController.OnTurnStarted -= TurnStarted;
            }
            Destroy(this);
            
            WindCut newWindCut = target.gObject.AddComponent<WindCut>();
            newWindCut.ApplyDebuff(target, caster);
            return;
        }
        
        base.ApplyDebuff(target);
    }

    protected override void ApplyDebuffEffect()
    {
    }

    protected override void RemoveDebuffEffect()
    {
    }

    protected override void TurnStarted(TurnUnit turnUnit)
    {
        if (!_isActive || _debuffTarget == null)
            return;

        if (turnUnit == _debuffTarget)
        {
            int damageDealt = DamagePerTurn;
            
            if (_debuffTarget.IsAlive)
            {
                _debuffTarget.stats.TakeDamage(damageDealt);
            }
            
            if (_caster != null && _orderController != null)
            {
                bool isCasterEnemy = _caster.stats.isEnemy;
                
                foreach (var unit in _orderController.units)
                {
                    if (!unit.IsAlive) continue;
                    
                    if (unit.stats.isEnemy == isCasterEnemy && unit != _debuffTarget)
                    {
                        unit.stats.Heal(damageDealt);
                    }
                }
            }
            
            _turnsLeft--;
            if (_turnsLeft <= 0)
            {
                RemoveDebuffEffect();
                _isActive = false;
                _debuffTarget = null;
                _caster = null;
                
                if (_orderController != null)
                {
                    _orderController.OnTurnStarted -= TurnStarted;
                }
                
                Destroy(this);
            }
        }
    }
}

