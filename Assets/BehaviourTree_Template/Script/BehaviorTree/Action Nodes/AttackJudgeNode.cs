using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackJudgeNode : ActionNode
    {
        public AIAgent Agent;
        //[Space(10f)]
        public float judgTimer = 0;
        public float animTimer = 0;
        private float curTimer = 0;
        private bool isAttack = false;
        protected override void OnStart()
        {
            if (Agent == null)
                Agent = AIAgent.Instance;
            Agent.GetStateMachine.attackCount++;
            isAttack = false;
            Agent.GetStateMachine.isNormalAttack = true;
        }

        protected override void OnStop()
        {
            Agent.GetStateMachine.isNormalAttack = false;
        }

        protected override State OnUpdate()
        {
            if (Agent.GetStateMachine.isGuard)
                return State.Failure;
            curTimer += Time.deltaTime;
            if(judgTimer > curTimer)
            {
                // 애니메이션이 끝난 이후 데미지 판정 -> 데미지를 넣는데 성공하면 다음 공격, 시간이 지나도 공격 못하면 Idle
                // 애니메이션이 재생 중이라면 Running
                if(isAttack == false) isAttack = ActionJudge();
                if (curTimer > animTimer && isAttack)
                {
                    curTimer = 0;
                    return State.Success;
                }
                return State.Running;
            }
            curTimer = 0;
            return State.Failure;
        }

        private bool ActionJudge()
        {
            // 판정 범위 계산.
            Vector3 right = Vector3.right * (Agent.directionX == true ? 1 : -1);
            Vector3 center = Agent.transform.position + right;

            Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f);
            System.Tuple<Transform, float> temp = null;

            foreach(var target in targets)
            {
                float distance = Vector3.Distance(center, target.transform.position);
                if(temp == null || (temp.Item2 >= distance && target.transform != Agent.transform))
                {
                    temp = System.Tuple.Create(target.transform, distance);
                }
            }

            // 공격 범위에 적이 존재하지 않음.
            if(temp == null)
            {
                // 아직 공격을 성공하지 못함.
                return false;
            }
            else
            {
                Vector3 v= Agent.CaculateVelocity(
                        temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position + (temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position - Agent.transform.position).normalized,
                        temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position, 0.5f, 0.5f);
                if (temp.Item1 != Agent.transform)
                {
                    temp.
                    Item1.GetComponent<Playable>().
                    GetStateMachine.
                    HitDamaged(Agent.GetStateMachine.attackCount - 1 < 2 ? Vector3.zero : v);
                    //damageInCount = true; <- 이건 좀 생각해봐야 할 듯...
                    temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = Agent.transform;
                    //Debug.Log($"Attack In Count = {stateMachine.attackCount}");

                    return true;
                }
            }
            return false;
        }


    }
}