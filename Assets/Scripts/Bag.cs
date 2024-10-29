using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bag : Item
{
    private bool isClosed;
    private List<Item> itemList;

    private void Awake()
    {
        itemList = new List<Item>();
        isClosed = false;       
    }

    public override void Interact(PlayerInteraction player)
    {
        if (!isClosed) // Bag is open
        {
            if (player.HasItem()) // Player is holding an item
            {
                Item playerItem = player.GetItem(player.GetItemFollowTransform());
                TryAddItemToBag(playerItem);
                player.ClearItem(playerItem);
                playerItem.DestroySelf();
            }
        }
        else // Bag is closed
        {
            if (GetRigidbody() == null)
            {
                Rigidbody rb = this.AddComponent<Rigidbody>();
            }

            base.Interact(player);
        }
    }

    public List<Item> GetBagItemList()
    {
        return itemList;
    }

    // Mange mes fesses, Oli 2024

    private void TryAddItemToBag(Item item)
    {
        if (item != null)
        {
            itemList.Add(item);
        }
    }

    public void CloseBag()
    {
        isClosed = true;
    }

    public bool CheckBagStatus()
    {
        return isClosed;
    }
}
