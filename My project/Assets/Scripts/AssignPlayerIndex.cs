using UnityEngine;

public class AssignPlayerIndex : MonoBehaviour
{
    private OrderController _orderController;
    public TurnUnit turnUnit;
    private BattleAttack _battleAttack;

    private void Awake()
    {
        if (_orderController == null)
        {
            _orderController = FindFirstObjectByType<OrderController>();
        }
    }
    private void OnEnable()
    {
        turnUnit = _orderController.currentUnit;
        _battleAttack = turnUnit.gObject.GetComponent<BattleAttack>();
    }

    public void PerformBasicAttack()
    {
        _battleAttack.attackApplied = false;
        _battleAttack.StartAttacking();
    }
    
    
}
