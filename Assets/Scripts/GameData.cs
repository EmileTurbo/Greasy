using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField] private List<RecipeSO> availableRecipeSOList;
    [SerializeField] private List<ItemSO> availableSecondaryOrderItemList;
    private static int difficultyIndex = 1;
    public static GameData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Y'a plus que un GameData esti !");
        }

        Instance = this;
    }

    public int GetDiffIndex()
    {
        return difficultyIndex;
    }

    public void IncreaseDiffIndex()
    {
        difficultyIndex++;
    }

    public void AddRecipeToAvailableRecipeSOList(RecipeSO recipeSO)
    {
        availableRecipeSOList.Add(recipeSO);
    }

    public void AddItemToAvailableSecondaryItemList(ItemSO itemSO)
    {
        availableSecondaryOrderItemList.Add(itemSO);
    }

    public List<RecipeSO> GetAvailableRecipeSOList()
    {
        return availableRecipeSOList;
    }

    public List<ItemSO> GetAvailableSecondaryItemList()
    {
        return availableSecondaryOrderItemList;
    }
}
