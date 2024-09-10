using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
    public List<ItemSO> ingredientItemSOList;
    public List<ItemSO> optionalIngredientItemSOList;
    public string recipeName;
    
}
