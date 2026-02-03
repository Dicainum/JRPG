using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BattleAttack : MonoBehaviour
{
    [SerializeField] protected int damageMultiplier = 1;
    [SerializeField] protected float timeToMove = 0.5f;
    [SerializeField] protected float timeToMoveBack = 0.5f;
    protected Transform[] _targetPositions;
    protected MeleePositionsReference _targetPositionsParent;
    protected int _damage;
    public TurnUnit baseAttackTarget;
    protected OrderController _orderController;
    public bool attackApplied = true;
    public Action<TurnUnit> Attacked;
    private TurnUnit _currentUnit;
    private Animator _anim;
    private CharacterPositions _characterPositions;
    protected Transform _currentUnitPosition;
    

    protected virtual void OnEnable()
    {
        _orderController = FindFirstObjectByType<OrderController>();
    }
    
    private void Awake()
    {
        StartCoroutine(InvokeOrderUpdatedNextFrame());
    }
    protected virtual void OnAwake()
    {
        CalculateTargetPositions();
        _anim = gameObject.GetComponent<Animator>();
        _characterPositions = FindFirstObjectByType<CharacterPositions>();
        Debug.Log(_characterPositions.charPositions.Count()); 
    }

    protected virtual void MoveToAttackPosition(int index)
    {
        //transform.DOMove(_targetPositions[index].position, timeToMove)
         //   .SetEase(Ease.Linear);
    }

    protected virtual void StartAttackAnimation()
    {
        //_anim.SetTrigger("Attack");
    }

    private void CalculateTargetPositions()
    {
        _targetPositionsParent = FindFirstObjectByType<MeleePositionsReference>();
        _targetPositions = _targetPositionsParent.MeleePositions;
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
        if (_currentUnitPosition == null)
        {
            _currentUnitPosition = _characterPositions.charPositions[_currentUnit.stats.index];
        }
        Debug.Log(_currentUnit);
        Attacked?.Invoke(_currentUnit);
    }
    
}
