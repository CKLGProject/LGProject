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
        [SerializeField] private CollisionZone PlatformZone;

        // 여긴 실행만 담당함. 각 기능들은 TriggerSpace로 기능을 실행함.
        // 그리고 AABB, 즉 현재 콜라이더가 충돌했다는 것을 알려야 하기 때문에 return 값은 Boolean값으로 한다.
        public bool CallZoneFunction(ZoneType zoneType, Transform playerTrasnform)
        {
            switch (zoneType)
            {
                case ZoneType.Platform:
                    // 여긴 아직 리팩터링 안함.
                    break;
                case ZoneType.DeadZone:
                    foreach(var item in DeadZoneContainer)
                    {
                        if (item.TriggerSpace(playerTrasnform))
                            return true;
                    }
                    break;
                case ZoneType.CameraZone:
                    foreach (var item in CameraZoneContainer)
                    {
                        if( item.TriggerSpace(playerTrasnform))
                            return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

    }
}