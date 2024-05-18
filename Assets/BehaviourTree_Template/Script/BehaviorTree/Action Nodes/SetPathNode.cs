using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pathFinding;

using Grid = pathFinding.Grid;

namespace BehaviourTree
{
    
    // 경로를 세팅 
    public class SetPathNode : ActionNode
    {
        public AIAgent Agent;
        // 움직일 포인트를 찍어줌
        // 움직일 포인트가 찍히면 MoveNode로 움직임을 체크하고 목표지점에 도달할 때까지
        // (Ex -> 0.5거리 이하 )
        // 계속 이동.
        // 앞으로 가는 와중 길이 없으면, 점프를 한다. 점프의 경우 2단까지 가능하나
        // 2단 점프의 경우는 아직 로직을 생각 안해둠.

        protected override void OnStart()
        {
            #region Legarcy
            //if (agent.target != null)
            //{
            //    agent.target.GetComponent<Renderer>().material.color
            //        = new Color(Color.black.r, Color.black.g, Color.black.b, 0.25f);
            //}
            //agent.target = LGProject.MovePointManager.Instance.GetPoint();
            //agent.target.GetComponent<Renderer>().material.color
            //    = new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
            //pathFinding.PathRequestManager.RequestPath();
            #endregion
            if (Agent == null)
                Agent = AIAgent.Instance;
            
            // 상대방을 기준으로 공격을 진행해야함.
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // 타이머를 재생해서 0이 되면 실행되게 하기

            if (Agent.GetStateMachine.IsDamaged)
                return State.Failure;
            if (Agent.path == null)
                return State.Running;
            else
            {
                //_curTimer = 0;
                PathRequestManager.RequestPath(new PathRequest(Agent.transform.position, Grid.Instance.GetRandPoint(Agent.player.position), Agent.GetPath));
                return State.Success;
            }
            #region Legarcy

            //// 경로를 잘 받아왔는가?
            //if (agent.target != null)
            //    return State.Success;

            //return State.Failure;

            #endregion

        }
    }

}