using System.Collections;
using UnityEngine;

namespace BehaviourTree
{
    // 조건
    // -> 상대를 찾으면 바로 공격 
    // -> 그러나 상대에게 공격을 받고있는 상태라면 숨어야함.
    // -> 어떻게 숨어야 할까? 주변에 가장 가까운 은신처를 찾아야함.
    //
    //
    public class AIAgent : LGProject.PlayerState.Playable
    {
        private static AIAgent instance = null;
        public static AIAgent Instance => instance;

        private float _attackRange;

        public Transform target;
        public Grid grid;
        public Vector3[] path;
        public bool chasing = false;
        public bool finding = false;

        public float speed = 10;
        public int targetIndex;

        public Transform player;
        public bool isGround;

        //public Animator animator;

        [System.Obsolete]
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            Random.seed = System.DateTime.Now.Millisecond;
            stateMachine = new LGProject.PlayerState.PlayerStateMachine();
            stateMachine = LGProject.PlayerState.PlayerStateMachine.CreateStateMachine(this.gameObject);
            
            InitEffectManager();
        }

        private void Start()
        {
            effectManager.InitParticles();
            for (int i = 0; i < stateMachine.animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                string name = stateMachine.animator.runtimeAnimatorController.animationClips[i].name;
                float time = stateMachine.animator.runtimeAnimatorController.animationClips[i].length;
                stateMachine.SetAnimPlayTime(name, time);
                //Debug.Log($"{ stateMachine.animator.runtimeAnimatorController.animationClips[i].name} / { stateMachine.animator.runtimeAnimatorController.animationClips[i].length}'s");
            }
        }

        private void Update()
        {
            isGround = stateMachine.isHit;
            // 일단 여기에 넣어보자
            IsPushDownKey();
            PlatformCheck();
            // 바라보는 방향 -> 일단 무조건 플레이어를 바라보게 설정

        }

        public void LookPlayer()
        {
            // 플레이어를 바라봄.


        }
        
        public void GetPath(Vector3[] newPath, bool pathSuccessful)
        {
            if(pathSuccessful)
            {
                chasing = true;
                targetIndex = 0;
                path = newPath;
            }
        }

        public void SetAttacRange(float range)
        {
            _attackRange = range;
        }

        private void OnDrawGizmos()
        {
            if (path != null)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    if (i % 2 == 0)
                        Gizmos.color = Color.black;
                    else
                        Gizmos.color = Color.gray;
                    Gizmos.DrawCube(path[i], Vector3.one * .5f);

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
            try
            {
                Gizmos.color = Color.blue;

                // 현재 바라보는 방향
                Vector3 right = Vector3.right * (directionX == true ? 1 : -1);

                Gizmos.DrawLine(transform.position, transform.position + right * 0.5f);
                Gizmos.DrawLine(transform.position + (Vector3.up * 0.75f), transform.position + (Vector3.up * 0.75f) + right * 0.5f);

                // stateMachine을 사용하긴 하지만, currentNode를 쓰는 것이 아니기 떄문에 판정을 달리 해야한다.

                if (stateMachine.isNormalAttack)
                {
                    switch (stateMachine.attackCount)
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
                        default:
                            break;
                    }
                    Gizmos.DrawWireCube(transform.position + right, Vector3.one * .75f);
                }
                else if(stateMachine.isDashAttack)
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.75f;
                    hitBoxSize.x *= _attackRange;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(transform.position + right, hitBoxSize);
                }
                else if(stateMachine.isJumpAttack)
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.75f;
                    hitBoxSize.x *= _attackRange;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(transform.position + right, hitBoxSize);
                }
            }
            catch
            {

            }
        }

    }
}