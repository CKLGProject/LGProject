using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BehaviourTree
{
    // 움직임을 판단. 상대를 향해 이동
    // 점프를 할 때도 있을 것이다. 이는 
    public class MoveNode : ActionNode
    {

        public AIAgent Agent;
        [Space(10f)]

        // 어디로 이동할 것인가가 관건
        // 점프는 별개의 영역이므로 이동자체의 로직만 생각하자.
        // 목표 지점을 두는 것은 어떨까?
        // 내가 이동할 목적지가 존재하면 나름 알고리즘에서 제약이 없을 것 같다.
        // 점프도 어차피 Velocity를 줘서 그 방향으로 날리는 것이기 때문에 이동과 점프 로직은 달리 하면 좋을 것 같다.

        // 현재 이동을 A*로 진행하려 생각 중.
        // Platform을 기준으로 이동할 수 있는 노드이며 대각선 이동 또는 y축 이동일 경우 점프를 하게 대체.
        public float jumpDelay = 0;
        private float curTimer = 0;
        private int count = 0;
        //private Vector3 startPoint = Vector3.zero;
        //private int targetIndex = 0;
        //int count = 0;
        ////Vector3 currentWaypoint = Vec;

        protected override void OnStart()
        {
            // Path 설정
            if (Agent == null)
                Agent = AIAgent.Instance;
            curTimer = jumpDelay;
            Agent.targetIndex = 0;
            //startPoint = pathFinding.Grid.Instance.NodeFromWorldPoint(agent.transform.position).worldPosition - Vector3.up * 0.45f;
        }

        protected override void OnStop()
        {
            //Vector3 v = agent.GetStateMachine.physics.velocity;
            //agent.GetStateMachine.physics.velocity = new Vector3(0, v.y, 0);
            //Debug.Log($"Stop = {agent.GetStateMachine.jumpInCount}");
        }

        protected override State OnUpdate()
        {
            #region legarcy
            //float distance = Vector3.Distance(agent.transform.position, agent.target.position);
            //// 이동 
            //if (agent.target == null)
            //    return State.Failure;
            //if (distance <= 0.5f)
            //{
            //    //agent.GetStateMachine.physics.velocity = new Vector3(0, agent.GetStateMachine.physics.velocity.y, 0);
            //    return State.Success;
            //}
            //MovingPoint();
            //return State.Running;

            #endregion
            if (Agent == null)
                Agent = AIAgent.Instance;
            float distance = Vector3.Distance(Agent.transform.position, Agent.player.position);

            // path가 있는지 확인. || 내 앞에 적이 있는 지 확인
            if ((Agent.path == null || distance <= 1.5f) && Agent.GetStateMachine.isGrounded)
            {
                return State.Failure;
            }
            // path가 있다면 움직이게 하기.
            // Running 상태가 필요함.
            // 마지막 경로에 도착했는가를 체크해야함.
            if(FollowPath())
            {
                // 이동을 해야함.
                return State.Running;
            }
            return State.Success;   

        }

        bool FollowPath()
        {
            curTimer += Time.deltaTime;
            Vector3 currentWaypoint = new Vector3(Agent.path[Agent.targetIndex].x, Agent.path[Agent.targetIndex].y -0.45f, Agent.path[Agent.targetIndex].z);
            if (Agent.transform.position == currentWaypoint)
            {
                //startPoint = currentWaypoint;
                Agent.targetIndex++;
                if (Agent.targetIndex >= Agent.path.Length)
                {
                    return false;
                }
                currentWaypoint = new Vector3(Agent.path[Agent.targetIndex].x, Agent.path[Agent.targetIndex].y - 0.45f, Agent.path[Agent.targetIndex].z);
            }

            // 대각선 위인지 체크
            if (AcrossTargetNode(currentWaypoint))
            {
                // 목표지점가지 점프해야하는데 일단 디버그 찍고 끝내자
                //Debug.Log("AA");
                SetJumpVelocity();
            }
            //CheckDownNode(currentWaypoint);
            //if ()


            if (CheckTargetPosition())
            {
                return false;
            }
            currentWaypoint.z = Agent.transform.position.z;

            //Vector3 direction = currentWaypoint - agent.transform.position;
            //;
            //if (agent.GetStateMachine.physics.velocity.x < 3f)
            //{
            //    agent.GetStateMachine.physics.velocity += Vector3.right * direction.normalized.x;
            //}
            Agent.transform.position = Vector3.MoveTowards(Agent.transform.position, currentWaypoint, Agent.speed * Time.deltaTime);

            Vector3 direction = currentWaypoint - Agent.transform.position;
            Agent.directionX = direction.x >= 0.1f ? true : false;
            Vector3 rot = new Vector3(currentWaypoint.x, Agent.transform.position.y, Agent.transform.position.z);
            Agent.transform.LookAt(rot);
            return true;
        }

        private bool CheckDownNode(Vector3 currentWayPoint)
        {
            Vector3 direction = currentWayPoint - Agent.transform.position;

            if (direction.y < -0.4)
            {
                //agent.transform.position += Vector3.down * 0.3f;
                //agent.GetStateMachine.isDown = true;
                return false;
            }
            return true;
        }

        private bool AcrossTargetNode(Vector3 currentWayPoint)
        {
            Vector3 direction = currentWayPoint - Agent.transform.position;
            Vector2 directionOld = Vector2.zero;
            int i = 0;
            RaycastHit hit;
            // y가 0이 아닌 곳이면 점프 또는 내려가기를 진행
            if(direction.y > 0.1f)
            {
                for(i = Agent.targetIndex; i < Agent.path.Length - 1; i++) 
                {
                    Vector2 directionNew = new Vector2(Agent.path[i].x - Agent.path[i+1].x, Agent.path[i + 1].y - Agent.path[i].y);
                    Ray ray = new Ray(Agent.path[i], Vector3.down);
                    // 조건
                    // 노드의 간격 중 y값이 0이면서 노드 아래에 플랫폼이 존재하는 경우.
                    if(directionNew.y < 0.1f && Physics.Raycast(ray, out hit, 0.5f, layerMask: 1 << 6))
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

            // 1.5 * n
            if (distance > 1.5f * 4)
            {
                return true;
            }

            return false;
        }

        // 여기서 이동을 주관하되 공격, 점프 등을 유연하게 할 수 있어야 한다.
        // 4를 넘지 않았다면 계속 Physics 를 줘라
        private void MovingPoint()
        {
            Vector3 direction = Agent.target.position - Agent.transform.position;

            JumpingPointCheck(direction);

            if (Mathf.Abs(Agent.GetStateMachine.physics.velocity.x) < 1 && Mathf.Abs(direction.x) > 0.1f)
            {
                // 방향을 체크
                direction = new Vector3(direction.x, 0, 0);
                direction.Normalize();
                Agent.GetStateMachine.physics.velocity += direction * 1;
                // 여기서 가야하는 방향에 따라 바라보는 방향을 달리 해줌.
                Vector3 viewTarget = new Vector3(Agent.target.position.x, Agent.transform.position.y, Agent.target.position.z);
                Agent.transform.LookAt(viewTarget);
            }
            else if(Mathf.Abs(direction.x) < 0.1f)
                Agent.GetStateMachine.physics.velocity = new Vector3(0, Agent.GetStateMachine.physics.velocity.y, 0);
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
            if ((direction.y >= 1f && Mathf.Abs( direction.x ) < 0.75f) && Agent.GetStateMachine.jumpInCount < 2)
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

        private void SetJumpVelocity()
        {
            if (curTimer >= jumpDelay)
            {
                count++;
                //Debug.Log($"{curTimer} / {count}");
                Agent.GetStateMachine.jumpInCount++;
                Agent.GetStateMachine.JumpVelocity();
                Agent.GetStateMachine.physics.velocity += Vector3.up * Agent.jumpScale;
                curTimer = 0;
                //Debug.Log($"Jump = {Agent.GetStateMachine.physics.velocity}");
            }
        }


    }

}