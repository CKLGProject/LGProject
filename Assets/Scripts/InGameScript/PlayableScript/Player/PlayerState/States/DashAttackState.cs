using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace LGProject.PlayerState
{
    // 상속은 다음 기회에 
    public class DashAttackState : State
    {
        private float _curTimer;
        private float _aniDelay = 0;
        private bool damageInCount;
        public DashAttackState(PlayerStateMachine _stateMachine, ref float _aniDelay) : base(_stateMachine)
        {
            this._aniDelay = _aniDelay;
            //aniDelay = 
        }

        public override void Enter()
        {
            base.Enter();
            _curTimer = 0;
            stateMachine.animator.SetTrigger("DashAttack");
            damageInCount = false;
            // velocity 초기화 X
            // 그런데 브레이크는 걸면 좋을 듯? 대충 Drag값 조절해서 끼이익 하는 느낌을 줘보자.
            //Debug.Log("Sert");
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _curTimer += Time.deltaTime;
            if(_curTimer > 0.2f )
            {
                if (damageInCount == false) AttackJudge();
            }
            if (_curTimer > stateMachine.GetAnimPlayTime("DashAttack")) 
            {
                stateMachine.ChangeState(stateMachine.playable.idleState);
                return ;
            }
            // 공격 판정 
            // -> 아직 없음.

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
        }

        public override void Exit()
        {
            base.Exit();
        }

        public void AttackJudge()
        {

            if (!damageInCount)
            {
                Vector3 right = Vector3.right * (stateMachine.playable.directionX == true ? 1 : -1);
                Vector3 center = stateMachine.transform.position + right + Vector3.up * 0.5f;
                // 생각보다 판정이 후하진 않게 하기
                // hit box의 크기를 따라감.
                Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f, Quaternion.identity, 1 << 3);
                // 박스 내부에 들어온 적을 생각했을 때, Playable Character와 가까운 적을 타겟으로 삼는다.
                System.Tuple<Playable, float> temp = null;

                foreach (var t in targets)
                {
                    float distance = Vector3.Distance(center, t.transform.position);
                    if (temp == null || (temp.Item2 >= distance && t.transform != stateMachine.transform))
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
                           temp.Item1.GetStateMachine.transform.position + direction * 1.2f,
                              temp.Item1.GetStateMachine.transform.position, 0.5f, 0.75f);
                        // 가드를 올리지 않았을 경우
                        if (temp.Item1 != stateMachine.transform)
                        {
                            temp.
                            Item1.GetStateMachine.
                            HitDamaged(v);
                            damageInCount = true;

                            temp.Item1.effectManager.PlayOneShot(EffectManager.EFFECT.Hit);
                        }
                    }
                    catch
                    {
                        Debug.Log("AA");
                    }
                }
            }
        }
    }

}