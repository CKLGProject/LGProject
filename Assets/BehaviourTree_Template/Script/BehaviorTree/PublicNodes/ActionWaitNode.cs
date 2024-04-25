using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class ActionWaitNode : ActionNode
    {
        private float curTimer = 0;
        public float judgmentTime = 0;
        bool damageInCount = false;
        protected override void OnStart()
        {
        
        }

        protected override void OnStop()
        {
        
        }

        protected override State OnUpdate()
        {
            // �ð� ���� ���� ���� ���� ���� ������ OK
            curTimer += Time.deltaTime;
            
            if(judgmentTime > curTimer)
            {
                // Boolean type Method Running �� 
                return State.Running;
            }
            return State.Failure;
        }

        private bool AttackRange()
        {
            // ������ �����.
            Vector3 right = Vector3.right * (agent.directionX == true ? 1 : -1);
            Vector3 center = agent.transform.position + right;
            // �������� ������ ������ �ʰ� �ϱ�
            // hit box�� ũ�⸦ ����.
            Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f);
            System.Tuple<Transform, float> temp = null;

            foreach (var t in targets)
            {
                float distance = Vector3.Distance(center, t.transform.position);
                if (temp == null || (temp.Item2 >= distance && t.transform != agent.transform))
                {
                    temp = System.Tuple.Create(t.GetComponent<Transform>(), distance);
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
                    Vector3 v = agent.CaculateVelocity(
                        temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position + (temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position - agent.transform.position).normalized,
                        temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position, 0.5f, 0.5f);
                    // ������Ʈ �ӽ��� �����;� �Ѵ�. ��� �����ñ�?
                    if (temp.Item1 != agent.transform)
                    {
                        temp.
                        Item1.GetComponent<Playable>().
                        GetStateMachine.
                        HitDamaged(agent.GetStateMachine.attackCount - 1 < 2 ? Vector3.zero : v);
                        damageInCount = true;
                        temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = agent.transform;
                        //Debug.Log($"Attack In Count = {stateMachine.attackCount}");
                        return true;
                    }
                }
                catch
                {

                }
            }
            return false;
        }
    }

}