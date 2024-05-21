using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BehaviourTree
{
    // 움직임을 판단. 상대를 향해 이동
    // 점프를 할 때도 있을 것이다. 이는 
    public class MoveNode : ActionNode
    {

        private LGProject.PlayerState.PlayerStateMachine _stateMachine;
        private AIAgent _agent;
        [Space(10f)]

        // 어디로 이동할 것인가가 관건
        // 점프는 별개의 영역이므로 이동자체의 로직만 생각하자.
        // 목표 지점을 두는 것은 어떨까?
        // 내가 이동할 목적지가 존재하면 나름 알고리즘에서 제약이 없을 것 같다.
        // 점프도 어차피 Velocity를 줘서 그 방향으로 날리는 것이기 때문에 이동과 점프 로직은 달리 하면 좋을 것 같다.

        // 현재 이동을 A*로 진행하려 생각 중.
        // Platform을 기준으로 이동할 수 있는 노드이며 대각선 이동 또는 y축 이동일 경우 점프를 하게 대체.
        public float jumpDelay = 0;
        private float _curTimer = 0;
        private int _count = 0;
        
        //private Vector3 startPoint = Vector3.zero;
        //private int targetIndex = 0;
        //int count = 0;
        ////Vector3 currentWaypoint = Vec;

        protected override void OnStart()
        {
            // Path 설정
            if (_agent == null)
                _agent = AIAgent.Instance;
            StateMachineLogic();
            _stateMachine.physics.velocity = Vector3.zero;
            _curTimer = jumpDelay;
            _agent.targetIndex = 0;
        }

        protected override void OnStop()
        {
            _stateMachine.animator.SetFloat("Run", 0f);
        }

        protected override State OnUpdate()
        {
            #region legarcy
            //float distance = Vector3.Distance(agent.transform.position, agent.target.position);
            //// 이동 
            //if (agent.target == null)
            //    return State.Failure;
            //if (distance <= 0.5f)
            //{
            //    //agent.GetStateMachine.physics.velocity = new Vector3(0, agent.GetStateMachine.physics.velocity.y, 0);
            //    return State.Success;
            //}
            //MovingPoint();
            //return State.Running;

            #endregion
            if (_agent == null)
                _agent = AIAgent.Instance;

            if (_stateMachine.IsDamaged || _stateMachine.IsDead)
                return State.Failure;

            if (_agent.GetStateMachine.IsGrounded)
                _agent.GetStateMachine.playable.effectManager.Play(EffectManager.EFFECT.Run).Forget();
            else
                _agent.GetStateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Run);

            float distance = Vector3.Distance(_stateMachine.transform.position, _agent.player.position);

            // path가 있는지 확인. || 내 앞에 적이 있는 지 확인
            if ((_agent.path == null || distance <= 1.5f) && _agent.GetStateMachine.IsGrounded)
            {
                return State.Failure;
            }
            // path가 있다면 움직이게 하기.
            // Running 상태가 필요함.
            // 마지막 경로에 도착했는가를 체크해야함.
            if (FollowPath())
            {
                // 이동을 해야함.
                return State.Running;
            }
            return State.Success;   

        }

        bool FollowPath()
        {
            if (_agent == null)
                _agent = AIAgent.Instance;
            try
            {
                _curTimer += Time.deltaTime;
                Vector3 currentWaypoint = new Vector3(_agent.path[_agent.targetIndex].x, _agent.path[_agent.targetIndex].y - 0.45f, _agent.path[_agent.targetIndex].z);
                if (Mathf.Abs(Vector3.Distance(_agent.transform.position, currentWaypoint)) < 0.5f)
                {
                    _agent.targetIndex++;
                    if (_agent.targetIndex >= _agent.path.Length)
                        return false;

                    currentWaypoint = new Vector3(_agent.path[_agent.targetIndex].x, _agent.path[_agent.targetIndex].y - 0.45f, _agent.path[_agent.targetIndex].z);
                }

                // 대각선 위인지 체크
                if (AcrossTargetNode(currentWaypoint))
                {
                    // 목표지점가지 점프해야하는데 일단 디버그 찍고 끝내자
                    SetJumpVelocity();
                }
                if (CheckTargetPosition())
                {
                    return false;
                }
                currentWaypoint.z = _agent.transform.position.z;

                _agent.transform.position = Vector3.MoveTowards(_agent.transform.position, currentWaypoint, _stateMachine.playable.MaximumSpeed * Time.deltaTime);

                Vector3 direction = currentWaypoint - _agent.transform.position;
                _agent.directionX = direction.x >= 0.1f ? true : false;
                Vector3 rot = new Vector3(currentWaypoint.x, _agent.transform.position.y, _agent.transform.position.z);
                _agent.transform.LookAt(rot);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool CheckDownNode(Vector3 currentWayPoint)
        {
            Vector3 direction = currentWayPoint - _agent.transform.position;

            if (direction.y < -0.4)
            {
                return false;
            }
            return true;
        }

        private bool AcrossTargetNode(Vector3 currentWayPoint)
        {
            Vector3 direction = currentWayPoint - _agent.transform.position;
            Vector2 directionOld = Vector2.zero;
            int i = 0;
            RaycastHit hit;
            // y가 0이 아닌 곳이면 점프 또는 내려가기를 진행
            if(direction.y > 0.1f)
            {
                for(i = _agent.targetIndex; i < _agent.path.Length - 1; i++) 
                {
                    Vector2 directionNew = new Vector2(_agent.path[i].x - _agent.path[i+1].x, _agent.path[i + 1].y - _agent.path[i].y);
                    Ray ray = new Ray(_agent.path[i], Vector3.down);
                    // 조건
                    // 노드의 간격 중 y값이 0이면서 노드 아래에 플랫폼이 존재하는 경우.
                    if(directionNew.y < 0.1f && Physics.Raycast(ray, out hit, 0.5f, layerMask: 1 << 6))
                    {
                        break;
                    }
                }
                _agent.targetIndex = i;
                return true;
            }
            return false;
        }

        public bool CheckTargetPosition()
        {
            if (_agent.target == null)
                return false;
            // 타겟 노드
            Vector3 targetPos = pathFinding.Grid.Instance.NodeFromWorldPoint(_agent.target.position).WorldPosition;
            // 최종 목표
            Vector3 FinTarget = _agent.path[_agent.path.Length - 1];

            float distance = Mathf.Abs(Vector3.Distance(targetPos, FinTarget));

            // 1.5 * n
            if (distance > 1.5f * 4)
            {
                return true;
            }

            return false;
        }

        // 여기서 이동을 주관하되 공격, 점프 등을 유연하게 할 수 있어야 한다.
        // 4를 넘지 않았다면 계속 Physics 를 줘라
        private void MovingPoint()
        {
            Vector3 direction = _agent.target.position - _agent.transform.position;

            JumpingPointCheck(direction);

            if (Mathf.Abs(_agent.GetStateMachine.physics.velocity.x) < 1 && Mathf.Abs(direction.x) > 0.1f)
            {
                // 방향을 체크
                direction = new Vector3(direction.x, 0, 0);
                direction.Normalize();
                _agent.GetStateMachine.physics.velocity += direction * 1;
                // 여기서 가야하는 방향에 따라 바라보는 방향을 달리 해줌.
                Vector3 viewTarget = new Vector3(_agent.target.position.x, _agent.transform.position.y, _agent.target.position.z);
                _agent.transform.LookAt(viewTarget);
            }
            else if(Mathf.Abs(direction.x) < 0.1f)
                _agent.GetStateMachine.physics.velocity = new Vector3(0, _agent.GetStateMachine.physics.velocity.y, 0);
        }

        RaycastHit hit;
        private void JumpingPointCheck(Vector3 direction)
        {
            // 점프를 하기 위해 바로 대각선 아래를 체크
            // 대각선 아래에 공간이 없으면 점프를 함.
            // 점프를 하고 떨어지는 와중에도 공간이 없음 추가 점프를 해야하는데,
            // 추가 점프(2단)는 나중에 구현하는 것으로 하자.
            // 앞을 바라보고 있다는 것을 어떻게 표현해야 할까?
            // lookAt? 아니... 그냥 바라보는 곳을 명확하게 하는 것이 좋아보인다.
            
            // 점프를 하자마자 타이머가 돌아가야하는데, 이건 어떻게 표현할까?
            Ray ray = new Ray(_agent.transform.position + Vector3.up * 0.25f + Vector3.forward * 0.2f, Vector3.down);
            _curTimer += Time.deltaTime;
            bool case1 = /*Mathf.Abs(direction.x) < 1.5f && */direction.y >= 0.5f;
            bool case2 = !Physics.Raycast(ray, out hit, 0.45f, 1 << 6) && _agent.GetStateMachine.JumpInCount < 2; 
            // 높은 곳이면 점프를 한다.
            if ((direction.y >= 1f && Mathf.Abs( direction.x ) < 0.75f) && _agent.GetStateMachine.JumpInCount < 2)
            {
                // 점프를 하는 조건
                // isGrounded가 True거나, timer가 넘어설 경우
                // isGrounded 때문인 것 같음.
                SetJumpVelocity();
            }
            else if (case1 && case2)
            {
                SetJumpVelocity();
            }
            // 위를 체크하고 싶은데...

        }

        private void SetJumpVelocity()
        {
            if (_curTimer >= jumpDelay)
            {
                _count++;
                _agent.GetStateMachine.JumpInCount++;
                _agent.GetStateMachine.JumpVelocity();
                _agent.HandleJumpping();
                _curTimer = 0;
                _agent.GetStateMachine.animator.SetTrigger("Jump" + _agent.GetStateMachine.JumpInCount.ToString());
            }
        }

        private void StateMachineLogic()
        {
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;
            //_stateMachine.animator.SetTrigger("Idle");
            _stateMachine.AttackCount = 0;
            _stateMachine.animator.SetInteger("Attack", 0);
            _stateMachine.animator.SetFloat("Run", 1f);
        }


    }

}