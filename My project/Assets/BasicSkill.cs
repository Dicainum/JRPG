using System;
using System.Collections.Generic;
using UnityEngine;
public class BasicSkill : MonoBehaviour
{
    private TurnUnit _currentUnit;
    public string skillName;
    public string skillDescription;
    [SerializeField] protected int _damage = 1;
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
    private OrderController _orderController;
    

    protected virtual void OnAwake()
    {
        
    }
    protected virtual void OnEnable()
    { 
        _orderController.OnTurnStarted += TurnStarted;
    }
    protected virtual void OnDisable()
    { 
        _orderController.OnTurnStarted -= TurnStarted;
    }

    private void Awake()
    {
        _orderController = FindFirstObjectByType<OrderController>();
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

    private void StartCooldown()
    {
        _inCooldown = true;
        _turnsLeft = _cooldownTime;
    }

    protected virtual void TurnStarted(TurnUnit turnUnit)
    {
        
        if (_currentUnit == null || _currentUnit.gObject == null)
        {
            FindThisUnit(_orderController.units);
            Debug.Log("Findinig Unit");
        }
        
        Debug.Log(_turnsLeft);
        Debug.Log(_inCooldown);
        Debug.Log(_orderController.currentUnit.gObject);
        Debug.Log(_currentUnit.gObject);
        Debug.Log(_orderController.currentUnit == _currentUnit);

        if (_turnsLeft <= 0 && _inCooldown && _orderController.currentUnit == _currentUnit)
        {
            _turnsLeft--;
            if(_turnsLeft < 1)
            {
                _inCooldown = false;
            }
            Debug.Log(_currentUnit+" is turn left and " + _turnsLeft + " turns left");
        }

    }
    
    protected virtual void FindThisUnit(List<TurnUnit> units)
    {
        Debug.Log(units.Count + " units count");

        if (_orderController != null && units.Count > 0)
        {
            foreach (var u in units)
            {
                Debug.Log($"Unit: {u.stats.name}, GameObject: {u.gObject.name}");
            }
            _currentUnit = units.Find(u => u.gObject == gameObject);
            Debug.Log(_currentUnit);
            Debug.Log(_currentUnit.stats.name + " is currently attacking");
        }
        else
        {
            Debug.LogError("Cant find order controller");
        }
    }
}
