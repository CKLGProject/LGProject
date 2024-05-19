using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions.Must;


namespace pathFinding
{
    // �̰� State�� ������
    public class Unit : MonoBehaviour
    {
        //Thread Research;
        public Transform target;
        public Grid grid;
        public Vector3[] path;
        public bool chasing;
        public bool finding;

        private float _speed = 1;
        private int _targetIndex;

        private void Start()
        {
            PathRequestManager.RequestPath(new PathRequest( transform.position, target.position, OnPathFound));
        }

        private void Update()
        {
            if (!chasing && !finding)
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        }


        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                chasing = true;
                _targetIndex = 0;
                path = newPath;
                StopCoroutine(FollowPath());
                StartCoroutine(FollowPath());
            }
        }

        // ��ǥ���� �̵��� �����Ǹ� üũ�� ��.
        // Rect�� ĭ �� ������ �����ϰ� ������ ������ üũ�ϴ� ����
        public bool CheckTargetPosition()
        {
            // Ÿ�� ���
            Vector3 targetPos = grid.NodeFromWorldPoint(target.position).WorldPosition;
            // ���� ��ǥ
            Vector3 finTarget = path[path.Length - 1];

            float distance = Mathf.Abs(Vector3.Distance(targetPos, finTarget));

            // 1.5 * n
            if (distance > 1.5f * 4)
            {
                return true;
            }

            return false;
        }

        private IEnumerator FollowPath()
        {
            int count = 0;
            Vector3 currentWaypoint = path[0];
            while (true)
            {
                count++;
                if (transform.position == currentWaypoint)
                {
                    _targetIndex++;
                    if (_targetIndex >= path.Length)
                    {
                        finding = true;
                        chasing = false;
                        yield break;
                    }
                    currentWaypoint = path[_targetIndex];
                }
                if (count > 100000)
                {
                    transform.GetComponent<Unit>().enabled = false;
                    yield break;
                }
                if (CheckTargetPosition())
                {
                    chasing = false;
                    break;
                }

                currentWaypoint.z = transform.position.z;

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _speed * Time.deltaTime);
                yield return null;
            }
        }

        public void OnDrawGizmos()
        {
            if (path != null)
            {
                for (int i = _targetIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Vector3 temp = new Vector3(path[i].x, path[i].y, path[i].z - 0.5f);
                    Gizmos.DrawCube(temp, Vector3.one * .5f);
                    if (i == _targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
        }
    }
}