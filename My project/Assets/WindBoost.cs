using UnityEngine;

public class WindBoost : BasicBuff
{
    private const float DamageMultiplierPerStack = 1.05f;
    private int _stackCount = 0;

    public float GetDamageMultiplier()
    {
        if (_stackCount <= 0) return 1f;
        return Mathf.Pow(DamageMultiplierPerStack, _stackCount);
    }

    public override void ApplyBuff(TurnUnit target)
    {
        if (!_isActive)
        {
            base.ApplyBuff(target);
            _stackCount = 1;
        }
        else
        {
            _stackCount++;
        }
    }

    protected override void ApplyBuffEffect()
    {
    }

    protected override void RemoveBuffEffect()
    {
    }

    protected override void TurnStarted(TurnUnit turnUnit)
    {
    }
}

