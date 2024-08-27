using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
    public List<ItemSO> itemSOList;
    public string recipeName;
    public ItemSO outputItemSO;
}
