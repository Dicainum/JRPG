using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAttack : MonoBehaviour
{
    [SerializeField] protected int damageMultiplier = 1;
    protected int _damage;
    public TurnUnit baseAttackTarget;
    protected OrderController _orderController;
    public bool attackApplied = true;
    public Action<TurnUnit> Attacked;
    private TurnUnit _currentUnit;
    

    protected virtual void OnEnable()
    {
        _orderController = FindFirstObjectByType<OrderController>();
        //_orderController.OnOrderUpdated += FindThisUnit;
    }
    
    private void Awake()
    {
        StartCoroutine(InvokeOrderUpdatedNextFrame());
    }
    protected virtual void OnAwake()
    {
    }

    protected virtual void OnDisable()
    { 
        _orderController.OnOrderUpdated -= FindThisUnit;
    }

    protected virtual void OnStart()
    {
        attackApplied = true;

    }

    protected virtual void FindThisUnit(List<TurnUnit> units)
    {
        _orderController = FindFirstObjectByType<OrderController>();
        Debug.Log(units.Count + " units count");  //it`s 0 but should be 6

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
    
    private IEnumerator InvokeOrderUpdatedNextFrame()
    {
        yield return null;
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    protected virtual void CalculateDamage()
    {
        _damage = gameObject.GetComponent<Stats>().damage * damageMultiplier;
    }
    public virtual void StartAttacking()
    {
        baseAttackTarget = null;
    }
    protected virtual void ApplyAttack(TurnUnit target)
    {
        FindThisUnit(_orderController.units);
        Debug.Log(_currentUnit);
        Attacked?.Invoke(_currentUnit);
    }
    
}
