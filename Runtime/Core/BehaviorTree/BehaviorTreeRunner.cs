using UnityEngine;
using BehaviorTreePackage;
public class BehaviorTreeRunner : MonoBehaviour
{
    public BehaviorTree tree;

    private void Start() {
        tree = tree.Clone();
    }
    void Update()
    {
        tree.Update();
    }
}
