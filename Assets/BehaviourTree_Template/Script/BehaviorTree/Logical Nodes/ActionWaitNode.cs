using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class ActionWaitNode : ActionNode
    {
        //public AIAgent Agent;
        [Space(10f)]
        private float curTimer = 0;
        public float judgmentTime = 0;
        bool damageInCount = false;
        protected override void OnStart()
        {
        
        }

        protected override void OnStop()
        {
        
        }

        protected override State OnUpdate()
        {
            // 시간 내에 공격 판정 범위 내에 들어오면 OK
            curTimer += Time.deltaTime;
            
            if(judgmentTime > curTimer)
            {
                // Boolean type Method Running 중 
                return State.Running;
            }
            return State.Failure;
        }

        private bool AttackRange()
        {
            // 판정만 계산함.
            Vector3 right = Vector3.right * (AIAgent.Instance.directionX == true ? 1 : -1);
            Vector3 center = AIAgent.Instance.transform.position + right;
            // 생각보다 판정이 후하진 않게 하기
            // hit box의 크기를 따라감.
            Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f);
            System.Tuple<Transform, float> temp = null;

            foreach (var t in targets)
            {
                float distance = Vector3.Distance(center, t.transform.position);
                if (temp == null || (temp.Item2 >= distance && t.transform != AIAgent.Instance.transform))
                {
                    temp = System.Tuple.Create(t.GetComponent<Transform>(), distance);
                }
            }

            if (temp == null)
            {
                damageInCount = false;
            }
            else
            {
                try
                {
                    Vector3 v = AIAgent.Instance.CaculateVelocity(
                        temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position + (temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position - AIAgent.Instance.transform.position).normalized,
                        temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position, 0.5f, 0.5f);
                    // 스테이트 머신을 가져와야 한다. 어떻게 가져올까?
                    if (temp.Item1 != AIAgent.Instance.transform)
                    {
                        temp.
                        Item1.GetComponent<Playable>().
                        GetStateMachine.
                        HitDamaged(AIAgent.Instance.GetStateMachine.attackCount - 1 < 2 ? Vector3.zero : v);
                        damageInCount = true;
                        temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = AIAgent.Instance.transform;
                        //Debug.Log($"Attack In Count = {stateMachine.attackCount}");
                        return true;
                    }
                }
                catch
                {

                }
            }
            return false;
        }
    }

}