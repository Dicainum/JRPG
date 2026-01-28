using UnityEngine;
using System.Collections.Generic;

public class DarkAndLightStacksBuff : BasicBuff
{
    public enum Form
    {
        Light,
        Dark
    }

    public enum StackType
    {
        Light,
        Dark
    }

    [SerializeField] private Form _currentForm = Form.Dark;
    [SerializeField] private int _maxStacks = 3;
    [SerializeField] private float _damageBonusPerStack = 0.1f;

    private List<StackType> _stacks = new List<StackType>();
    private CharacterStats _myStats;

    public Form CurrentForm
    {
        get => _currentForm;
        set => _currentForm = value;
    }

    public int StackCount => _stacks.Count;

    protected override void OnAwake()
    {
        base.OnAwake();
        _myStats = GetComponent<CharacterStats>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        _isActive = true; 
        
        if (_orderController != null)
        {
            _buffTarget = _orderController.units.Find(u => u.gObject == gameObject);
        }
    }

    public void AddStacks(int amount, TurnUnit target, StackType? forcedType = null)
    {
        if (target != null && _buffTarget == null)
        {
            _buffTarget = target;
        }

        StackType typeToAdd;
        if (forcedType.HasValue)
        {
            typeToAdd = forcedType.Value;
        }
        else
        {
            typeToAdd = (_currentForm == Form.Dark) ? StackType.Dark : StackType.Light;
        }

        for (int i = 0; i < amount; i++)
        {
            if (_stacks.Count >= _maxStacks)
            {
                _stacks.RemoveAt(0);
            }
            _stacks.Add(typeToAdd);
        }
        
        _turnsLeft = _duration;
    }

    public int GetStackCount(StackType type)
    {
        int count = 0;
        foreach (var stack in _stacks)
        {
            if (stack == type) count++;
        }
        return count;
    }

    public void RemoveAllStacksOfType(StackType type)
    {
        _stacks.RemoveAll(s => s == type);
    }

    public float GetDamageMultiplier(BasicSkill.DamageType skillType)
    {
        if (_stacks.Count == 0) return 1f;

        int darkStacks = 0;
        int lightStacks = 0;

        foreach (var stack in _stacks)
        {
            if (stack == StackType.Dark) darkStacks++;
            else if (stack == StackType.Light) lightStacks++;
        }

        float multiplier = 1f;

        if ((skillType & BasicSkill.DamageType.Light) != 0)
        {
            multiplier += (darkStacks * _damageBonusPerStack);
        }
        
        if ((skillType & BasicSkill.DamageType.Dark) != 0)
        {
            multiplier += (lightStacks * _damageBonusPerStack);
        }

        return multiplier;
    }

    protected override void TurnStarted(TurnUnit turnUnit)
    {
    }

    protected override void TurnEnded(TurnUnit turnUnit)
    {
        base.TurnEnded(turnUnit);

        if (turnUnit != null && turnUnit.gObject == gameObject)
        {
            if (_currentForm == Form.Dark)
            {
                _currentForm = Form.Light;
            }
            else
            {
                _currentForm = Form.Dark;
            }
            
            Debug.Log($"[DarkAndLightStacksBuff] Switched to {_currentForm}. Stacks: {GetStacksString()}");
        }
    }

    private string GetStacksString()
    {
        string s = "[";
        foreach (var stack in _stacks) s += stack.ToString() + ", ";
        if (_stacks.Count > 0) s = s.Substring(0, s.Length - 2);
        s += "]";
        return s;
    }
}
