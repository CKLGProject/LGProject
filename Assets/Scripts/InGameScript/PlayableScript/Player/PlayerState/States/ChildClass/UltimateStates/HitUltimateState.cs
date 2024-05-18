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
            MovementPointSet();
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
                stateMachine.transform.position = Vector3.MoveTowards(stateMachine.transform.position, _movingPoint, 1000f * Time.deltaTime);
            else if (_isMove)
                stateMachine.ChangeState(stateMachine.idleState);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
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

            //nodeList[node.gridX];

        }
        protected async UniTaskVoid WaitStart()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            _isMove = true;
            Time.timeScale = 1f;
            //stateMachine.animator.stop
        }

    }
}