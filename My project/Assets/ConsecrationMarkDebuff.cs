using UnityEngine;

public class ConsecrationMarkDebuff : BasicDebuff
{
    private float _damagePercent = 0.5f; 
    private TurnUnit _caster;

    protected override void OnAwake()
    {
        base.OnAwake();
        _duration = 2;
    }

    public void ApplyMark(TurnUnit target, TurnUnit caster, float damagePercent)
    {
        _caster = caster;
        _damagePercent = damagePercent;
        
        if (_orderController == null)
        {
            _orderController = OrderController.Order ?? FindFirstObjectByType<OrderController>();
        }

        if (_isActive && _debuffTarget == target)
        {
            _turnsLeft = _duration;
            return;
        }

        ApplyDebuff(target);
    }

    protected override void ApplyDebuffEffect()
    {
        base.ApplyDebuffEffect();
        if (_debuffTarget != null && _debuffTarget.stats != null)
        {
            _debuffTarget.stats.OnDamageTaken -= OnTargetDamageTaken;
            _debuffTarget.stats.OnDamageTaken += OnTargetDamageTaken;
        }
    }

    protected override void RemoveDebuffEffect()
    {
        base.RemoveDebuffEffect();
        if (_debuffTarget != null && _debuffTarget.stats != null)
        {
            _debuffTarget.stats.OnDamageTaken -= OnTargetDamageTaken;
        }
    }

    private void OnTargetDamageTaken(int damage)
    {
        if (_debuffTarget == null || _caster == null || _orderController == null) return;

        TurnUnit attacker = _orderController.currentUnit;
        if (attacker == null) return;

        bool isCasterPlayer = _caster.gObject.CompareTag("Player");
        bool isAttackerPlayer = attacker.gObject.CompareTag("Player");

        if (isCasterPlayer == isAttackerPlayer)
        {
            if (_debuffTarget.stats != null)
                 _debuffTarget.stats.OnDamageTaken -= OnTargetDamageTaken;

            int bonusDamage = Mathf.RoundToInt(damage * _damagePercent);
            if (bonusDamage < 1) bonusDamage = 1;
            
            Debug.Log($"[ConsecrationMark] Triggered by {attacker.gObject.name}! Dealing {bonusDamage} bonus damage.");
            _debuffTarget.stats.TakeDamage(bonusDamage);
            
            RemoveDebuffEffect();
            Destroy(this);
        }
    }
}
