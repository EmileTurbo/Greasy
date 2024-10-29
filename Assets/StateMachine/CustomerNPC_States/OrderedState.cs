using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderedState : BaseState<CustomerNPC.ECustomerNPCState>
{
    private CustomerNPC customerNPC;

    public OrderedState(CustomerNPC customerNPC) : base(CustomerNPC.ECustomerNPCState.Ordered)
    {
        this.customerNPC = customerNPC;
    }

    public override void EnterState()
    {
        Debug.Log("Hello from Ordered State");
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
        return CustomerNPC.ECustomerNPCState.Ordered;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}
