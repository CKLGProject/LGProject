using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Ȯ���� ���ϴ� ���
    /// </summary>
    public class IsPercentageNode : ActionNode
    {
        private float randTime = 0; 
        private float curTimer = 0;
        private const float maxPercent = 100;

        // n%�� Ȯ���� ���
        [Range(0, maxPercent), Min(0)]
        public float percent = 0;


        [Range(0.2f, 1.0f), Min(0.2f)]
        public float randMin = 0;
        [Range(0.5f, 1.0f), Min(0.5f)]
        public float randMax = 0;
        
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if(randTime <= 0)
            {
                // ó�� 0�̱� ������ ���ο� ������ ���� �ʿ䰡 ����.
                randTime = Random.Range(randMin, randMax);
            }
            curTimer += Time.deltaTime;
            if(curTimer > randTime)
            {
                curTimer = 0;
                return GetPercent() ? State.Success : State.Failure;
            }
            return State.Failure;
        }

        // �� �� õ�� ���ðŶ� ������?
        // �׷� ������ Ÿ�ֿ̹� ������ ��Ȳ���� ���带 ��� �ø���?
        private bool GetPercent()
        {
            float randFloat = Random.Range(0, maxPercent);
            Debug.Log($"random Float Value = {randFloat}");
            if(percent > randFloat)
            {
                return true;
            }
            return false;
        }
    }

}