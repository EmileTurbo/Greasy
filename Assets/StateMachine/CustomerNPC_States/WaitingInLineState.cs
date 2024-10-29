using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingInLineState : BaseState<CustomerNPC.ECustomerNPCState>
{
    private CustomerNPC customerNPC;

    public WaitingInLineState(CustomerNPC customerNPC) : base(CustomerNPC.ECustomerNPCState.WaitingInLine)
    {
        this.customerNPC = customerNPC;
    }

    public override void EnterState()
    {
        Debug.Log("Hello from WaitingInLine State");
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
        if (customerNPC.isReadyToOrder)
        {
            return CustomerNPC.ECustomerNPCState.WaitingToOrder;
        }
        else
        {
            return customerNPC.ReachedSpot() ? CustomerNPC.ECustomerNPCState.WaitingInLine : CustomerNPC.ECustomerNPCState.WalkingToWaitingSpot;
        }
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}
