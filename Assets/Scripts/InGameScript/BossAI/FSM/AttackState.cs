using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Cinemachine;

namespace LGProject.BossAI
{
    public class AttackState : State
    {
        // 자 공격을 어떻게 할 것이냐!
        // 공격은 
        private float _waitTimer;
        private float _attackDelayTimer;
        private float _moveSpeed;
        private ArmType _targetArm;
        private CinemachineVirtualCamera _camera;
        private bool _endPlay;
        private Transform target;
        public AttackState(AiStateMachine stateMachine, float waitTimer, float attackDelayTimer, float moveSpeed) : base(stateMachine)
        {
            _waitTimer = waitTimer;
            _attackDelayTimer = attackDelayTimer;
            _moveSpeed = moveSpeed;
            _endPlay = false;
        }


        public override void Enter()
        {
            base.Enter();

            _endPlay = false;
            // 플레이어의 위치를 찾아 먼저 손을 결정 
            // n초 후 공격 지시,
            FuckingAttack().Forget();
            target = FindLowGagePlayer();
            _targetArm = _stateMachine.GetArmType(target.position);
        }

        public override void Exit()
        {
        
        }

        public override void Update()
        {
            // 공격이 끝나면 State 변경y
            if (_endPlay)
            {
                // 리턴 후 Next State로 
                if (Mathf.Abs(Vector3.Distance(_stateMachine.OriginPos[(int)_targetArm], _stateMachine.Animators[(int)_targetArm].transform.position)) > 0)
                {
                    _stateMachine.MoveArm(_stateMachine.OriginPos[(int)_targetArm], _moveSpeed, _targetArm);
                    return;
                }
                _stateMachine.NextState();
                return;
            }

            // 이동 지시                                                                    A 부터 B까지 초당 n의 속도로 이동하기 
            // 각 팔은 서로의 영역 0 이상의 범위를 침범하지 않는다.
            ////if()
            _stateMachine.MoveArm(target.position, _moveSpeed, _targetArm);
            //_stateMachine.Transform.position = Vector3.MoveTowards(_stateMachine.Transform.position, target.position, 0.5f * Time.deltaTime);
 

        }

        // Player를 향해 공격함.
        // GameManager 또는 중간에 플레이어간의 체력을 중계하는 무엇인가 필요.
        // 일단 임시로 플레이어를 파라미터로 받는 것으로 하자.
        private async UniTaskVoid FuckingAttack()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_waitTimer));
            _stateMachine.SetAnimationTrigger("Normal Attack");

            
            // Shake를 주고 싶은데.
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDelayTimer));
            _endPlay = true;
            //target = _stateMachine.OriginPos[(int)_targetArm]
        }
        
        // 게이지가 낮은 대상 찾기
        private Transform FindLowGagePlayer()
        {
            PlayerState.Playable lowGagePlayer = null;
            foreach(var player in _stateMachine.PlayersProperties)
            {
                if (lowGagePlayer == null)
                    lowGagePlayer = player;
                else if (lowGagePlayer.GetStateMachine.playable.DamageGage > player.GetStateMachine.playable.DamageGage)
                {
                    // 작을 경우 lowGagePlayer를 변경해준다.
                    lowGagePlayer = player;
                }
            }
            if (lowGagePlayer == null)
                Debug.Log("탐색된 플레이어 없음.");
            else
                Debug.Log("탐색된 플레이어 있음.");
            return lowGagePlayer.transform;
        }

    }
}