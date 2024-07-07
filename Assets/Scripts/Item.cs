using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemSO;
    private IItemParent itemParent;

    public ItemSO GetItemSO()
    {
        return itemSO;
    }


    public void SetItemParent(IItemParent itemParent)
    {
        if (this.itemParent != null)
        {
            this.itemParent.ClearItem();
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
            itemParent.SetItem(this);
            transform.parent = itemParent.GetItemFollowTransform();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }


    public IItemParent GetItemParent()
    {
        return itemParent;
    }

    public void DestroySelf()
    {
        itemParent.ClearItem();
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

    public Rigidbody GetRigidbody()
    {
        return GetComponent<Rigidbody>();
    }

    public static Item SpawnItem(ItemSO itemSO, IItemParent itemParent)
    {
        Transform itemTransform = Instantiate(itemSO.prefab);
        itemTransform.GetComponent<Item>().SetItemParent(itemParent);
        Item item = itemTransform.GetComponent<Item>();
        itemParent.GetItem().transform.localPosition = Vector3.zero;
        itemParent.GetItem().transform.localRotation = Quaternion.identity;

        Rigidbody rb = itemParent.GetItem().GetRigidbody();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        return item;
    }
}
