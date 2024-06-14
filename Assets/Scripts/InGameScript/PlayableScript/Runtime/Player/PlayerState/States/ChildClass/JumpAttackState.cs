using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGProject.PlayerState
{
    public class JumpAttackState : State
    {
        private float _maximumSpeed;
        private bool _damageInCount;
        private static readonly int JumpAttack = Animator.StringToHash("JumpAttack");
        private static readonly int Landing = Animator.StringToHash("Landing");
        public JumpAttackState(PlayerStateMachine stateMachine, float maximumSpeed) : base(stateMachine)
        {
            _maximumSpeed = maximumSpeed;
        }

        public override void Enter()
        {
            base.Enter();
            // Velocity는 초기화시키지 않도록 하자.
            //Debug.Log($"Enter = {StateMachine.JumpInCount}");
            StateMachine.animator.ResetTrigger("Jump1");
            StateMachine.animator.ResetTrigger("Jump2");
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.JumpAttack, 0.1f).Forget();
            StateMachine.animator.SetTrigger(JumpAttack);
            _damageInCount = false;
        }

        public override void Exit()
        {
            base.Exit();
            //stateMachine.jumpInCount = 0;
            //Debug.Log($"Exit = {StateMachine.JumpInCount}");
            StateMachine.animator.SetTrigger(Landing);

            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.JumpAttack);

        }

        public override void LogicUpdate()
        {
            //base.LogicUpdate();

            if (Damaged())
                return;
            // 점프 공격을 한 상태에서 움직이게 할 것인가?
            // 점프 공격을 한 상태에서 체공 시간을 늘릴 것인가?
            // 1단 점프를 한 후 공격을 한 상태라면 2단 점프가 가능하게 할 것인가?

            if (_damageInCount == false)
                AttackJudge();

            if (Mathf.Abs(StateMachine.moveAction.ReadValue<float>()) >= 0.2f)
            {
                // 진행 방향에 적이 있어? 없으면 이동
                if (StateMachine.CheckEnemy() == null && StateMachine.physics.velocity.x <= _maximumSpeed && StateMachine.physics.velocity.x >= -_maximumSpeed)
                    StateMachine.physics.velocity += Vector3.right * (StateMachine.moveAction.ReadValue<float>());
            }
            else
            {
                StateMachine.StandingVelocity();
            }

            // 공격 판정 -> 아직 없음.

            if (StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.idleState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

        }

        public void AttackJudge()
        {
            if (!_damageInCount)
            {
                Vector3 right = Vector3.right * (StateMachine.playable.directionX == true ? 0.5f : -0.5f);
                Vector3 center = StateMachine.transform.position + right + Vector3.up * 0.5f;
                // 생각보다 판정이 후하진 않게 하기
                // hit box의 크기를 따라감.
                Vector3 hitBox = Vector3.one * 0.7f;
                hitBox.x *= 1.3f;
                Collider[] targets = Physics.OverlapBox(center, hitBox, Quaternion.identity, 1 << 3);
                // 박스 내부에 들어온 적을 생각했을 때, Playable Character와 가까운 적을 타겟으로 삼는다.
                System.Tuple<Playable, float> temp = null;

                foreach (var t in targets)
                {
                    float distance = Vector3.Distance(center, t.transform.position);
                    if (temp == null || (temp.Item2 >= distance && t.transform != StateMachine.transform))
                    {
                        if (t.transform != StateMachine.transform)
                            temp = System.Tuple.Create(t.GetComponent<Playable>(), distance);
                    }
                }
                if (temp == null)
                {
                    _damageInCount = false;
                }
                else
                {
                    try
                    {
                        Vector3 direction = (temp.Item1.GetStateMachine.transform.position - StateMachine.transform.position).normalized;
                        direction.x *= 2;
                        direction.y *= 1.5f;
                        //Vector3 velocity = (StateMachine.transform.forward * 3f + StateMachine.transform.up * 1.5f) * 1.5f;

                        // 가드를 올리지 않았을 경우
                        if (temp.Item1 != StateMachine.transform)
                        {
                            temp.
                            Item1.GetStateMachine.
                            ApplyHitDamaged(Vector3.zero, 0, StateMachine, DATA_TYPE.DashAttackHit);
                            _damageInCount = true;

                            if (!temp.Item1.GetStateMachine.IsGuard && !temp.Item1.GetStateMachine.IsDown && !temp.Item1.GetStateMachine.IsSuperArmor && !StateMachine.IsUseUltimate)
                            {
                                // 100 % gage로 일단 계산
                                StateMachine.playable.SetUltimateGage(StateMachine.playable.UltimateGage + 10);
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
    }
}