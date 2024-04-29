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
        private SerializedProperty GuardEffect;
        private SerializedProperty HitEffect;
        private SerializedProperty KnockbackEffect;
        private SerializedProperty LandingEffect;
        private SerializedProperty RunEffect;
        private SerializedProperty UltimateEffect;

        private static GUIStyle BoldLabelStyle;
        private static GUIStyle BoldFoldStyle;

        /* 스타일 관련 내용 */
        private static bool _AcidBombSettingFoldOut = false;
        private static bool _VineAttackFoldOut = false;
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
                AttackEffect = serializedObject.FindProperty("_AttackEffect");
            }

            if (GuardEffect == null)
            {
                GuardEffect = serializedObject.FindProperty("_GuardEffect");
            }

            if (HitEffect == null)
            {
                HitEffect = serializedObject.FindProperty("_HitEffect");
            }

            if (KnockbackEffect== null)
            {
                KnockbackEffect = serializedObject.FindProperty("_KnockbackEffect");
            }

            if (LandingEffect == null)
            {
                LandingEffect = serializedObject.FindProperty("_LandingEffect");
            }

            if (RunEffect == null)
            {
                RunEffect = serializedObject.FindProperty("_RunEffect");
            }

            if (UltimateEffect == null)
            {
                UltimateEffect = serializedObject.FindProperty("_UltimateEffect");
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
            if (!(_VineAttackFoldOut = EditorGUILayout.Foldout(_VineAttackFoldOut, "Init_Effects", BoldFoldStyle)))
            {
                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;
            /* 왼쪽 덩쿨 프리팹 참조필드 표시..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("AttackEffectPrefab", AttackEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    AttackEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(15f);

            /* 오른쪽 덩쿨 프리팹 참조필드 표시..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("GuardEffectPrefab", GuardEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    GuardEffect.objectReferenceValue = value;
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

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("LandingEffectPrefab", LandingEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    LandingEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("RunEffectPrefab", RunEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    RunEffect.objectReferenceValue = value;
                }
            }

            EditorGUILayout.Space(15f);

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("UltimateEffectPrefab", UltimateEffect.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    UltimateEffect.objectReferenceValue = value;
                }
            }

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

    [SerializeField, HideInInspector]
    public GameObject _AttackEffect;

    [SerializeField, HideInInspector]
    public GameObject _GuardEffect;

    [SerializeField, HideInInspector]
    public GameObject _HitEffect;

    [SerializeField, HideInInspector]
    public GameObject _KnockbackEffect;

    [SerializeField, HideInInspector]
    public GameObject _RunEffect;

    [SerializeField, HideInInspector]
    public GameObject _LandingEffect;

    [SerializeField, HideInInspector]
    public GameObject _UltimateEffect;

    Dictionary<EFFECT, GameObject> _EffectContainer = new Dictionary<EFFECT, GameObject>();

    public enum EFFECT
    {
        Attack = 0,
        Guard = 1,
        Hit = 2,
        Knockback = 3,
        Run = 4,
        Landing = 5,
        Ultimate = 6,
    }


    private void Start()
    {
        // 일단 여기에 이펙트들을 세팅
        GameObject tempEffect = null;
        if (_AttackEffect != null)
        {
            tempEffect = Instantiate(_AttackEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Attack, tempEffect);
            tempEffect.SetActive(false);
        }
        
        if(_GuardEffect != null)
        {
            tempEffect = Instantiate(_GuardEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Guard, tempEffect);
            tempEffect.SetActive(false);
        }

        if(_HitEffect != null)
        {
            tempEffect = Instantiate(_HitEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Hit, tempEffect);
            tempEffect.SetActive(false);
        }

        if(_RunEffect  != null)
        {
            tempEffect = Instantiate(_RunEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Run, tempEffect);
            tempEffect.SetActive(false);
        }

        if (_KnockbackEffect != null)
        {
            tempEffect = Instantiate(_KnockbackEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Knockback, tempEffect);
            tempEffect.SetActive(false);
        }

        if(_LandingEffect != null)
        {
            tempEffect = Instantiate(_LandingEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Knockback, tempEffect);
            tempEffect.SetActive(false);
        }

        if(_UltimateEffect != null)
        {
            tempEffect = Instantiate(_UltimateEffect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = this.transform;
            _EffectContainer.Add(EFFECT.Ultimate, tempEffect);
            tempEffect.SetActive(false);
        }
    }

    public void Play(EFFECT effectType)
    {
        try
        {
            GameObject effect;
            _EffectContainer.TryGetValue(effectType, out effect);

            if (effect != null)
            {
                effect.SetActive(true);
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
            GameObject effect;
            _EffectContainer.TryGetValue(effectType, out effect);

            if (effect != null)
            {
                effect.GetComponent<ParticleSystem>().Stop();
                effect.SetActive(false);
            }
        }
        catch
        {
            Debug.LogError("EffectManager Error");
        }
    }
    


}
