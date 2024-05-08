using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;

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
        private SerializedProperty FirstAttackEffect;
        private SerializedProperty FirstAttackOffset;

        private SerializedProperty SecondAttackEffect;
        private SerializedProperty SecondAttackOffset;

        private SerializedProperty ThirdAttackEffect;
        private SerializedProperty ThirdAttackOffset;
        
        private SerializedProperty GuardEffect;
        private SerializedProperty GuardOffset;

        private SerializedProperty DamageEffect;
        private SerializedProperty DamageOffset;

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
        private static bool _FoldOut = false;

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
            GUI_ShowEffectProperties();
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

            #region AttackProperty

            if (FirstAttackEffect == null)
            {
                FirstAttackEffect = serializedObject.FindProperty("_AttackEffect1");
            }

            if (FirstAttackOffset == null)
            {
                FirstAttackOffset = serializedObject.FindProperty("_AttackOffset1");
            }

            if(SecondAttackEffect == null)
            {
                SecondAttackEffect = serializedObject.FindProperty("_AttackEffect2");
            }

            if(SecondAttackOffset == null)
            {
                SecondAttackOffset = serializedObject.FindProperty("_AttackOffset2");
            }

            if(ThirdAttackEffect == null)
            {
                ThirdAttackEffect = serializedObject.FindProperty("_AttackEffect3");
            }

            if(ThirdAttackOffset == null)
            {
                ThirdAttackOffset = serializedObject.FindProperty("_AttackOffset3");
            }
            #endregion

            #region GuardProperty

            if (GuardEffect == null)
            {
                GuardEffect = serializedObject.FindProperty("_GuardEffect");
            }

            if (GuardOffset == null)
            {
                GuardOffset = serializedObject.FindProperty("_GuardOffset");
            }
            #endregion

            #region DamageProperty

            if (DamageEffect == null)
            {
                DamageEffect = serializedObject.FindProperty("_DamageEffect");
            }

            if (DamageOffset == null)
            {
                DamageOffset = serializedObject.FindProperty("_DamageOffset");
            }

            if (AirborneEffect == null)
            {
                AirborneEffect = serializedObject.FindProperty("_AirborneEffect");
            }

            if (AirborneOffset == null)
            {
                AirborneOffset = serializedObject.FindProperty("_AirborneOffset");
            }

            if (KnockbackEffect == null)
            {
                KnockbackEffect = serializedObject.FindProperty("_KnockbackEffect");
            }

            if (KnockbackOffset == null)
            {
                KnockbackOffset = serializedObject.FindProperty("_KnockbackOffset");
            }
            #endregion

            #region MovementProperty

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
                RunOffset = serializedObject.FindProperty("_RunOffset");
            }

            #endregion

            #region UltimateEffect
            if (UltimateEffect == null)
            {
                UltimateEffect = serializedObject.FindProperty("_UltimateEffect");
            }
            
            if (UltimateOffset == null)
            {
                UltimateOffset = serializedObject.FindProperty("_UltimateOffset");
            }
            #endregion

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

        private void GUI_ShowEffectProperties()
        {
            #region Omit
            if (FirstAttackEffect == null || GuardEffect == null)
                return;
            if (!(_FoldOut = EditorGUILayout.Foldout(_FoldOut, "Init_Effects", BoldFoldStyle)))
            {
                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;
            #region AttackScopes

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("First Attack Effect Prefab", FirstAttackEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    FirstAttackEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("First Attack Effect Offset", FirstAttackOffset.vector3Value);
                if (changeScope.changed)
                {
                    FirstAttackOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Second Attack Effect Prefab", SecondAttackEffect.objectReferenceValue, typeof(GameObject), true);
                if(changeScope.changed)
                {
                    SecondAttackEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Second Attack Effect Offset", SecondAttackOffset.vector3Value);
                if (changeScope.changed)
                {
                    SecondAttackOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(10f);


            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Third Attack Effect Prefab", ThirdAttackEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    ThirdAttackEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Third Attack Effect Offset", ThirdAttackOffset.vector3Value);
                if (changeScope.changed)
                {
                    ThirdAttackOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(10f);

            #endregion

            #region GuardScope
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("GuardEffectPrefab", GuardEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    GuardEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Guard Effect Offset", GuardOffset.vector3Value);
                if (changeScope.changed)
                {
                    GuardOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(10f);
            #endregion

            #region DamageScope
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Damaged Effect Prefab", DamageEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    DamageEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Damaged Effect Offset", DamageOffset.vector3Value);
                if (changeScope.changed)
                {
                    DamageOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Airborne Effect Prefab", AirborneEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    AirborneEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Airborne Effect Offset", DamageOffset.vector3Value);
                if (changeScope.changed)
                {
                    AirborneOffset.vector3Value = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("KnockBack Effect Prefab", KnockbackEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    KnockbackEffect.objectReferenceValue = value;
                }
            }

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("KnockBack Effect Offset", KnockbackOffset.vector3Value);
                if (changeScope.changed)
                {
                    KnockbackOffset.vector3Value = value;
                }
            }
            #endregion

            #region Movement Scope
            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("LandingEffectPrefab", LandingEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    LandingEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Landing Effect Offset", LandingOffset.vector3Value);
                if (changeScope.changed)
                {
                    LandingEffect.vector3Value = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Run Effect Prefab", RunEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    RunEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Run Effect Offset", RunOffset.vector3Value);
                if (changeScope.changed)
                {
                    RunEffect.vector3Value = value;
                }
            }

            EditorGUILayout.Space(10f);
            #endregion

            #region Ultimate
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Ultimate Effect Prefab", UltimateEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    UltimateEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(10f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 value = EditorGUILayout.Vector3Field("Ultimate Effect Offset", UltimateOffset.vector3Value);
                if (changeScope.changed)
                {
                    UltimateEffect.vector3Value = value;
                }
            }
            #endregion

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
    public GameObject _AttackEffect2;

    [SerializeField, HideInInspector]
    public GameObject _AttackEffect3;

    [SerializeField, HideInInspector]
    public Vector3 _AttackOffset1;

    [SerializeField, HideInInspector]
    public Vector3 _AttackOffset2;

    [SerializeField, HideInInspector]
    public Vector3 _AttackOffset3;

    [SerializeField, HideInInspector]
    public GameObject _GuardEffect;

    [SerializeField, HideInInspector]
    public Vector3 _GuardOffset;

    [SerializeField, HideInInspector]
    public GameObject _DamageEffect;

    [SerializeField, HideInInspector]
    public Vector3 _DamageOffset;

    [SerializeField, HideInInspector]
    public GameObject _AirborneEffect;

    [SerializeField, HideInInspector]
    public Vector3 _AirborneOffset;

    [SerializeField, HideInInspector]
    public GameObject _KnockbackEffect;

    [SerializeField, HideInInspector]
    public Vector3 _KnockbackOffset;

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

    [SerializeField, HideInInspector]
    public Vector3 _UltimateOffset;

    Dictionary<EFFECT, ParticleSystem> _EffectContainer = new Dictionary<EFFECT, ParticleSystem>();

    [SerializeField] private List<ParticleSystem> playList;

    #endregion



    public enum EFFECT
    {
        Attack1 = 0,
        Attack2 = 1,
        Attack3 = 2,
        Guard = 3,
        Hit = 4,
        Airborne = 5,
        Knockback = 6,
        Run = 7,
        Landing = 8,
        Ultimate = 9,
    }


    private void Start()
    {

    }

    public void InitParticles()
    {
        // 일단 여기에 이펙트들을 세팅
        GameObject tempEffect = null;
        Vector3 posOffset = Vector3.zero;
        Vector3 rotOffset = Vector3.zero;
        if (_AttackEffect1 != null)
        {
            rotOffset = new Vector3(0, 180, 0);
            posOffset = _AttackOffset1;
            tempEffect = Instantiate(_AttackEffect1, transform.position + posOffset, Quaternion.Euler(rotOffset));
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Attack1, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }
        
        if (_AttackEffect2 != null)
        {
            rotOffset = new Vector3(0, 180, 0);
            posOffset = _AttackOffset2;
            tempEffect = Instantiate(_AttackEffect2, transform.position + posOffset, Quaternion.Euler(rotOffset));
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Attack2, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }
        
        if (_AttackEffect3 != null)
        {
            rotOffset = new Vector3(0, 180, 0);
            posOffset = _AttackOffset3;
            tempEffect = Instantiate(_AttackEffect3, transform.position + posOffset, Quaternion.Euler(rotOffset));
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Attack3, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }
        if (_GuardEffect != null)
        {
            tempEffect = Instantiate(_GuardEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Guard, tempEffect.GetComponent<ParticleSystem>());
            tempEffect.SetActive(false);
        }

        if (_DamageEffect != null)
        {
            tempEffect = Instantiate(_DamageEffect, transform.position, Quaternion.identity);
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

    ///// <summary>
    ///// Default Delay Value  1
    ///// </summary>
    ///// <param name="effectType"></param>
    ///// <param name="delayTime"></param>
    //public void PlayToDelay(EFFECT effectType, float delayTime = 1f)
    //{
    //    try
    //    {
    //        ParticleSystem effect;
    //        _EffectContainer.TryGetValue(effectType, out effect);

    //        if (effect != null)
    //        {
    //            effect.gameObject.SetActive(true);

    //            effect.GetComponent<ParticleSystem>().Play();
    //        }
    //        else
    //        {
    //            Debug.Log("Notting");
    //        }
    //    }
    //    catch
    //    {
    //        Debug.LogError("EffectManager Error");
    //    }
    //}

    public async UniTaskVoid Play(EFFECT effectType, float delaySeconds = 0f)
    {
        try
        {
            ParticleSystem effect;
            _EffectContainer.TryGetValue(effectType, out effect);

            if (effect != null)
            {
                effect.gameObject.SetActive(true);
                effect.Stop();
                await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds));
                effect.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                Debug.Log("Not thing");
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
