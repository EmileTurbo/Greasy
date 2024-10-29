using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingToOrderState : BaseState<CustomerNPC.ECustomerNPCState>
{
    private CustomerNPC customerNPC;

    public WaitingToOrderState(CustomerNPC customerNPC) : base(CustomerNPC.ECustomerNPCState.WaitingToOrder)
    {
        this.customerNPC = customerNPC;
    }

    public override void EnterState()
    {
        Debug.Log("Hello from WaitingToOrder State");
        customerNPC.GetAnimator().SetBool("isWalking", false);
    }

    public override void UpdateState()
    {
       
    }

    public override void ExitState()
    {

    }

    public override CustomerNPC.ECustomerNPCState GetNextState()
    {
        if (customerNPC.GetOrder() != null)
        {
            return CustomerNPC.ECustomerNPCState.Ordered;
        }
        else
        {
            return CustomerNPC.ECustomerNPCState.WaitingToOrder;
        }
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}
