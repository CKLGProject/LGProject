using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



namespace BehaviourTree
{
    public class OneAttackNode : ActionNode
    {
        PlayerStateMachine stateMachine;
        [SerializeField, Range(0f, 2f)] private float dashAttackRange;
        [SerializeField, Range(0f, 2f)] private float jumpAttackRange;
        [SerializeField, Range(0f, 3f)] private float dashAttackPower;
        [SerializeField, Range(0f, 3f)] private float jashAttackPower;
        [Space(10f)]
        [SerializeField] private float judgeTimer = 0;
        [SerializeField] private float aniDelay = 0;

        private float _curTimer;

        //private float _curTimer = 0;
        private bool _isAttack = false;
        // 대쉬 어택.
        protected override void OnStart()
        {
            if (stateMachine == null)
                stateMachine = AIAgent.Instance.GetStateMachine;
            stateMachine.isDashAttack = true;
            _curTimer = 0;
            AIAgent.Instance.SetAttacRange(stateMachine.isGrounded ? dashAttackRange : jumpAttackRange);
            stateMachine.animator.SetTrigger("DashAttack");
        }

        protected override void OnStop()
        {
            AIAgent.Instance.GetStateMachine.isDashAttack = false;
            _isAttack = false;
        }

        // Dash Attack을 할 때 이동 방향을 바라보며 공격을 해야함.
        protected override State OnUpdate()
        {
            _curTimer += Time.deltaTime;
            if(judgeTimer > _curTimer)
            {
                if (_isAttack == false) _isAttack = ActionJudge();
                if(!AIAgent.Instance.GetStateMachine.isGrounded)
                    FollowPath();
                return State.Running;
            }
            if (AIAgent.Instance.GetStateMachine.isHit)
                return State.Failure;

            return State.Success;
        }

        bool FollowPath()
        {
            if (AIAgent.Instance.GetStateMachine.isGrounded)
            {
            }
            Vector3 currentWaypoint = new Vector3(AIAgent.Instance.path[AIAgent.Instance.targetIndex].x, AIAgent.Instance.path[AIAgent.Instance.targetIndex].y - 0.45f, AIAgent.Instance.path[AIAgent.Instance.targetIndex].z);
            if (AIAgent.Instance.transform.position == currentWaypoint)
            {
                //startPoint = currentWaypoint;
                AIAgent.Instance.targetIndex++;
                if (AIAgent.Instance.targetIndex >= AIAgent.Instance.path.Length)
                {
                    return false;
                }
                currentWaypoint = new Vector3(AIAgent.Instance.path[AIAgent.Instance.targetIndex].x, AIAgent.Instance.path[AIAgent.Instance.targetIndex].y - 0.45f, AIAgent.Instance.path[AIAgent.Instance.targetIndex].z);
            }

            currentWaypoint.z = AIAgent.Instance.transform.position.z;

            AIAgent.Instance.transform.position = Vector3.MoveTowards(AIAgent.Instance.transform.position, currentWaypoint, AIAgent.Instance.speed * Time.deltaTime);

            Vector3 direction = currentWaypoint - AIAgent.Instance.transform.position;
            AIAgent.Instance.directionX = direction.x >= 0.1f ? true : false;
            Vector3 rot = new Vector3(currentWaypoint.x, AIAgent.Instance.transform.position.y, AIAgent.Instance.transform.position.z);
            AIAgent.Instance.transform.LookAt(rot);
            return true;
        }

        private bool ActionJudge()
        {
            // 판정 범위 계산.
            Vector3 right = Vector3.right * (AIAgent.Instance.directionX == true ? 1 : -1);
            Vector3 center = AIAgent.Instance.transform.position + right;

            Collider[] targets = Physics.OverlapBox(center,
                Vector3.one * (AIAgent.Instance.GetStateMachine.isGrounded ? dashAttackRange : jumpAttackRange),
                Quaternion.identity, 1 << 3);

            System.Tuple<Playable, float> temp = null;

            foreach (var target in targets)
            {
                float distance = Vector3.Distance(center, target.transform.position);
                if (temp == null || (temp.Item2 >= distance && target.transform != AIAgent.Instance.transform))
                {
                    temp = System.Tuple.Create(target.GetComponent<Playable>(), distance);
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
                Vector3 direction = temp.Item1.GetStateMachine.transform.position - AIAgent.Instance.transform.position;

                Vector3 v = AIAgent.Instance.CaculateVelocity(direction * dashAttackPower, AIAgent.Instance.transform.position, 0.5f, 1f);

                if (temp.Item1 != AIAgent.Instance.transform/* && !temp.Item1.GetStateMachine.isGuard*/)
                {
                    temp.
                    Item1.GetComponent<Playable>().
                    GetStateMachine.
                    HitDamaged(v);
                    
                    temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = AIAgent.Instance.transform;

                    temp.Item1.GetComponent<Playable>().effectManager.PlayOneShot(EffectManager.EFFECT.Hit);

                    return true;
                }
            }
            return false;
        }

    }

}