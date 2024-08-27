using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemSO;

    private Outline outline;
    private IItemParent itemParent;

    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        DisableOutline();
    }

    public ItemSO GetItemSO()
    {
        return itemSO;
    }

    public void SetItemParent(IItemParent itemParent)
    {
        bool isGettingReplaced;

        if (this.itemParent != null && this.itemParent.HasMultipleSlots())
        {
            isGettingReplaced = true;
        }
        else
        {
            isGettingReplaced = false;
        }

        if (this.itemParent != null)
        {
            this.itemParent.ClearItem(this);
        }

        this.itemParent = itemParent;

        if (itemParent == null)
        {
            transform.parent = null;
            return;
        }

        if (itemParent.HasItem())
        {
            Debug.LogError("The item parent already has an item");
        }
        else
        {
            if (isGettingReplaced)
            {
                Transform slot = itemParent.GetSlotForItem(this);
                if (slot != null)
                {
                    itemParent.SetItem(this, slot);
                    transform.parent = slot;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
                else
                {
                    Debug.LogError("No available slot found for the item.");
                }
            }
            else
            {
                Transform slot = itemParent.GetItemFollowTransform();
                if (slot != null)
                {
                    itemParent.SetItem(this, slot);
                    transform.parent = slot;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
                else
                {
                    Debug.LogError("No available slot found for the item.");
                }
            }
            
        }
    }


    public IItemParent GetItemParent()
    {
        return itemParent;
    }

    public void DestroySelf()
    {
        itemParent.ClearItem(this);
        Destroy(gameObject);
    }

    public void Interact(PlayerInteraction player)
    {
        Rigidbody rb = GetRigidbody();

        if (!player.HasItem())
        {
            SetItemParent(player);
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
        
    }

    public void EnableOutline()
    {
        if (outline != null)
        {
            outline.enabled |= true;
        }
    }

    public Rigidbody GetRigidbody()
    {
        return GetComponent<Rigidbody>();
    }

    public static Item SpawnItem(ItemSO itemSO, IItemParent itemParent)
    {
        Transform itemTransform = Instantiate(itemSO.prefab);
        itemTransform.GetComponent<Item>().SetItemParent(itemParent);
        Item item = itemTransform.GetComponent<Item>();
        Transform slot = itemParent.GetSlotForItem(item);
        itemParent.GetItem(slot).transform.localPosition = Vector3.zero;
        itemParent.GetItem(slot).transform.localRotation = Quaternion.identity;

        Rigidbody rb = itemParent.GetItem(slot).GetRigidbody();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        return item;
    }
}

