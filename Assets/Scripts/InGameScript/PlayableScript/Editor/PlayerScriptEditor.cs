using LGProject.PlayerState;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerScript), true)]
[CanEditMultipleObjects]
public class PlayerScriptEditor : Editor
{
    /*프로퍼티*/
    /*************************************
     * 움직임 관련 프로퍼티
     */
    private SerializedProperty _maximumJumpCountProperty;
    private SerializedProperty _maximumSpeedProperty;

    private SerializedProperty _dashSpeedProperty;
    private SerializedProperty _jumpScaleProperty;

    /*************************************
     * 공격 관련 프로퍼티
     */
    private SerializedProperty _firstAttackDelayProperty;
    private SerializedProperty _firstAttackJudgeDelayProperty;
    private SerializedProperty _firstAttackMovingValueProperty;

    private SerializedProperty _secondAttackDelayProperty;
    private SerializedProperty _secondAttackJudgeDelayProperty;
    private SerializedProperty _secondAttackMovingValueProperty;

    private SerializedProperty _thirdAttackDelayProperty;
    private SerializedProperty _thirdAttackJudgeDelayProperty;
    private SerializedProperty _thirdAttackMovingValueProperty;

    private SerializedProperty _dashAttackDelayProperty;

    private SerializedProperty _hitDelayProperty;

    private SerializedProperty _downWaitDelayProperty;

    private SerializedProperty _wakeUpDelayProperty;

    private static GUIStyle BoldLabelStyle;
    private static GUIStyle BoldFoldStyle;

    private static bool MovementValuesFoldOut;
    private static bool ActionValuesFoldOut;
    //private static bool _hitValuesFoldOut = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        /*****************************************
         * 모든 프로퍼티를 인스펙터에 표시
         * ***/
        serializedObject.Update();

        FindProperty();
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("Effects settings", BoldLabelStyle);
        DrawLine(5f, 20f);

        MovementPropertiesGUI();
        AttackPropertiesGUI();

