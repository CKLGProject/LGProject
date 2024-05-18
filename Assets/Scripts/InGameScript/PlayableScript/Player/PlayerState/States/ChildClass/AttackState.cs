using BehaviourTree;
using Postgrest.Exceptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    // Attack State는 Defualt Attack State를 상속받아 공격 판정과 더불어
    // 같은 프로퍼티를 공유 받을 수 있도록 해야겠다.
    public class AttackState : State
    {
        // 체크 해야할 것
        // 공격을 연속으로 하는가?
        // 공중에 있는가?
        // 앉아 있는가?
        // 달리고 있는가?
        // 

        //float AttackCont = 0;
        //int maximumCount = 0;
        float firstJudgeDelay = 0;
        float firstAttackDelay = 0;
        float secondJudgeDelay = 0;
        float secondAttackDelay = 0;  
        float thirdJudgeDelay = 0;
        float thirdAttackDelay = 0;

        float curTimer;
        bool damageInCount = false;

        public AttackState(PlayerStateMachine _stateMachine, ref float firstJudgeAttack, ref float firstAttackDelay,ref float secondJudgeAttack,ref float secondAttackDelay, ref float thirdJudgeAttack, ref float thirdAttackDelay) : base(_stateMachine)
        {
            curTimer = 0;

            this.firstJudgeDelay = firstJudgeAttack;
            this.firstAttackDelay = firstAttackDelay;
            this.secondJudgeDelay = secondJudgeAttack;
            this.secondAttackDelay = secondAttackDelay;
            this.thirdJudgeDelay = thirdJudgeAttack;
            this.thirdAttackDelay = thirdAttackDelay;
        }

        public override void Enter()
        {
            base.Enter();
            // 어디서 왔는지 체크가 필요할까?
            curTimer = 0;

            #region 공격 로직 
            stateMachine.AttackCount++;
            stateMachine.StandingVelocity();
            float moveValue = 0;
            switch (stateMachine.AttackCount)
            {
                case 1:
                    moveValue = stateMachine.playable.FirstAttackMovingValue;
                    break;
                case 2:
                    moveValue = stateMachine.playable.SecondAttackMovingValue;
                    break;
                case 3:
                    moveValue = stateMachine.playable.ThirdAttackMovingValue;
                    break;
                default:
                    break;
            }
            // 공격하면서 전진.
            if (stateMachine.playable.movingAttack)
                stateMachine.physics.velocity += stateMachine.playable.directionX ? Vector3.right * moveValue : Vector3.left * moveValue;
            damageInCount = false;
            #endregion

            #region 애니메이션 출력
            stateMachine.animator.SetInteger("Attack", stateMachine.AttackCount);
            #endregion

            #region 이펙트 출력
            switch (stateMachine.AttackCount)
            {
                case 1:
                    stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack1).Forget();
                    break;
                case 2:
                    stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack2).Forget();
                    break;
                case 3:
                    stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack3, 0.25f).Forget();
                    break;
                default:
                    break;  
            }
            #endregion
        }

        public override void Exit()
        {

        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 코루틴이나 쓰레드 등으로 카운팅 또는 불리안 값을 사용하여 상태를 변경해 줄 예정
            // 공격 시엔 콤보 입력도 필요할 것이라 생각하기 때문에 히트 스테이트나 다운 스테이트 등이 필요할 것으로 예상됨.
            // 그럼 공격은 어떻게 할 것인가? 
            
            curTimer += Time.deltaTime;

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
            float time = stateMachine.GetAnimPlayTime("Attack" + stateMachine.AttackCount.ToString());
            float animDelay = 1 ;
            switch (stateMachine.AttackCount)
            {
                case 1:
                    //animDelay = 0.2f;
                    animDelay = firstJudgeDelay;
                    time = firstAttackDelay;
                    //time = 0.35f;
                    break;
                case 2:
                    //animDelay = 0.4f;
                    animDelay = secondJudgeDelay;
                    time = secondAttackDelay;
                    //time = 0.6f;
                    break;
                case 3:
                    //animDelay = 0.2f;
                    animDelay = thirdJudgeDelay;
                    time = thirdAttackDelay;
                    //time = 0.6f;
                    break;
            }
            //Debug.Log($"sm = {stateMachine.AttackCount} / attackDelay = {animDelay} / time {time }");
            // 딜레이가 끝난 이후 추가 키 입력이 들어가면? 
            if (curTimer > animDelay)   
            {
                // 공격 진행
                if (damageInCount == false) AttackJudge();
                if (stateMachine.attackAction.triggered && stateMachine.AttackCount < 3)
                {
                    // 다음 공격의 게이지가 100일 경우 Ultimate공격을 진행 아닐 경우 attackState
                    if (stateMachine.UltimateGage >= 100)
                    {
                        stateMachine.ChangeState(stateMachine.ultimateState);
                    }
                    else
                    {
                        stateMachine.ChangeState(stateMachine.attackState);
                    }
                }
                // 모션이 끝나면?
                else if (curTimer >= time)
                {
                    // 모션이 끝났으니 기본 상태로 되돌아감.
                    stateMachine.animator.SetTrigger("Idle");
                    stateMachine.AttackCount = 0;
                    stateMachine.animator.SetInteger("Attack", 0);
                    stateMachine.ChangeState(stateMachine.idleState);
                    return;
                }
            }
        }

        public void AttackJudge()
        {

            if (!damageInCount)
            {
                Vector3 right = Vector3.right * (stateMachine.playable.directionX == true ? 0.7f : -0.7f);
                Vector3 center = stateMachine.transform.position + right + Vector3.up * 0.5f;
                // 생각보다 판정이 후하진 않게 하기
                // hit box의 크기를 따라감.
                Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f, Quaternion.identity, 1 << 3);
                // 박스 내부에 들어온 적을 생각했을 때, Playable Character와 가까운 적을 타겟으로 삼는다.
                System.Tuple<Playable, float> temp = null;

                foreach (var t in targets)
                {
                    float distance = Vector3.Distance(center, t.transform.position);
                    if ((temp == null || temp.Item2 >= distance )&& t.transform != stateMachine.transform)
                    {
                        temp = System.Tuple.Create(t.GetComponent<Playable>(), distance);
                    }
                }
                if (temp == null)
                {
                    damageInCount = false;
                }
                else
                {
                    try
                    {
                        Vector3 direction = (temp.Item1.GetStateMachine.transform.position - stateMachine.transform.position).normalized;
                        Vector3 v = stateMachine.playable.CaculateVelocity(
                           temp.Item1.GetStateMachine.transform.position + direction ,
                              temp.Item1.GetStateMachine.transform.position, 0.5f, 1f);
                        // 가드를 올리지 않았을 경우
                        if (temp.Item1 != stateMachine.transform)
                        {
                            temp.
                            Item1.GetStateMachine.
                            HitDamaged(stateMachine.AttackCount - 1 < 2 ? Vector3.zero : v);
                            damageInCount = true;

                            if (!temp.Item1.GetStateMachine.IsGuard && !temp.Item1.GetStateMachine.IsDown)
                            {// 100 % gage로 일단 계산
                                stateMachine.UltimateGage += 10;

                                stateMachine.playable.UltimateGageImage.fillAmount = stateMachine.UltimateGage / 100f;

                                temp.Item1.effectManager.PlayOneShot(EffectManager.EFFECT.Hit);
                            }
                        }
                    }
                    catch
                    {
                        Debug.Log("AA");
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