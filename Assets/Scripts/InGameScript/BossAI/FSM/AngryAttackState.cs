using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGProject.BossAI
{
    public class AngryAttackState : State
    {
        private float _curTimer;
        private float _waitTimer;

        public AngryAttackState(AiStateMachine stateMachine, float waitTimer) : base(stateMachine)
        {
            _waitTimer = waitTimer;
        }
        
        public override void Enter()
        {
            base.Enter();
            _curTimer = 0;
            _stateMachine.SetAnimationTrigger("Angry Attack");
        }

        public override void Update()
        {
            base.Update();
            _curTimer += Time.deltaTime;
            if (_curTimer > _waitTimer)
            {
                // 일정 시간이 지나면 다음 State를 실행한다.
                _stateMachine.NextState();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }

        // 공격 시 플레이어를 타격함.




    }
}