        /**변경사항이 있다면 값을 갱신한다...*/
        if (GUI.changed) 
            serializedObject.ApplyModifiedProperties();
    }

    private void FindProperty()
    {
        #region Omit

        if (_maximumJumpCountProperty == null)
        {
            _maximumJumpCountProperty = serializedObject.FindProperty("MaximumJumpCount");
        }

        if (_maximumSpeedProperty == null)
        {
            _maximumSpeedProperty = serializedObject.FindProperty("MaximumSpeed");
        }

        if (_dashSpeedProperty == null)
        {
            _dashSpeedProperty = serializedObject.FindProperty("DashSpeed");
        }

        if (_jumpScaleProperty == null)
        {
            _jumpScaleProperty = serializedObject.FindProperty("JumpScale");
        }

        /*************************************
         * 공격 관련 프로퍼티
         */
        if (_firstAttackDelayProperty == null)
        {
            _firstAttackDelayProperty = serializedObject.FindProperty("FirstAttackDelay");
        }

        if (_firstAttackJudgeDelayProperty == null)
            _firstAttackJudgeDelayProperty = serializedObject.FindProperty("FirstAttackJudgeDelay");

        if (_firstAttackMovingValueProperty == null)
            _firstAttackMovingValueProperty = serializedObject.FindProperty("FirstAttackMovingValue");

        if (_secondAttackDelayProperty == null)
            _secondAttackDelayProperty = serializedObject.FindProperty("SecondAttackDelay");

        if (_secondAttackJudgeDelayProperty == null)
            _secondAttackJudgeDelayProperty = serializedObject.FindProperty("SecondAttackJudgeDelay");

        if (_secondAttackMovingValueProperty == null)
            _secondAttackMovingValueProperty = serializedObject.FindProperty("SecondAttackMovingValue");

        if (_thirdAttackDelayProperty == null)
            _thirdAttackDelayProperty = serializedObject.FindProperty("ThirdAttackDelay");

        if (_thirdAttackJudgeDelayProperty == null)
            _thirdAttackJudgeDelayProperty = serializedObject.FindProperty("ThirdAttackJudgeDelay");

        if (_thirdAttackMovingValueProperty == null)
            _thirdAttackMovingValueProperty = serializedObject.FindProperty("ThirdAttackMovingValue");

        if (_dashAttackDelayProperty == null)
            _dashAttackDelayProperty = serializedObject.FindProperty("DashAttackDelay");

        if (_hitDelayProperty == null)
            _hitDelayProperty = serializedObject.FindProperty("HitDelay");

        if (_downWaitDelayProperty == null)
            _downWaitDelayProperty = serializedObject.FindProperty("DownWaitDelay");

        if (_wakeUpDelayProperty == null)
            _wakeUpDelayProperty = serializedObject.FindProperty("WakeUpDelay");

        if (BoldLabelStyle == null)
        {
            BoldLabelStyle = new GUIStyle(GUI.skin.label);
            BoldLabelStyle.fontStyle = FontStyle.Bold;
            BoldLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            BoldLabelStyle.fontSize = 14;
        }

        if (BoldFoldStyle == null)
        {
            BoldFoldStyle = new GUIStyle(EditorStyles.foldout);
            BoldFoldStyle.fontStyle = FontStyle.Bold;
        }

        #endregion
    }

    private void DrawLine(float space = 5f, float subOffset = 0f)
    {
        #region Omit

        EditorGUILayout.Space(15f);
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15 + subOffset, rect.y),
            new Vector2(rect.width + 15 - subOffset * 2, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10f);

        #endregion
    }

    private void MovementPropertiesGUI()
    {
        #region Omit

        if (!(MovementValuesFoldOut =
                EditorGUILayout.Foldout(MovementValuesFoldOut, "Init_MovementValues_Properties", BoldFoldStyle)))
        {
            DrawLine(5f, 20f);
            return;
        }

        EditorGUI.indentLevel++;
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            int value = EditorGUILayout.IntField("최대 점프 횟수", _maximumJumpCountProperty.intValue);
            if (changeScope.changed)
            {
                _maximumJumpCountProperty.intValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("대쉬 최대 속도", _maximumSpeedProperty.floatValue);
            if (changeScope.changed)
            {
                _maximumSpeedProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("이동 시 증가 속도", _dashSpeedProperty.floatValue);
            if (changeScope.changed)
            {
                _dashSpeedProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("점프 높이", _jumpScaleProperty.floatValue);
            if (changeScope.changed)
            {
                _jumpScaleProperty.floatValue = value;
            }
        }

        EditorGUI.indentLevel--;
        DrawLine(5f, 20f);

        #endregion
    }

    private void AttackPropertiesGUI()
    {
        #region Omit

        if (!(ActionValuesFoldOut =
                EditorGUILayout.Foldout(ActionValuesFoldOut, "Init_AttackValues_Properties", BoldFoldStyle)))
        {
            DrawLine(5f, 20f);
            return;
        }

        EditorGUI.indentLevel++;

        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("첫번째 공격 딜레이", _firstAttackDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _firstAttackDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("첫번째 공격 후판정", _firstAttackJudgeDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _firstAttackJudgeDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("첫번째 공격 이동 판정", _firstAttackMovingValueProperty.floatValue);
            if (changeScope.changed)
            {
                _firstAttackMovingValueProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("두번째 공격 딜레이", _secondAttackDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _secondAttackDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("두번째 공격 후판정", _secondAttackJudgeDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _secondAttackJudgeDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("두번째 공격 이동 판정", _secondAttackMovingValueProperty.floatValue);
            if (changeScope.changed)
            {
                _secondAttackMovingValueProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("세번째 공격 딜레이", _thirdAttackDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _thirdAttackDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("세번째 공격 후판정", _thirdAttackJudgeDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _thirdAttackJudgeDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("세번째 공격 이동 판정", _thirdAttackMovingValueProperty.floatValue);
            if (changeScope.changed)
            {
                _thirdAttackMovingValueProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("대쉬공격 후 딜레이", _dashAttackDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _dashAttackDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("피격 후 딜레이", _hitDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _hitDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("다운 유지 시간", _downWaitDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _downWaitDelayProperty.floatValue = value;
            }
        }

        EditorGUILayout.Space(10f);
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            float value = EditorGUILayout.FloatField("기상 딜레이", _wakeUpDelayProperty.floatValue);
            if (changeScope.changed)
            {
                _wakeUpDelayProperty.floatValue = value;
            }
        }

        EditorGUI.indentLevel--;
        DrawLine(5f, 20f);

        #endregion
    }
}