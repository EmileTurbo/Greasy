using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;


public class Grill : MonoBehaviour, IInteractable, IItemParent
{
    [SerializeField] private Transform[] grillSpaces;
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private Button button;

    public event Action<Transform> OnItemFryingStarted;
    public event Action<Transform> OnItemFryingStopped;
    public event Action<bool> OnPowerStateChanged;

    private Dictionary<Transform, ItemData> items = new Dictionary<Transform, ItemData>();
    private bool isPowered = false;
    private Outline outline;

    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        DisableOutline();

        button.OnButtonOn += Button_OnButtonOn;
        button.OnButtonOff += Button_OnButtonOff;
    }

    private void Button_OnButtonOff(object sender, EventArgs e)
    {
        isPowered = false;
        OnPowerStateChanged.Invoke(isPowered);
    }

    private void Button_OnButtonOn(object sender, EventArgs e)
    {
        isPowered = true;
        OnPowerStateChanged.Invoke(isPowered);
    }

    private void Update()
    {
        if (isPowered) // powered
        {
            foreach (var slot in grillSpaces)
            {
                if (items.ContainsKey(slot))
                {
                    ItemData itemData = items[slot];
                    FryingRecipeSO recipe = GetFryingRecipeSOWithInput(itemData.Item.GetItemSO());

                    OnItemFryingStarted?.Invoke(slot);

                    if (recipe != null)
                    {
                        // Update the frying timer
                        itemData.FryingTimer += Time.deltaTime;
                        if (itemData.FryingTimer >= recipe.fryingTimerMax)
                        {
                            itemData.Item.DestroySelf();
                            
                            Item cookedItem = Item.SpawnItem(recipe.output, this);
                            cookedItem.transform.parent = slot;
                            cookedItem.transform.localPosition = Vector3.zero;
                            cookedItem.transform.localRotation = Quaternion.identity;

                        }
                        
                    }
                }
            }
            
        }
        else // Not powered
        {
            foreach (var slot in grillSpaces)
            {
                if (items.ContainsKey(slot))
                {
                    OnItemFryingStopped?.Invoke(slot);
                }
            }
        }

    }

    public void Interact(PlayerInteraction player)
    {
        if (player != null)
        {
            if (player.HasItem())
            {
                Item playerItem = player.GetItem(player.GetItemFollowTransform());
                Transform emptySlot = GetItemFollowTransform();

                if (emptySlot != null)
                {
                    if (HasFryingRecipeWithInput(playerItem.GetItemSO()))
                    {
                        playerItem.SetItemParent(this);
                        items[emptySlot] = new ItemData(playerItem);

                        if (isPowered)
                        {
                            OnItemFryingStarted?.Invoke(emptySlot);
                        }

                        Debug.Log("Item added to grill: " + playerItem.GetItemSO().name);
                    }
                }
                else
                {
                    Debug.Log("No empty slots on the grill.");
                }
            }
            else
            {
                Debug.Log("Player does not have an item to grill.");
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
            outline.enabled = true;
        }
    }

    private bool HasFryingRecipeWithInput(ItemSO inputItemSO)
    {
        return GetFryingRecipeSOWithInput(inputItemSO) != null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(ItemSO inputItemSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputItemSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    public Transform GetItemFollowTransform()
    {
        foreach (var slot in grillSpaces)
        {
            if (!IsSlotOccupied(slot))
            {
                return slot;
            }
        }
        return null;

    }

    public void SetItem(Item item, Transform slot)
    {
        items[slot] = new ItemData(item);
    }

    public Item GetItem(Transform slot)
    {
        if (items.ContainsKey(slot))
        {
            return items[slot].Item;
        }
        return null;
    }

    public void ClearItem(Item item)
    {
        Transform slotToRemove = null;
        foreach (var pair in items)
        {
            if (pair.Value.Item == item)
            {
                slotToRemove = pair.Key;
                break;
            }
        }

        if (slotToRemove != null)
        {
            items.Remove(slotToRemove);
            OnItemFryingStopped?.Invoke(slotToRemove);
        }
    }

    public bool HasItem()
    {
        return items.Count >= grillSpaces.Length;
    }

    public bool IsSlotOccupied(Transform slot)
    {
        return items.ContainsKey(slot);
    }

    public bool HasMultipleSlots()
    {
        return true;
    }

    public Transform GetSlotForItem(Item item)
    {
        foreach (var pair in items)
        {
            if (pair.Value.Item == item)
            {
                return pair.Key;
            }
        }
        return null; // Slot not found
    }

    public class ItemData
    {
        public Item Item { get; private set; }
        public float FryingTimer { get; set; }

        public ItemData(Item item)
        {
            Item = item;
            FryingTimer = 0f;
        }
    }


}
