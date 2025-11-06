using UnityEngine;

public class BattleAttack : MonoBehaviour
{
    [SerializeField] protected int damageMultiplier = 1;
    protected int _damage;
    public TurnUnit baseAttackTarget;
    protected OrderController _orderController;
    public bool attackApplied = true;


    protected virtual void OnAwake()
    {
        _orderController = FindFirstObjectByType<OrderController>();
    }

    protected virtual void OnStart()
    {
        attackApplied = true;
        _damage = gameObject.GetComponent<Stats>().damage * damageMultiplier;
    }
    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }
    
    public virtual void StartAttacking()
    {
        baseAttackTarget = null;
    }
    protected virtual void ApplyAttack(TurnUnit target)
    {
    }
    
}
