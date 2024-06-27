using UnityEngine;

namespace BehaviourTree
{
    public class ChasingNode : ActionNode
    {
        private AIAgent _agent;
        private LGProject.PlayerState.PlayerStateMachine _stateMachine;
        private LGProject.PlayerState.PlayerStateMachine _playerStateMachine;
        [SerializeField] private float jumpDelay = 1.25f;
        [SerializeField] private float stopRange;
        private float _curTimer;
        private int _count;
        
        private Vector3 chasingPoint = Vector3.zero;

        protected override void OnStart()
        {
            if (_agent == null)
                _agent = AIAgent.Instance;
            StateMachineLogic();
            jumpDelay = 0.25f;
            // 플레이어 
            pathFinding.PathRequestManager.RequestPath(new pathFinding.PathRequest(_stateMachine.transform.position, _agent.player.position, _agent.GetPath));
            _curTimer = 0;
            chasingPoint = pathFinding.Grid.Instance.NodeFromWorldPoint(_stateMachine.transform.position).WorldPosition;
            _stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Run).Forget();
            _stateMachine.DataSet(LGProject.DATA_TYPE.Chasing);
            _agent.GetStateMachine.animator.ResetTrigger("Landing");
        }

        protected override void OnStop()
        {
            _stateMachine.animator.SetFloat("Run", 0f);
        }

        protected override State OnUpdate()
        {
            try
            {
                // path가 있는지 확인. || 내가 피해를 입은 경우 |}| 최종 경로에 도착한 경우 || 플레이어가 공중에 떠있을때 || 플레이어가 누워있을 때 || 플레이어가 떨어졌을 때,
                if (EscapeConditions())
                {
                    return State.Failure;
                }

                if (_agent.path.Length > 0 && TargetPointToPlayerPositionDistance() > 3f)
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
                _stateMachine.StandingVelocity();
                return State.Success;
            }
            catch
            {
                return State.Failure;
            }
            
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
            //Debug.Log($"_curTimer / {_curTimer}");
            Vector3 currentWaypoint = new Vector3(_agent.path[_agent.targetIndex].x, _agent.path[_agent.targetIndex].y - 0.45f, _agent.path[_agent.targetIndex].z);

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
            currentWaypoint.y = _stateMachine.transform.position.y;
            //if (_agent.transform.position == currentWaypoint)
            if (Mathf.Abs(Vector3.Distance(_agent.transform.position, currentWaypoint)) < 0.25f)
            {
                _agent.targetIndex++;
                if (_agent.targetIndex >= _agent.path.Length)
                {
                    return false;
                }
                currentWaypoint = new Vector3(_agent.path[_agent.targetIndex].x, _agent.path[_agent.targetIndex].y - 0.45f, _agent.path[_agent.targetIndex].z);

                currentWaypoint.z = _stateMachine.transform.position.z;
                currentWaypoint.y = _stateMachine.transform.position.y;
            }

            //_stateMachine.transform.position = Vector3.MoveTowards(_stateMachine.transform.position, currentWaypoint, _agent.MaximumSpeed * Time.deltaTime);
            Movement(3f);

            Vector3 direction = currentWaypoint - _stateMachine.transform.position;

            _agent.directionX = direction.x >= 0.1f ? true : false; 

            Vector3 rot = new Vector3(currentWaypoint.x, _stateMachine.transform.position.y, _stateMachine.transform.position.z);
            _agent.transform.LookAt(rot);
            return true;
        }
        private void Movement(float speedSpeed)
        {
            if (_stateMachine.physics.velocity.x <= speedSpeed * (_stateMachine.IsPet ? 1.05f : 1) && _stateMachine.physics.velocity.x >= -speedSpeed * (_stateMachine.IsPet ? 1.05f : 1))

                _stateMachine.physics.velocity += _stateMachine.transform.forward * 1.5f;

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
                    if (directionNew.y < 0.1f && Physics.Raycast(ray, out RaycastHit hit, 0.5f, layerMask: 1 << 6))
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
                _stateMachine.IsKnockback ||
                _playerStateMachine.IsKnockback || 
                _playerStateMachine.IsDown ||
                _playerStateMachine.transform.position.y < -0.5f ||
                _stateMachine.IsDead)
                return true;
            return false;
        }

        private void SetJumpVelocity()
        {
            if (_curTimer >= jumpDelay && _agent.GetStateMachine.JumpInCount < 2)
            {
                _agent.GetStateMachine.JumpInCount++;

                _agent.HandleJumpping();
                _stateMachine.IsGrounded = false;
                _agent.GetStateMachine.collider.isTrigger = true;
                _curTimer = 0;
                //_agent.effectManager.Stop(EffectManager.EFFECT.Run);
                //_agent.effectManager.Play(EffectManager.EFFECT.Airborne).Forget();
                _agent.GetStateMachine.animator.SetTrigger("Jump" + _agent.GetStateMachine.JumpInCount.ToString());
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