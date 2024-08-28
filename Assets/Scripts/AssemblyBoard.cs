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
    private bool burgerCompleted;

    private void Awake()
    {
        itemSOList = new List<ItemSO>();
        burgerCompleted = false;
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

                itemSOList.Add(itemSO);
                SpawnIngredient(itemSO);
                OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
                {
                    itemSO = itemSO
                });

                Debug.Log("Ingredient added" + itemSO);

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
            itemSOList.Add(itemSO);
            SpawnIngredient(itemSO);
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                itemSO = itemSO
            });
            Debug.Log("Ingredient added" + itemSO + "Burger Completed");

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
            itemSOList.Add(itemSO);
            SpawnIngredient(itemSO);
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                itemSO = itemSO
            });
            Debug.Log("Ingredient added" + itemSO);
            return true;
        }

        
    }

    private void InitializeBurgerGameObject()
    {
        burger = new GameObject("Burger");
        burger.transform.position = burgerPosition.position; 
        burger.transform.rotation = burgerPosition.rotation;
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
            ingredientVisual.transform.localPosition = new Vector3(0, itemSOList.Count * 0.05f, 0);
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

            boxCollider.size = new Vector3(0.35f, ((itemSOList.Count - 2) * 0.05f) + 0.22f, 0.35f);
            boxCollider.center = new Vector3(0, itemSOList.Count * 0.04f, 0);

            // Add Burger component
            Burger completedBurger = burger.AddComponent<Burger>();
            completedBurger.Initialize(GetItemSOList());

            completedBurger.SetItemParent(player);

            // Clear the assembly board visuals
            itemSOList.Clear();
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
