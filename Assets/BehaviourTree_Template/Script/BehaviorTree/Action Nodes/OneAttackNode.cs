using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



namespace BehaviourTree
{
    public class OneAttackNode : ActionNode
    {
        private AIAgent _agent;
        private PlayerStateMachine _stateMachine;
        [SerializeField, Range(0f, 2f)] private float dashAttackRange;
        [SerializeField, Range(0f, 2f)] private float jumpAttackRange;
        [SerializeField, Range(0f, 3f)] private float dashAttackPower;
        [SerializeField, Range(0f, 3f)] private float jashAttackPower;
        [Space(10f)]
        //[SerializeField] private float _judgeTimer = 0;
        //[SerializeField] private float _aniDelay = 0;

        private float _curTimer;

        //private float _curTimer = 0;
        private bool _isAttack = false;
        // 대쉬 어택.
        protected override void OnStart()
        {
            StartExceptionHandling();
        }

        protected override void OnStop()
        {
            ExitExceptionHandling();
        }

        // Dash Attack을 할 때 이동 방향을 바라보며 공격을 해야함.
        protected override State OnUpdate()
        {
            _curTimer += Time.deltaTime;

            if (_stateMachine.IsDamaged || _stateMachine.IsKnockback)
            {
                Debug.Log("AA");
                return State.Failure;
            }
            if (_stateMachine.playable.DashAttackDelay > _curTimer)
            {
                if (_isAttack == false) _isAttack = ActionJudge();
                if(!_stateMachine.IsGrounded && _agent.targetIndex < _agent.path.Length)
                    FollowPath();
                return State.Running;
            }

            return State.Success;
        }

        bool FollowPath()
        {
            //if (_stateMachine.IsGrounded)
            //{
            //}
            Vector3 currentWaypoint = new Vector3(_agent.path[_agent.targetIndex].x, _agent.path[_agent.targetIndex].y - 0.45f, _agent.path[_agent.targetIndex].z);
            if (_stateMachine.transform.position == currentWaypoint)
            {
                //startPoint = currentWaypoint;
                _agent.targetIndex++;
                if (_agent.targetIndex >= _agent.path.Length)
                {
                    return false;
                }
                currentWaypoint = new Vector3(_agent.path[_agent.targetIndex].x, _agent.path[_agent.targetIndex].y - 0.45f, _agent.path[_agent.targetIndex].z);
            }

            currentWaypoint.z = _stateMachine.transform.position.z;

            _stateMachine.transform.position = Vector3.MoveTowards(AIAgent.Instance.transform.position, currentWaypoint, _agent.speed * Time.deltaTime);

            Vector3 direction = currentWaypoint - AIAgent.Instance.transform.position;
            _agent.directionX = direction.x >= 0.1f ? true : false;
            Vector3 rot = new Vector3(currentWaypoint.x, _stateMachine.transform.position.y, _agent.transform.position.z);
            AIAgent.Instance.transform.LookAt(rot);
            return true;
        }

        private bool ActionJudge()
        {
            // 판정 범위 계산.
            Vector3 right = Vector3.right * (_agent.directionX == true ? 1 : -1);
            Vector3 center = _agent.transform.position + right + Vector3.up * 0.5f;
            Vector3 hitboxSize = Vector3.one * 0.5f;
            hitboxSize.x *= (_stateMachine.IsGrounded ? 1.3f : 1.3f);
            Collider[] targets = Physics.OverlapBox(center,
                hitboxSize,
                Quaternion.identity, 1 << 3);

            System.Tuple<Playable, float> temp = null;

            foreach (var target in targets)
            {
                float distance = Vector3.Distance(center, target.transform.position);
                if ((temp == null || temp.Item2 >= distance) && target.transform != _stateMachine.transform)
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
                Vector3 direction = (temp.Item1.GetStateMachine.transform.position - _stateMachine.transform.position).normalized;
                direction.x *= 2;
                direction.y *= 1.5f;
                Vector3 velocity = (_stateMachine.transform.forward * 3f + _stateMachine.transform.up * 1.5f) * 1.5f;


                if (temp.Item1 != AIAgent.Instance.transform/* && !temp.Item1.GetStateMachine.isGuard*/)
                {
                    //Debug.Log($"v = {v}, direct = {direction}");
                    temp.
                    Item1.GetComponent<Playable>().
                    GetStateMachine.
                    HitDamaged(velocity, 0, _stateMachine, LGProject.DATA_TYPE.DashAttack);

                    //temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = AIAgent.Instance.transform;

                    if (!temp.Item1.GetStateMachine.IsGuard && !temp.Item1.GetStateMachine.IsDown )
                    {
                        // 100 % gage로 일단 계산
                        _stateMachine.playable.SetUltimateGage(_stateMachine.playable.UltimateGage + 10);
                    }
                    return true;
                }
            }
            return false;
        }

        private void StartExceptionHandling()
        {
            if (_agent == null)
                _agent = AIAgent.Instance;
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;
            _stateMachine.IsDashAttack = true;
            _curTimer = 0;
            
            AIAgent.Instance.SetAttacRange(_stateMachine.IsGrounded ? dashAttackRange : jumpAttackRange);
            if (!_stateMachine.IsGrounded)
                _stateMachine.animator.SetTrigger("JumpAttack");
            else
                _stateMachine.animator.SetTrigger("DashAttack");
            _stateMachine.animator.SetFloat("Run", 0);
        }

        private void ExitExceptionHandling()
        {
            AIAgent.Instance.GetStateMachine.IsDashAttack = false;
            _isAttack = false;
        }
    }

}