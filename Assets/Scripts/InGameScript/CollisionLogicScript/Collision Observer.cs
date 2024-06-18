using UnityEngine;

namespace LGProject.CollisionZone
{
    public enum ZoneType
    {
        Platform,
        DeadZone,
        CameraZone,
    }
    // 여기선 각 각의 콜리전 영역이 서칭되면 업데이트 시켜줌.
    public class CollisionObserver : MonoBehaviour
    {
        [SerializeField, Header("사망 보험 처리")] private CollisionZone[] DeadZoneContainer;
        [SerializeField,Header("카메라가 잡는 범위")] private CollisionZone[] CameraZoneContainer;

        // 이건 비어있을 예정
        [SerializeField, Header("플랫폼")] private CollisionZone[] PlatformContainer;
        
        // 여긴 실행만 담당함. 각 기능들은 TriggerSpace로 기능을 실행함.
        // 그리고 AABB, 즉 현재 콜라이더가 충돌했다는 것을 알려야 하기 때문에 return 값은 Boolean값으로 한다.
        public CollisionZone CallZoneFunction(ZoneType zoneType, Transform playerTrasnform)
        {
            switch (zoneType)
            {
                case ZoneType.Platform:
                    foreach (var item in PlatformContainer)
                    {
                        if (item.TriggerSpace(playerTrasnform))
                        {
                            return item;
                        }
                    }
                    break;
                case ZoneType.DeadZone:
                    foreach(var item in DeadZoneContainer)
                    {
                        if (item.TriggerSpace(playerTrasnform))
                            return item;
                    }
                    break;
                case ZoneType.CameraZone:
                    foreach (var item in CameraZoneContainer)
                    {
                        if( item.TriggerSpace(playerTrasnform))
                            return item;
                    }
                    break;
                default:
                    break;
            }
            return null;
        }
        public bool CallUnderPlatformZone(ZoneType zoneType, Vector3 vecPlayerDown)
        {
            foreach(var item in PlatformContainer)
            {
                if(item.TriggerSpace(vecPlayerDown))
                {
                    return true;
                }
            }
            return false;
        }

        public CameraZone GetCameraZone()
        {
            foreach(var item in CameraZoneContainer)
            {
                return item as CameraZone;
            }
            return null;
        }
    }

}