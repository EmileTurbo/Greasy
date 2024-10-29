using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderCounter : MonoBehaviour
{
    [SerializeField] private List<Transform> waitingLinePointsList;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private Transform customerNPCPrefab;
    [SerializeField] private float spawnTimer = 15f;
    private Dictionary<Transform, CustomerNPC> customers = new Dictionary<Transform, CustomerNPC>();
    private float timer = 0f;
    private GameData gameData;
    private Order currentOrder;

    private void Start()
    {
        gameData = GameData.Instance;
    }

    private void Update()
    {
        if (customers.Count != waitingLinePointsList.Count)
        {
            UpdateCustomerNPCTargetWaitingSpot();
        }
        SpawnCustomerNPC(); // Temporaire
        HandleCustomerAtCounter();
    }

    private Transform GetWaitingLinePoint()
    {
        if (customers.Count >= waitingLinePointsList.Count)
        {
            return null;
        }

        foreach (var spot in waitingLinePointsList)
        {
            if (!customers.ContainsKey(spot))
            {
                return spot;
            }
        }

        return null;
    }

    private void SpawnCustomerNPC()
    {
        if (customers.Count < waitingLinePointsList.Count)
        {
            timer += Time.deltaTime;

            if (timer >= spawnTimer)
            {
                Transform waitingSpot = GetWaitingLinePoint();

                Transform npcTransform = Instantiate(customerNPCPrefab, spawnLocation.position, Quaternion.identity);
                CustomerNPC customerNPC = npcTransform.GetComponent<CustomerNPC>();

                customerNPC.SetTargetWaitingSpot(waitingSpot);
                customers[waitingSpot] = customerNPC;

                timer = 0f;
            }       
        }
        else
        {
            timer = 0f;
        }
    }

    private void UpdateCustomerNPCTargetWaitingSpot()
    {
        List<Transform> availableSpots = new List<Transform>();

        // Find all available spots in order
        foreach (Transform spot in waitingLinePointsList)
        {
            if (!customers.ContainsKey(spot))
            {
                availableSpots.Add(spot);
            }
        }

        // Reassign NPCs to the nearest available spots
        foreach (Transform spot in waitingLinePointsList)
        {
            if (customers.ContainsKey(spot))
            {
                CustomerNPC customerNPC = customers[spot];
                Transform newSpot = availableSpots.Find(s => waitingLinePointsList.IndexOf(s) < waitingLinePointsList.IndexOf(spot));

                if (newSpot != null)
                {
                    customerNPC.SetTargetWaitingSpot(newSpot);
                    customers.Remove(spot);
                    customers[newSpot] = customerNPC;
                    availableSpots.Remove(newSpot);
                }
            }
        }
    }

    private void HandleCustomerAtCounter()
    {
        foreach (CustomerNPC customerNPC in customers.Values)
        {
            if (customerNPC.GetTargetWaitingSpot() == waitingLinePointsList[0])
            {
                if (customerNPC.GetCurrentTransform() != null)
                {
                    if (Vector3.Distance(customerNPC.GetCurrentTransform().position, waitingLinePointsList[0].position) < 0.1f)
                    {
                        customerNPC.isReadyToOrder = true;
                    }
                    else
                    {
                        customerNPC.isReadyToOrder = false;
                    }
                }
            }
            else
            {
                customerNPC.isReadyToOrder = false;
            }
        }
    }

    public void RemoveCustomerNPCFromDictionary(CustomerNPC customerNPC)
    {
        Transform targetWaitingSpot = customerNPC.GetTargetWaitingSpot();
        customers.Remove(targetWaitingSpot, out customerNPC);     
    } 

    public List<Transform> GetWaitingPointList()
    {
        return waitingLinePointsList;
    }

    public Order GetCurrentOrder()
    {
        return currentOrder;
    }

    private MainItem GenerateMainItem()
    {
        // Retrieve available recipes
        List<RecipeSO> availableRecipes = gameData.GetAvailableRecipeSOList();

        RecipeSO selectedRecipe = availableRecipes[Random.Range(0, availableRecipes.Count)];
        List<ItemSO> ingredients = new List<ItemSO>(selectedRecipe.ingredientItemSOList);
        List<ItemSO> optionalIngredients = new List<ItemSO>();

        // Randomize optional ingredients
        foreach (var optionalIngredient in selectedRecipe.optionalIngredientItemSOList)
        {
            if (UnityEngine.Random.Range(0, 2) == 1) // 50% chance of adding each optional ingredient
            {
                optionalIngredients.Add(optionalIngredient);
            }
        }

        MainItem newMainItem = new MainItem(selectedRecipe, ingredients, optionalIngredients);
        return newMainItem;
    }

    public Order GenerateOrder()
    {
        List<MainItem> mainItems = new List<MainItem>();
        List<ItemSO> secondaryItems = new List<ItemSO>();
        // Retrieve available recipes
        List<ItemSO> availableSecondaryItems = gameData.GetAvailableSecondaryItemList();

        // Determine number of main items based on difficulty
        int mainItemCount = 1; // Minimum of 1 main item
        if (Random.value <= 0.25f) // 25% chance to add one main item
        {
            mainItemCount++;
        }

        for (int i = 0; i < gameData.GetDiffIndex();  i++)
        {
            if (Random.value < (0.1f * GameData.Instance.GetDiffIndex())) // Higher difficulty, more chance for additional items
            {
                mainItemCount++;
            }
        }

        // Create main items
        for (int i = 0; i < mainItemCount; i++)
        {
            MainItem newMainItem = GenerateMainItem();
            mainItems.Add(newMainItem);
        }

        // Determine number of secondary items based on difficulty
        int secondaryItemCount = 0; // Minimum of 1 main item
        if (Random.value <= 0.50f) // 50% chance to add one main item
        {
            secondaryItemCount++;
        }

        for (int i = 0; i < gameData.GetDiffIndex(); i++)
        {
            if (Random.value < (0.1f * GameData.Instance.GetDiffIndex())) // Higher difficulty, more chance for additional items
            {
                secondaryItemCount++;
            }
        }

        if (secondaryItemCount != 0)
        {
            for (int i = 0; i < secondaryItemCount; i++)
            {
                if (availableSecondaryItems.Count > 0)
                {
                    ItemSO selectedSecondaryItem = availableSecondaryItems[Random.Range(0, availableSecondaryItems.Count)];
                    secondaryItems.Add(selectedSecondaryItem);
                }
            }
        }

        Order newOrder = new Order(mainItems, secondaryItems);
        currentOrder = newOrder; // Set the current order
        return newOrder;
    }

    public class Order
    {
        public List<MainItem> mainItemList { get; private set; }
        public List<ItemSO> secondaryItemList { get; private set; }

        public Order(List<MainItem> mainItems, List<ItemSO> secondaryItems)
        {
            mainItemList = mainItems;
            secondaryItemList = secondaryItems;
        }
    }

    public class MainItem
    {
        public RecipeSO baseRecipeSO { get; private set; } // e.g., a basic burger or cheeseburger... or Jeremy's throbbing ballsack :)
        public List<ItemSO> ingredientsList { get; private set; } // List of mandatory ingredients (bread, meat, etc.)
        public List<ItemSO> optionalIngredientsList { get; private set; } // Optional ingredients (lettuce, tomato, etc.)

        public MainItem(RecipeSO baseRecipe, List<ItemSO> ingredients, List<ItemSO> optionalIngredients)
        {
            this.baseRecipeSO = baseRecipe;
            this.ingredientsList = ingredients;
            this.optionalIngredientsList = optionalIngredients;
        }
    }
}
