using UnityEngine;

namespace BehaviourTree
{
    public class ChasingNode : ActionNode
    {
        private AIAgent _agent;
        private LGProject.PlayerState.PlayerStateMachine _stateMachine;
        private LGProject.PlayerState.PlayerStateMachine _playerStateMachine;
        [SerializeField] private float jumpDelay;
        [SerializeField] private float stopRange;
        private float _curTimer;
        private int _count;
        
        private Vector3 chasingPoint = Vector3.zero;

        protected override void OnStart()
        {
            if (_agent == null)
                _agent = AIAgent.Instance;
            StateMachineLogic();
            // 플레이어 
            pathFinding.PathRequestManager.RequestPath(new pathFinding.PathRequest(_stateMachine.transform.position, _agent.player.position, _agent.GetPath));

            chasingPoint = pathFinding.Grid.Instance.NodeFromWorldPoint(_stateMachine.transform.position).worldPosition;
            
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // path가 있는지 확인. || 내가 피해를 입은 경우 |}| 최종 경로에 도착한 경우 || 플레이어가 공중에 떠있을때 || 플레이어가 누워있을 때 || 플레이어가 떨어졌을 때,
            if (EscapeConditions())
            {
                return State.Failure;
            }

            if (_agent.path.Length > 0 && TargetPointToPlayerPositionDistance() > 2f)
            {
                pathFinding.PathRequestManager.RequestPath(new pathFinding.PathRequest(_stateMachine.transform.position, _agent.player.position, _agent.GetPath));
                return State.Running;
            }

            float distance = Vector3.Distance(_agent.transform.position, _agent.player.position);
            if (distance < stopRange)
            {
                return State.Success;
            }

            // Running 상태가 필요함.
            // path가 있다면 움직이게 하기.
            // 마지막 경로에 도착했는가를 체크해야함.
            if (_agent.targetIndex < _agent.path.Length && FollowPath())
            {
                // 이동을 해야함.
                return State.Running;
            }
            return State.Success;
            
        }

        private float TargetPointToPlayerPositionDistance()
        {
            float a = Vector3.Distance(_agent.path[_agent.path.Length - 1], _agent.player.position);
            if (!_agent.GetStateMachine.IsGrounded)
                a = 0;
            return a;
        }


        bool FollowPath()
        {
            if (_agent == null)
                _agent = AIAgent.Instance;
            _curTimer += Time.deltaTime;
            Vector3 currentWaypoint = new Vector3(_agent.path[_agent.targetIndex].x, _agent.path[_agent.targetIndex].y - 0.45f, _agent.path[_agent.targetIndex].z);
            //if (_agent.transform.position == currentWaypoint)
            if (Mathf.Abs(Vector3.Distance(_agent.transform.position, currentWaypoint)) < 0.25f)
            {
                _agent.targetIndex++;
                if (_agent.targetIndex >= _agent.path.Length)
                {
                    return false;
                }
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
            currentWaypoint.z = _stateMachine.transform.position.z;

            _stateMachine.transform.position = Vector3.MoveTowards(_stateMachine.transform.position, currentWaypoint, _agent.MaximumSpeed * Time.deltaTime);

            Vector3 direction = currentWaypoint - _stateMachine.transform.position;

            _agent.directionX = direction.x >= 0.1f ? true : false; 

            Vector3 rot = new Vector3(currentWaypoint.x, _stateMachine.transform.position.y, _stateMachine.transform.position.z);
            _agent.transform.LookAt(rot);
            return true;
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
            Ray ray = new Ray(_stateMachine.transform.position + Vector3.up * 0.25f + Vector3.forward * 0.2f, Vector3.down);
            _curTimer += Time.deltaTime;
            bool case1 = /*Mathf.Abs(direction.x) < 1.5f && */direction.y >= 0.5f;
            bool case2 = !Physics.Raycast(ray, out hit, 0.45f, 1 << 6) && _stateMachine.JumpInCount < 2;
            // 높은 곳이면 점프를 한다.
            if ((direction.y >= 1f && Mathf.Abs(direction.x) < 0.75f) && _stateMachine.JumpInCount < 2)
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


            // 가는 길에 길이 없으면 쓰는 로직

        }

        private bool AcrossTargetNode(Vector3 currentWayPoint)
        {
            Vector3 direction = currentWayPoint - _stateMachine.transform.position;
            int i = 0;
            // y가 0이 아닌 곳이면 점프 또는 내려가기를 진행
            if (direction.y > 0.1f)
            {
                for (i = _agent.targetIndex; i < _agent.path.Length - 1; i++)
                {
                    Vector2 directionNew = new Vector2(_agent.path[i].x - _agent.path[i + 1].x, _agent.path[i + 1].y - _agent.path[i].y);
                    Ray ray = new Ray(_agent.path[i], Vector3.down);
                    // 조건
                    // 노드의 간격 중 y값이 0이면서 노드 아래에 플랫폼이 존재하는 경우.
                    if (directionNew.y < 0.1f && Physics.Raycast(ray, out hit, 0.5f, layerMask: 1 << 6))
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
            Vector3 targetPos = pathFinding.Grid.Instance.NodeFromWorldPoint(_agent.target.position).worldPosition;
            // 최종 목표
            Vector3 FinTarget = _agent.path[_agent.path.Length - 1];

            float distance = Mathf.Abs(Vector3.Distance(targetPos, FinTarget));

            if (distance > 1.5f * 4)
            {
                return true;
            }

            return false;
        }

        private bool EscapeConditions()
        {
            if (_agent.path == null || 
                _agent.path.Length < 1 || 
                _stateMachine.IsDamaged || 
                _playerStateMachine.IsKnockback || 
                _playerStateMachine.IsDown ||
                _playerStateMachine.transform.position.y < -0.5f)

                return true;
            return false;
        }



        private void SetJumpVelocity()
        {
            if (_curTimer >= jumpDelay)
            {
                _count++;
                _stateMachine.JumpInCount++;
                _stateMachine.JumpVelocity();
                _stateMachine.physics.velocity += Vector3.up * _agent.JumpScale;
                _curTimer = 0;
            }
        }

        private void StateMachineLogic()
        {
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;

            if (_playerStateMachine == null)
            {
                _playerStateMachine = _agent.player.GetComponent<LGProject.PlayerState.Playable>().GetStateMachine;
            }

            //_stateMachine.animator.SetTrigger("Idle");
            _stateMachine.AttackCount = 0;
            _stateMachine.animator.SetInteger("Attack", 0);
            _stateMachine.animator.SetFloat("Run", 1f);
        }
    }
}