using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    // ����
    // -> ��븦 ã���� �ٷ� ���� 
    // -> �׷��� ��뿡�� ������ �ް��ִ� ���¶�� �������.
    // -> ��� ����� �ұ�? �ֺ��� ���� ����� ����ó�� ã�ƾ���.
    //
    //
    public class AIAgent : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;

        public bool FindTarget = false;

        public float chasingRange;
        public float shootingRange;
        public bool shoot = false;

        // ���� �����ƴ�!!
        public bool isEncounter = false;
        public bool isHit = false;
        public bool isConcealment = false;
        public bool isHide = false;

        private const float HorizontalViewAngle = 75;
        private float m_horizontalViewHalfAngle = 0f;
        private float rotateAngle = 0;
        
        [SerializeField] private float m_viewRotateZ = 0f;

        [HideInInspector] public GameObject target;
        [HideInInspector] public Vector3 LookPosition;
        // �ǰ� ����
        [HideInInspector] public Vector3 hitDirection;

        [HideInInspector] public Transform bestCoverSpot;
        #region Find View Targets

        public static Vector3 AngleToDirY(Transform _transform, float angleInDegree)
        {
            #region Omit
            float radian = (angleInDegree + _transform.eulerAngles.y) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
            #endregion
        }

        public GameObject FindViewTarget(float SearchRange, LayerMask hideMask, LayerMask targetMask )
        {
            Vector3 targetPos, dir, lookDir;
            Vector3 originPos = transform.position;
            // �÷��̾� ��Ī
            Collider[] hitedTargets = Physics.OverlapSphere(originPos, SearchRange, targetMask);
            Transform faraway = null;
            float dot, angle;
            float distanceOld, distanceNew;

            foreach (var hitedTarget in hitedTargets)
            {
                targetPos = hitedTarget.transform.position;
                dir = (targetPos - originPos).normalized;
                lookDir = AngleToDirY(this.transform, rotateAngle);

                dot = Vector3.Dot(lookDir, dir);
                angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                bool hitWall = Physics.Raycast(originPos, dir, Vector3.Distance(originPos, targetPos), hideMask);
                //Debug.Log($"aa = {a}");
                if (angle <= HorizontalViewAngle * .5f &&
                    !hitWall
                    && hitedTarget != this)
                {
                    // �þ� �� ���� ������ �Ÿ��� ��
                    // ���� �� ������ ������.
                    if (faraway == null)
                    {
                        faraway = hitedTarget.transform;
                        distanceOld = Vector3.Distance(transform.position, faraway.position);
                    }
                    else
                    {
                        // �Ÿ��� ���ؼ� ���� �� ���� ����.
                        distanceOld = Vector3.Distance(transform.position, faraway.position);
                        distanceNew = Vector3.Distance(transform.position, hitedTarget.transform.position);
                        if (distanceNew <= distanceOld)
                        {
                            // ���� ����� ���� ���� ����
                            faraway = hitedTarget.transform;
                            distanceOld = distanceNew;
                        }
                    }
                }
            }
            // �ɸ� �ֵ� �߿� ���� ����� �ֵ��� ���
            // �ɸ��°� ������? �����Դϴ�.

            return faraway == null ? null : faraway.gameObject;
        }


        private void OnDrawGizmos()
        {
            m_horizontalViewHalfAngle = HorizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position - (transform.forward * 1.5f);

            // �ǰ� ����
            Gizmos.DrawWireSphere(transform.position, 2);

            Vector3 horizontalLeftDir = AngleToDirY(transform, -m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 horizontalRightDir = AngleToDirY(transform, m_horizontalViewHalfAngle + m_viewRotateZ);
            //Vector3 lookDir = AngleToDirY(transform, m_viewRotateZ);

            Debug.DrawRay(originPos, horizontalRightDir * chasingRange, Color.cyan);
            //Debug.DrawRay(originPos, lookDir * chasingRange, Color.green);
            Debug.DrawRay(originPos, horizontalLeftDir * chasingRange, Color.cyan);

            Debug.DrawLine(transform.position + transform.forward, transform.position + transform.forward - transform.right, Color.red);
            Debug.DrawLine(transform.position + transform.forward, transform.position + transform.forward + transform.right, Color.blue);

        }
        #endregion

        private void Awake()
        {
            // ĳ��.
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }



        private float SearchTimer = 0.5f;
        private float curTimer = 0;
        // Update is called once per frame
        void Update()
        {
            // ������ �ް� �����鼭 ���� ���� �����ΰ�?
            if(!isHit && UnderAttack() )
            {
                Debug.Log("AA");
            }
            if(target != null)
            {
                Debug.DrawLine(transform.position, target.transform.position);
            }
            
            // ������ ���� �ٶ󺸴°Ŷ� Dir�� ���� �ʿ䰡 ���� �׷� ��F�� dir�� üũ�� ���ΰ�?
            // hit point�� ��� transform class �ƴ� vector3 ����ü�� �̷���� �ֱ� ������ �⺻������ set�� �� ��� (0, 0, 0);
            //if(true)
                //transform.forward = Vector3.Lerp(transform.forward, dir, 5 * Time.deltaTime);
        }

        public bool UnderAttack()
        {
            //Collider[] bullets = Physics.OverlapSphere(transform.position, 2f, 1 << 10);
            ////Debug.Log($"bullets Count {bullets.Length}");
            //foreach (var bullet in bullets)
            //{
            //    Bullet bullet1 = bullet.GetComponent<Bullet>();
            //    if (bullet1)
            //    {
            //        // �Ѿ��� ���ƿ��� �Ѿ��� ���ƿ� ������ �������� ���� �� �ִ� ���� ����� ������ ���´�.
            //        isHit = true;
            //        target = bullet1.getOner.gameObject;
            //        return true;
            //    }
            //}
            return false;
        }

        private bool CheckTimer()
        {
            curTimer += Time.deltaTime;
            if(curTimer >= SearchTimer)
            {
                shoot = false;
                curTimer = 0;
                return true;
            }
            return false;
        }

        public Transform GetBestCoverSpot()
        {
            return bestCoverSpot;
        }

        public void SetBestCoverSopt(Transform _bestCoverSpot)
        {
            bestCoverSpot = _bestCoverSpot;
        }

    }

}