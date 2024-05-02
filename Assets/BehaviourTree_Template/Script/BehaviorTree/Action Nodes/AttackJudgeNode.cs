using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackJudgeNode : ActionNode
    {
        public PlayerStateMachine stateMachine;
        //[Space(10f)]
        [SerializeField, Range(0f, 2f)] private float attackRange;
        public float judgTimer = 0;
        public float animTimer = 0;
        private float _curTimer = 0;
        private bool _isAttack = false;
        private float _time = 0;
        protected override void OnStart()
        {
            if (stateMachine == null)
                stateMachine = AIAgent.Instance.GetStateMachine;
            if (stateMachine.attackCount > 2)
                stateMachine.attackCount = 0;
            _isAttack = false;
            stateMachine.isNormalAttack = true;
            AIAgent.Instance.SetAttacRange(attackRange);
            stateMachine.attackCount++;
            stateMachine.animator.SetInteger("Attack", stateMachine.attackCount);
            _curTimer = 0;
            _time = stateMachine.GetAnimPlayTime("Attack" + (stateMachine.attackCount).ToString());
        }

        protected override void OnStop()
        {

        }
        protected override State OnUpdate()
        {
            if (stateMachine == null)
                stateMachine = AIAgent.Instance.GetStateMachine;
            // ?? 왜 가드를 여기서 올림?
            if (stateMachine.isGuard && stateMachine.isHit)
            {
                stateMachine.animator.SetInteger("Attack", stateMachine.attackCount);
                return State.Failure;
            }

            _curTimer += Time.deltaTime;

            #region ComboSystem
            //AttackLogic();

            //return State.Success;
            #endregion

            
            switch (stateMachine.attackCount)
            {
                case 1:
                    animTimer = 0.25f;
                    break;
                case 2:
                    animTimer = 0.4f;
                    break;
                case 3:
                    animTimer = 0.4f;
                    break;
            }
            #region legarcy
            if (_curTimer > animTimer)
            {
                //Debug.Log($"{animTimer} / {_time} ");
                // 애니메이션이 끝난 이후 데미지 판정 -> 데미지를 넣는데 성공하면 다음 공격, 시간이 지나도 공격 못하면 Idle
                // 애니메이션이 재생 중이라면 Running
                if (_isAttack == false ) _isAttack = ActionJudge();
                if (_isAttack && stateMachine.attackCount < 3  && !stateMachine.isHit)
                {
                    return State.Success;
                }
                else if(_curTimer >= _time && !stateMachine.isHit)
                {
                    _curTimer = 0;
                    stateMachine.isNormalAttack = false;
                    stateMachine.attackCount = 0;
                    _isAttack = false;
                    stateMachine.animator.SetTrigger("Idle");
                    stateMachine.animator.SetInteger("Attack", stateMachine.attackCount);
                    return State.Success;
                }
            }
            return State.Running;
            #endregion
        }


        //private void AttackLogic()
        //{
        //    float time = stateMachine.GetAnimPlayTime("Attack" + stateMachine.attackCount.ToString());
        //    float animDelay = 1;
        //    switch (stateMachine.attackCount)
        //    {
        //        case 1:
        //            animDelay = 0.25f;
        //            break;
        //        case 2:
        //            animDelay = 0.4f;
        //            break;
        //        case 3:
        //            animDelay = 0.4f;
        //            break;
        //    }
        //    Debug.Log($"count: {stateMachine.attackCount} / {time}");
        //    // 딜레이가 끝난 이후 추가 키 입력이 들어가면? 
        //    if (_curTimer > animDelay)
        //    {
        //        // 공격 진행
        //        if (_damageInCount == false) AttackJudge();
        //        if (stateMachine.attackAction.triggered && stateMachine.attackCount < 3)
        //        {
        //            stateMachine.ChangeState(stateMachine.playable.attackState);
        //        }
        //        // 모션이 끝나면?
        //        else if (curTimer >= time)
        //        {
        //            // 모션이 끝났으니 기본 상태로 되돌아감.
        //            Debug.Log("Stop");

        //            stateMachine.animator.SetTrigger("Idle");
        //            stateMachine.attackCount = 0;
        //            stateMachine.animator.SetInteger("Attack", stateMachine.attackCount);
        //            stateMachine.ChangeState(stateMachine.playable.idleState);
        //            return;
        //        }
        //    }
        //}

        private bool ActionJudge()
        {
            // 판정 범위 계산.
            Vector3 right = Vector3.right * (AIAgent.Instance.directionX == true ? 1 : -1);
            Vector3 center = AIAgent.Instance.transform.position + right;

            Collider[] targets = Physics.OverlapBox(center, Vector3.one * attackRange, Quaternion.identity, 1 << 3);
            System.Tuple<Playable, float> temp = null;

            // 자기 공격에 자기가 맞음.
            foreach(var target in targets)
            {
                float distance = Vector3.Distance(center, target.transform.position);
                if((temp == null || temp.Item2 >= distance )&& target.transform != stateMachine.transform)
                {
                    temp = System.Tuple.Create(target.GetComponent<Playable>(), distance);
                    //Debug.Log(target.name);
                }
            }

            // 공격 범위에 적이 존재하지 않음.
            if(temp == null)
            {
                // 아직 공격을 성공하지 못함.
                return false;
            }
            else
            {
                Vector3 v = Vector3.zero;
                if (AIAgent.Instance.GetStateMachine.attackCount >= 3)
                {
                    Vector3 direction = (temp.Item1.GetStateMachine.transform.position - AIAgent.Instance.transform.position).normalized;
                    v = AIAgent.Instance.CaculateVelocity(
                       temp.Item1.GetStateMachine.transform.position + direction * 0.5f,
                          temp.Item1.GetStateMachine.transform.position, 0.5f, 1f);
                }
                if (temp.Item1 != AIAgent.Instance.transform/* && !temp.Item1.GetStateMachine.isGuard*/)
                {
                    temp.
                    Item1.GetStateMachine.
                    HitDamaged(AIAgent.Instance.GetStateMachine.attackCount < 3 ? Vector3.zero : v);
                    temp.Item1.GetStateMachine.hitPlayer = AIAgent.Instance.transform;

                    temp.Item1.effectManager.PlayOneShot(EffectManager.EFFECT.Hit);

                    return true;
                }
            }
            return false;
        }
    }
}