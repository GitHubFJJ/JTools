using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    int currentRunningNode;
    protected override void OnStart()
    {
        currentRunningNode=0;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        var child = children[currentRunningNode];
        switch(child.Update()){
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Failure;
            case State.Success:
                currentRunningNode++;
                break;
        }
        return currentRunningNode == children.Count ? State.Success : State.Running;
    }
}
