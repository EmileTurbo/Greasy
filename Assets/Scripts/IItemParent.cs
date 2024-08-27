using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemParent
{
    public Transform GetItemFollowTransform();
    public void SetItem(Item item, Transform slot);
    public Item GetItem(Transform slot);
    public void ClearItem(Item item);
    public bool HasItem();
    public bool IsSlotOccupied(Transform slot);
    public bool HasMultipleSlots();
    public Transform GetSlotForItem(Item item);  // Get the slot for a specific item
}
