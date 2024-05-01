using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EffectManager : MonoBehaviour
{

    #region Editor_Extension
#if UNITY_EDITOR

    [CustomEditor(typeof(EffectManager))]
    private sealed class EffectManagerEditor : Editor
    {
        //===================================
        //////          Fields          /////
        //===================================

        /* 이펙트 */
        //========================================
        //////          GameObject          /////
        //========================================
        private SerializedProperty AttackEffect;
        private SerializedProperty AttackOffset;
        
        private SerializedProperty GuardEffect;
        private SerializedProperty GuardOffset;

        private SerializedProperty HitEffect;
        private SerializedProperty HitOffset;

        private SerializedProperty AirborneEffect;
        private SerializedProperty AirborneOffset;

        private SerializedProperty KnockbackEffect;
        private SerializedProperty KnockbackOffset;

        private SerializedProperty LandingEffect;
        private SerializedProperty LandingOffset;

        private SerializedProperty RunEffect;
        private SerializedProperty RunOffset;

        private SerializedProperty UltimateEffect;
        private SerializedProperty UltimateOffset;

        private static GUIStyle BoldLabelStyle;
        private static GUIStyle BoldFoldStyle;

        /* 스타일 관련 내용 */
        private static bool _AcidBombSettingFoldOut = false;
        private static bool _FoldOut = false;
        private static bool _OneShotAcidBombFoldOut = false;
        private static bool _ShotGunAcidBombFoldOut = false;


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

            //GUI_DefaultAcidBomb();
            GUI_ShowVineAttackState();
            //GUI_OneShotSetting();
            //GUI_ShotGunState();

            /**변경사항이 있다면 값을 갱신한다...*/
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        //========================================
        //////         GUI methods            ////
        //========================================
        private void GUI_Initalized()
        {
            #region Omit
            /***************************************
             * 모든 프로퍼티를 초기화 한다.
             * **/
            if (AttackEffect == null)
            {
                AttackEffect = serializedObject.FindProperty("_AttackEffect1");
            }

            if (AttackOffset == null)
            {
                AttackOffset = serializedObject.FindProperty("_AttackOffset");
            }

            if (GuardEffect == null)
            {
                GuardEffect = serializedObject.FindProperty("_GuardEffect");
            }

            if (GuardOffset == null)
            {
                GuardOffset = serializedObject.FindProperty("_GuardOffset");
            }

            if (HitEffect == null)
            {
                HitEffect = serializedObject.FindProperty("_HitEffect");
            }

            if (HitOffset == null)
            {
                HitOffset = serializedObject.FindProperty("_HitOffset");
            }

            if (AirborneEffect == null)
            {
                AirborneEffect = serializedObject.FindProperty("_AirborneEffect");
            }

            if(AirborneOffset == null)
            {
                AirborneOffset = serializedObject.FindProperty("_AirborneOffset");
            }

            if (KnockbackEffect== null)
            {
                KnockbackEffect = serializedObject.FindProperty("_KnockbackEffect");
            }

            if (KnockbackOffset == null)
            {
                KnockbackOffset = serializedObject.FindProperty("_KnockbackOffset");
            }

            if (LandingEffect == null)
            {
                LandingEffect = serializedObject.FindProperty("_LandingEffect");
            }

            if (LandingOffset == null)
            {
                LandingOffset = serializedObject.FindProperty("_LandingOffset");
            }

            if (RunEffect == null)
            {
                RunEffect = serializedObject.FindProperty("_RunEffect");
            }

            if (RunOffset == null)
            {
                RunOffset = serializedObject.FindProperty("_RunkOffset");
            }

            if (UltimateEffect == null)
            {
                UltimateEffect = serializedObject.FindProperty("_UltimateEffect");
            }
            
            if (UltimateOffset == null)
            {
                UltimateOffset = serializedObject.FindProperty("_UltimateOffset");
            }

            /******************************************
             * 모든 스타일을 초기화
             * ***/
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

        private void GUI_ShowVineAttackState()
        {
            #region Omit
            if (AttackEffect == null || GuardEffect == null)
                return;
            if (!(_FoldOut = EditorGUILayout.Foldout(_FoldOut, "Init_Effects", BoldFoldStyle)))
            {
                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("AttackEffectPrefab", AttackEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    AttackEffect.objectReferenceValue = value;
                }
            }

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Attack Effect Offset", AttackOffset.vector3Value);
                if (changeScope.changed)
                {
                    AttackOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("GuardEffectPrefab", GuardEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    GuardEffect.objectReferenceValue = value;
                }
            }

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Guard Effect Offset", GuardOffset.vector3Value);
                if (changeScope.changed)
                {
                    GuardOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("HitEffectPrefab", HitEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    HitEffect.objectReferenceValue = value;
                }
            }

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Hit Effect Offset", HitOffset.vector3Value);
                if (changeScope.changed)
                {
                    HitOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Airborne Effect Prefab", AirborneEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    AirborneEffect.objectReferenceValue = value;
                }
            }

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Airborne Effect Offset", HitOffset.vector3Value);
                if (changeScope.changed)
                {
                    AirborneOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("KnockBack Effect Prefab", KnockbackEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    KnockbackEffect.objectReferenceValue = value;
                }
            }

            //using (var changeScope = new EditorGUI.ChangeCheckScope())
            //{
            //    Vector3 value = EditorGUILayout.Vector3Field("Hit Effect Offset", KnockbackOffset.vector3Value);
            //    if (changeScope.changed)
            //    {
            //        KnockbackOffset.vector3Value = value;
            //    }
            //}

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("LandingEffectPrefab", LandingEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    LandingEffect.objectReferenceValue = value;
                }
            }

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Landing Effect Offset", LandingOffset.vector3Value);
                if (changeScope.changed)
                {
                    LandingEffect.vector3Value = value;
                }
            }

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Run Effect Prefab", RunEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    RunEffect.objectReferenceValue = value;
                }
            }

            //using (var changeScope = new EditorGUI.ChangeCheckScope())
            //{
            //    Vector3 value = EditorGUILayout.Vector3Field("Run Effect Offset", RunOffset.vector3Value);
            //    if (changeScope.changed)
            //    {
            //        RunEffect.vector3Value = value;
            //    }
            //}

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Ultimate Effect Prefab", UltimateEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    UltimateEffect.objectReferenceValue = value;
                }
            }

            //using (var changeScope = new EditorGUI.ChangeCheckScope())
            //{
            //    Vector3 value = EditorGUILayout.Vector3Field("Ultimate Effect Offset", UltimateOffset.vector3Value);
            //    if (changeScope.changed)
            //    {
            //        UltimateEffect.vector3Value = value;
            //    }
            //}

            EditorGUI.indentLevel--;
            GUI_DrawLine(5f, 20f);
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
    }

