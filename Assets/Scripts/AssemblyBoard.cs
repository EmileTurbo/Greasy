using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyBoard : MonoBehaviour, IInteractable
{

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public ItemSO itemSO;
    }

    [SerializeField] private Transform burgerPosition;
    [SerializeField] private ItemSO bottomBun;
    [SerializeField] private ItemSO topBun;
    [SerializeField] private List<ItemSO> validItemSOList;
    private List<ItemSO> itemSOList;

    private void Awake()
    {
        itemSOList = new List<ItemSO>();
    }

    public bool TryAddIngredient(ItemSO itemSO)
    {
        if (itemSOList.Count == 0)
        {
            if (itemSO != bottomBun)
            {
                // Le premier ingrédient n'est pas un pain 
                return false;
            }
            else if (!validItemSOList.Contains(itemSO))
            {
                return false;
            }
            else
            {
                itemSOList.Add(itemSO);

                OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
                {
                    itemSO = itemSO
                });

                return true;
            }


        }
        else if (!validItemSOList.Contains(itemSO))
        {
            return false;
        }
    }
}
