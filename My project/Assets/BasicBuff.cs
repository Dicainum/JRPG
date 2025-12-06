using System;
using UnityEngine;

public class BasicBuff : MonoBehaviour
{
    [SerializeField] protected int _duration = 1;
    protected int _turnsLeft = 0;
    protected TurnUnit _buffTarget;
    protected bool _isActive = false;
    private OrderController _orderController;

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
        _turnsLeft = 0;
        _isActive = false;
    }

    private void Start()
    {
        OnStart();
        if (_orderController != null)
        {
            _orderController.OnTurnStarted += TurnStarted;
        }
    }

    public virtual void ApplyBuff(TurnUnit target)
    {
        _buffTarget = target;
        _turnsLeft = _duration;
        _isActive = true;
        ApplyBuffEffect();
    }

    protected virtual void ApplyBuffEffect()
    {
    }

    protected virtual void RemoveBuffEffect()
    {
    }

    protected virtual void TurnStarted(TurnUnit turnUnit)
    {
        if (!_isActive || _buffTarget == null)
            return;

        if (turnUnit == _buffTarget)
        {
            _turnsLeft--;
            if (_turnsLeft <= 0)
            {
                RemoveBuffEffect();
                _isActive = false;
                _buffTarget = null;
            }
        }
    }
}
