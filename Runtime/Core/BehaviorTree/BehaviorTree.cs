using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviorTreePackage{
    [CreateAssetMenu()]
    public class BehaviorTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State treeState = Node.State.Running;
        public List<Node> nodes = new List<Node>();

        public Node.State Update() {
            if(rootNode.state == Node.State.Running){
                treeState = rootNode.Update();
            }
            return treeState;
        }
#if UNITY_EDITOR
        public Node CreateNode(System.Type type){
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name =type.Name;
            node.guid = GUID.Generate().ToString();
         
            Undo.RecordObject(this,"Behaviour Tree (CreateNode)");

            nodes.Add(node);
            if(!Application.isPlaying){
                AssetDatabase.AddObjectToAsset(node,this);
            }
            Undo.RegisterCreatedObjectUndo(node,"Behaviour Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return node;
        }
        public Node DeleteeNode(Node node){
            Undo.RecordObject(this,"Behaviour Tree (DeleteNode)");
            nodes.Remove(node);
            // AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void AddChild(Node parent, Node child){
            RootNode rootNode = parent as RootNode;
            if(rootNode){
                Undo.RecordObject(rootNode,"Behaviour Tree (AddChild)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }
            DecoratorNode decorator = parent as DecoratorNode;
            if(decorator){
                Undo.RecordObject(decorator,"Behaviour Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }
            CompositeNode composite = parent as CompositeNode;
            if(composite){
                Undo.RecordObject(composite,"Behaviour Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }
        public void RemoveChild(Node parent, Node child){
            RootNode rootNode = parent as RootNode;
            if(rootNode){
                Undo.RecordObject(rootNode,"Behaviour Tree (RemoveChild)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }
            DecoratorNode decorator = parent as DecoratorNode;
            if(decorator){
                Undo.RecordObject(decorator,"Behaviour Tree (RemoveChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }
            CompositeNode composite = parent as CompositeNode;
            if(composite){
                Undo.RecordObject(composite,"Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }
        public List<Node> GetChildren(Node parent){
            List<Node> Children = new List<Node>();
            RootNode rootNode = parent as RootNode;
            if(rootNode && rootNode.child != null){
                Children.Add(rootNode.child);
            }
            DecoratorNode decorator = parent as DecoratorNode;
            if(decorator && decorator.child != null){
                Children.Add(decorator.child);
            }
            CompositeNode composite = parent as CompositeNode;
            if(composite){
               return composite.children;
            }
            return Children;
        }
#endif
        public void Traverse(Node node, System.Action<Node> visiter){
            if(node){
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n,visiter));
            }
        }
        public BehaviorTree Clone(){
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, (n)=>{
                tree.nodes.Add(n);
            });
            return tree;
        }
    }
}