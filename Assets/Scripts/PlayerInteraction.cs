using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour, IItemParent
{
    public static PlayerInteraction Instance { get; private set; }

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private float InteractDistance = 3f;
    [SerializeField] private GameObject pickUpTextObj;
    [SerializeField] private GameObject takeTextObj;
    [SerializeField] private GameObject emptyTextObj;
    [SerializeField] private float dropForwardForce, dropUpwardForce;
    [SerializeField] private Transform itemHoldPoint;

    RaycastHit hit;
    private Item item;

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
        if (hit.collider != null)
        {
            pickUpTextObj.SetActive(false);
            takeTextObj.SetActive(false);
            emptyTextObj.SetActive(false);
        }

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, InteractDistance))
        {
            if (hit.collider.gameObject.GetComponent<Item>() && !HasItem())
            {
                pickUpTextObj.SetActive(true);
                
            }

            if (hit.collider.gameObject.GetComponent<ItemBox>() && !HasItem())
            {
                if (hit.collider.gameObject.GetComponent<ItemBox>().isEmpty)
                {
                    emptyTextObj.SetActive(true);
                }
                else
                {
                    takeTextObj.SetActive(true);
                }
                
            }

            if (hit.collider.gameObject.GetComponent<Grill>() && HasItem())
            {
                pickUpTextObj.SetActive(true);

            }
        }
    }

    private void Drop()
    {
        Rigidbody rbody = item.GetRigidbody();

        if (item != null && HasItem())
        {
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

            ClearItem();
        }
    }

    public Transform GetItemFollowTransform()
    {
        return itemHoldPoint;
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
