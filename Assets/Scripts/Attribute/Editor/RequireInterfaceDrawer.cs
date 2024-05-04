using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NKStudio
{
    /// <summary>
    /// <see cref="RequireInterfaceAttribute"/>로 표시된 필드를 그리고 참조된 값이
    /// 속성에 제공된 인터페이스를 구현하는지 검증합니다.
    /// 참조된 객체가 유효하지 않으면, 인스펙터의 GUI 컨트롤 아래에 경고 메시지가 표시됩니다.
    /// 이 클래스는 인스펙터의 폴드아웃 배열(또는 목록)로 객체를 드래그할 때 인터페이스 구현을 검증하지 않습니다.
    /// </summary>
    /// <remarks>
    /// 이는 몇 가지 ImGUI 이벤트를 수신하고 Object Field 동작을 변경하여 달성됩니다:
    /// <list type="bullet">
    /// <item>
    /// <description>Object Field의 버튼 클릭 검사가 주어진 인터페이스를 구현하는 유효한 필드 타입의 최소 집합을 포함하는 검색
    /// 필터를 가진 Object Selector 창을 호출하도록 재정의됩니다 (<see cref="OnGUI"/> 참조).</description>
    /// </item>
    /// <item>
    /// <description>Object Field의 Repaint 이벤트는 인스펙터에서 인터페이스 타입
    /// 이름을 올바르게 표시하기 위해 다른 객체 타입을 가집니다 (<see cref="GetObjectFieldType"/> 참조).</description>
    /// </item>
    /// <item>
    /// <description>Object Field의 Drag Update 및 Perform 이벤트는 유효하지 않은
    /// 드래그된 참조를 올바르게 버리기 위해 다른 객체 타입을 가집니다 (<see cref="GetObjectFieldType"/> 참조).</description>
    ///</item>
    /// </list>
    /// </remarks>
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    internal class RequireInterfaceDrawer : PropertyDrawer
    {
        private static class Contents
        {
            public const float ObjectFieldMiniThumbnailHeight = 18f;
            public const float ObjectFieldMiniThumbnailWidth = 32f;
            public const float MismatchImplementationMessageHeight = 20f;

            public static GUIContent InvalidTypeMessage { get; } =
                EditorGUIUtility.TrTextContent(
                    $"Use {nameof(RequireInterfaceAttribute)} with Object reference fields.");

            public static GUIContent InvalidAttributeMessage { get; } =
                EditorGUIUtility.TrTextContent($"The attribute is not a {nameof(RequireInterfaceAttribute)}.");

            public static GUIContent InvalidInterfaceMessage { get; } =
                EditorGUIUtility.TrTextContent("The required type is not an interface.");

            public static GUIContent MismatchImplementationMessage { get; } =
                EditorGUIUtility.TrTextContent("The referenced object does not implement {0}.");
        }

        private const string k_ObjectSelectorUpdateCommand = "ObjectSelectorUpdated";

        /// <summary>
        /// Map that caches the search filter of a field and interface type pair.
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<Type, string>> s_FilterMapByFieldType =
            new Dictionary<Type, Dictionary<Type, string>>();

        /// <summary>
        /// Reusable string builder to create search filters.
        /// </summary>
        private static readonly StringBuilder s_SearchFilterBuilder = new StringBuilder();

        /// <summary>
        /// Reusable list used to store the minimum assignable field types that implement the given interface.
        /// </summary>
        private static readonly List<Type> s_MinimumAssignableImplementations = new List<Type>();

        #region Object Field

        /// <summary>
        /// Copied from the internal EditorGUI.ObjectFieldVisualType
        /// </summary>
        private enum ObjectFieldVisualType
        {
            IconAndText,
            LargePreview,
            MiniPreview,
        }

        /// <summary>
        /// 내부의 EditorGUI.GetButtonRect에서 복사하고, ObjectFieldVisualType 대신 객체 타입을 매개변수로 받도록 수정했습니다.
        /// </summary>
        /// <param name="objectType">버튼 rect를 얻기 위한 타입입니다.</param>
        /// <param name="position">속성 rect의 위치입니다.</param>
        /// <returns>Object Field 버튼 선택기의 rect 위치를 반환합니다.</returns>
        private static Rect GetObjectFieldButtonRect(Type objectType, Rect position)
        {
            var hasThumbnail = EditorGUIUtility.HasObjectThumbnail(objectType);
            var visualType = ObjectFieldVisualType.IconAndText;

            if (hasThumbnail && position.height <= Contents.ObjectFieldMiniThumbnailHeight &&
                position.width <= Contents.ObjectFieldMiniThumbnailWidth)
                visualType = ObjectFieldVisualType.MiniPreview;
            else if (hasThumbnail && position.height > EditorGUIUtility.singleLineHeight)
                visualType = ObjectFieldVisualType.LargePreview;

            switch (visualType)
            {
                case ObjectFieldVisualType.IconAndText:
                    return new Rect(position.xMax - 19, position.y, 19, position.height);
                case ObjectFieldVisualType.MiniPreview:
                    return new Rect(position.xMax - 14, position.y, 14, position.height);
                case ObjectFieldVisualType.LargePreview:
                    return new Rect(position.xMax - 36, position.yMax - 14, 36, 14);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Type GetObjectFieldType(Rect position, Type fieldType, Type interfaceType,
            out bool? dragAndDropAssignable)
        {
            dragAndDropAssignable = null;

            // 인터페이스 유형 이름을 올바르게 표시하는 데 사용됩니다.
            if (Event.current.type == EventType.Repaint)
                return interfaceType;

            // 할당할 수 없는 참조를 드래그할 때 DragAndDrop.visualMode를 올바르게 업데이트하는 데 사용됩니다.
            if (GUI.enabled &&
                (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) &&
                DragAndDrop.objectReferences.Length > 0 &&
                position.Contains(Event.current.mousePosition))
            {
                var referencedValue = DragAndDrop.objectReferences[0];
                if (referencedValue != null)
                {
                    dragAndDropAssignable = TryGetAssignableObject(referencedValue, fieldType, interfaceType, out _);
                    if (!dragAndDropAssignable.Value)
                        return interfaceType;
                }
            }

            return fieldType;
        }

        #endregion

        #region Search Filter

        private static bool IsDirectImplementation(Type type, Type interfaceType)
        {
            var directImplementedInterfaces = type.BaseType == null
                ? type.GetInterfaces()
                : type.GetInterfaces().Except(type.BaseType.GetInterfaces());
            return directImplementedInterfaces.Contains(interfaceType);
        }

        private static void GetDirectImplementations(Type fieldType, Type interfaceType, List<Type> resultList)
        {
            if (!interfaceType.IsInterface)
                return;

            ReflectionUtils.ForEachType(t =>
            {
                if (!t.IsInterface && fieldType.IsAssignableFrom(t) && interfaceType.IsAssignableFrom(t) &&
                    IsDirectImplementation(t, interfaceType))
                    resultList.Add(t);
            });
        }

        private static string GetSearchFilter(Type fieldType, Type interfaceType)
        {
            if (!s_FilterMapByFieldType.TryGetValue(fieldType, out var filterByInterfaceType))
            {
                filterByInterfaceType = new Dictionary<Type, string>();
                s_FilterMapByFieldType.Add(fieldType, filterByInterfaceType);
            }
            else if (filterByInterfaceType.TryGetValue(interfaceType, out var cachedSearchFilter))
            {
                return cachedSearchFilter;
            }

            s_MinimumAssignableImplementations.Clear();
            GetDirectImplementations(fieldType, interfaceType, s_MinimumAssignableImplementations);

            s_SearchFilterBuilder.Clear();
            foreach (var type in s_MinimumAssignableImplementations)
            {
                s_SearchFilterBuilder.Append("t:");
                s_SearchFilterBuilder.Append(type.Name);
                s_SearchFilterBuilder.Append(" ");
            }

            var searchFilter = s_SearchFilterBuilder.ToString();

            filterByInterfaceType.Add(interfaceType, searchFilter);
            return searchFilter;
        }

        #endregion

        #region Helper Methods

        private static bool TryGetAssignableObject(Object objectToValidate, Type fieldType, Type interfaceType,
            out Object assignableObject)
        {
            if (objectToValidate == null)
            {
                assignableObject = null;
                return true;
            }

            var valueType = objectToValidate.GetType();
            if (fieldType.IsAssignableFrom(valueType) && interfaceType.IsAssignableFrom(valueType))
            {
                assignableObject = objectToValidate;
                return true;
            }

            // If the given objectToValidate is a GameObject, search its components as well
            if (objectToValidate is GameObject gameObject)
            {
                assignableObject = gameObject.GetComponent(interfaceType);
                if (assignableObject != null && fieldType.IsInstanceOfType(assignableObject) &&
                    interfaceType.IsInstanceOfType(assignableObject))
                    return true;
            }

            assignableObject = null;
            return false;
        }

        private static Type GetFieldOrElementType(Type fieldType)
        {
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var types = fieldType.GetGenericArguments();
                return types.Length <= 0 ? null : types[0];
            }

            if (fieldType.IsArray)
                return fieldType.GetElementType();

            return fieldType;
        }

        #endregion

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyHeight = base.GetPropertyHeight(property, label);
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                property.objectReferenceValue != null &&
                attribute is RequireInterfaceAttribute requireInterfaceAttr &&
                requireInterfaceAttr.InterfaceType.IsInterface &&
                !requireInterfaceAttr.InterfaceType.IsInstanceOfType(property.objectReferenceValue))
            {
                propertyHeight += Contents.MismatchImplementationMessageHeight + 4f;
            }

            return propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectPickerID = GUIUtility.GetControlID(FocusType.Passive);

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label, Contents.InvalidTypeMessage);
                return;
            }

            if (!(attribute is RequireInterfaceAttribute requireInterfaceAttr))
            {
                EditorGUI.LabelField(position, label, Contents.InvalidAttributeMessage);
                return;
            }

            if (requireInterfaceAttr.InterfaceType == null || !requireInterfaceAttr.InterfaceType.IsInterface)
            {
                EditorGUI.LabelField(position, label, Contents.InvalidInterfaceMessage);
                return;
            }

            if (property.objectReferenceValue != null &&
                !requireInterfaceAttr.InterfaceType.IsInstanceOfType(property.objectReferenceValue))
            {
                var messagePosition = position;
                position.height -= Contents.MismatchImplementationMessageHeight + 4f;
                messagePosition.y = position.yMax + 2f;
                messagePosition.height = Contents.MismatchImplementationMessageHeight;
                EditorGUI.HelpBox(messagePosition,
                    string.Format(Contents.MismatchImplementationMessage.text, requireInterfaceAttr.InterfaceType.Name),
                    MessageType.Warning);
            }

            var fieldType = GetFieldOrElementType(fieldInfo.FieldType);

            using (var scope = new EditorGUI.PropertyScope(position, label, property))
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var allowSceneObjs = !EditorUtility.IsPersistent(property.serializedObject.targetObject);
                var objectFieldType = GetObjectFieldType(position, fieldType, requireInterfaceAttr.InterfaceType,
                    out var dragAndDropAssignable);

                // Override the Object Field button to call the Object Selector window with a filter containing the minimum set of assignable field types that implement the required interface
                if (GUI.enabled && Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                    position.Contains(Event.current.mousePosition))
                {
                    var buttonRect = GetObjectFieldButtonRect(objectFieldType, position);
                    if (buttonRect.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.editingTextField = false;

                        var searchFilter = GetSearchFilter(fieldType, requireInterfaceAttr.InterfaceType);
                        EditorGUIUtility.ShowObjectPicker<Object>(property.objectReferenceValue, allowSceneObjs,
                            searchFilter, objectPickerID);

                        Event.current.Use();
                        GUIUtility.ExitGUI();
                    }
                }

                if (dragAndDropAssignable.HasValue && !dragAndDropAssignable.Value)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    Event.current.Use();
                }

                var value = EditorGUI.ObjectField(position, scope.content, property.objectReferenceValue,
                    objectFieldType, allowSceneObjs);

                // Get the value of the selected Object in the Object Selector window
                if (Event.current.commandName == k_ObjectSelectorUpdateCommand &&
                    EditorGUIUtility.GetObjectPickerControlID() == objectPickerID)
                {
                    GUI.changed = true;
                    value = EditorGUIUtility.GetObjectPickerObject();
                }

                if (check.changed && TryGetAssignableObject(value, fieldType, requireInterfaceAttr.InterfaceType,
                        out var assignableValue))
                    property.objectReferenceValue = assignableValue;
            }
        }
    }
}