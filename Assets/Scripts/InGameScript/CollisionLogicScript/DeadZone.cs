using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace LGProject
{
    public class DeadZone : MonoBehaviour
    {   
        public Rect DeadSpace = new Rect();
        public Vector3 DeadLineBoxScale = Vector3.zero;
        public GameObject prefab;
        public GameObject TargetGroup;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, DeadLineBoxScale);


        }

        private void Awake()
        {
            DeadSpace.Set((transform.position.x + DeadLineBoxScale.x) * 0.5f, (transform.position.y + DeadLineBoxScale.y) * .5f, (transform.position.x - DeadLineBoxScale.x) * .5f, (transform.position.y - DeadLineBoxScale.y) * .5f);;
            TargetGroup = GameObject.Find("Target Group (1)");
        }

        public bool TriggerdSpace(Transform pTransform)
        {
            if (pTransform.position.x < DeadSpace.x && pTransform.position.x > DeadSpace.width &&
                pTransform.position.y < DeadSpace.y && pTransform.position.y > DeadSpace.height)
            {
                return true;
            }
            return false;
        }

        public void SubTarget(Transform pTransform)
        {
            //Debug.Log(TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform));
            int num = TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform);
            TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[num].target = null;
        }
        
        public void AddTarget(Transform pTransform)
        {
            //Debug.Log(TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform));
            if(TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform) == -1)
            {
                for(int i =0; i < TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets.Length; i++)
                {
                    if (TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[i].target == null)
                    {
                        TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[i].target = pTransform;
                        return;
                    }
                }
            }
        }
    }
}