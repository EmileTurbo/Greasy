using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burger : Item
{
    private List<ItemSO> ingredientsList;

    public void Initialize(List<ItemSO> ingredients)
    {
        ingredientsList = new List<ItemSO>(ingredients);
    }

    public List<ItemSO> GetIngredientsList()
    {
        return ingredientsList;
    }
}
