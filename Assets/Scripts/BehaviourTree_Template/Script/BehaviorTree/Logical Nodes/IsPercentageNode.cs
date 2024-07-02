using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// 확률을 구하는 노드
    /// </summary>
    public class IsPercentageNode : ActionNode
    {
        //public AIAgent Agent;
        //[Space(10f)]
        private float randTime = 0; 
        private float curTimer = 0;
        private const float maxPercent = 100;

        // n%의 확률로 통과
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
                // 처음 0이기 때문에 새로운 세팅을 해줄 필요가 있음.
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

        // 한 몇 천번 들어올거란 말이지?
        // 그럼 랜덤한 타이밍에 랜덤한 상황에서 가드를 어떻게 올릴까?
        private bool GetPercent()
        {
            float randFloat = Random.Range(0, maxPercent);
            if(percent > randFloat)
            {
                return true;
            }
            return false;
        }
    }

}