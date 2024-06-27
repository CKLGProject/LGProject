using UnityEngine;


namespace LGProject.PlayerState
{
    // 상속은 다음 기회에 
    public class DashAttackState : State
    {
        private float _currentTimer;
        private float _animationDelay;
        private bool _damageInCount;
        
        private static readonly int DashAttack = Animator.StringToHash("DashAttack");

        private static string _sfxName = string.Empty;
        public DashAttackState(PlayerStateMachine stateMachine, ref float animationDelay, string SFXName) : base(stateMachine)
        {
            _animationDelay = animationDelay;
            _sfxName = SFXName;
            //aniDelay = 
        }

        public override void Enter()
        {
            base.Enter();
            _currentTimer = 0;
            StateMachine.animator.SetTrigger(DashAttack);
            StateMachine.physics.velocity = Vector3.zero;
            _damageInCount = false;
            StateMachine.physics.velocity += Vector3.right * (StateMachine.moveAction.ReadValue<Vector2>().x * 7f);
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.DashAttack, 0.1f).Forget();
            StateMachine.PlayAudioClip(_sfxName);
            // velocity 초기화 X
            // 그런데 브레이크는 걸면 좋을 듯? 대충 Drag값 조절해서 끼이익 하는 느낌을 줘보자.
        }
        public override void LogicUpdate()
        {
            if (Damaged())
                return;
            _currentTimer += Time.deltaTime;
            
            if(_currentTimer > 0.2f )
            {
                if (_damageInCount == false) 
                    AttackJudge();
            }
            
            if(_currentTimer > _animationDelay)
            {
                StateMachine.ChangeState(StateMachine.idleState);
                return ;
            }
            // 공격 판정 
            // -> 아직 없음.

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
        }

        public override void Exit()
        {
            base.Exit();
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.DashAttack);
        }

        public void AttackJudge()
        {
            if (!_damageInCount)
            {
                Vector3 right = StateMachine.transform.forward;
                Vector3 center = StateMachine.transform.position + right + Vector3.up * 0.5f;
                // 생각보다 판정이 후하진 않게 하기
                // hit box의 크기를 따라감.
                Vector3 hitBox =
                 new Vector3(0.5f, 1f, 1f);

                Collider[] targets = Physics.OverlapBox(center, hitBox, Quaternion.identity, 1 << 3);
                // 박스 내부에 들어온 적을 생각했을 때, Playable Character와 가까운 적을 타겟으로 삼는다.
                System.Tuple<Playable, float> temp = null;

                foreach (var t in targets)
                {
                    float distance = Vector3.Distance(center, t.transform.position);
                    if (temp == null || (temp.Item2 >= distance && t.transform != StateMachine.transform))
                    {
                        if(t.transform != StateMachine.transform)
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
                        //Vector3 direction = (temp.Item1.GetStateMachine.transform.position - StateMachine.transform.position).normalized;
                        //direction.x *= 2;
                        //direction.y *= 1.5f;
                        Vector3 velocity = (StateMachine.transform.forward * 2f + StateMachine.transform.up * 3f) * 2f;

                        // 가드를 올리지 않았을 경우
                        if (temp.Item1 != StateMachine.transform)
                        {
                            temp.
                            Item1.GetStateMachine.
                            ApplyHitDamaged(velocity, 0, StateMachine);
                            _damageInCount = true;

                            if (!temp.Item1.GetStateMachine.IsGuard && !temp.Item1.GetStateMachine.IsDown && !temp.Item1.GetStateMachine.IsSuperArmor && !StateMachine.IsUseUltimate)
                            {
                                // 100 % gage로 일단 계산
                                StateMachine.playable.SetUltimateGage(StateMachine.playable.UltimateGage + 10);
                            }
                        }
                    }
                    catch
                    {
#if UNITY_EDITOR
#endif
                    }
                }
            }
        }
    }

}