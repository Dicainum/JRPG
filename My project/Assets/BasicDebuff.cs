using System;
using UnityEngine;

public class BasicDebuff : MonoBehaviour
{
    [SerializeField] protected int _duration = 1;
    protected int _turnsLeft = 0;
    protected TurnUnit _debuffTarget;
    protected bool _isActive = false;
    protected OrderController _orderController;
    public bool isPeriodicDamage = false;
    public int basicDamage;
    public int DamagePerTurn;
    protected virtual void OnAwake()
    {
        
    }

    protected virtual void OnEnable()
    {
        if (_orderController != null)
        {
            _orderController.OnTurnStarted += TurnStarted;
        }
    }

    protected virtual void OnDisable()
    {
        if (_orderController != null)
        {
            _orderController.OnTurnStarted -= TurnStarted;
        }
    }

    private void Awake()
    {
        _orderController = FindFirstObjectByType<OrderController>();
        OnAwake();
    }

    protected virtual void OnStart()
    {
        if (!_isActive)
        {
            _turnsLeft = 0;
        }
    }

    private void Start()
    {
        OnStart();
    }

    public virtual void ApplyDebuff(TurnUnit target)
    {
        _debuffTarget = target;
        _turnsLeft = _duration;
        _isActive = true;
        
        if (_orderController == null)
        {
            _orderController = OrderController.Order ?? FindFirstObjectByType<OrderController>();
        }
        
        ApplyDebuffEffect();
    }

    protected virtual void ApplyDebuffEffect()
    {
    }

    protected virtual void RemoveDebuffEffect()
    {
    }

    protected virtual void TurnStarted(TurnUnit turnUnit)
    {
        if (!_isActive || _debuffTarget == null)
            return;

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
            }
        }
    }
}

