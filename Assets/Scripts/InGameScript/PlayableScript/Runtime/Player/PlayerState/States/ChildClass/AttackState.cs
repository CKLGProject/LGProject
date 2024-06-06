using UnityEngine;

namespace LGProject.PlayerState
{
    // Attack State는 Defualt Attack State를 상속받아 공격 판정과 더불어
    // 같은 프로퍼티를 공유 받을 수 있도록 해야겠다.
    public class AttackState : State
    {
        // 체크 해야할 것
        // 공격을 연속으로 하는가?
        // 공중에 있는가?
        // 앉아 있는가?
        // 달리고 있는가?
        // 

        //private float AttackCont = 0;
        //private int maximumCount = 0;
        private float _firstJudgeDelay;
        private float _firstAttackDelay;
        private float _secondJudgeDelay;
        private float _secondAttackDelay;
        private float _thirdJudgeDelay;
        private float _thirdAttackDelay;

        private float _currentTimer;
        private bool _damageInCount;
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Idle = Animator.StringToHash("Idle");

        public AttackState(PlayerStateMachine stateMachine, ref float firstJudgeAttack, ref float firstAttackDelay,ref float secondJudgeAttack,ref float secondAttackDelay, ref float thirdJudgeAttack, ref float thirdAttackDelay) : base(stateMachine)
        {
            _currentTimer = 0;

            _firstJudgeDelay = firstJudgeAttack;
            _firstAttackDelay = firstAttackDelay;
            _secondJudgeDelay = secondJudgeAttack;
            _secondAttackDelay = secondAttackDelay;
            _thirdJudgeDelay = thirdJudgeAttack;
            _thirdAttackDelay = thirdAttackDelay;
        }

