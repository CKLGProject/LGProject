using System.Collections;
using LGProject;
using UnityEngine;
using UnityEditor;
using UnityEngine.Singleton;

namespace BehaviourTree
{

    public class AIAgent : LGProject.PlayerState.Playable
    {
        private static AIAgent instance;
        public static AIAgent Instance => instance;

        private float _attackRange;

        [HideInInspector] public Transform target;
        public pathFinding.Grid grid;
        [HideInInspector] public Vector3[] path;
        [HideInInspector] public bool chasing;
        [HideInInspector] public bool finding;

        [HideInInspector] public float speed = 10;
        [HideInInspector] public int targetIndex;

        [HideInInspector] public bool isGround;

        [SerializeField] private GameObject[] ChildModels;

       
        [HideInInspector] public int GuardPercent;
        [HideInInspector] public int AttackPercent;
        [HideInInspector] public int ChasingPercent;
        [HideInInspector] public int NormalMovePercent;

        // 상대방
        [HideInInspector] public Transform player;

        #region magicMathods
        [System.Obsolete]
        protected override void Awake()
        {
            base.Awake();
            
            if (instance == null) 
                instance = this;
            Random.seed = System.DateTime.Now.Millisecond;
            StateMachine = new LGProject.PlayerState.PlayerStateMachine();
            StateMachine = LGProject.PlayerState.PlayerStateMachine.CreateStateMachine(this.gameObject);
            SetAIModel();
        }

        private void SetAIModel()
        {
            int rand = Random.Range(0, ChildModels.Length);
            ChildModels[rand].SetActive(true);
            Animator = ChildModels[rand].GetComponent<Animator>();
        }


        public void SetData()
        {
            GuardPercent = (FileManager.Instance.TotalData.NormalAttackCount + FileManager.Instance.TotalData.DashAttackCount) / (FileManager.Instance.TotalData.DashAttackHitCount + FileManager.Instance.TotalData.NormalAttackHitCount) * 10;
            AttackPercent = (FileManager.Instance.TotalData.NormalAttackCount + FileManager.Instance.TotalData.DashAttackCount)  / (FileManager.Instance.TotalData.NormalAttackCount + FileManager.Instance.TotalData.DashAttackCount) * 10;

            ChasingPercent = (FileManager.Instance.TotalData.ChasingCount - FileManager.Instance.TotalData.DashAttackHitCount) < 0 ? 3 : 7;
            NormalMovePercent = (FileManager.Instance.TotalData.ChasingCount - FileManager.Instance.TotalData.DashAttackHitCount) < 0 ? 7 : 3;
        }

        private void Start()
        {
            player = BattleSceneManager.Instance.GetPlayers();
            InitEffectManager();
            SetupJumpVariables();
            effectManager.InitParticles();
            SetUltimateGage(0);
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            for (int i = 0; i < StateMachine.animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                string name = StateMachine.animator.runtimeAnimatorController.animationClips[i].name;
                float time = StateMachine.animator.runtimeAnimatorController.animationClips[i].length;
                StateMachine.SetAnimationPlayTime(name, time);
            }
            FileManager.Instance.LoadData();

        }

        float curTimer = 0;
        float minTimer = 0.1f;

        private void Update()
        {
            // 바라보는 방향 -> 일단 무조건 플레이어를 바라보게 설정
            // 일단 여기에 넣어보자
            if(StateMachine.IsKnockback)
            {
                curTimer += Time.deltaTime;
                if (curTimer < minTimer)
                    return;
            }
            else
            {
                curTimer = 0;
            }
            PlayableGravity();
            GetStateMachine.Update();
            NewPlatformCheck();
            DeadSpaceCheck();
            CameraCheck();
            UnderPlatformCheck();
        }

