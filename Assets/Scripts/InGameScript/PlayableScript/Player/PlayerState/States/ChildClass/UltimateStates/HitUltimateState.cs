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
        private bool _isMove;

        private RaycastHit hit;
        public HitUltimateState(PlayerStateMachine _stateMachine) : base(_stateMachine)
        {
            _nodeList = pathFinding.Grid.Instance.walkableNodeList;
            _nodeList.Reverse();
            //_delay = _delayTime;
        }

        public override void Enter()
        {
            base.Enter();
            stateMachine.animator.SetTrigger("Ultimate");
            stateMachine.IsUltimate = true;
            stateMachine.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _isMove = false;
            WaitStart().Forget();
            Time.timeScale = 0.1f;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 로직의 경우 한 번 누르면 작동함.
            // 바라보고 있는 방향으로 돌진
            // velociy 돌진이 아니라 해당 위치까지 이동을 시켜주는 것이 좋음
            float distance = Vector3.Distance(stateMachine.transform.position, _movingPoint);
            if (distance >= 1f && _isMove)
            {

                // 현재 6칸 / 6m 이동 중 6m사이에 적이 있으면 해당 적 앞에서 멈추고 애니메이션 실행 
                stateMachine.transform.position = Vector3.MoveTowards(stateMachine.transform.position, _movingPoint, 1000f * Time.deltaTime);
            }
            else if (_isMove)
                stateMachine.ChangeState(stateMachine.idleState);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        // 이동 경로에 적이 존재하는가에 대한 의문
        private bool PathCheck()
        {
            if(Physics.Raycast(stateMachine.transform.position + Vector3.up * .5f, stateMachine.playable.directionX ?  Vector3.right : Vector3.left, out hit, 6f))
            {
                return true;
            }
            return false;
        }

        private void AttackJudge()
        {

        }

        private void MovementPointSet()
        {
            int pointX = 0;
            int pointY = 0;
            pathFinding.Node node = pathFinding.Grid.Instance.NodeFromWorldPoint(stateMachine.transform.position);
            pointX = node.gridX;
            pointY = node.gridY;
            // 맥스치는 0 ~ 19 1/3만큼 이동할 것이다 그럼?;
            int point = 19 / 3; // 6;
            pointX = stateMachine.playable.directionX ? pointX + point : pointX - point;
            pointX = pointX < 1 ? 1 : pointX;
            pointX = pointX > 19 ? 18 : pointX;

            _movingNode = _nodeList[pointX];
            _movingPoint = new Vector3(_movingNode.worldPosition.x, stateMachine.transform.position.y, stateMachine.transform.position.z);
        }

        private void MovementTargetPointSet()
        {
            PlayerStateMachine targetStateMachine = hit.transform.GetComponent<Playable>().GetStateMachine;
            pathFinding.Node node = pathFinding.Grid.Instance.NodeFromWorldPoint(hit.transform.position);
            _movingPoint = node.worldPosition;
            // 그리고 타겟에게 피해를 입힘.
            Vector3 KnockbackValue = Vector3.zero;
            Vector3 direction = (targetStateMachine.transform.position - stateMachine.transform.position).normalized;
            direction.x *= 2;
            direction.y *= 1.5f;
            KnockbackValue = stateMachine.playable.CaculateVelocity(
               targetStateMachine.transform.position + direction,
                  targetStateMachine.transform.position, 0.5f, 3f);

            targetStateMachine.HitDamaged(KnockbackValue);


        }

        protected async UniTaskVoid WaitStart()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.175f));
            _isMove = true;
            stateMachine.IsUltimate = false;
            Time.timeScale = 1f;
            stateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Ultimate);
            stateMachine.animator.updateMode = AnimatorUpdateMode.Normal;
            //stateMachine.animator.stop
            // 플레이어 앞에 있는 놈들 전부 잡아 족쳐
            if (PathCheck())
            {
                MovementTargetPointSet();
            }
            else
            {
                MovementPointSet();
            }

        }

    }
}