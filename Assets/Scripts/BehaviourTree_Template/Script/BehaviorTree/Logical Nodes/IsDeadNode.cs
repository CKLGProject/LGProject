using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class IsDeadNode : ActionNode
    {
        LGProject.PlayerState.PlayerStateMachine _stateMachine;
        protected override void OnStart()
        {
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            try
            {
                if (_stateMachine.IsDead)
                {
                    return State.Running;
                }
            }
            catch
            {

            }
            return State.Success;

        }
    }
}