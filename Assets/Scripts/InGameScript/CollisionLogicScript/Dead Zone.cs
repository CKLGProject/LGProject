using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace LGProject.CollisionZone
{
    public class DeadZone : CollisionZone
    {   
        private void Awake()
        {
            InitSize();
        }
        public override bool TriggerSpace(Vector3 playableVector)
        {
            throw new System.NotImplementedException();
        }

        public override bool TriggerSpace(Transform pTransform)
        {
            if (pTransform.position.x < RectSpace.x && pTransform.position.x > RectSpace.width &&
                pTransform.position.y < RectSpace.y && pTransform.position.y > RectSpace.height)
            {
                return false;
            }
            return true;
        }

    }
}