using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static OrderCounter;

public class OrderScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orderText;
    private OrderCounter orderCounter;

    private void Start()
    {
        orderCounter = GetComponentInParent<OrderCounter>();
    }

    private void Update()
    {
        if (orderCounter != null)
        {
            Order currentOrder = orderCounter.GetCurrentOrder();

            if (orderCounter.GetCurrentOrder() != null) // There is an order
            {
                DisplayOrder(currentOrder);
            }
            else
            {
                orderText.text = string.Empty;
            }
        }
    }

    private void DisplayOrder(Order order)
    {
        orderText.text = ""; // Clear previous order text

        // Display main items and their ingredients
        foreach (var mainItem in order.mainItemList)
        {
            // Add the main item name
            orderText.text += $"{mainItem.baseRecipeSO.recipeName}: ";
            if (mainItem.optionalIngredientsList.Count > 0) // There are extras
            {
                orderText.text += $"Extra ";
                foreach (var extra in mainItem.optionalIngredientsList)
                {
                    orderText.text += $"{extra.objectName},";
                }
            }

            orderText.text += "\n";
        }

        // Display secondary items
        if (order.secondaryItemList.Count > 0)
        {
            foreach (var secondaryItem in order.secondaryItemList)
            {
                orderText.text += $"{secondaryItem.objectName}\n";
            }
        }
    }
}
