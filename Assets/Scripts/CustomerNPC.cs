using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomerNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private float speed = 1.5f;
    private RecipeSO waitingRecipeSO;
    private List<ItemSO> selectedOptionalIngredients;
    private Outline outline;
    private OrderCounter orderCounter;
    private Transform targetWaitingSpot;
    private Transform currentTransform;
    public bool isReadyToOrder;
    

    private void Start()
    {
        orderCounter = FindObjectOfType<OrderCounter>(); // J'espere que sa marche hihi

        outline = GetComponentInChildren<Outline>();
        DisableOutline();

        currentTransform = GetComponent<Transform>(); //idk if its necesaire lol

        isReadyToOrder = false;
    }

    private void Update()
    {
        if (targetWaitingSpot != null)
        {
            currentTransform.position = Vector3.MoveTowards(currentTransform.position, targetWaitingSpot.position, speed * Time.deltaTime);
        }
    }

    public void Interact(PlayerInteraction player)
    {
        if (isReadyToOrder)
        {
            GenerateRecipe();
        }
        else
        {
            Debug.Log("Fuck you");
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

    public Transform GetTargetWaitingSpot() { return targetWaitingSpot; }

    public void SetTargetWaitingSpot(Transform targetWaitingSpot)
    {
        this.targetWaitingSpot = targetWaitingSpot;
    }

    public Transform GetCurrentTransform() { return currentTransform; }

    public void DestroySelf()
    {
        orderCounter.RemoveCustomerNPCFromDictionary(this);
        Destroy(gameObject);
    }

    private void GenerateRecipe()
    {
        // Generate the main recipe
        waitingRecipeSO = orderCounter.recipes[UnityEngine.Random.Range(0, orderCounter.recipes.Count)];
        // Randomly select optional ingredients
        selectedOptionalIngredients = new List<ItemSO>();

        foreach (var optionalIngredient in waitingRecipeSO.optionalIngredientItemSOList)
        {
            if (UnityEngine.Random.Range(0, 2) == 1) // 50% chance of adding each optional ingredient
            {
                selectedOptionalIngredients.Add(optionalIngredient);
            }
        }

        Debug.Log("I want a " + waitingRecipeSO.recipeName +
          (selectedOptionalIngredients.Count > 0 ? " with extra " + string.Join(", ", selectedOptionalIngredients.Select(i => i.objectName)) : ""));

    }
}
