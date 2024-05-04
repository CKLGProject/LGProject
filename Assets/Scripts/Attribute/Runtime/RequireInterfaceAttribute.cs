using System;
using UnityEngine;

namespace NKStudio
{
    /// <summary>
    /// 직렬화된 Unity 개체 필드(또는 MonoBehaviour 및 ScriptableObjects를 포함한 해당 하위 클래스)에 이 특성을 추가합니다.
    /// 참조된 객체가 주어진 인터페이스를 구현하는지 검사기가 확인할 수 있도록 합니다.¬
    /// </summary>
    /// <remarks>
    /// 이 어트리뷰트는 다중 인터페이스나 일반 인터페이스를 지원하지 않습니다.
    /// 인터페이스 구현 검사는 인스펙터에서 폴드아웃 배열(또는 목록)로 드래그한 참조에 대해서는 무시됩니다.
    /// </remarks>
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        /// <summary>
        /// 참조된 개체가 구현해야 하는 인터페이스 유형입니다.
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// 참조 Unity 객체가 구현해야 하는 인터페이스를 지정하는 속성을 초기화합니다.
        /// </summary>
        /// <param name="interfaceType">인터페이스 유형입니다.</param>
        public RequireInterfaceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}