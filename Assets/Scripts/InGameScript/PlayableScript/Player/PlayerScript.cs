using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LGProject.PlayerState
{
    public class PlayerScript : Playable
    {
        #region CUSTOM_ETIDOR
#if UNITY_EDITOR
        [CustomEditor(typeof(PlayerScript), true)]
        [CanEditMultipleObjects]
        public class PlayableEditor : Editor
        {
            /*프로퍼티*/
            /*************************************
             * 움직임 관련 프로퍼티
             */
            private SerializedProperty _maximumJumpCount;
            private SerializedProperty _maximumSpeed;

            private SerializedProperty _dashSpeed;
            private SerializedProperty _jumpScale;

            /*************************************
             * 공격 관련 프로퍼티
             */
            private SerializedProperty _firstAttackDelay;
            private SerializedProperty _firstAttackJudgeDelay;
            private SerializedProperty _firstAttackMovingValue;

            private SerializedProperty _secondAttackDelay;
            private SerializedProperty _secondAttackJudgeDelay;
            private SerializedProperty _secondAttackMovingValue;

            private SerializedProperty _thirdAttackDelay;
            private SerializedProperty _thirdAttackJudgeDelay;
            private SerializedProperty _thirdAttackMovingValue;

            private SerializedProperty _dashAttackDelay;

            private SerializedProperty _hitDelay;

            private SerializedProperty _downWaitDelay;

            private SerializedProperty _wakeUpDelay;

            private static GUIStyle BoldLabelStyle;
            private static GUIStyle BoldFoldStyle;

            private static bool _movementValuesFoldOut = false;
            private static bool _actionValuesFoldOut = false;
            //private static bool _hitValuesFoldOut = false;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                /*****************************************
                 * 모든 프로퍼티를 인스펙터에 표시
                 * ***/
                serializedObject.Update();

                GUI_Initalized();
                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField("Effects settings", BoldLabelStyle);
                GUI_DrawLine(5f, 20f);

                GUI_ShowMovementProprties();
                GUI_ShowAttackProperties();

                /**변경사항이 있다면 값을 갱신한다...*/
                if (GUI.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            private void GUI_Initalized()
            {
                #region Omit
                if (_maximumJumpCount == null)
                {
                    _maximumJumpCount = serializedObject.FindProperty("MaximumJumpCount");
                }

                if (_maximumSpeed == null)
                {
                    _maximumSpeed = serializedObject.FindProperty("MaximumSpeed");
                }

                if (_dashSpeed == null)
                {
                    _dashSpeed = serializedObject.FindProperty("DashSpeed");
                }

                if (_jumpScale == null)
                {
                    _jumpScale = serializedObject.FindProperty("JumpScale");
                }

                /*************************************
                 * 공격 관련 프로퍼티
                 */
                if (_firstAttackDelay == null)
                {
                    _firstAttackDelay = serializedObject.FindProperty("FirstAttackDelay");
                }
                if (_firstAttackJudgeDelay == null)
                    _firstAttackJudgeDelay = serializedObject.FindProperty("FirstAttackJudgeDelay");
                
                if(_firstAttackMovingValue == null)
                    _firstAttackMovingValue = serializedObject.FindProperty("FirstAttackMovingValue");

                if (_secondAttackDelay == null)
                    _secondAttackDelay = serializedObject.FindProperty("SecondAttackDelay");

                if (_secondAttackJudgeDelay == null)
                    _secondAttackJudgeDelay = serializedObject.FindProperty("SecondAttackJudgeDelay");

                if(_secondAttackMovingValue == null)
                    _secondAttackMovingValue = serializedObject.FindProperty("SecondAttackMovingValue");

                if (_thirdAttackDelay == null)
                    _thirdAttackDelay = serializedObject.FindProperty("ThirdAttackDelay");

                if (_thirdAttackJudgeDelay == null)
                    _thirdAttackJudgeDelay = serializedObject.FindProperty("ThirdAttackJudgeDelay");

                if (_thirdAttackMovingValue == null)
                    _thirdAttackMovingValue = serializedObject.FindProperty("ThirdAttackMovingValue");

                if (_dashAttackDelay == null)
                    _dashAttackDelay = serializedObject.FindProperty("DashAttackDelay");

                if (_hitDelay == null)
                    _hitDelay = serializedObject.FindProperty("HitDelay");

                if (_downWaitDelay == null)
                    _downWaitDelay = serializedObject.FindProperty("DownWaitDelay");

                if (_wakeUpDelay == null)
                    _wakeUpDelay = serializedObject.FindProperty("WakeUpDelay");

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

            private void GUI_DrawLine(float space = 5f, float subOffset = 0f)
            {
                #region Omit
                EditorGUILayout.Space(15f);
                var rect = EditorGUILayout.BeginHorizontal();
                Handles.color = Color.gray;
                Handles.DrawLine(new Vector2(rect.x - 15 + subOffset, rect.y), new Vector2(rect.width + 15 - subOffset * 2, rect.y));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(10f);
                #endregion
            }

            private void GUI_ShowMovementProprties()
            {
                #region Omit    
                if (!(_movementValuesFoldOut = EditorGUILayout.Foldout(_movementValuesFoldOut, "Init_MovementValues_Properties", BoldFoldStyle)))
                {
                    GUI_DrawLine(5f, 20f);
                    return;
                }

                EditorGUI.indentLevel++;
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    int value = (int)EditorGUILayout.IntField("최대 점프 횟수", _maximumJumpCount.intValue);
                    if (changeScope.changed)
                    {
                        _maximumJumpCount.intValue = value;
                    }
                }
                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("대쉬 최대 속도", _maximumSpeed.floatValue);
                    if (changeScope.changed)
                    {
                        _maximumSpeed.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("이동 시 증가 속도", _dashSpeed.floatValue);
                    if (changeScope.changed)
                    {
                        _dashSpeed.floatValue = value;
                    }
                }
                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("점프 높이", _jumpScale.floatValue);
                    if (changeScope.changed)
                    {
                        _jumpScale.floatValue = value;
                    }
                }
                EditorGUI.indentLevel--;
                GUI_DrawLine(5f, 20f);
                #endregion
            }

            private void GUI_ShowAttackProperties()
            {
                #region Omit
                if (!(_actionValuesFoldOut = EditorGUILayout.Foldout(_actionValuesFoldOut, "Init_AttackValues_Properties", BoldFoldStyle)))
                {
                    GUI_DrawLine(5f, 20f);
                    return;
                }

                EditorGUI.indentLevel++;

                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("첫번째 공격 딜레이", _firstAttackDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _firstAttackDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("첫번째 공격 후판정", _firstAttackJudgeDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _firstAttackJudgeDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("첫번째 공격 이동 판정", _firstAttackMovingValue.floatValue);
                    if (changeScope.changed)
                    {
                        _firstAttackMovingValue.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("두번째 공격 딜레이", _secondAttackDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _secondAttackDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("두번째 공격 후판정", _secondAttackJudgeDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _secondAttackJudgeDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("두번째 공격 이동 판정", _secondAttackMovingValue.floatValue);
                    if (changeScope.changed)
                    {
                        _secondAttackMovingValue.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("세번째 공격 딜레이", _thirdAttackDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _thirdAttackDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("세번째 공격 후판정", _thirdAttackJudgeDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _thirdAttackJudgeDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("세번째 공격 이동 판정", _thirdAttackMovingValue.floatValue);
                    if (changeScope.changed)
                    {
                        _thirdAttackMovingValue.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("대쉬공격 후 딜레이", _dashAttackDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _dashAttackDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("피격 후 딜레이", _hitDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _hitDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("다운 유지 시간", _downWaitDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _downWaitDelay.floatValue = value;
                    }
                }

                EditorGUILayout.Space(10f);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = (float)EditorGUILayout.FloatField("기상 딜레이", _wakeUpDelay.floatValue);
                    if (changeScope.changed)
                    {
                        _wakeUpDelay.floatValue = value;
                    }
                }

                EditorGUI.indentLevel--;
                GUI_DrawLine(5f, 20f);
                #endregion  
            }
        }
#endif
        #endregion

        #region Gizmos
        private void OnDrawGizmos()
        {
            // Attack Collider를 한 곳에 고정할 필요가 있음.
            try
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position - transform.up * 1f);

                Gizmos.color = Color.blue;

                Vector3 center = transform.position + Vector3.up * 0.5f;
                Vector3 right = Vector3.right * (directionX == true ? 0.7f : -0.7f);
                Gizmos.DrawLine(transform.position, transform.position + right * 0.5f);
                Gizmos.DrawLine(transform.position + (Vector3.up * 0.75f), transform.position + (Vector3.up * 0.75f) + right * 0.5f);

                if (stateMachine.CurrentState.GetType() == typeof(AttackState))
                {
                    switch (stateMachine.AttackCount - 1)
                    {
                        case 0:
                            Gizmos.color = Color.red;
                            break;
                        case 1:
                            Gizmos.color = Color.blue;
                            break;
                        case 2:
                            Gizmos.color = Color.yellow;
                            break;
                    }
                    Gizmos.DrawWireCube(center + right, Vector3.one * 0.5f);
                }
                else if (stateMachine.CurrentState.GetType() == typeof(DashAttackState))
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.7f;
                    hitBoxSize.x *= 1.3f;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(center + right, hitBoxSize );
                }
                else if (stateMachine.CurrentState.GetType() == typeof(JumpAttackState))
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.7f;
                    hitBoxSize.x *= 1.5f;
                    Gizmos.DrawWireCube(center + right, hitBoxSize);
                }
            }
            catch
            {

            }
        }
        #endregion

        private void InitStates()
        {
            // ref을 쓰는 이유
            // 일반적으로 사용하면 복사생성자를 쓰기 때문에 메모리 누수가 일어날 수 있는데, ref을 사용하면 레퍼런스 주소값으로 전달하기 때문에 복사하여 메모리를 사용하는 불상사를 막을 수 있음
            // 그럼 Out을 쓰지 않는 이유?
            // 기본적으로 Out을 사용하면 매서드 내부에서 직접적인 선언 ex) (out int a)가 메서드로 들어갔을 때 a = ? 을 반드시 해줘야 한다.

            stateMachine = new PlayerStateMachine();
            stateMachine = PlayerStateMachine.CreateStateMachine(this.gameObject);
            setupJumpVariables();
            SetUnderPlatform();
        }

        void Start()
        {
            InitStates();
            InitEffectManager();
            stateMachine.UltimateGage = 100;
            effectManager.InitParticles();
            UltimateGageImage.fillAmount = 0;
            for (int i = 0; i < stateMachine.animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                string name = stateMachine.animator.runtimeAnimatorController.animationClips[i].name;
                float time = stateMachine.animator.runtimeAnimatorController.animationClips[i].length;
                stateMachine.SetAnimPlayTime(name, time);
            }
        }

        private void FixedUpdate()
        {

        }

        void Update()
        {
            if(InGameManager.Instance.IsStart)
            {
                stateMachine.CurrentState.LogicUpdate();
                stateMachine.Update();
                PlayableGravity();
                velocity = stateMachine.physics.velocity;
                NewPlatformCheck();
                DeadLineCheck();
            }
        }
    }
}