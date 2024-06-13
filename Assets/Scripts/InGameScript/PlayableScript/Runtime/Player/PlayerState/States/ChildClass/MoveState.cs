using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class MoveState : State
    {
        private float _speed;
        private float _maximumSpeed;
        private bool _isPlayingMoveEffect;
        private static readonly int Run = Animator.StringToHash("Run");

        public MoveState(PlayerStateMachine stateMachine, ref float speed, float maximumSpeed) : base(stateMachine)
        {
            _speed = speed;
            _maximumSpeed = maximumSpeed;
        }

        public override void Enter()
        {
            base.Enter();

            // 방향 판정
            StateMachine.playable.directionX = StateMachine.moveAction.ReadValue<float>() >= 0.1f ? true : false;
        }

        public override void Exit()
        {
            StateMachine.animator.SetFloat(Run, 0);
        }

        public override void LogicUpdate()
        {
            // 아하! 여기서 문제가 생기는구나!
            //base.LogicUpdate();

            if (Damaged())
                return;

            //if (StateMachine.IsKnockback || StateMachine.IsDamaged)
            //    return;

            float moveThreshold = Mathf.Abs(StateMachine.moveAction.ReadValue<float>());
            StateMachine.animator.SetFloat(Run, moveThreshold);
            if (StateMachine.IsGrounded && !_isPlayingMoveEffect)
            {
                StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Run).Forget();
                _isPlayingMoveEffect = true;
            }
            else if (!StateMachine.IsGrounded && _isPlayingMoveEffect)
            {
                _isPlayingMoveEffect = false;
                StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Run);
            }

            if (moveThreshold <= 0.2f)
            {
                StateMachine.ChangeState(StateMachine.idleState);
                return;
            }

            if (StateMachine.attackAction.triggered)
            {
                AttackLogic();
                return;
            }

            if (StateMachine.jumpAction.triggered && StateMachine.JumpInCount < 2)
            {
                StateMachine.ChangeState(StateMachine.jumpState);
                return;
            }

            // 점프 가드는 한번만!!
            if (StateMachine.guardAction.triggered && StateMachine.GuardGage > 0)
            {
                StateMachine.ChangeState(StateMachine.guardState);
                return;
            }

            // 진행 방향에 적이 있어?
            if (StateMachine.CheckEnemy())
            {
                StateMachine.StandingVelocity();
                return;
            }

            Movement(_maximumSpeed);

            //if (StateMachine.physics.velocity.x <= _maximumSpeed && StateMachine.physics.velocity.x >= -_maximumSpeed)
            //{
            //    // 바로 앞에 적이 있으면 더이상 이동하지 않음(애니메이션은 재생)
            //    // 머리와 다리쪽에서 Ray를 쏠 예정
            //    StateMachine.physics.velocity += Vector3.right * (StateMachine.moveAction.ReadValue<float>());

            //    Vector3 left = new Vector3((StateMachine.transform.position + Vector3.right).x + 2f,
            //        StateMachine.transform.position.y, StateMachine.transform.position.z);
            //    Vector3 right = new Vector3((StateMachine.transform.position + Vector3.left).x - 2f,
            //        StateMachine.transform.position.y, StateMachine.transform.position.z);

            //    StateMachine.transform.LookAt(StateMachine.moveAction.ReadValue<float>() < 0 ? right : left);

            //    Vector3 euler = StateMachine.transform.GetChild(1).GetComponent<RectTransform>().localRotation.eulerAngles;
            //    Debug.Log($"{euler} / {StateMachine.transform.GetChild(1).GetComponent<RectTransform>().transform.name}");
            //    StateMachine.transform.GetChild(1).GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, StateMachine.moveAction.ReadValue<float>() < 0 ? 90 :-90, 0);
            //}
        }

        private void AttackLogic()
        {
            if (StateMachine.playable.UltimateGage >= 100)
            {
                StateMachine.ChangeState(StateMachine.ultimateState);
                return;
            }

            if (!StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.jumpAttackState);
                return;
            }

            if (StateMachine.IsGrounded && Mathf.Abs(StateMachine.physics.velocity.x) > 0.2f)
            {
                StateMachine.ChangeState(StateMachine.dashAttackState);
                return;
            }
            else
            {
                StateMachine.ChangeState(StateMachine.attackState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}