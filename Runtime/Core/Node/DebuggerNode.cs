using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebuggerNode : ActionNode
{
    [HideInInspector][Flags]
    public enum DebugStatus{
        Nothing = 0,
        OnStart = 1 << 0,
        OnUpdate = 1 << 1,
        OnStop = 1 << 2,
        Everything = OnStart | OnUpdate | OnStop
    }
    public string message;
    public DebugStatus debugStatus = DebugStatus.OnUpdate;
    protected override void OnStart()
    {
        if((debugStatus & DebugStatus.OnStart) != 0){
            Debug.Log($"OnStart{message}");
        }
    }

    protected override void OnStop()
    {
        if((debugStatus & DebugStatus.OnStop) != 0){
        Debug.Log($"OnStop{message}");
        }
    }

    protected override State OnUpdate()
    {
        if((debugStatus & DebugStatus.OnUpdate) != 0){
            Debug.Log($"OnUpdate{message}");
        }
        return State.Success;
    }
}
