using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    // ���ظ� �Ծ��� �� �۵��ϴ� ���
    public class IsDamagedNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            //agent.isHit = false;
        }

        protected override State OnUpdate()
        {
            return agent.GetStateMachine.isHit ? State.Success : State.Failure;
        }
    }

}