using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour, IItemParent
{
    public static PlayerInteraction Instance { get; private set; }

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float InteractDistance = 3f;
    [SerializeField] private float dropForwardForce, dropUpwardForce;
    [SerializeField] private Transform itemHoldPoint;

    RaycastHit hit;
    private Item item;
    IInteractable currentInteractable;

    private void Start()
    {
        gameInput.OnInteractAction += InteractInput_OnInteractAction;
        gameInput.OnDropAction += DropInput_OnDropAction;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Y'a plus que un joueur esti !");
        }

        Instance = this;
    }

    private void DropInput_OnDropAction(object sender, System.EventArgs e)
    {
        Drop();
    }

    private void InteractInput_OnInteractAction(object sender, System.EventArgs e)
    {       
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact(this);
            }
        }
    }

    private void Update()
    {      
        HandleDetection();
    }

    private void HandleDetection()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, InteractDistance))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (currentInteractable != null)
                {
                    if (interactObj != currentInteractable)
                    {
                        currentInteractable.DisableOutline();
                    }
                }

                if (HasItem())
                {
                    if (hit.collider.gameObject.GetComponent<Grill>() || hit.collider.gameObject.GetComponent<Button>()) 
                    {
                        SetNewCurrentInteractable(interactObj);
                    }
                }
                else
                {
                    SetNewCurrentInteractable(interactObj);
                }

                


            }
            else // if not interactable
            {
                DisableCurrentInteractable();
            }
        }
        else // if nothing in reach
        {
            DisableCurrentInteractable();
        }
    }

    private void SetNewCurrentInteractable(IInteractable newInteractObj)
    {
        currentInteractable = newInteractObj;
        currentInteractable.EnableOutline();
    }

    private void DisableCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }

    private void Drop()
    {
        if (item != null && HasItem())
        {
            Rigidbody rbody = item.GetRigidbody();
            item.SetItemParent(null);

            if (rbody != null)
            {
                rbody.isKinematic = false;
                rbody.velocity = GetComponent<CharacterController>().velocity;
                rbody.AddForce(playerCameraTransform.forward * dropForwardForce, ForceMode.Impulse);
                rbody.AddForce(playerCameraTransform.up * dropUpwardForce, ForceMode.Impulse);
                // Add Random rotation
                float random = Random.Range(-1f, 1f);
                rbody.AddTorque(new Vector3(random, random, random) * 10);
            }

            ClearItem(item);
        }
    }

    public Transform GetItemFollowTransform()
    {
        return itemHoldPoint;
    }

    public void SetItem(Item item, Transform slot)
    {
        this.item = item;
    }

    public Item GetItem(Transform slot)
    {
        return item;
    }

    public void ClearItem(Item itemToRemove)
    {
        if (item == itemToRemove)
        {
            item = null;
        }
    }

    public bool HasItem()
    {
        return item != null;
    }

    public bool IsSlotOccupied(Transform slot)
    {
        return false;
    }

    public bool HasMultipleSlots()
    {
        return false;
    }

    public Transform GetSlotForItem(Item item)
    {
        return itemHoldPoint;
    }
}
