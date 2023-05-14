using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using BehaviorTreePackage;
using System;

public class BehaviorTreeEditor : EditorWindow
{
    BehaviorTreeViewer behaviorTreeViewer;
    InspectorViewer inspectorViewer;
    [MenuItem("JayEditor/BehaviorTreeEditor")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId,int line){
        if(Selection.activeObject is BehaviorTree){
            OpenWindow();
            return true;
        }
        return false;
    }
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/JayEditor/Editor/UI/StateMachineEditor/BehaviorTreeEditor.uxml");
        visualTree.CloneTree(root);
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/JayEditor/Editor/UI/StateMachineEditor/BehaviorTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        behaviorTreeViewer = root.Q<BehaviorTreeViewer>();
        inspectorViewer = root.Q<InspectorViewer>();
        behaviorTreeViewer.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }
    private void OnEnable() {
        EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
        EditorApplication.playModeStateChanged += OnplayModeStateChanged;
    }
    private void OnDisable() {
        EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
    }
    private void OnplayModeStateChanged(PlayModeStateChange obj) {
        switch(obj){
            case PlayModeStateChange.EnteredEditMode :
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode :
                break;
            case PlayModeStateChange.EnteredPlayMode :
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode :
                break;
        }
    }

    private void OnSelectionChange() {
        BehaviorTree tree = Selection.activeObject as BehaviorTree;
        if(!tree){
            if(Selection.activeGameObject){
                BehaviorTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                if(runner){
                    tree = runner.tree;
                }
            }
        }
        if(Application.isPlaying){
            if(tree){
                if(behaviorTreeViewer != null){
                    behaviorTreeViewer.PopulateView(tree);
                }
            }
        }else{
            if(tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())){
                if(behaviorTreeViewer != null){
                    behaviorTreeViewer.PopulateView(tree);
                }
            }
        }
    }
    void OnNodeSelectionChanged(NodeView node){
        inspectorViewer.UpdateSelection(node);
    }
    private void OnInspectorUpdate() {
        behaviorTreeViewer?.UpdateNodeStates();
    }
}