using Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PetDebug : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/Analysis/Pet Debug")]
    public static void ShowExample()
    {
        PetDebug wnd = GetWindow<PetDebug>();
        wnd.titleContent = new GUIContent("Pet Debug");
        wnd.minSize = new Vector2(400, 250);
        wnd.maxSize = new Vector2(400, 250);
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement uxml = m_VisualTreeAsset.Instantiate();
        root.Add(uxml);
        
        EnumField petEnumField = uxml.Q<EnumField>("pet-selection-field");
        Button enableButton = uxml.Q<Button>("enable-button");
        Button hasButton = uxml.Q<Button>("have-button");
        Button checkPetButton = uxml.Q<Button>("has-pet-list-button");
        Button resetPetButton = uxml.Q<Button>("pet-reset-button");
        
        enableButton.clickable.clicked += () =>
        {
            string isEnablePetMapJson = PlayerPrefs.GetString("IsEnablePetMap", "{}");
            Dictionary<EPetType, bool> isEnablePetMap = JsonConvert.DeserializeObject<Dictionary<EPetType, bool>>(isEnablePetMapJson);
            
            isEnablePetMap[(EPetType)petEnumField.value] = true;
            PlayerPrefs.SetString("IsEnablePetMap", JsonConvert.SerializeObject(isEnablePetMap));
            
            Debug.Log($"Pet Type: {petEnumField.value}, IsEnable: {isEnablePetMap[(EPetType)petEnumField.value]}");
        };
        
        hasButton.clickable.clicked += () =>
        {
            Debug.Log("--Check Has List--");
            
            string hasPetMapJson = PlayerPrefs.GetString("HasPetMap", "{}");
            Dictionary<EPetType, bool> hasPetMap = JsonConvert.DeserializeObject<Dictionary<EPetType, bool>>(hasPetMapJson);
            
            hasPetMap[(EPetType)petEnumField.value] = true;
            PlayerPrefs.SetString("HasPetMap", JsonConvert.SerializeObject(hasPetMap));
            
            Debug.Log($"Pet Type: {petEnumField.value}, Has: {hasPetMap[(EPetType)petEnumField.value]}");
        };
        
        checkPetButton.clickable.clicked += () =>
        {
            Debug.Log("--Check Pet List--");
            
            // 펫을 활성화 했는지 체크
            string isEnablePetMapJson = PlayerPrefs.GetString("IsEnablePetMap", "{}");
            Dictionary<EPetType, bool> isEnablePetMap = JsonConvert.DeserializeObject<Dictionary<EPetType, bool>>(isEnablePetMapJson);
            
            foreach (KeyValuePair<EPetType, bool> pet in isEnablePetMap) 
                Debug.Log($"Pet Type: {pet.Key}, IsEnable: {pet.Value}");
        };
        
        resetPetButton.clickable.clicked += () =>
        {
            PlayerPrefs.SetString("HasPetMap", "{}");
            PlayerPrefs.SetString("IsEnablePetMap", "{}");
            
            Debug.Log("--Reset Pet List--");
            EditorUtility.DisplayDialog("Pet Reset", "Pet Data Reset", "OK");
        };
    }
}
