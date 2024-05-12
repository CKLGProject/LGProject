using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BehaviourTree
{
    public class DashAttackNode : ActionNode
    {
        [SerializeField, Range(0f, 1f)] private float attackRange;
        [SerializeField] private float judgeTimer = 0;
        [SerializeField] private float aniDelay = 0;

        private float _curTimer;

        //private float _curTimer = 0;
        private bool _isAttack = false;
        // 대쉬 어택.
        protected override void OnStart()
        {
            AIAgent.Instance.GetStateMachine.IsDashAttack = true;
            _curTimer = 0;
            AIAgent.Instance.SetAttacRange(attackRange);
        }

        protected override void OnStop()
        {
            AIAgent.Instance.GetStateMachine.IsDashAttack = false;
        }

        // Dash Attack을 할 때 이동 방향을 바라보며 공격을 해야함.
        protected override State OnUpdate()
        {
            _curTimer += Time.deltaTime;
            if(judgeTimer > _curTimer)
            {
                if (_isAttack == false) _isAttack = ActionJudge();
                return State.Running;
            }
            if (AIAgent.Instance.GetStateMachine.IsDamaged)
                return State.Failure;

            return State.Success;
        }

        private bool ActionJudge()
        {
            // 판정 범위 계산.
            Vector3 right = Vector3.right * (AIAgent.Instance.directionX == true ? 1 : -1);
            Vector3 center = AIAgent.Instance.transform.position + right;

            Collider[] targets = Physics.OverlapBox(center, Vector3.one * attackRange, Quaternion.identity, 1 << 3);
            System.Tuple<Transform, float> temp = null;

            foreach (var target in targets)
            {
                float distance = Vector3.Distance(center, target.transform.position);
                if (temp == null || (temp.Item2 >= distance && target.transform != AIAgent.Instance.transform))
                {
                    temp = System.Tuple.Create(target.transform, distance);
                }
            }

            // 공격 범위에 적이 존재하지 않음.
            if (temp == null)
            {
                // 아직 공격을 성공하지 못함.
                return false;
            }
            else
            {
                Vector3 v = Vector3.zero;
                v = AIAgent.Instance.CaculateVelocity(
                   temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position + (temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position - AIAgent.Instance.transform.position).normalized * 2f,
                      temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position, 0.5f, 1f);
                if (temp.Item1 != AIAgent.Instance.transform)
                {
                    temp.
                    Item1.GetComponent<Playable>().
                    GetStateMachine.
                    HitDamaged(v);
                    //damageInCount = true; <- 이건 좀 생각해봐야 할 듯...
                    temp.Item1.GetComponent<Playable>().GetStateMachine.HitPlayer = AIAgent.Instance.transform;

                    return true;
                }
            }
            return false;
        }

    }

}