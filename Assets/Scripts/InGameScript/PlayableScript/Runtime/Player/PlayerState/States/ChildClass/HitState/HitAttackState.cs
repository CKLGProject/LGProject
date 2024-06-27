using FMODUnity;
using UnityEngine;


namespace LGProject.PlayerState
{
    public class HitAttackState : AttackState
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <param name="firstJudgeAttack"></param>
        /// <param name="firstAttackDelay"></param>
        /// <param name="secondJudgeAttack"></param>
        /// <param name="secondAttackDelay"></param>
        /// <param name="thirdJudgeAttack"></param>
        /// <param name="thirdAttackDelay"></param>

        public HitAttackState(PlayerStateMachine stateMachine, ref float firstJudgeAttack, ref float firstAttackDelay, ref float secondJudgeAttack, ref float secondAttackDelay, ref float thirdJudgeAttack, ref float thirdAttackDelay) : base(stateMachine, ref firstJudgeAttack, ref firstAttackDelay, ref secondJudgeAttack, ref secondAttackDelay, ref thirdJudgeAttack, ref thirdAttackDelay)
        {
            CurrentTimer = 0;

            FirstJudgeDelay = firstJudgeAttack;
            FirstAttackDelay = firstAttackDelay;
            SecondJudgeDelay = secondJudgeAttack;
            SecondAttackDelay = secondAttackDelay;
            ThirdJudgeDelay = thirdJudgeAttack;
            ThirdAttackDelay = thirdAttackDelay;
            AttackSoundName = "Hit_Attack";
        }

        public override void Enter()
        {
            //base.Enter();
            CurrentTimer = 0;

            #region 공격 로직 
            StateMachine.AttackCount++;
            StateMachine.StandingVelocity();
            float moveValue = 0;
            switch (StateMachine.AttackCount)
            {
                case 1:
                    moveValue = StateMachine.playable.FirstAttackMovingValue;
                    break;
                case 2:
                    moveValue = StateMachine.playable.SecondAttackMovingValue;
                    break;
                case 3:
                    moveValue = StateMachine.playable.ThirdAttackMovingValue;
                    break;
                default:
                    break;
            }
            // 공격하면서 전진.
            if (StateMachine.playable.movingAttack)
                StateMachine.physics.velocity += StateMachine.transform.forward * moveValue;
            DamageInCount = false;
            #endregion

            #region 애니메이션 출력
            StateMachine.animator.SetTrigger(GuardEnd);
            StateMachine.animator.SetInteger(Attack, StateMachine.AttackCount);
            #endregion

            #region 이펙트 출력
            switch (StateMachine.AttackCount)
            {
                case 1:
                    StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack1).Forget();
                    break;
                case 2:
                    StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack2).Forget();
                    break;
                case 3:
                    StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack3, 0.25f).Forget();
                    break;
                default:
                    break;
            }
            #endregion

            #region 오디오 출력
            StateMachine.PlayAudioClip(AttackSoundName);
            #endregion

        }

        public override void Exit()
        {

        }
        public override void LogicUpdate()
        {
            //base.LogicUpdate();
            if (Damaged())
            {
                //StateMachine.AttackCount = 0;
                return;
            }

            CurrentTimer += Time.deltaTime;

            #region ComboSystem
            AttackLogic();
            #endregion

            #region SingleAttack
            // 단타 공격
            //if(curTimer >= aniDelay)
            //{
            //    stateMachine.attackCount = 0;
            //    stateMachine.ChangeState(stateMachine.playable.idleState);
            //}

            #endregion

        }

        private void AttackLogic()
        {
            float time = StateMachine.GetAnimationPlayTime("Attack" + StateMachine.AttackCount);
            float animDelay = 1;
            switch (StateMachine.AttackCount)
            {
                case 1:
                    animDelay = FirstJudgeDelay;
                    time = FirstAttackDelay;
                    break;
                case 2:
                    animDelay = SecondJudgeDelay;
                    time = SecondAttackDelay;
                    break;
                case 3:
                    animDelay = ThirdJudgeDelay;
                    time = ThirdAttackDelay;
                    break;
            }

            if (CurrentTimer > animDelay)
            {
                StateMachine.animator.SetInteger(Attack, 0);
                if (DamageInCount == false) AttackJudge();
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


        /// <summary>
        /// 공격 판정 체크하는 매서드
        /// </summary>
        public void AttackJudge()
        {

            if (!DamageInCount)
            {
                Vector3 right = Vector3.right * (StateMachine.playable.directionX == true ? 0.7f : -0.7f);
                Vector3 center = StateMachine.transform.position + right + Vector3.up * 0.5f;

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

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

        }
    }
}