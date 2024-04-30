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
            // �÷��̾� 
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

            // path�� �ִ��� Ȯ��. || ���� ���� ���� ���
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

            // Running ���°� �ʿ���.
            // path�� �ִٸ� �����̰� �ϱ�.
            // ������ ��ο� �����ߴ°��� üũ�ؾ���.
            if (FollowPath())
            {
                // �̵��� �ؾ���.
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

            // �밢�� ������ üũ
            if (AcrossTargetNode(currentWaypoint))
            {
                // ��ǥ�������� �����ؾ��ϴµ� �ϴ� ����� ��� ������
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
            // ������ �ϱ� ���� �ٷ� �밢�� �Ʒ��� üũ
            // �밢�� �Ʒ��� ������ ������ ������ ��.
            // ������ �ϰ� �������� ���߿��� ������ ���� �߰� ������ �ؾ��ϴµ�,
            // �߰� ����(2��)�� ���߿� �����ϴ� ������ ����.
            // ���� �ٶ󺸰� �ִٴ� ���� ��� ǥ���ؾ� �ұ�?
            // lookAt? �ƴ�... �׳� �ٶ󺸴� ���� ��Ȯ�ϰ� �ϴ� ���� ���ƺ��δ�.

            // ������ ���ڸ��� Ÿ�̸Ӱ� ���ư����ϴµ�, �̰� ��� ǥ���ұ�?
            Ray ray = new Ray(Agent.transform.position + Vector3.up * 0.25f + Vector3.forward * 0.2f, Vector3.down);
            curTimer += Time.deltaTime;
            bool case1 = /*Mathf.Abs(direction.x) < 1.5f && */direction.y >= 0.5f;
            bool case2 = !Physics.Raycast(ray, out hit, 0.45f, 1 << 6) && Agent.GetStateMachine.jumpInCount < 2;
            // ���� ���̸� ������ �Ѵ�.
            if ((direction.y >= 1f && Mathf.Abs(direction.x) < 0.75f) && Agent.GetStateMachine.jumpInCount < 2)
            {
                // ������ �ϴ� ����
                // isGrounded�� True�ų�, timer�� �Ѿ ���
                // isGrounded ������ �� ����.
                SetJumpVelocity();
            }
            else if (case1 && case2)
            {
                SetJumpVelocity();
            }
            // ���� üũ�ϰ� ������...


            // ���� �濡 ���� ������ ���� ����

        }

        private bool AcrossTargetNode(Vector3 currentWayPoint)
        {
            Vector3 direction = currentWayPoint - Agent.transform.position;
            int i = 0;
            // y�� 0�� �ƴ� ���̸� ���� �Ǵ� �������⸦ ����
            if (direction.y > 0.1f)
            {
                for (i = Agent.targetIndex; i < Agent.path.Length - 1; i++)
                {
                    Vector2 directionNew = new Vector2(Agent.path[i].x - Agent.path[i + 1].x, Agent.path[i + 1].y - Agent.path[i].y);
                    Ray ray = new Ray(Agent.path[i], Vector3.down);
                    // ����
                    // ����� ���� �� y���� 0�̸鼭 ��� �Ʒ��� �÷����� �����ϴ� ���.
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
            // Ÿ�� ���
            Vector3 targetPos = pathFinding.Grid.Instance.NodeFromWorldPoint(Agent.target.position).worldPosition;
            // ���� ��ǥ
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