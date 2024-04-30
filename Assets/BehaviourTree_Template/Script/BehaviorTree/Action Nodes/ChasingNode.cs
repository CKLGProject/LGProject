using UnityEngine;

namespace BehaviourTree
{
    public class ChasingNode : ActionNode
    {
        public AIAgent Agent;
        [SerializeField] private float jumpDelay;
        private float curTimer;
        private int count;
        
        private Vector3 chasingPoint = Vector3.zero;

        protected override void OnStart()
        {
            // 플레이어 
            pathFinding.PathRequestManager.RequestPath(new pathFinding.PathRequest(AIAgent.Instance.transform.position, AIAgent.Instance.player.position, AIAgent.Instance.GetPath));

            if (Agent == null)
                Agent = AIAgent.Instance;
            chasingPoint = pathFinding.Grid.Instance.NodeFromWorldPoint(Agent.transform.position).worldPosition;
            

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (Agent == null)
                Agent = AIAgent.Instance;

            // path가 있는지 확인. || 내가 적중 당한 경우
            if (((Agent.path == null || Agent.path.Length < 1) || Agent.GetStateMachine.isHit))
            {
                return State.Failure;
            }

            if (Agent.path.Length > 0 && TargetPointToPlayerPositionDistance() > 2f)
            {
                pathFinding.PathRequestManager.RequestPath(new pathFinding.PathRequest(AIAgent.Instance.transform.position, AIAgent.Instance.player.position, AIAgent.Instance.GetPath));
                Debug.Log("AA");
                return State.Running;
            }

            // Running 상태가 필요함.
            // path가 있다면 움직이게 하기.
            // 마지막 경로에 도착했는가를 체크해야함.
            if (FollowPath())
            {
                // 이동을 해야함.
                return State.Running;
            }

            float distance = Vector3.Distance(Agent.transform.position, Agent.player.position);

            if (distance < 1.5f)
            {
                return State.Success;
            }

            return State.Success;
            
        }

        private float TargetPointToPlayerPositionDistance()
        {
            float a = Vector3.Distance(Agent.path[Agent.path.Length - 1], Agent.player.position);
            return a;

        }


        bool FollowPath()
        {
            curTimer += Time.deltaTime;
            Vector3 currentWaypoint = new Vector3(Agent.path[Agent.targetIndex].x, Agent.path[Agent.targetIndex].y - 0.45f, Agent.path[Agent.targetIndex].z);
            if (Agent.transform.position == currentWaypoint)
            {
                Agent.targetIndex++;
                if (Agent.targetIndex >= Agent.path.Length)
                {
                    return false;
                }
                currentWaypoint = new Vector3(Agent.path[Agent.targetIndex].x, Agent.path[Agent.targetIndex].y - 0.45f, Agent.path[Agent.targetIndex].z);
            }

            //Vector3 direction = Agent.target.position - Agent.transform.position;

            //JumpingPointCheck(direction);

            // 대각선 위인지 체크
            if (AcrossTargetNode(currentWaypoint))
            {
                // 목표지점가지 점프해야하는데 일단 디버그 찍고 끝내자
                SetJumpVelocity();
            }


            if (CheckTargetPosition())
            {
                return false;
            }
            currentWaypoint.z = Agent.transform.position.z;

            Agent.transform.position = Vector3.MoveTowards(Agent.transform.position, currentWaypoint, Agent.speed * Time.deltaTime);
            
            Vector3 rot = new Vector3(currentWaypoint.x, Agent.transform.position.y, Agent.transform.position.z);
            Agent.transform.LookAt(rot);
            return true;
        }

        RaycastHit hit;
        private void JumpingPointCheck(Vector3 direction)
        {
            // 점프를 하기 위해 바로 대각선 아래를 체크
            // 대각선 아래에 공간이 없으면 점프를 함.
            // 점프를 하고 떨어지는 와중에도 공간이 없음 추가 점프를 해야하는데,
            // 추가 점프(2단)는 나중에 구현하는 것으로 하자.
            // 앞을 바라보고 있다는 것을 어떻게 표현해야 할까?
            // lookAt? 아니... 그냥 바라보는 곳을 명확하게 하는 것이 좋아보인다.

            // 점프를 하자마자 타이머가 돌아가야하는데, 이건 어떻게 표현할까?
            Ray ray = new Ray(Agent.transform.position + Vector3.up * 0.25f + Vector3.forward * 0.2f, Vector3.down);
            curTimer += Time.deltaTime;
            bool case1 = /*Mathf.Abs(direction.x) < 1.5f && */direction.y >= 0.5f;
            bool case2 = !Physics.Raycast(ray, out hit, 0.45f, 1 << 6) && Agent.GetStateMachine.jumpInCount < 2;
            // 높은 곳이면 점프를 한다.
            if ((direction.y >= 1f && Mathf.Abs(direction.x) < 0.75f) && Agent.GetStateMachine.jumpInCount < 2)
            {
                // 점프를 하는 조건
                // isGrounded가 True거나, timer가 넘어설 경우
                // isGrounded 때문인 것 같음.
                SetJumpVelocity();
            }
            else if (case1 && case2)
            {
                SetJumpVelocity();
            }
            // 위를 체크하고 싶은데...


            // 가는 길에 길이 없으면 쓰는 로직

        }

        private bool AcrossTargetNode(Vector3 currentWayPoint)
        {
            Vector3 direction = currentWayPoint - Agent.transform.position;
            int i = 0;
            // y가 0이 아닌 곳이면 점프 또는 내려가기를 진행
            if (direction.y > 0.1f)
            {
                for (i = Agent.targetIndex; i < Agent.path.Length - 1; i++)
                {
                    Vector2 directionNew = new Vector2(Agent.path[i].x - Agent.path[i + 1].x, Agent.path[i + 1].y - Agent.path[i].y);
                    Ray ray = new Ray(Agent.path[i], Vector3.down);
                    // 조건
                    // 노드의 간격 중 y값이 0이면서 노드 아래에 플랫폼이 존재하는 경우.
                    if (directionNew.y < 0.1f && Physics.Raycast(ray, out hit, 0.5f, layerMask: 1 << 6))
                    {
                        break;
                    }
                }
                Agent.targetIndex = i;
                return true;
            }
            return false;
        }

        public bool CheckTargetPosition()
        {
            if (Agent.target == null)
                return false;
            // 타겟 노드
            Vector3 targetPos = pathFinding.Grid.Instance.NodeFromWorldPoint(Agent.target.position).worldPosition;
            // 최종 목표
            Vector3 FinTarget = Agent.path[Agent.path.Length - 1];

            float distance = Mathf.Abs(Vector3.Distance(targetPos, FinTarget));

            if (distance > 1.5f * 4)
            {
                return true;
            }

            return false;
        }


        private void SetJumpVelocity()
        {
            if (curTimer >= jumpDelay)
            {
                count++;
                Agent.GetStateMachine.jumpInCount++;
                Agent.GetStateMachine.JumpVelocity();
                Agent.GetStateMachine.physics.velocity += Vector3.up * Agent.jumpScale;
                curTimer = 0;
            }
        }
    }
}