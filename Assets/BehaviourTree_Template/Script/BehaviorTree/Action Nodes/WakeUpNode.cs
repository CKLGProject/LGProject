using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class WakeUpNode : ActionNode
    {
        public float wakeUpTime;
        private float curTimer;
        protected override void OnStart()
        {
            Debug.Log("Wake Up");
            curTimer = 0;
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            curTimer += Time.deltaTime;
            // �Ͼ�� ���̸� ���� ����
            if (wakeUpTime < curTimer )
            {
                return State.Success;
            }
            return State.Running;
        }
    }

}