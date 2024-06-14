using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackJudgeNode : ActionNode
    {
        private PlayerStateMachine _stateMachine;
        //[Space(10f)]
        //[SerializeField, Range(0f, 2f)] private float attackRange;
        public float judgTimer = 0;
        public float animTimer = 0;
        private float _curTimer = 0;
        private bool _isAttack = false;
        private float _time = 0;

        protected override void OnStart()
        {
            StartExceptionHandling();
        }

        protected override void OnStop()
        {
            //Debug.Log("AttackJudgeNodeEnd");
        }
        protected override State OnUpdate()
        {
            #region Omit
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;
            // ?? 왜 가드를 여기서 올림?
            if (_stateMachine.IsGuard || _stateMachine.IsDamaged)
            {
                _stateMachine.animator.SetInteger("Attack", 0);
                return State.Failure;
            }

            _curTimer += Time.deltaTime;
            
            switch (_stateMachine.AttackCount)
            {
                case 1:
                    animTimer = _stateMachine.playable.FirstAttackJudgeDelay;
                    _time = _stateMachine.playable.FirstAttackDelay;
                    break;
                case 2:
                    animTimer = _stateMachine.playable.SecondAttackJudgeDelay;
                    _time = _stateMachine.playable.SecondAttackDelay;
                    break;
                case 3:
                    animTimer = _stateMachine.playable.ThirdAttackJudgeDelay;
                    _time = _stateMachine.playable.ThirdAttackDelay;
                    break;
            }
            #region legarcy
            if (_curTimer > animTimer)
            {
                _stateMachine.animator.SetInteger("Attack", 0);
                // 애니메이션이 끝난 이후 데미지 판정 -> 데미지를 넣는데 성공하면 다음 공격, 시간이 지나도 공격 못하면 Idle
                // 애니메이션이 재생 중이라면 Running
                if (_isAttack == false && !_stateMachine.IsDamaged) _isAttack = ActionJudge();
                if (_isAttack && _stateMachine.AttackCount < 3  )
                {
                    return State.Success;

                }
                else if(_curTimer >= _time && !_stateMachine.IsDamaged)
                {
                    _curTimer = 0;
                    _stateMachine.IsNormalAttack = false;
                    _stateMachine.AttackCount = 0;
                    _isAttack = false;
                    _stateMachine.animator.SetTrigger("Idle");
                    _stateMachine.animator.SetFloat("Run", 0);
                    _stateMachine.animator.SetInteger("Attack", _stateMachine.AttackCount);
                    return State.Success;
                }
            }
            return State.Running;
            #endregion
            #endregion
        }

        private void StartExceptionHandling()
        {
            #region Omit    
            float moveValue = 0;
            if (_stateMachine == null)
            {
                _stateMachine = AIAgent.Instance.GetStateMachine;
            }
            if (_stateMachine.AttackCount > 2)
            {
                _stateMachine.AttackCount = 0;
            }
            _isAttack = false;
            _stateMachine.IsNormalAttack = true;
            //AIAgent.Instance.SetAttacRange(attackRange);
            _stateMachine.AttackCount++;
            _stateMachine.animator.SetInteger("Attack", _stateMachine.AttackCount);
            _curTimer = 0;
            _time = _stateMachine.GetAnimationPlayTime("Attack" + (_stateMachine.AttackCount).ToString());
            switch (_stateMachine.AttackCount)
            {
                case 1:
                    _stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack1).Forget();
                    moveValue = _stateMachine.playable.FirstAttackMovingValue;
                    //_stateMachine.UpdateData(LGProject.DATA_TYPE.NormalAttack);
                    break;
                case 2:
                    _stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack2).Forget();
                    moveValue = _stateMachine.playable.SecondAttackMovingValue;
                    //_stateMachine.UpdateData(LGProject.DATA_TYPE.NormalAttack);
                    break;
                case 3:
                    moveValue = _stateMachine.playable.ThirdAttackMovingValue;
                    _stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack3, 0.25f).Forget();
                    //_stateMachine.UpdateData(LGProject.DATA_TYPE.NormalAttack);
                    break;
                default:
                    break;
            }
            // 공격하면서 전진.

            #endregion
        }

        private bool ActionJudge()
        {
            #region Omit
            // 판정 범위 계산.
            Vector3 pDirct = AIAgent.Instance.player.position - _stateMachine.transform.position ;
            Vector3 right = Vector3.right * (pDirct.x > 0 ? 1f : -1f);
            Vector3 center = AIAgent.Instance.transform.position + right + Vector3.up * 0.5f;

            Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f, Quaternion.identity, 1 << 3);
            System.Tuple<Playable, float> temp = null;

            // 자기 공격에 자기가 맞음.
            foreach(var target in targets)
            {
                float distance = Vector3.Distance(center, target.transform.position);
                if((temp == null || temp.Item2 >= distance )&& target.transform != _stateMachine.transform)
                {
                    temp = System.Tuple.Create(target.GetComponent<Playable>(), distance);
                }
            }
            // 공격 범위에 적이 존재하지 않음.
            if(temp == null || (temp.Item1.GetStateMachine.IsKnockback || temp.Item1.GetStateMachine.IsDown))
            {
                // 아직 공격을 성공하지 못함.
                return false;
            }
            else
            {
                Vector3 velocity = (_stateMachine.transform.forward * 1.5f + _stateMachine.transform.up * 3) * 1.5f;

                //Vector3 v = Vector3.zero;
                if (_stateMachine.AttackCount >= 3)
                {
                    //Vector3 direction = (temp.Item1.GetStateMachine.transform.position - AIAgent.Instance.transform.position).normalized;

                }
                if (temp.Item1 != _stateMachine.transform)
                {
                    temp.
                    Item1.GetStateMachine.
                    ApplyHitDamaged(_stateMachine.AttackCount < 3 ? Vector3.zero : velocity, 0, _stateMachine, LGProject.DATA_TYPE.NormalAttack);
                    temp.Item1.GetStateMachine.HitPlayer = _stateMachine.transform;

                    _stateMachine.animator.SetInteger("Attack", 0);
                    if (_stateMachine.playable.movingAttack)
                    {
                        // temp 방향으로 공격하자
                        float moveValue = (temp.Item1.transform.position - _stateMachine.transform.position).normalized.x;
                        _stateMachine.physics.velocity += Vector3.right * moveValue;
                    }
                    return true;
                }
            }
            return false;
            #endregion
        }

    }
}