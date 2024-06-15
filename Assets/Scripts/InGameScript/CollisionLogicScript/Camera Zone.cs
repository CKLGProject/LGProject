using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace LGProject.CollisionZone
{
    public class CameraZone : CollisionZone
    {
        public GameObject TargetGroup;
        private CinemachineTargetGroup _targetGroup;


        private void Awake()
        {
            TargetGroup = GameObject.Find("Target Group (1)");
            _targetGroup = TargetGroup.GetComponent<CinemachineTargetGroup>();
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
        public override bool TriggerSpace(Vector3 playableVector)
        {
            throw new System.NotImplementedException();
        }

        public void SubTarget(Transform pTransform)
        {
            //Debug.Log(TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform));
            int num = _targetGroup.FindMember(pTransform);
            if(num >= 0)
                _targetGroup.m_Targets[num].target = null;
        }

        public void AddTarget(Transform pTransform)
        {
            //Debug.Log(TargetGroup.GetComponent<CinemachineTargetGroup>().FindMember(pTransform));
            if (_targetGroup.FindMember(pTransform) == -1)
            {
                for (int i = 0; i < _targetGroup.m_Targets.Length; i++)
                {
                    if (_targetGroup.m_Targets[i].target == null)
                    {
                        _targetGroup.m_Targets[i].target = pTransform;
                        //_targetGroup.m_Targets[i].weight = 3;
                        return;
                    }
                }
            }
        }

        public void ForcusPlayer(Transform player)
        {
            for (int i = 0; i < _targetGroup.m_Targets.Length; i++)
                {
                if (_targetGroup.m_Targets[i].target == player)
                {
                    _targetGroup.m_Targets[i].weight = 3;
                    return;
                }
            }
        }
    }

}
