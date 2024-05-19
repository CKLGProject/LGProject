using UnityEngine;

namespace LGProject.PlayerState
{
    public class PlayerScript : Playable
    {
        private void InitStates()
        {
            // ref을 쓰는 이유
            // 일반적으로 사용하면 복사생성자를 쓰기 때문에 메모리 누수가 일어날 수 있는데, ref을 사용하면 레퍼런스 주소값으로 전달하기 때문에 복사하여 메모리를 사용하는 불상사를 막을 수 있음
            // 그럼 Out을 쓰지 않는 이유?
            // 기본적으로 Out을 사용하면 매서드 내부에서 직접적인 선언 ex) (out int a)가 메서드로 들어갔을 때 a = ? 을 반드시 해줘야 한다.

            stateMachine = new PlayerStateMachine();
            stateMachine = PlayerStateMachine.CreateStateMachine(this.gameObject);
            setupJumpVariables();
            SetUnderPlatform();
        }

        private void Start()
        {
            InitStates();
            InitEffectManager();
            stateMachine.UltimateGage = 100;
            effectManager.InitParticles();
            UltimateGageImage.fillAmount = 0;
            for (int i = 0; i < stateMachine.animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                string name = stateMachine.animator.runtimeAnimatorController.animationClips[i].name;
                float time = stateMachine.animator.runtimeAnimatorController.animationClips[i].length;
                stateMachine.SetAnimPlayTime(name, time);
            }
        }

        private void Update()
        {
            if (BattleSceneManager.Instance.IsStart)
            {
                stateMachine.CurrentState.LogicUpdate();
                stateMachine.Update();
                PlayableGravity();
                velocity = stateMachine.physics.velocity;
                NewPlatformCheck();
                DeadLineCheck();
            }
        }

        #region Gizmos

        private void OnDrawGizmos()
        {
            // Attack Collider를 한 곳에 고정할 필요가 있음.
            try
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position - transform.up * 1f);

                Gizmos.color = Color.blue;

                Vector3 center = transform.position + Vector3.up * 0.5f;
                Vector3 right = Vector3.right * (directionX == true ? 0.7f : -0.7f);
                Gizmos.DrawLine(transform.position, transform.position + right * 0.5f);
                Gizmos.DrawLine(transform.position + (Vector3.up * 0.75f),
                    transform.position + (Vector3.up * 0.75f) + right * 0.5f);

                if (stateMachine.CurrentState.GetType() == typeof(AttackState))
                {
                    switch (stateMachine.AttackCount - 1)
                    {
                        case 0:
                            Gizmos.color = Color.red;
                            break;
                        case 1:
                            Gizmos.color = Color.blue;
                            break;
                        case 2:
                            Gizmos.color = Color.yellow;
                            break;
                    }

                    Gizmos.DrawWireCube(center + right, Vector3.one * 0.5f);
                }
                else if (stateMachine.CurrentState.GetType() == typeof(DashAttackState))
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.7f;
                    hitBoxSize.x *= 1.3f;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(center + right, hitBoxSize);
                }
                else if (stateMachine.CurrentState.GetType() == typeof(JumpAttackState))
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.7f;
                    hitBoxSize.x *= 1.5f;
                    Gizmos.DrawWireCube(center + right, hitBoxSize);
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}