using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    // Attack State�� Defualt Attack State�� ��ӹ޾� ���� ������ ���Ҿ�
    // ���� ������Ƽ�� ���� ���� �� �ֵ��� �ؾ߰ڴ�.
    public class AttackState : State
    {
        // üũ �ؾ��� ��
        // ������ �������� �ϴ°�?
        // ���߿� �ִ°�?
        // �ɾ� �ִ°�?
        // �޸��� �ִ°�?
        // 

        //float AttackCont = 0;
        //int maximumCount = 0;
        float curTimer;
        float aniDelay = 0;
        float comboDelay = 0;
        bool movingAttack = false;
        bool damageInCount = false;
        //float aniDelay

        public AttackState(PlayerStateMachine _stateMachine, ref float _comboDelay, ref float _animDelay, ref bool _movingAttack) : base(_stateMachine)
        {
            comboDelay = _comboDelay;
            aniDelay = _animDelay;
            curTimer = 0;
            movingAttack = _movingAttack;
        }

        public override void Enter()
        {
            base.Enter();
            // ��� �Դ��� üũ�� �ʿ��ұ�?
            curTimer = 0;

            if (Mathf.Abs(stateMachine.moveAction.ReadValue<float>() ) > 0.2f)
            {
                // ���� �ʱ�ȭ X 
                return;
            }

            stateMachine.attackCount++;
            Vector3 temp = stateMachine.physics.velocity;
            temp.x = 0;
            temp.z = 0;
            stateMachine.physics.velocity = temp;
            Debug.Log($"AttackCount = {stateMachine.attackCount}");

            // ���� ����
            if(movingAttack)
                stateMachine.physics.velocity += Vector3.right * 1.5f;
            damageInCount = false;
        }

        public override void Exit()
        {

        }
        // �տ� �ִ� ģ���� ���� ���ΰ�?
        // ���?
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // �ڷ�ƾ�̳� ������ ������ ī���� �Ǵ� �Ҹ��� ���� ����Ͽ� ���¸� ������ �� ����
            // ���� �ÿ� �޺� �Էµ� �ʿ��� ���̶� �����ϱ� ������ ��Ʈ ������Ʈ�� �ٿ� ������Ʈ ���� �ʿ��� ������ �����.
            // �׷� ������ ��� �� ���ΰ�? 
            curTimer += Time.deltaTime;
            if(!damageInCount)
            {
                Vector3 right = Vector3.right * (stateMachine.playable.directionX == true ? 1 : -1);
                Vector3 center = stateMachine.transform.position + right;
                // �������� ������ ������ �ʰ� �ϱ�
                // hit box�� ũ�⸦ ����.
                Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f);
                // �ڽ� ���ο� ���� ���� �������� ��, Playable Character�� ����� ���� Ÿ������ ��´�.
                System.Tuple<Transform, float> temp = null;
                
                foreach(var t in targets)
                {
                    float distance = Vector3.Distance(center, t.transform.position);
                    if(temp == null || (temp.Item2 >= distance && t.transform != stateMachine.transform))
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
                        Vector3 v = stateMachine.playable.CaculateVelocity(temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position + temp.Item1.GetComponent<Playable>().GetStateMachine.transform.right, temp.Item1.GetComponent<Playable>().GetStateMachine.transform.position, 0.2f, 0.5f);
                        // ������Ʈ �ӽ��� �����;� �Ѵ�. ��� �����ñ�?
                        if(temp.Item1 != stateMachine.transform)
                        {
                            temp.
                           Item1.GetComponent<Playable>().
                           GetStateMachine.
                           HitDamaged(stateMachine.attackCount - 1 < 2 ? Vector3.zero : v);
                            damageInCount = true;
                            temp.Item1.GetComponent<Playable>().GetStateMachine.hitPlayer = stateMachine.transform;
                            //Debug.Log($"Attack In Count = {stateMachine.attackCount}");
                            
                        }
                    }
                    catch
                    {
                        Debug.Log("AA");
                    }
                }
            }

            #region ComboSystem

            // �����̰� ���� ���� �߰� Ű �Է��� ����? 
            if (curTimer >= comboDelay && stateMachine.attackAction.triggered && stateMachine.attackCount < 3)
            {
                // ���� ����
                stateMachine.ChangeState(stateMachine.playable.attackState);
            }
            // ����� ������?
            else if (curTimer >= aniDelay)
            {
                // ����� �������� �⺻ ���·� �ǵ��ư�.
                stateMachine.attackCount = 0;
                stateMachine.ChangeState(stateMachine.playable.idleState);
                return;
            }
            #endregion

            #region SingleAttack
            // ��Ÿ ����
            //if(curTimer >= aniDelay)
            //{
            //    stateMachine.attackCount = 0;
            //    stateMachine.ChangeState(stateMachine.playable.idleState);
            //}

            #endregion

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

        }
    }

}