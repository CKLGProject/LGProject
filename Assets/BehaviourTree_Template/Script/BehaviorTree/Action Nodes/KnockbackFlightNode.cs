using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class KnockbackFlightNode : ActionNode
    {
        protected override void OnStart()
        {
            AIAgent.Instance.effectManager.Play(EffectManager.EFFECT.Airborne);
            //AIAgent.Instance.GetStateMachine.
        }

        protected override void OnStop()
        {
            AIAgent.Instance.effectManager.Stop(EffectManager.EFFECT.Airborne);

        }

        protected override State OnUpdate()
        {
            if(AIAgent.Instance.GetStateMachine.isGrounded)
            {
                AIAgent.Instance.GetStateMachine.animator.SetTrigger("WakeUp");
                return State.Success;
            }

            return State.Running;

        }
    }

}