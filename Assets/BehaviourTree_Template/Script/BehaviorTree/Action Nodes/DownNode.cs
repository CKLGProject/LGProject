using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DownNode : ActionNode
    {
        public float downTimer;
        private float curTimer;
        protected override void OnStart()
        {
            curTimer = 0;
            Debug.Log("Down");
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (agent.GetStateMachine.isGrounded)
            {

                curTimer += Time.deltaTime;
                // ������ �����ִ� �ð��� ������.
                if (downTimer < curTimer)
                {
                    // �����ִ� �ð��� ������ Idle ���°� �Ǹ鼭 �Ͼ.
                    return State.Success;
                }
            }
            else
            {
                // ���߿� �� ���¿����� �ƹ����� ����
                curTimer = 0;
            }

            return State.Running;
        }
    }

}