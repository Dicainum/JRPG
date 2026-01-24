using UnityEngine;

public class AttackBuff : BasicBuff
{
    [SerializeField] private float _damageMultiplier = 1.5f; // +50% damage

    protected override void OnAwake()
    {
        base.OnAwake();
        _duration = 2;
    }

    public float GetDamageMultiplier()
    {
        return _isActive ? _damageMultiplier : 1f;
    }
}
