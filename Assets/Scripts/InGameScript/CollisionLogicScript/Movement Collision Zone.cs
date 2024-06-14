using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.CollisionZone
{
    public class MovementCollisionZone : CollisionZone
    {
        private void Awake()
        {
            InitSize();
        }

        public override bool TriggerSpace(Transform plableTransform)
        {
            // 범위 내에 들어가면 기능을 실행함.
            if (plableTransform.position.x < RectSpace.x && plableTransform.position.x > RectSpace.width &&
                plableTransform.position.y < RectSpace.y && plableTransform.position.y > RectSpace.height)
            {
                return true;
            }
            return false;
        }

        public override bool TriggerSpace(Vector3 playableVector)
        {
            throw new System.NotImplementedException();
        }
    }

}