using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackNode : ActionNode
    {
        // ������ �����ϴ� ��� 
        // �ִϸ��̼��� ������.
        int count = 0;
        protected override void OnStart()
        {
            // �ִϸ��̼� ����
        }

        protected override void OnStop()
        {
            agent.GetStateMachine.isNormalAttack = false;

        }

        protected override State OnUpdate()
        {
            // ������ ������ ���� ������ ���⼭ �ؾ��ϴµ�...
            return State.Success;
        }


    }
}