        private void OnDrawGizmos()
        {
            if (path != null)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    if (i % 2 == 0)
                        Gizmos.color = Color.black;
                    else
                        Gizmos.color = Color.gray;
                    Gizmos.DrawCube(path[i], Vector3.one * .5f);

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
            try
            {
                Gizmos.color = Color.blue;

                // 현재 바라보는 방향
                Vector3 right = Vector3.right * (directionX == true ? 1 : -1);

                Gizmos.DrawLine(transform.position, transform.position + right * 0.5f);
                Gizmos.DrawLine(transform.position + (Vector3.up * 0.75f), transform.position + (Vector3.up * 0.75f) + right * 0.5f);

                // stateMachine을 사용하긴 하지만, currentNode를 쓰는 것이 아니기 떄문에 판정을 달리 해야한다.

                if (StateMachine.IsNormalAttack)
                {
                    switch (StateMachine.AttackCount)
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
                        default:
                            break;
                    }
                    Gizmos.DrawWireCube(transform.position + right, Vector3.one * .75f);
                }
                else if (StateMachine.IsDashAttack)
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.75f;
                    hitBoxSize.x *= _attackRange;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(transform.position + right, hitBoxSize);
                }
                else if (StateMachine.IsJumpAttack)
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.75f;
                    hitBoxSize.x *= _attackRange;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(transform.position + right, hitBoxSize);
                }
            }
            catch
            {

            }
        }
    #endregion

        public void LoadCSVData()
        {
            
        }

        public void SaveCSVData()
        {
            // 게임이 종료되면 해당 게임의 데이터를 csv파일로 뽑아내는 매서드가 될 예정
        }
        
        public void GetPath(Vector3[] newPath, bool pathSuccessful)
        {
            if(pathSuccessful)
            {
                chasing = true;
                targetIndex = 0;
                path = newPath;
            }
        }

        public void SetAttacRange(float range)
        {
            _attackRange = range;
        }
    }

    #region CUSTOM_EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(AIAgent), true)]
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
        private SerializedProperty _firstAttackMovingValueProperty;

        private SerializedProperty _secondAttackDelay;
        private SerializedProperty _secondAttackJudgeDelay;
        private SerializedProperty _secondAttackMovingValueProperty;

        private SerializedProperty _thirdAttackDelay;
        private SerializedProperty _thirdAttackJudgeDelay;
        private SerializedProperty _thirdAttackMovingValueProperty;

        private SerializedProperty _dashAttackDelay;

        private SerializedProperty _hitDelay;

        private SerializedProperty _downWaitDelay;

        private SerializedProperty _wakeUpDelay;

        private static GUIStyle BoldLabelStyle;
        private static GUIStyle BoldFoldStyle;

        private static bool _movementValuesFoldOut;
        private static bool _actionValuesFoldOut;
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

            if (_firstAttackMovingValueProperty == null)
                _firstAttackMovingValueProperty = serializedObject.FindProperty("FirstAttackMovingValue");

            if (_secondAttackDelay == null)
                _secondAttackDelay = serializedObject.FindProperty("SecondAttackDelay");

            if (_secondAttackJudgeDelay == null)
                _secondAttackJudgeDelay = serializedObject.FindProperty("SecondAttackJudgeDelay");

            if (_secondAttackMovingValueProperty == null)
                _secondAttackMovingValueProperty = serializedObject.FindProperty("SecondAttackMovingValue");


            if (_thirdAttackDelay == null)
                _thirdAttackDelay = serializedObject.FindProperty("ThirdAttackDelay");

            if (_thirdAttackJudgeDelay == null)
                _thirdAttackJudgeDelay = serializedObject.FindProperty("ThirdAttackJudgeDelay");

            if (_thirdAttackMovingValueProperty == null)
                _thirdAttackMovingValueProperty = serializedObject.FindProperty("ThirdAttackMovingValue");

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
                float value = EditorGUILayout.FloatField("첫번째 공격 딜레이", _firstAttackDelay.floatValue);
                if (changeScope.changed)
                {
                    _firstAttackDelay.floatValue = value;
                }
            }
            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("첫번째 공격 후판정", _firstAttackJudgeDelay.floatValue);
                if (changeScope.changed)
                {
                    _firstAttackJudgeDelay.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("첫번째 공격 이동 거리", _firstAttackMovingValueProperty.floatValue);
                if (changeScope.changed)
                {
                    _firstAttackMovingValueProperty.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("두번째 공격 딜레이", _secondAttackDelay.floatValue);
                if (changeScope.changed)
                {
                    _secondAttackDelay.floatValue = value;
                }
            }
            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("두번째 공격 후판정", _secondAttackJudgeDelay.floatValue);
                if (changeScope.changed)
                {
                    _secondAttackJudgeDelay.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("두번째 공격 이동 거리", _secondAttackMovingValueProperty.floatValue);
                if (changeScope.changed)
                {
                    _secondAttackMovingValueProperty.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("세번째 공격 딜레이", _thirdAttackDelay.floatValue);
                if (changeScope.changed)
                {
                    _thirdAttackDelay.floatValue = value;
                }
            }
            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("세번째 공격 후판정", _thirdAttackJudgeDelay.floatValue);
                if (changeScope.changed)
                {
                    _thirdAttackJudgeDelay.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("세번째 공격 이동 거리", _thirdAttackMovingValueProperty.floatValue);
                if (changeScope.changed)
                {
                    _thirdAttackMovingValueProperty.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("대쉬공격 후 딜레이", _dashAttackDelay.floatValue);
                if (changeScope.changed)
                {
                    _dashAttackDelay.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("피격 후 딜레이", _hitDelay.floatValue);
                if (changeScope.changed)
                {
                    _hitDelay.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("다운 유지 시간", _downWaitDelay.floatValue);
                if (changeScope.changed)
                {
                    _downWaitDelay.floatValue = value;
                }
            }

            EditorGUILayout.Space(10f);
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("다운 후 딜레이", _wakeUpDelay.floatValue);
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
}