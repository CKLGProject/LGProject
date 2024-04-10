using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
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
        float curTimer;
        float aniDelay = 0;
        float comboDelay = 0;
        bool movingAttack = false;
        bool damageInCount = false;
        //float aniDelay

        public AttackState(PlayerStateMachine _stateMachine, ref float _comboDelay, ref float _animDelay, ref bool _movingAttack) : base(_stateMachine)
        {
            comboDelay = _comboDelay;
            aniDelay = _animDelay;
            curTimer = 0;
            movingAttack = _movingAttack;
        }

        public override void Enter()
        {
            base.Enter();
            // 어디서 왔는지 체크가 필요할까?
            curTimer = 0;

            if (Mathf.Abs(stateMachine.moveAction.ReadValue<float>() ) > 0.2f)
            {
                // 물리 초기화 X 
                return;
            }

            stateMachine.attackCount++;
            Vector3 temp = stateMachine.physics.velocity;
            temp.x = 0;
            temp.z = 0;
            stateMachine.physics.velocity = temp;
            Debug.Log($"AttackCount = {stateMachine.attackCount}");

            // 전진 어택
            if(movingAttack)
                stateMachine.physics.velocity += Vector3.right * 1.5f;
            damageInCount = false;
        }

        public override void Exit()
        {

        }
        // 앞에 있는 친구를 때릴 것인가?
        // 어떻게?
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 코루틴이나 쓰레드 등으로 카운팅 또는 불리안 값을 사용하여 상태를 변경해 줄 예정
            // 공격 시엔 콤보 입력도 필요할 것이라 생각하기 때문에 히트 스테이트나 다운 스테이트 등이 필요할 것으로 예상됨.
            // 그럼 공격은 어떻게 할 것인가? 
            curTimer += Time.deltaTime;
            if(!damageInCount)
            {
                Vector3 right = Vector3.right * (stateMachine.playable.directionX == true ? 1 : -1);
                Vector3 center = stateMachine.transform.position + right;
                // 생각보다 판정이 후하진 않게 하기
                // hit box의 크기를 따라감.
                Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f);
                // 박스 내부에 들어온 적을 생각했을 때, Playable Character와 가까운 적을 타겟으로 삼는다.
                System.Tuple<Transform, float> temp = null;
                
                foreach(var t in targets)
                {
                    float distance = Vector3.Distance(center, t.transform.position);
                    if(temp == null || (temp.Item2 >= distance && t.transform != stateMachine.transform))
                    {
                        temp = System.Tuple.Create(t.GetComponent<Transform>(), distance);
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
                        Vector3 v = stateMachine.playable.CaculateVelocity(temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position + temp.Item1.GetComponent<Playable>().GetStateMachine.transform.right, temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position, 0.2f, 0.5f);
                        // 스테이트 머신을 가져와야 한다. 어떻게 가져올까?
                        if(temp.Item1 != stateMachine.transform)
                        {
                            temp.
                           Item1.GetComponent<Playable>().
                           GetStateMachine.
                           HitDamaged(stateMachine.attackCount - 1 < 2 ? Vector3.zero : v);
                            damageInCount = true;
                            temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = stateMachine.transform;
                            //Debug.Log($"Attack In Count = {stateMachine.attackCount}");
                            
                        }
                    }
                    catch
                    {
                        Debug.Log("AA");
                    }
                }
            }

            #region ComboSystem

            // 딜레이가 끝난 이후 추가 키 입력이 들어가면? 
            if (curTimer >= comboDelay && stateMachine.attackAction.triggered && stateMachine.attackCount < 3)
            {
                // 공격 진행
                stateMachine.ChangeState(stateMachine.playable.attackState);
            }
            // 모션이 끝나면?
            else if (curTimer >= aniDelay)
            {
                // 모션이 끝났으니 기본 상태로 되돌아감.
                stateMachine.attackCount = 0;
                stateMachine.ChangeState(stateMachine.playable.idleState);
                return;
            }
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

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

        }
    }

}