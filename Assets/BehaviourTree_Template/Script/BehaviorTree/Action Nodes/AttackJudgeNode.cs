using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackJudgeNode : ActionNode
    {
        //public AIAgent Agent;
        //[Space(10f)]
        public float judgTimer = 0;
        public float animTimer = 0;
        private float _curTimer = 0;
        private bool _isAttack = false;
        protected override void OnStart()
        {
            //if (Agent == null)
            //    Agent = AIAgent.Instance;
            if (AIAgent.Instance.GetStateMachine.attackCount > 2)
                AIAgent.Instance.GetStateMachine.attackCount = 0;
            _isAttack = false;
            AIAgent.Instance.GetStateMachine.isNormalAttack = true;
        }

        protected override void OnStop()
        {
            AIAgent.Instance.GetStateMachine.isNormalAttack = false;

        }
        protected override State OnUpdate()
        {
            if (AIAgent.Instance.GetStateMachine.isGuard)
                return State.Failure;
            _curTimer += Time.deltaTime;
            if(judgTimer > _curTimer)
            {
                // 애니메이션이 끝난 이후 데미지 판정 -> 데미지를 넣는데 성공하면 다음 공격, 시간이 지나도 공격 못하면 Idle
                // 애니메이션이 재생 중이라면 Running
                if(_isAttack == false) _isAttack = ActionJudge();
                if (_curTimer > animTimer && _isAttack)
                {
                    _curTimer = 0;
                    Debug.Log($"ATK Count = {AIAgent.Instance.GetStateMachine.attackCount}");
                    AIAgent.Instance.GetStateMachine.attackCount++;
                    return State.Success;
                }
                return State.Running;
            }
            _curTimer = 0;
            return State.Failure;
        }

        private bool ActionJudge()
        {
            // 판정 범위 계산.
            Vector3 right = Vector3.right * (AIAgent.Instance.directionX == true ? 1 : -1);
            Vector3 center = AIAgent.Instance.transform.position + right;

            Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f, Quaternion.identity, 1 << 3);
            System.Tuple<Transform, float> temp = null;

            foreach(var target in targets)
            {
                float distance = Vector3.Distance(center, target.transform.position);
                if(temp == null || (temp.Item2 >= distance && target.transform != AIAgent.Instance.transform))
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
                Vector3 v = Vector3.zero;
                if (AIAgent.Instance.GetStateMachine.attackCount >= 2)
                {
                    v = AIAgent.Instance.CaculateVelocity(
                       temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position + (temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position - AIAgent.Instance.transform.position).normalized,
                          temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position, 0.5f, 0.5f);
                }
                if (temp.Item1 != AIAgent.Instance.transform)
                {
                    temp.
                    Item1.GetComponent<Playable>().
                    GetStateMachine.
                    HitDamaged(AIAgent.Instance.GetStateMachine.attackCount < 2 ? Vector3.zero : v);
                    //damageInCount = true; <- 이건 좀 생각해봐야 할 듯...
                    temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = AIAgent.Instance.transform;
                    //Debug.Log($"Attack In Count = {stateMachine.attackCount}");
                    return true;
                }
            }
            return false;
        }


    }
}