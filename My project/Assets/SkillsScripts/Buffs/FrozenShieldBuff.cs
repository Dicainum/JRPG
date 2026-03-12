using UnityEngine;

public class FrozenShieldBuff : BasicBuff
{
    private int _shieldAmount = 0;
    private GameObject _vfxPrefab;
    private GameObject _activeVfx;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        _duration = 2; 
    }

    public void Initialize(int shieldAmount, GameObject vfxPrefab = null)
    {
        _shieldAmount = shieldAmount;
        _duration = 2;
        if (vfxPrefab != null) _vfxPrefab = vfxPrefab;
    }

    protected override void ApplyBuffEffect()
    {
        if (_buffTarget != null && _buffTarget.stats != null)
        {
            _buffTarget.stats.OnDamageTaken -= OnDamageTaken;
            _buffTarget.stats.OnDamageTaken += OnDamageTaken;
            
            _buffTarget.stats.ChangeShield(_shieldAmount);

            if (_vfxPrefab != null && _activeVfx == null)
            {
                _activeVfx = Instantiate(_vfxPrefab, _buffTarget.gObject.transform);
            }
        }
    }

    protected override void RemoveBuffEffect()
    {
        if (_buffTarget != null && _buffTarget.stats != null)
        {
            _buffTarget.stats.OnDamageTaken -= OnDamageTaken;
            _buffTarget.stats.ChangeShield(-_shieldAmount);
        }
        
        if (_activeVfx != null)
        {
            Destroy(_activeVfx);
            _activeVfx = null;
        }
    }
    
    
    private void OnDamageTaken(int damage)
    {
        if (_orderController == null)
             _orderController = FindFirstObjectByType<OrderController>();
             
        if (_orderController != null && _orderController.currentUnit != null)
        {
            TurnUnit attacker = _orderController.currentUnit;
            
            if (attacker != _buffTarget)
            {
                FreezingDebuff freezingDebuff = attacker.gObject.GetComponent<FreezingDebuff>();
                if (freezingDebuff == null)
                {
                    freezingDebuff = attacker.gObject.AddComponent<FreezingDebuff>();
                }
                
                freezingDebuff.ApplyFreezeDebuff(attacker);
            }
        }
    }
}
