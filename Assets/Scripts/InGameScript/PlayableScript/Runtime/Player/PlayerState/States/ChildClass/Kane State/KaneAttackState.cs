using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class KaneAttackState : AttackState
    {
        
        public KaneAttackState(PlayerStateMachine stateMachine, ref float firstJudgeAttack, ref float firstAttackDelay, ref float secondJudgeAttack, ref float secondAttackDelay, ref float thirdJudgeAttack, ref float thirdAttackDelay) : base(stateMachine, ref firstJudgeAttack, ref firstAttackDelay, ref secondJudgeAttack, ref secondAttackDelay, ref thirdJudgeAttack, ref thirdAttackDelay)
        {
            CurrentTimer = 0;

            FirstJudgeDelay = firstJudgeAttack;
            FirstAttackDelay = firstAttackDelay;
            SecondJudgeDelay = secondJudgeAttack;
            SecondAttackDelay = secondAttackDelay;
            ThirdJudgeDelay = thirdJudgeAttack;
            ThirdAttackDelay = thirdAttackDelay;
            AttackSoundName = "Kane_Attack";
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
            {
                StateMachine.AttackCount = 0;
                return;
            }
            CurrentTimer += Time.deltaTime;
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
                    animDelay = FirstJudgeDelay;
                    time = FirstAttackDelay;
                    //time = 0.35f;
                    break;
                case 2:
                    //animDelay = 0.4f;
                    animDelay = SecondJudgeDelay;
                    time = SecondAttackDelay;
                    //time = 0.6f;
                    break;
                case 3:
                    //animDelay = 0.2f;
                    animDelay = ThirdJudgeDelay;
                    time = ThirdAttackDelay;
                    //time = 0.6f;
                    break;
            }
            //Debug.Log($"sm = {stateMachine.AttackCount} / attackDelay = {animDelay} / time {time }");
            // 딜레이가 끝난 이후 추가 키 입력이 들어가면? 
            if (CurrentTimer > animDelay)
            {
                StateMachine.animator.SetInteger(Attack, 0);

                if (DamageInCount == false) Shoot();
                if (StateMachine.attackAction.triggered && StateMachine.AttackCount < 3)
                {
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
                else if (CurrentTimer >= time)
                {
                    StateMachine.AttackCount = 0;
                    StateMachine.animator.SetInteger(Attack, 0);
                    StateMachine.ChangeState(StateMachine.idleState);
                    return;
                }
            }
        }

        private void Shoot()
        {
            #region 오디오 출력
            StateMachine.PlayAudioClip(AttackSoundName);
            #endregion
            DamageInCount = true;
            StateMachine.ShootProjectile();
        }
    }

}