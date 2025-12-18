using System.Collections.Generic;
using UnityEngine;

public class FreezingDebuff : BasicDebuff
{
    private const int _speed = -10;

    protected override void OnAwake()
    {
        base.OnAwake();
        _duration = 2;
    }


    public void ApplyFreezeDebuff(TurnUnit target)
    {
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

            FreezingDebuff newFreezingDebuff = target.gObject.AddComponent<FreezingDebuff>();
            newFreezingDebuff.ApplyDebuff(target);
            return;
        }

        base.ApplyDebuff(target);
    }

    protected override void ApplyDebuffEffect()
    {
        _debuffTarget.stats.ChangeSpeed(_speed);
    }

    protected override void RemoveDebuffEffect()
    {
        _debuffTarget.stats.ChangeSpeed(10);
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
        //ApplyDebuffEffect();
    }
}
