using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class KaneAttackState : AttackState
    {
        
        public KaneAttackState(PlayerStateMachine stateMachine, ref float firstJudgeAttack, ref float firstAttackDelay, ref float secondJudgeAttack, ref float secondAttackDelay, ref float thirdJudgeAttack, ref float thirdAttackDelay) : base(stateMachine, ref firstJudgeAttack, ref firstAttackDelay, ref secondJudgeAttack, ref secondAttackDelay, ref thirdJudgeAttack, ref thirdAttackDelay)
        {
            _currentTimer = 0;

            _firstJudgeDelay = firstJudgeAttack;
            _firstAttackDelay = firstAttackDelay;
            _secondJudgeDelay = secondJudgeAttack;
            _secondAttackDelay = secondAttackDelay;
            _thirdJudgeDelay = thirdJudgeAttack;
            _thirdAttackDelay = thirdAttackDelay;
        }

        public override void Enter()
        {
            base.Enter();


        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            //base.LogicUpdate();
            if (Damaged())
                return;
            _currentTimer += Time.deltaTime;
            AttackLogic();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        private void AttackLogic()
        {
            float time = StateMachine.GetAnimationPlayTime("Attack" + StateMachine.AttackCount);
            float animDelay = 1;
            switch (StateMachine.AttackCount)
            {
                case 1:
                    //animDelay = 0.2f;
                    animDelay = _firstJudgeDelay;
                    time = _firstAttackDelay;
                    //time = 0.35f;
                    break;
                case 2:
                    //animDelay = 0.4f;
                    animDelay = _secondJudgeDelay;
                    time = _secondAttackDelay;
                    //time = 0.6f;
                    break;
                case 3:
                    //animDelay = 0.2f;
                    animDelay = _thirdJudgeDelay;
                    time = _thirdAttackDelay;
                    //time = 0.6f;
                    break;
            }
            //Debug.Log($"sm = {stateMachine.AttackCount} / attackDelay = {animDelay} / time {time }");
            // 딜레이가 끝난 이후 추가 키 입력이 들어가면? 
            if (_currentTimer > animDelay)
            {
                StateMachine.animator.SetInteger(Attack, 0);
                // 공격 진행
                if (_damageInCount == false) Shoot();
                if (StateMachine.attackAction.triggered && StateMachine.AttackCount < 3)
                {
                    // 다음 공격의 게이지가 100일 경우 Ultimate공격을 진행 아닐 경우 attackState
                    if (StateMachine.playable.UltimateGage >= 100)
                    {
                        StateMachine.AttackCount = 0;
                        StateMachine.animator.SetInteger(Attack, 0);
                        StateMachine.ChangeState(StateMachine.ultimateState);
                    }
                    else
                    {
                        StateMachine.ChangeState(StateMachine.attackState);
                    }
                }
                // 모션이 끝나면?
                else if (_currentTimer >= time)
                {
                    // 모션이 끝났으니 기본 상태로 되돌아감.
                    StateMachine.animator.SetTrigger(Idle);
                    StateMachine.AttackCount = 0;
                    StateMachine.animator.SetInteger(Attack, 0);
                    StateMachine.ChangeState(StateMachine.idleState);
                    return;
                }
            }
        }

        private void Shoot()
        {
            _damageInCount = true;
            StateMachine.ShootProjectile();
        }
    }

}