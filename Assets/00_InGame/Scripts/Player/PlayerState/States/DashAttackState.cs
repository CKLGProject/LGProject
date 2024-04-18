using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerState
{
    // ����� ���� ��ȸ�� 
    public class DashAttackState : State
    {
        float curTimer;
        float aniDelay = 0;
        public DashAttackState(PlayerStateMachine _stateMachine, ref float _aniDelay) : base(_stateMachine)
        {
            aniDelay = _aniDelay;
        }

        public override void Enter()
        {
            base.Enter();
            curTimer = 0;
            // velocity �ʱ�ȭ X
            // �׷��� �극��ũ�� �ɸ� ���� ��? ���� Drag�� �����ؼ� ������ �ϴ� ������ �ຸ��.

        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            curTimer += Time.deltaTime; 
            if(curTimer >= aniDelay)
            {
                stateMachine.ChangeState(stateMachine.playable.idleState);
                return ;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
        }

        public override void Exit()
        {
            base.Exit();
        }

    }

}