        public override void Enter()
        {
            base.Enter();
            // 어디서 왔는지 체크가 필요할까?
            _currentTimer = 0;

            #region 공격 로직 
            StateMachine.AttackCount++;
            StateMachine.StandingVelocity();
            float moveValue = 0;
            switch (StateMachine.AttackCount)
            {
                case 1:
                    moveValue = StateMachine.playable.FirstAttackMovingValue;
                    break;
                case 2:
                    moveValue = StateMachine.playable.SecondAttackMovingValue;
                    break;
                case 3:
                    moveValue = StateMachine.playable.ThirdAttackMovingValue;
                    break;
                default:
                    break;
            }
            // 공격하면서 전진.
            if (StateMachine.playable.movingAttack)
                StateMachine.physics.velocity += StateMachine.transform.forward * moveValue;
            _damageInCount = false;
            #endregion

            #region 애니메이션 출력
            StateMachine.animator.SetInteger(Attack, StateMachine.AttackCount);
            #endregion

            #region 이펙트 출력
            switch (StateMachine.AttackCount)
            {
                case 1:
                    StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack1).Forget();
                    break;
                case 2:
                    StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack2).Forget();
                    break;
                case 3:
                    StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Attack3, 0.25f).Forget();
                    break;
                default:
                    break;  
            }
            #endregion
        }

        public override void Exit()
        {

        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 코루틴이나 쓰레드 등으로 카운팅 또는 불리안 값을 사용하여 상태를 변경해 줄 예정
            // 공격 시엔 콤보 입력도 필요할 것이라 생각하기 때문에 히트 스테이트나 다운 스테이트 등이 필요할 것으로 예상됨.
            // 그럼 공격은 어떻게 할 것인가? 
            
            _currentTimer += Time.deltaTime;

            #region ComboSystem
            AttackLogic();
            #endregion

            #region SingleAttack
            // 단타 공격
            //if(curTimer >= aniDelay)
            //{
            //    stateMachine.attackCount = 0;
            //    stateMachine.ChangeState(stateMachine.playable.idleState);
            //}

            #endregion

        }

        private void AttackLogic()
        {
            float time = StateMachine.GetAnimPlayTime("Attack" + StateMachine.AttackCount);
            float animDelay = 1 ;
            switch (StateMachine.AttackCount)
            {
                case 1:
                    //animDelay = 0.2f;
                    animDelay = _firstJudgeDelay;
                    time = _firstAttackDelay;
                    //time = 0.35f;
                    break;
                case 2:
                    //animDelay = 0.4f;
                    animDelay = _secondJudgeDelay;
                    time = _secondAttackDelay;
                    //time = 0.6f;
                    break;
                case 3:
                    //animDelay = 0.2f;
                    animDelay = _thirdJudgeDelay;
                    time = _thirdAttackDelay;
                    //time = 0.6f;
                    break;
            }
            //Debug.Log($"sm = {stateMachine.AttackCount} / attackDelay = {animDelay} / time {time }");
            // 딜레이가 끝난 이후 추가 키 입력이 들어가면? 
            if (_currentTimer > animDelay)
            {
                StateMachine.animator.SetInteger(Attack, 0);
                // 공격 진행
                if (_damageInCount == false) AttackJudge();
                if (StateMachine.attackAction.triggered && StateMachine.AttackCount < 3)
                {
                    // 다음 공격의 게이지가 100일 경우 Ultimate공격을 진행 아닐 경우 attackState
                    if (StateMachine.playable.UltimateGage >= 100)
                    {
                        StateMachine.AttackCount = 0;
                        StateMachine.animator.SetInteger(Attack ,0);
                        StateMachine.ChangeState(StateMachine.ultimateState);
                    }
                    else
                    {
                        StateMachine.ChangeState(StateMachine.attackState);
                    }
                }
                // 모션이 끝나면?
                else if (_currentTimer >= time)
                {
                    // 모션이 끝났으니 기본 상태로 되돌아감.
                    StateMachine.animator.SetTrigger(Idle);
                    StateMachine.AttackCount = 0;
                    StateMachine.animator.SetInteger(Attack, 0);
                    StateMachine.ChangeState(StateMachine.idleState);
                    return;
                }
            }
        }

        public void AttackJudge()
        {

            if (!_damageInCount)
            {
                Vector3 right = Vector3.right * (StateMachine.playable.directionX == true ? 0.7f : -0.7f);
                Vector3 center = StateMachine.transform.position + right + Vector3.up * 0.5f;
                // 생각보다 판정이 후하진 않게 하기
                // hit box의 크기를 따라감.
                Collider[] targets = Physics.OverlapBox(center, Vector3.one * 0.5f, Quaternion.identity, 1 << 3);
                // 박스 내부에 들어온 적을 생각했을 때, Playable Character와 가까운 적을 타겟으로 삼는다.
                System.Tuple<Playable, float> temp = null;

                foreach (var t in targets)
                {
                    float distance = Vector3.Distance(center, t.transform.position);
                    if ((temp == null || temp.Item2 >= distance )&& t.transform != StateMachine.transform)
                    {
                        temp = System.Tuple.Create(t.GetComponent<Playable>(), distance);
                    }
                }
                if (temp == null)
                {
                    _damageInCount = false;
                }
                else
                {
                    try
                    {
                        Vector3 direction = (temp.Item1.GetStateMachine.transform.position - StateMachine.transform.position).normalized;
                        Vector3 v = StateMachine.playable.CalculateVelocity(
                           temp.Item1.GetStateMachine.transform.position + direction ,
                              temp.Item1.GetStateMachine.transform.position, 0.5f, 1f);
                        // 가드를 올리지 않았을 경우
                        if (temp.Item1 != StateMachine.transform)
                        {
                            temp.
                            Item1.GetStateMachine.
                            HitDamaged(StateMachine.AttackCount - 1 < 2 ? Vector3.zero : v, 0.1f, StateMachine, DATA_TYPE.NormalAttackHit);
                            _damageInCount = true;

                            if (!temp.Item1.GetStateMachine.IsGuard && !temp.Item1.GetStateMachine.IsDown && !temp.Item1.GetStateMachine.IsSuperArmor && !StateMachine.IsUseUltimate)
                            {
                                // 100 % gage로 일단 계산
                                StateMachine.playable.SetUltimateGage(StateMachine.playable.UltimateGage + 10);
                                StateMachine.UltimateGageisFull();
                            }
                        }
                    }
                    catch
                    {
                        Debug.Log("AA");
                    }
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

        }
    }

}