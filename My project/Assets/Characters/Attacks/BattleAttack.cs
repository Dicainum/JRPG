using UnityEngine;

public class BattleAttack : MonoBehaviour
{
    public GameObject target;
    private OrderController _orderController;
    protected virtual void ApplyAttack()
    {
        //_orderController = FindObjectOfType<OrderController>();
        //target = _orderController.Target;
    }
    
}
