using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace LGProject.CollisionZone
{
    public class CameraZone : CollisionZone
    {
        public GameObject TargetGroup;


        private void Awake()
        {
            TargetGroup = GameObject.Find("Target Group (1)");
            InitSize();
        }

        public override bool TriggerSpace(Transform plableTransform)
        {
            // 범위 내에 들어가면 기능을 실행함.
            if (plableTransform.position.x < RectSpace.x && plableTransform.position.x > RectSpace.width &&
                plableTransform.position.y < RectSpace.y && plableTransform.position.y > RectSpace.height)
            {
                AddTarget(plableTransform);
                return true;
            }
            SubTarget(plableTransform);
            return false;
        }

        public void SubTarget(Transform pTransform)
        {
            //Debug.Log(TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform));
            int num = TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform);
            if(num >= 0)
                TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[num].target = null;
        }

        public void AddTarget(Transform pTransform)
        {
            //Debug.Log(TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform));
            if (TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform) == -1)
            {
                for (int i = 0; i < TargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets.Length; i++)
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
