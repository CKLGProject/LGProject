using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pathFinding;

using Grid = pathFinding.Grid;

namespace BehaviourTree
{
    // ��θ� ���� 
    public class SetPathNode : ActionNode
    {
        // ������ ����Ʈ�� �����
        // ������ ����Ʈ�� ������ MoveNode�� �������� üũ�ϰ� ��ǥ������ ������ ������
        // (Ex -> 0.5�Ÿ� ���� )
        // ��� �̵�.
        // ������ ���� ���� ���� ������, ������ �Ѵ�. ������ ��� 2�ܱ��� �����ϳ�
        // 2�� ������ ���� ���� ������ ���� ���ص�.
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
            // �̰� ���� ���� �� �ȹ޾ƿ�
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

            //// ��θ� �� �޾ƿԴ°�?
            //if (agent.target != null)
            //    return State.Success;

            //return State.Failure;

            #endregion

        }
    }

}