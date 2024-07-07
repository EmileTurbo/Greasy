using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grill : MonoBehaviour, IInteractable, IItemParent
{
    //[SerializeField] private Transform[] grillSpaceArray;
    [SerializeField] private Transform grillSpace;
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    //private List<Item> items = new List<Item>();
    private Item item;
    //private Dictionary<Item, float> fryingTimers = new Dictionary<Item, float>();
    //private Dictionary<Item, State> state = new Dictionary<Item, State>();
    private float fryingTimer;
    private State state;
    private FryingRecipeSO fryingRecipeSO;

    private enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;

            case State.Frying:
                fryingTimer += Time.deltaTime;
                if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                {
                    // Fried
                    GetItem().DestroySelf();

                    Item.SpawnItem(fryingRecipeSO.output, this);

                    state = State.Fried;
                }
                break;

            case State.Fried:
                break;

            case State.Burned:
                break;
            
        }
    }

    public void Interact(PlayerInteraction player)
    {
        if (player != null)
        {
            if (!HasItem()) // There is no item here
            {
                if (player.HasItem()) // Player has an item
                {
                    if (HasRecipeWithInput(player.GetItem().GetItemSO())) // Player has an item that can be cooked
                    {
                        player.GetItem().SetItemParent(this);

                        fryingRecipeSO = GetFryingRecipeSOWithInput(GetItem().GetItemSO());

                        state = State.Frying;
                        fryingTimer = 0f;
                    }
                }
                else // Player has no item
                {

                }
            }
            else // There is no slots available here
            {
                if (player.HasItem()) // Player has an item
                {

                }
                else // Player has no item
                {
                    
                }
            }
        }
    }

    private bool HasRecipeWithInput(ItemSO inputItemSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputItemSO);
        return fryingRecipeSO != null;
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

    private ItemSO GetOutputForInput(ItemSO inputItemSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputItemSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    public Transform GetItemFollowTransform()
    {
        //for (int i = 0; i < grillSpaceArray.Length; i++)
        //{
        //    if (grillSpaceArray[i].childCount == 0)
        //    {
        //        return grillSpaceArray[i];
        //    }
        //}
        //return null;

        return grillSpace;
        
    }

    public void SetItem(Item item)
    {
        this.item = item;
    }

    public Item GetItem()
    {
        return item;
    }

    public void ClearItem()
    {
        item = null;
    }

    public bool HasItem()
    {
        return item != null;
    }

}
