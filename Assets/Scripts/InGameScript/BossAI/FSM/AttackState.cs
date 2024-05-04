using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.BossAI
{
    public class AttackState : State
    {
        private float _curTimer;
        private float _waitTimer;
        public AttackState(AiStateMachine stateMachine, float waitTimer) : base(stateMachine)
        {
            _waitTimer = waitTimer;
        }


        public override void Enter()
        {
            base.Enter();

            _curTimer = 0;
            
        }

        public override void Exit()
        {
        
        }

        public override void Update()
        {
            _curTimer += Time.deltaTime;
            if (_curTimer > _waitTimer)
            {
                // 일정 시간이 지나면 다음 State를 실행한다.
                _stateMachine.NextState();
            }


        }
    }

}