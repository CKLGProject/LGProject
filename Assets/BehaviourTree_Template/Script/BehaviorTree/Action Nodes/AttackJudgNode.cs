using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackJudgNode : ActionNode
    {
        public float judgTimer = 0;
        public float animTimer = 0;
        private float curTimer = 0;
        private bool isAttack = false;
        protected override void OnStart()
        {
            agent.GetStateMachine.attackCount++;
            isAttack = false;
            agent.GetStateMachine.isNormalAttack = true;
        }

        protected override void OnStop()
        {
            agent.GetStateMachine.isNormalAttack = false;
        }

        protected override State OnUpdate()
        {
            curTimer += Time.deltaTime;
            if(judgTimer > curTimer)
            {
                // �ִϸ��̼��� ���� ���� ������ ���� -> �������� �ִµ� �����ϸ� ���� ����, �ð��� ������ ���� ���ϸ� Idle
                // �ִϸ��̼��� ��� ���̶�� Running
                if(isAttack == false) isAttack = ActionJudge();
                if (curTimer > animTimer && isAttack)
                {
                    curTimer = 0;
                    return State.Success;
                }
                return State.Running;
            }
            curTimer = 0;
            return State.Failure;
        }

        private bool ActionJudge()
        {
            // ���� ���� ���.
            Vector3 right = Vector3.right * (agent.directionX == true ? 1 : -1);
            Vector3 center = agent.transform.position + right;

            Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f);
            System.Tuple<Transform, float> temp = null;

            foreach(var target in targets)
            {
                float distance = Vector3.Distance(center, target.transform.position);
                if(temp == null || (temp.Item2 >= distance && target.transform != agent.transform))
                {
                    temp = System.Tuple.Create(target.transform, distance);
                }
            }

            // ���� ������ ���� �������� ����.
            if(temp == null)
            {
                // ���� ������ �������� ����.
                return false;
            }
            else
            {
                Vector3 v= agent.CaculateVelocity(
                        temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position + (temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position - agent.transform.position).normalized,
                        temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position, 0.5f, 0.5f);
                if (temp.Item1 != agent.transform)
                {
                    temp.
                    Item1.GetComponent<Playable>().
                    GetStateMachine.
                    HitDamaged(agent.GetStateMachine.attackCount - 1 < 2 ? Vector3.zero : v);
                    //damageInCount = true; <- �̰� �� �����غ��� �� ��...
                    temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = agent.transform;
                    //Debug.Log($"Attack In Count = {stateMachine.attackCount}");

                    return true;
                }
            }
            return false;
        }
    }
}