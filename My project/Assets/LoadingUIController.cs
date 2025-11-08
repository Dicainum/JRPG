using UnityEngine;
using System;
using UnityEngine.UI;

public class LoadingUIController : MonoBehaviour
{
    private OrderController _orderController;
    private Animator _animator;
    public Action<TurnUnit> Loaded;
    [SerializeField] private GameObject _defendUI;
    [SerializeField] private GameObject _attackUI;
    private RectMask2D _mask;
    private Vector4 _padding;

    private void Awake()
    { 
        _mask = gameObject.GetComponent<RectMask2D>();
        _padding = _mask.padding;
        _padding.z = 1920f;
        _mask.padding = _padding;
        _animator = gameObject.GetComponent<Animator>();
        _orderController = FindFirstObjectByType<OrderController>();
    }
    
    public void StartAnimation()
    {
        if (_orderController.currentUnit.stats.isEnemy)
        {
            _attackUI.SetActive(false);
            Debug.Log("Defending");
        }
        else
        {
            _defendUI.SetActive(false);
            Debug.Log("Attacking");
            
        }
        _animator.SetTrigger("StartAnim");
    }
    
    public void LoadingUIAnimationFinished()
    {
        _orderController.StartNextTurn();
    }

    public void LoadingUIAnimationFaded()
    {
        _defendUI.SetActive(false);
        _attackUI.SetActive(false);
    }

}
