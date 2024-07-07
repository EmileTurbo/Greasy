using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private int stockMax = 30;
    [SerializeField] private GameObject itemBoxVisual100;
    [SerializeField] private GameObject itemBoxVisual50;
    [SerializeField] private GameObject itemBoxVisual10;
    private int stock;
    public bool isEmpty;
    

    private void Start()
    {
        stock = stockMax;
        isEmpty = false;

    }

    private void Update()
    {
        float stockPercentage = (stock * 100) / stockMax;

        if (stockPercentage > 50)
        {
            itemBoxVisual100.SetActive(true);
            itemBoxVisual50.SetActive(false);
            itemBoxVisual10.SetActive(false);
        }
        else if (stockPercentage > 10 && stockPercentage <= 50)
        {
            itemBoxVisual100.SetActive(false);
            itemBoxVisual50.SetActive(true);
            itemBoxVisual10.SetActive(false);
        }
        else if (stockPercentage > 0 && stockPercentage <=10)
        {
            itemBoxVisual100.SetActive(false);
            itemBoxVisual50.SetActive(false);
            itemBoxVisual10.SetActive(true);
        }
        else if (stock <= 0)
        {
            itemBoxVisual100.SetActive(false);
            itemBoxVisual50.SetActive(false);
            itemBoxVisual10.SetActive(false);
        }

        if (stock > 0)
        {
            isEmpty = false;
        }
        else if (stock <= 0)
        {
            isEmpty = true;
        }
        
    }

    public void Interact(PlayerInteraction player)
    {
        if (player != null)
        {
            if (!player.HasItem() && stock >= 1)
            {
                if (itemSO.prefab != null)
                {
                    stock--;
                    Item.SpawnItem(itemSO, player);
                }
            }
        }
    }

}
