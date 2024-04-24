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

        }

        protected override State OnUpdate()
        {
            count++;
            return State.Failure;
        }


    }
}