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

                if (DamageInCount == false) AttackJudge();
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
        public void AttackJudge()
        {

            if (!DamageInCount)
            {
                StateMachine.PlayAudioClip(AttackSoundName);
                Vector3 center = StateMachine.transform.position + StateMachine.transform.forward * 0.5f + StateMachine.transform.up * 0.5f;

                Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f, Quaternion.identity, 1 << 3);
                System.Tuple<Playable, float> temp = null;

                foreach (var t in targets)
                {
                    float distance = Vector3.Distance(center, t.transform.position);
                    if ((temp == null || temp.Item2 >= distance) && t.transform != StateMachine.transform)
                    {
                        temp = System.Tuple.Create(t.GetComponent<Playable>(), distance);
                    }
                }
                if (temp == null)
                {
                    DamageInCount = false;
                }
                else
                {
                    try
                    {
                        Vector3 direction = (temp.Item1.GetStateMachine.transform.position - StateMachine.transform.position).normalized;
                        Vector3 velocity = (StateMachine.transform.forward * 1.5f + StateMachine.transform.up * 3) * 1.5f;

                        if (temp.Item1 != StateMachine.transform)
                        {
                            temp.
                            Item1.GetStateMachine.
                            ApplyHitDamaged(StateMachine.AttackCount - 1 < 2 ? Vector3.zero : velocity, 0.1f, StateMachine);
                            DamageInCount = true;

                            if (!temp.Item1.GetStateMachine.IsGuard && !temp.Item1.GetStateMachine.IsDown && !temp.Item1.GetStateMachine.IsSuperArmor && !StateMachine.IsUseUltimate)
                            {
                                StateMachine.playable.SetUltimateGage(StateMachine.playable.UltimateGage + 10);
                                StateMachine.UltimateGageIsFull();
                            }
                        }
                    }
                    catch
                    {
#if UNITY_EDITOR
                        Debug.Log("AA");
#endif
                    }
                }
            }
        }
    }

}