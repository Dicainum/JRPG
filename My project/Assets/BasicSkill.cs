using System;
using System.Collections.Generic;
using UnityEngine;
public class BasicSkill : MonoBehaviour
{
    protected TurnUnit _currentUnit;
    protected SkillTargetSystem _skillTargetSystem;
    protected OrderController _myOrderController;
    public string skillName;
    public string skillDescription;
    [SerializeField] protected int _damage = 1;
    [SerializeField] protected int _boost = 1;
    private int _defaultBoost;
    [System.Flags]
    public enum DamageType
    {
        None   = 0,
        Physic = 1 << 0,
        Wind   = 1 << 1,
        Light  = 1 << 2,
        Dark   = 1 << 3,
        Pure   = 1 << 4,
        Water  = 1 << 5,
        Ice    = 1 << 6
    }
    
    [SerializeField] private DamageType _damageType;
    [SerializeField] protected int _cooldownTime = 0; //in turns
    protected int _turnsLeft = 0;
    public Action<TurnUnit> skillUsed;
    protected bool _inCooldown = false;
    

    protected virtual void OnAwake()
    {
        _skillTargetSystem = FindFirstObjectByType<SkillTargetSystem>();
        _myOrderController = FindFirstObjectByType<OrderController>();

        _defaultBoost = _boost;
    }
    protected virtual void OnEnable()
    {
        _myOrderController.OnTurnStarted += TurnStarted;
    }
    protected virtual void OnDisable()
    {
        _myOrderController.OnTurnStarted -= TurnStarted;
        _boost = _defaultBoost;
    }

    private void Awake()
    {
        //_orderController = FindFirstObjectByType<OrderController>();
        OnAwake();
    }

    protected virtual void OnStart()
    {
        _turnsLeft = 0;
        _inCooldown = false;
    }

    private void Start()
    {
        OnStart();
    }

    public virtual void TryCast()
    {
        if (!_inCooldown)
        {
            Cast();
            StartCooldown();
        }
    }

    protected virtual void Cast()
    {
        
    }

    protected void UseAction()
    {
        _currentUnit.stats.actions -= 1;
        OrderController orderController = OrderController.Order ?? _myOrderController;
        orderController?.OnActionPerformed?.Invoke(_currentUnit);

        if (_currentUnit.stats.actions <= 0)
        {
            _currentUnit.stats.actions = _currentUnit.stats.baseActions;
            orderController?.NextTurn();
        }
    }

    protected void StartCooldown()
    {
        _inCooldown = true;
        _turnsLeft = _cooldownTime;
    }

    protected virtual void TurnStarted(TurnUnit turnUnit)
    {
        
        if (_currentUnit == null || _currentUnit.gObject == null)
        {
            _currentUnit = GetCurrentUnit();
            Debug.Log("Findinig Unit");
        }
        
        Debug.Log(_turnsLeft);
        Debug.Log(_inCooldown);
        Debug.Log(_myOrderController.currentUnit.gObject);
        Debug.Log(_currentUnit.gObject);
        Debug.Log(_myOrderController.currentUnit == _currentUnit);

        if (_turnsLeft <= 0 && _inCooldown && _myOrderController.currentUnit == _currentUnit)
        {
            _turnsLeft--;
            if(_turnsLeft < 1)
            {
                _inCooldown = false;
            }
            Debug.Log(_currentUnit+" is turn left and " + _turnsLeft + " turns left");
        }

    }

    protected virtual TurnUnit GetCurrentUnit()
    {
        if (_currentUnit != null && _currentUnit.gObject == gameObject)
            return _currentUnit;

        OrderController orderController = OrderController.Order;
        if (orderController == null)
        {
            orderController = FindFirstObjectByType<OrderController>();
            if (orderController == null)
                return null;
        }

        if (orderController.currentUnit != null && orderController.currentUnit.gObject == gameObject)
        {
            _currentUnit = orderController.currentUnit;
            return _currentUnit;
        }

        _currentUnit = orderController.units.Find(u => u.gObject == gameObject);
        return _currentUnit;
    }
}
