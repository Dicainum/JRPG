using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShowOrder : MonoBehaviour
{
    [SerializeField] private OrderController orderController;
    [SerializeField] private GameObject orderImage;

    private void OnEnable()
    {
        orderController.OnOrderUpdated += UpdateOrder;
    }

    private void OnDisable()
    {
        orderController.OnOrderUpdated -= UpdateOrder;
    }

    private void UpdateOrder(List<TurnUnit> turnQueue)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var unit in turnQueue)
        {
            var image = Instantiate(orderImage, transform);
            image.GetComponent<Image>().sprite = unit.stats.characterIcon;
        }
    }
}