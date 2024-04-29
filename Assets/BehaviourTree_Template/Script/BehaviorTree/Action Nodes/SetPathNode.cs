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
        // 움직일 포인트를 찍어줌
        // 움직일 포인트가 찍히면 MoveNode로 움직임을 체크하고 목표지점에 도달할 때까지
        // (Ex -> 0.5거리 이하 )
        // 계속 이동.
        // 앞으로 가는 와중 길이 없으면, 점프를 한다. 점프의 경우 2단까지 가능하나
        // 2단 점프의 경우는 아직 로직을 생각 안해둠.
        //pathFinding.Grid
        //private pathFinding.Grid grid;
        //SetPathNode()
        //{
        //    grid = GameObject.Find("A*").GetComponent<pathFinding.Grid>();
        //}

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
            // 이거 에러 있음 잘 안받아옴
            PathRequestManager.RequestPath(new PathRequest(agent.transform.position, Grid.Instance.GetRandPoint(), agent.GetPath));
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (agent.path == null)
                return State.Running;
            else
                return State.Success;   
            #region Legarcy

            //// 경로를 잘 받아왔는가?
            //if (agent.target != null)
            //    return State.Success;

            //return State.Failure;

            #endregion

        }
    }

}