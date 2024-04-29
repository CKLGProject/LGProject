using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;
using BehaviourTree;
using System.Collections.Generic;


public class BehaviorTreeEditor : EditorWindow
{
    BehaviorTreeView treeView;
    InspectorView inspectorView;
    IMGUIContainer blackboardView;

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;
    
    public VisualTreeAsset BaseUXML;
    public StyleSheet BaseUSS;

    [MenuItem("BehaviorTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    // 에셋을 클릭하여 GUI Editor을 켜는 기능.
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviorTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;


        var BehaviourTreeEditor_uxml = AssetDatabase.GUIDToAssetPath("816d5837ca230b24c9898a1693e4a8ac");
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(BehaviourTreeEditor_uxml);
        visualTree.CloneTree(root);

        var BehaviourTreeEditor_uss = AssetDatabase.GUIDToAssetPath("0abae94caedf05944a403a050ef2358b");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(BehaviourTreeEditor_uss);
        root.styleSheets.Add(styleSheet);


        treeView = root.Q<BehaviorTreeView>();
        treeView.SetEditor(this);
        inspectorView = root.Q<InspectorView>();
        //blackboardView = root.Q<IMGUIContainer>();

        //blackboardView.onGUIHandler = () =>
        //{
        //    if (blackboardProperty == null) { return; }
        //    if (treeObject.targetObject == null) { return; }

        //    treeObject?.Update();
        //    EditorGUILayout.PropertyField(blackboardProperty);
        //    treeObject?.ApplyModifiedProperties();
        //};

        treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                break;
        }
    }

    private void OnSelectionChange()
    {
        BehaviorTree tree = Selection.activeObject as BehaviorTree;
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                var runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                if (runner)
                {
                    tree = runner.tree;
                }
            }
        }

        if (!tree) { return; }

        if (Application.isPlaying ? true : AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        {
            treeView?.PopulateView(tree);
        }

        treeObject = new SerializedObject(tree);
        blackboardProperty = treeObject.FindProperty("blackboard");
    }
    private void OnGUI()
    {
        if (Event.current?.type == EventType.KeyDown && Event.current?.keyCode == KeyCode.Delete)
        {
            DeleteSelectedNode();
            Event.current.Use(); // 이벤트 처리 완료
        }
    }

    private void DeleteSelectedNode()
    {
        // 그래프 뷰에서 선택된 요소들을 가져옴
        var selection = treeView.selection;
        var selections = new List<GraphElement>();

        // 선택된 각 요소에 대해 작업 수행
        foreach (var selectedElement in selection)
        {
            // 선택된 요소가 NodeView인 경우에만 삭제
            if (selectedElement is GraphElement elem)
            {
                selections.Add(elem);
            }
        }

        while (selections.Count > 0)
        {
            var nodeView = selections[0] as NodeView;

            if (nodeView != null)
            {
                Undo.RecordObject(nodeView.node, "Behavior Tree (DeleteNode)");
                nodeView.RemoveInputPort(treeView);
                nodeView.RemoveOutputPort(treeView);
            }

            // 선택된 노드를 삭제
            treeView.DeleteGraphElement(selections[0]);

            selections.RemoveAt(0);
        }

        treeView.PopulateView();
    }
    void OnNodeSelectionChanged(NodeView node)
    {
        inspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeState();
    }
}