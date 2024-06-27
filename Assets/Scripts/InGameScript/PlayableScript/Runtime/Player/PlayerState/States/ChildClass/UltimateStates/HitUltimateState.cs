using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;


namespace LGProject.PlayerState
{
    public class HitUltimateState : UltimateState
    {
        // 기술을 사용하면 자신을 제외한 모든 오브젝트는 정지된다!
        // 배경은 움직임
        // 공격을 진행 시 
        private pathFinding.Node _movingNode;
        private Vector3 _movingPoint;
        private List<pathFinding.Node> _nodeList;
        private bool _isAttack;

        private RaycastHit hit;

        private static readonly int Ultimate = Animator.StringToHash("Ultimate");


        public HitUltimateState(PlayerStateMachine stateMachine, float delay ) : base(stateMachine, delay)
        {
            _nodeList = pathFinding.Grid.Instance.WalkableNodeList;
            _nodeList.Reverse();
            UltimateSFXName = "Hit_Ultimate";
        }

        public override void Enter()
        {
            base.Enter();
            StateMachine.ResetVelocity();
            StateMachine.animator.SetTrigger(Ultimate);
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePreCenter).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePreRHand).Forget();

            StateMachine.IsUseUltimate = true;
            StateMachine.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _isAttack = false;
            StateMachine.battleModel.ShowCutScene(Data.ActorType.User, true);
            StateMachine.PlayAudioClip(UltimateReady);
            UsingUltimateSkill().Forget();
            WaitStart().Forget();
            Time.timeScale = 0.1f;
        }

        public override void Exit()
        {
            base.Exit();
            StateMachine.IsUseUltimate = false;
        }
        //RaycastHit hit;
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 로직의 경우 한 번 누르면 작동함.
            // 바라보고 있는 방향으로 돌진
            // velociy 돌진이 아니라 해당 위치까지 이동을 시켜주는 것이 좋음
            float distance = Vector3.Distance(StateMachine.transform.position, _movingPoint);
            // 거리가 다 되지 않았으면서 움직일 수 있고, 진행 중 플랫폼이 존재하지 않을 때 까지 이동하기.
            if (distance >= 1f && _isAttack && StateMachine.CheckPlatform(StateMachine.transform.position + StateMachine.transform.forward * 1.5f + StateMachine.transform.up * -0.5f))
            {

                // 현재 6칸 / 6m 이동 중 6m사이에 적이 있으면 해당 적 앞에서 멈추고 애니메이션 실행 
                StateMachine.transform.position = Vector3.MoveTowards(StateMachine.transform.position, _movingPoint, 100f * Time.deltaTime);
            }
            else if (_isMove)
            {
                StateMachine.ChangeState(StateMachine.idleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        // 이동 경로에 적이 존재하는가에 대한 의문
        private bool PathCheck()
        {
            if(Physics.Raycast(StateMachine.transform.position + Vector3.up * .5f, StateMachine.playable.directionX ?  Vector3.right : Vector3.left, out hit, 4f))
            {
                return true;
            }
            return false;
        }

        private void MovementPointSet()
        {

            // 앞으로 5거리만큼 이동.
            _movingPoint = StateMachine.transform.position + StateMachine.transform.forward * 4f;
        }

        private void MovementTargetPointSet()
        {
            PlayerStateMachine targetStateMachine = hit.transform.GetComponent<Playable>().GetStateMachine;

            pathFinding.Node node = pathFinding.Grid.Instance.NodeFromWorldPoint(hit.transform.position);
            _movingPoint = node.WorldPosition;
            // 그리고 타겟에게 피해를 입힘.
            Vector3 KnockbackValue = (StateMachine.transform.forward * 9f + StateMachine.transform.up * 9f) ;

            StateMachine.PlayAudioClip(UltimateSFXName);
           
            targetStateMachine.ApplyHitDamaged(KnockbackValue, 0, StateMachine);
        }

        protected async UniTaskVoid WaitStart()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.75f), DelayType.Realtime);
            //StateMachine.battleModel.PlayAnimatorControllerTrigger("Hide");
            StateMachine.battleModel.ShowCutScene(Data.ActorType.User, false);
            _isAttack = true;
            Time.timeScale = 1f;
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Ultimate);
            StateMachine.animator.updateMode = AnimatorUpdateMode.Normal;
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimateDash).Forget();
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.UltimatePreCenter);
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.UltimatePreRHand);

            //stateMachine.animator.stop

            if (PathCheck())
            {
                MovementTargetPointSet();
            }
            else
            {
                MovementPointSet();
            }
            StateMachine.playable.FocusUltimateUser(OriginWeight);
            await UniTask.Delay(TimeSpan.FromSeconds(2f), DelayType.Realtime);
            //StateMachine.battleModel.ShowCutScene(Data.ActorType.User, false);
        }
    }
}