#endif
    #endregion

    //===========================================
    //////          Property                /////
    //===========================================
    //public float FlightTime { get { return _FlightTime; } set { _FlightTime = (value < 0f ? 0f : value); } }
    //public float MaxSize { get { return _MaximumSize; } set { _MaximumSize = (value < 0f ? 0f : value); } }
    //public float MinSize { get { return _MinimumSize; } set { _MinimumSize = (value < 0f ? 0f : value); } }
    //public int ShootCount { get { return _ShootCount; } set { _ShootCount = (value < 0 ? 0 : value); } }
    //public int ShootArea { get { return _ShootArea; } set { _ShootArea = (value < 0 ? 0 : value); } }

    /*********************************************
     * 덩쿨 채찍 관련 프로퍼티...
     * ***/

    #region Preset

    [SerializeField, HideInInspector]
    public GameObject _AttackEffect1;

    [SerializeField, HideInInspector]
    public Vector3 _AttackOffset;

    [SerializeField, HideInInspector]
    public GameObject _GuardEffect;

    [SerializeField, HideInInspector]
    public Vector3 _GuardOffset;

    [SerializeField, HideInInspector]
    public GameObject _HitEffect;

    [SerializeField, HideInInspector]
    public Vector3 _HitOffset;

    [SerializeField, HideInInspector]
    public GameObject _AirborneEffect;

    [SerializeField, HideInInspector]
    public Vector3 _AirborneOffset;

    [SerializeField, HideInInspector]
    public GameObject _KnockbackEffect;

    [SerializeField, HideInInspector]
    public Vector3 _KnockBackOffset;

    [SerializeField, HideInInspector]
    public GameObject _RunEffect;

    [SerializeField, HideInInspector]
    public Vector3 _RunOffset;

    [SerializeField, HideInInspector]
    public GameObject _LandingEffect;

    [SerializeField, HideInInspector]
    public Vector3 _LandingOffset;

    [SerializeField, HideInInspector]
    public GameObject _UltimateEffect;

    Dictionary<EFFECT, ParticleSystem> _EffectContainer = new Dictionary<EFFECT, ParticleSystem>();

    [SerializeField] private List<ParticleSystem> playList;

    #endregion



    public enum EFFECT
    {
        Attack1 = 0,
        Guard = 1,
        Hit = 2,
        Airborne = 3,
        Knockback = 4,
        Run = 5,
        Landing = 6,
        Ultimate = 7,
    }


    private void Start()
    {

    }

    public void InitParticles()
    {
        // 일단 여기에 이펙트들을 세팅
        GameObject tempEffect = null;
        if (_AttackEffect1 != null)
        {
            tempEffect = Instantiate(_AttackEffect1, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Attack1, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }

        if (_GuardEffect != null)
        {
            tempEffect = Instantiate(_GuardEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Guard, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }

        if (_HitEffect != null)
        {
            tempEffect = Instantiate(_HitEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Hit, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }

        if (_RunEffect != null)
        {
            tempEffect = Instantiate(_RunEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Run, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.GetComponent<ParticleSystem>().Play();
            //tempEffect.SetActive(false);
        }

        if (_AirborneEffect != null)
        {
            tempEffect = Instantiate(_AirborneEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Airborne, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }

        if (_KnockbackEffect != null)
        {
            tempEffect = Instantiate(_KnockbackEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Knockback, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }

        if (_LandingEffect != null)
        {
            tempEffect = Instantiate(_LandingEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Landing, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }

        if (_UltimateEffect != null)
        {
            tempEffect = Instantiate(_UltimateEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Ultimate, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }
    }

    private void Update()
    {
        if(playList.Count > 0)
        {
            for(int i = 0; i < playList.Count; i++)
            {
                if(!playList[i].isPlaying)
                {
                    Stop(playList[i]);
                    playList.RemoveAt(i);
                }
            }
        }
    }


    public void PlayOneShot(EFFECT effectType)
    {
        try
        {
            ParticleSystem effect;
            _EffectContainer.TryGetValue(effectType, out effect);

            if (effect != null)
            {
                playList.Add(Instantiate(effect, effect.transform.position, Quaternion.identity));
                effect.gameObject.SetActive(true);
                
                effect.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                Debug.Log("Notting");
            }
        }
        catch
        {
            Debug.LogError("EffectManager Error");
        }
    }

    public void Play(EFFECT effectType)
    {
        try
        {
            ParticleSystem effect;
            _EffectContainer.TryGetValue(effectType, out effect);

            if (effect != null)
            {
                effect.gameObject.SetActive(true);

                effect.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                Debug.Log("Notting");
            }
        }
        catch
        {
            Debug.LogError("EffectManager Error");
        }
    }

    public void Stop(EFFECT effectType)
    {
        try
        {
            ParticleSystem effect;
            _EffectContainer.TryGetValue(effectType, out effect);

            if (effect != null)
            {
                effect.Stop();
                effect.gameObject.SetActive(false);
            }
        }
        catch
        {
            Debug.LogError("EffectManager Error");
        }
    }

    public void Stop(ParticleSystem effect)
    {
        try
        {
            if (effect != null)
            {
                effect.Stop();
                effect.gameObject.SetActive(false);
                Destroy(effect.gameObject);
            }
        }
        catch
        {
            Debug.LogError("EffectManager Error");
        }
    }
}
