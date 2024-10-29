using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private List<Transform> itemVisualList;
    private GameObject burger;
    private float burgerHeight;
    private float burgerCenter;
    private bool burgerCompleted;
    private Outline outline;
    private Transform currentTopItem;
    

    private void Awake()
    {
        itemSOList = new List<ItemSO>();
        itemVisualList = new List<Transform>();
        burgerCompleted = false;
        burgerHeight = 0f;
        burgerCenter = 0f;
    }

    private void Start()
    {
        outline = GetComponent<Outline>();
        DisableOutline();
    }

    private bool TryAddIngredient(ItemSO itemSO)
    {
        if (itemSOList.Count == 0) // Si il n'y a pas d'ingredient sur la table
        {
            if (itemSO == validItemSOList[0]) // Si c'est un Bottom Bun
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
        else if (itemSO == validItemSOList[1]) // Si un top bun est placé
        {
            AddIngredientToBurger(itemSO);

            burgerCompleted = true;

            return true;
        }
        else if (itemSO == validItemSOList[0] && itemSOList.Count != 0) // Si un bottom bun veux etre placé mais il y a deja des ingredients
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
            itemVisualList.Add(ingredientVisual);
            currentTopItem = ingredientVisual;
        }
        else
        {
            Transform ingredientVisual = Instantiate(itemSO.prefab_Visual);
            ingredientVisual.SetParent(burger.transform, false);
            ingredientVisual.transform.localPosition = new Vector3(0, burgerHeight, 0);
            ingredientVisual.transform.localRotation = Quaternion.identity;
            itemVisualList.Add(ingredientVisual);
            currentTopItem = ingredientVisual;
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
            // Add Outline component 
            Outline burgerOutline = burger.AddComponent<Outline>();
            burgerOutline.enabled = false;

            completedBurger.Initialize(GetItemSOList());

            completedBurger.SetItemParent(player, player.GetItemFollowTransform());

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
            if (player.HasItem() && !burgerCompleted) // Player is holding an item and the burger is not completed
            {
                Item playerItem = player.GetItem(player.GetItemFollowTransform());
                ItemSO playerItemSO = playerItem.GetItemSO();
                if (TryAddIngredient(playerItemSO))
                {
                    player.ClearItem(playerItem);
                    playerItem.DestroySelf();
                }
            }
            else if (!player.HasItem() && burgerCompleted) // Player is NOT holding an item and the burger is not completed
            {
                DisableOutline();
                PickupCompletedBurger(player);
            }
            else if (!player.HasItem() && !burgerCompleted)// Player is NOT holding an item and the burger is NOT not completed (pick up last placed ingredient)
            {
                if (itemSOList.Count > 0) // There is at least one ingredient on the table
                {
                    int nbIngredient = itemSOList.Count;
                    ItemSO itemSOToRemove = itemSOList[nbIngredient - 1];
                    Transform ingredientToPickUP = Instantiate(itemSOList[nbIngredient - 1].prefab);
                    if (ingredientToPickUP.TryGetComponent<Item>(out Item item))
                    {
                        BoxCollider collider = item.GetCollider();
                        item.SetItemParent(player, player.GetItemFollowTransform());
                        if (collider != null)
                        {
                            collider.enabled = false;
                        }
                    }

                    // Remove the item from the lists
                    itemSOList.RemoveAt(nbIngredient - 1);

                    // Remove the corresponding visual from the burger
                    Transform visualToRemove = itemVisualList[nbIngredient - 1];
                    Destroy(visualToRemove.gameObject);
                    itemVisualList.RemoveAt(nbIngredient - 1);

                    // Update burgerHeight since an item was removed
                    burgerHeight -= GetIngredientHeight(itemSOToRemove);

                    if (itemVisualList.Count == 0)
                    {
                        currentTopItem = null;
                    }
                    else
                    {
                        currentTopItem = itemVisualList[itemVisualList.Count - 1];
                    }
                }
            }
        }
    }

    public void DisableOutline()
    {
        if (itemSOList.Count == 0) // No ingredients on the table, disable the table's outline
        {
            if (outline != null)
            {
                outline.enabled = false;  // Disable the assembly board's outline
            }
        }
        else if (burgerCompleted)
        {
            foreach (var item in itemVisualList)
            {
                if (item != null)
                {
                    Outline itemOutline = item.GetComponent<Outline>();
                    if (itemOutline != null)
                    {
                        itemOutline.enabled = false;
                    }
                }
            }
        }
        else // Ingredients are on the table, disable the top item's outline
        {
            if (currentTopItem != null) // Disable the outline for the topmost item
            {
                Outline itemOutline = currentTopItem.GetComponent<Outline>();
                if (itemOutline != null)
                {
                    itemOutline.enabled = false;
                }
            }
        }
    }

    public void EnableOutline()
    {
        if (itemSOList.Count == 0) // No ingredients on the table, outline the table itself
        {
            if (outline != null)
            {
                outline.enabled = true;  // Enable the assembly board's outline
            }
        }
        else if (burgerCompleted) // Top bun is placed
        {
            if (outline != null)
            {
                outline.enabled = false;  // Disable the assembly board's outline
            }

            foreach (var item in itemVisualList)
            {
                if (item != null)
                {
                    Outline itemOutline = item.GetComponent<Outline>();
                    if(itemOutline != null)
                    {
                        itemOutline.enabled = true;
                    }
                }
            }
        }
        else // Ingredients are on the table, outline the top item
        {
            if (outline != null)
            {
                outline.enabled = false;  // Disable the assembly board's outline
            }

            // Disable the outline of the previous top item (if any)
            if (itemVisualList.Count > 1)
            {
                Transform previousTopItem = itemVisualList[itemVisualList.Count - 2];
                if (previousTopItem != null)
                {
                    Outline previousOutline = previousTopItem.GetComponent<Outline>();
                    if (previousOutline != null)
                    {
                        previousOutline.enabled = false;  // Disable the outline for the previous item
                    }
                }
            }

            // Enable the outline for the current top item
            if (currentTopItem != null)
            {
                Outline itemOutline = currentTopItem.GetComponent<Outline>();
                if (itemOutline != null)
                {
                    itemOutline.enabled = true;  // Enable the outline for the topmost item
                }
            }
        }
    }
}
