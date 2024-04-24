using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class RangeNode : ActionNode
    {
        // �Ÿ��� ����ϴ� ���
        public float range;
        [Space(10f)]
        public bool Reverse;

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // �÷��̾���� �Ÿ��� ���Ͽ� ��ȯ.
            // �÷��̾��� ��� �ϳ��ۿ� ���� ������ agent���� ������ ������ұ�?
            // �ϴ� ������ ���ִٰ� ���� ���� Ŭ������ ���� �ϴ� ������ ����.
            float distance = Vector3.Distance(agent.transform.position, agent.player.position);
            
            // �÷��̾���� �Ÿ��� ������� �����̵� �� �� �ֱ� ������ ������ ���� ����.
            return distance <= range ? State.Success : State.Failure;
        }
    }

}