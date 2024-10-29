using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WalkingToWaitingSpotState : BaseState<CustomerNPC.ECustomerNPCState>
{
    private CustomerNPC customerNPC;

    public WalkingToWaitingSpotState(CustomerNPC customerNPC) : base (CustomerNPC.ECustomerNPCState.WalkingToWaitingSpot)
    {
        this.customerNPC = customerNPC;
    }

    public override void EnterState()
    {
        customerNPC.GetAnimator().SetBool("isWalking", true);
    }

    public override void UpdateState()
    {
        if (customerNPC.GetTargetWaitingSpot() != null)
        {
            customerNPC.MoveAlongPath();

            if (customerNPC.ReachedSpot())
            {
                // Align rotation with the waiting spot's rotation
                customerNPC.GetCurrentTransform().rotation = customerNPC.GetTargetWaitingSpot().rotation;
            }
        }
    }

    public override void ExitState()
    {

    }

    public override CustomerNPC.ECustomerNPCState GetNextState()
    {
        if (!customerNPC.ReachedSpot())
        {
            return CustomerNPC.ECustomerNPCState.WalkingToWaitingSpot;
        }
        else if (customerNPC.isReadyToOrder)
        {
            return CustomerNPC.ECustomerNPCState.WaitingToOrder;
        }
        else
        {
            return CustomerNPC.ECustomerNPCState.WaitingInLine;
        }
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }


}
