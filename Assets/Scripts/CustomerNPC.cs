using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomerNPC : StateManager<CustomerNPC.ECustomerNPCState>, IInteractable
{
    public float speed = 2f;
    private int currentWaypointIndex; // Track the current waypoint
    private Outline outline;
    private OrderCounter.Order order;
    private OrderCounter orderCounter;
    private Transform targetWaitingSpot;
    private Transform currentTransform;
    private Animator characterAnimator;
    public bool isReadyToOrder;

    public enum ECustomerNPCState
    {
        WalkingToWaitingSpot,
        WaitingInLine,
        WaitingToOrder,
        Ordered,
    }

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();

        States.Add(ECustomerNPCState.WalkingToWaitingSpot, new WalkingToWaitingSpotState(this));
        States.Add(ECustomerNPCState.WaitingInLine, new WaitingInLineState(this));
        States.Add(ECustomerNPCState.WaitingToOrder, new WaitingToOrderState(this));
        States.Add(ECustomerNPCState.Ordered, new OrderedState(this));

        CurrentState = States[ECustomerNPCState.WalkingToWaitingSpot];
        CurrentState.EnterState();
    }

    private void Start()
    {
        orderCounter = FindObjectOfType<OrderCounter>();
        outline = GetComponentInChildren<Outline>();
        DisableOutline();
        currentTransform = GetComponent<Transform>();
        currentWaypointIndex = orderCounter.GetWaitingPointList().Count - 1;
        isReadyToOrder = false;
    }

    public void Interact(PlayerInteraction player)
    {
        switch(CurrentState)
        {
            case WalkingToWaitingSpotState:
                break;

            case WaitingInLineState:
                break;

            case WaitingToOrderState:
                order = orderCounter.GenerateOrder();
                break;

            case OrderedState:
                if (player.HasItem())
                {
                    if (player.GetItem(player.GetItemFollowTransform()).GetComponent<Bag>() != null)
                    {
                        Bag bag = player.GetItem(player.GetItemFollowTransform()).GetComponent<Bag>();
                        player.ClearItem(player.GetItem(player.GetItemFollowTransform()));
                        bag.DestroySelf();
                    }
                }
                break;
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

    public Animator GetAnimator()
    {
        return characterAnimator;
    }

    public void DestroySelf()
    {
        orderCounter.RemoveCustomerNPCFromDictionary(this);
        Destroy(gameObject);
    }

    public bool ReachedSpot()
    {
        // Logic to determine if NPC reached the target spot
        return Vector3.Distance(transform.position, targetWaitingSpot.position) < 0.1f;
    }

    public void MoveAlongPath()
    {
        if (!ReachedSpot())
        {
            currentTransform.position = Vector3.MoveTowards(currentTransform.position, orderCounter.GetWaitingPointList()[currentWaypointIndex].position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, orderCounter.GetWaitingPointList()[currentWaypointIndex].position) < 0.1f)
            {
                // Align rotation with the waiting spot's rotation
                currentTransform.rotation = orderCounter.GetWaitingPointList()[currentWaypointIndex].rotation;             

                currentWaypointIndex--;
            }
        }
    }

    public OrderCounter.Order GetOrder()
    {
        return order;
    }

    public bool CompareOrder(Bag bag)
    {
        if (order != null)
        {
            List<Item> itemsInBag = new List<Item>();
            itemsInBag = bag.GetBagItemList();

            
        }
        else
        {
            return false;
        }
    }

    private bool CompareMainItems(List<Item> itemsInBag)
    {
        foreach (var mainItem in order.mainItemList)
        {
            for (int i = 0; i < itemsInBag.Count; i++)
            {
                if (mainItem.baseRecipeSO.Type == MainItemType.Burger)
                {
                    // Continue plus trad busaufiubciu
                }
            }
        }
    }
}
