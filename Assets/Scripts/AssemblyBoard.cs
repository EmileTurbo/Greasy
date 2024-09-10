using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AssemblyBoard : MonoBehaviour, IInteractable
{

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded; 
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public ItemSO itemSO;
    }

    [SerializeField] private Transform burgerPosition;
    [SerializeField] private List<ItemSO> validItemSOList;
    private List<ItemSO> itemSOList;
    private GameObject burger;
    private float burgerHeight;
    private float burgerCenter;
    private bool burgerCompleted;

    private void Awake()
    {
        itemSOList = new List<ItemSO>();
        burgerCompleted = false;
        burgerHeight = 0f;
        burgerCenter = 0f;
    }

    private bool TryAddIngredient(ItemSO itemSO)
    {
        if (itemSOList.Count == 0)
        {
            if (itemSO == validItemSOList[0]) // Bottom Bun
            {
                if (burger == null)
                {
                    InitializeBurgerGameObject();
                }

                AddIngredientToBurger(itemSO);

                return true;
            }
            else
            {
                Debug.Log("Nope");
                return false;
            }
        }
        else if (itemSO == validItemSOList[1])
        {
            AddIngredientToBurger(itemSO);

            burgerCompleted = true;

            return true;
        }
        else if (itemSO == validItemSOList[0] && itemSOList.Count != 0)
        {
            Debug.Log("Nope");
            return false;
        }


        if (!validItemSOList.Contains(itemSO))
        {
            Debug.Log("Nope");
            return false;
        }
        else
        {
            AddIngredientToBurger(itemSO);
            return true;
        }

        
    }

    private void AddIngredientToBurger(ItemSO itemSO)
    {
        itemSOList.Add(itemSO);

        SpawnIngredient(itemSO);

        burgerHeight = burgerHeight + GetIngredientHeight(itemSO);
        burgerCenter = burgerCenter + GetIngredientColliderCenter(itemSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            itemSO = itemSO
        });
        Debug.Log(burgerHeight);
        Debug.Log("Ingredient added" + itemSO);
    }

    private void InitializeBurgerGameObject()
    {
        burger = new GameObject("Burger");
        burger.transform.position = burgerPosition.position; 
        burger.transform.rotation = burgerPosition.rotation;
    }

    private float GetIngredientHeight(ItemSO itemSO)
    {
        BoxCollider itemCollider = itemSO.prefab.GetComponent<BoxCollider>();
        if (itemCollider != null)
        {
            return itemCollider.size.y;
        }
        else
        {
            return 0;
        }
    }

    private float GetIngredientColliderCenter(ItemSO itemSO)
    {
        BoxCollider itemCollider = itemSO.prefab.GetComponent<BoxCollider>();
        if (itemCollider != null)
        {
            return itemCollider.center.y;
        }
        else
        {
            return 0;
        }
    }

    private void SpawnIngredient(ItemSO itemSO)
    {
        if (itemSOList.Count == 1)
        {
            Transform ingredientVisual = Instantiate(itemSO.prefab_Visual);
            ingredientVisual.SetParent(burger.transform, false);
            ingredientVisual.transform.localPosition = Vector3.zero;   
            ingredientVisual.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Transform ingredientVisual = Instantiate(itemSO.prefab_Visual);
            ingredientVisual.SetParent(burger.transform, false);
            ingredientVisual.transform.localPosition = new Vector3(0, burgerHeight, 0);
            ingredientVisual.transform.localRotation = Quaternion.identity;


        }
    }

    private void PickupCompletedBurger(PlayerInteraction player)
    {
        if (burger != null)
        {
            // Add a collider and rigidbody to make the burger interactable
            BoxCollider boxCollider = burger.AddComponent<BoxCollider>();
            Rigidbody rb = burger.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            boxCollider.size = new Vector3(0.32f, burgerHeight, 0.32f);
            boxCollider.center = new Vector3(0, (burgerHeight / 2f), 0);

            // Add Burger component
            Burger completedBurger = burger.AddComponent<Burger>();
            completedBurger.Initialize(GetItemSOList());

            completedBurger.SetItemParent(player);

            // Clear the assembly board visuals
            itemSOList.Clear();
            burgerHeight = 0f;
            burgerCompleted = false;
            burger = null;
        }
    }

    public List<ItemSO> GetItemSOList()
    {
        return itemSOList;
    }

    public void Interact(PlayerInteraction player)
    {
        if (player != null)
        {
            if (player.HasItem() && !burgerCompleted)
            {
                Item playerItem = player.GetItem(player.GetItemFollowTransform());
                ItemSO playerItemSO = playerItem.GetItemSO();
                if (TryAddIngredient(playerItemSO))
                {
                    player.ClearItem(playerItem);
                    playerItem.DestroySelf();
                }
            }
            else if (!player.HasItem() && burgerCompleted)
            {
                PickupCompletedBurger(player);
            }
        }
    }

    public void DisableOutline()
    {

    }

    public void EnableOutline()
    {

    }


}
