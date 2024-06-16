using UnityEngine;
using Utility;

/// <summary>
/// 모바일 환경에서만 활성화되는 오브젝트입니다.
/// </summary>
public class MobileOnlyActive : MonoBehaviour
{
    [Tooltip("모바일 환경에서만 활성화될 오브젝트")]
    public GameObject target;
    
    private void Start()
    {
        bool isActive = LGUtility.IsMobile();
        target.SetActive(isActive);
    }
}
