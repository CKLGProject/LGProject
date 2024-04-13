using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    public class HitState : State
    {
        float ����Ÿ�̸�;
        float curTimer; 
        // �ǰ� ������ �� ���ư���?
        public HitState(PlayerStateMachine _stateMachine, float _����Ÿ�̸�) : base(_stateMachine)
        {
            ����Ÿ�̸� = _����Ÿ�̸�;
        }
        public override void Enter()
        {
            base.Enter();
            // x,z Velocity�� �ʱ�ȭ
            //Vector3 v = stateMachine.physics.velocity;
            //Vector3.zero;
            //stateMachine.physics.velocity = stateMachine;

            stateMachine.isHit = false;     // �¾Ҿ�! �¾Ҵٰ�! �׸�����!

            //Debug.Log($"{stateMachine.transform.ToString()}who's hit? : {stateMachine.hitPlayer}");
            curTimer = 0;
        }

        public override void Exit()
        {
            base.Exit();
            curTimer = 0;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(stateMachine.isGrounded)
            {
                curTimer += Time.deltaTime;
                if (curTimer >= ����Ÿ�̸�)
                {
                    stateMachine.ChangeState(stateMachine.playable.downState);
                    stateMachine.damageGage += 10;
                    return;
                }
            }
            else
            {
                // ���߿��� �ǰݴ��� �� Idle state�� �ƴ� Down State�� ����
                // Down State�� ��� ���� ���� ��(is Grounded = true) ����
                // Down State�� �����ؾ� �ϸ�, 
                stateMachine.ChangeState(stateMachine.playable.downState);

            }

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
        }
    }
}
