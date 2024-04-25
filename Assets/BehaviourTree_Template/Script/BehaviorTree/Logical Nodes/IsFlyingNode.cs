using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class IsFlyingNode : ActionNode
    {

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        // ���߿� �ִ��� üũ�ϴ� ���
        // ��� ���� ���ԳĿ� ���� ���� ������ ��尡 �޶���.
        protected override State OnUpdate()
        {
            if(agent.GetStateMachine.isGrounded)
            {
                return State.Success;
            }

            return State.Failure;
        }
    }

}