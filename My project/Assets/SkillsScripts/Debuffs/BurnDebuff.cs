using UnityEngine;

public class BurnDebuff : BasicDebuff
{
    protected override void OnAwake()
    {
        base.OnAwake();
        _duration = 3;
        isPeriodicDamage = true;
        DamagePerTurn = 10;
    }

    public void ApplyBurnDebuff(TurnUnit target)
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

            BurnDebuff newBurnDebuff = target.gObject.AddComponent<BurnDebuff>();
            newBurnDebuff.ApplyDebuff(target);
            return;
        }

        base.ApplyDebuff(target);
    }

    protected override void TurnStarted(TurnUnit turnUnit)
    {
        if (_isActive && _debuffTarget != null && turnUnit == _debuffTarget)
        {
            Debug.Log($"[BurnDebuff] Dealing {DamagePerTurn} damage to {_debuffTarget.stats.characterName}");
            _debuffTarget.stats.TakeDamage(DamagePerTurn);
        }

        base.TurnStarted(turnUnit);
    }
}
