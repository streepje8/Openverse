using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SlyManager : MonoBehaviour
{
    private static List<SlyScriptComponent> scriptComponents = new List<SlyScriptComponent>();

    public static void recompileAll()
    {
        foreach(SlyScriptComponent ssc in scriptComponents)
        {
            if(ssc.Script != null) {
                ssc.Script.Compile();
                if (ssc.instance == null)
                {
                    ssc.instance = new SlyInstance(ssc.Script.compiledClass);
                }
                ssc.instance.recompile(ssc.Script.compiledClass);
            } 
        }
    }

    public static void recompileAllExceptSelf(SlyScript self)
    {
        foreach (SlyScriptComponent ssc in scriptComponents)
        {
            if (ssc.Script != null)
            {
                if(ssc.Script != self) { 
                    ssc.Script.Compile();
                }
                if (ssc.instance == null)
                {
                    ssc.instance = new SlyInstance(ssc.Script.compiledClass);
                }
                ssc.instance.recompile(ssc.Script.compiledClass);
            }
        }
    }

    public static void registerScriptComponent(SlyScriptComponent ssc)
    {
        if (!scriptComponents.Contains(ssc))
        {
            scriptComponents.Add(ssc);
        }
    }
    public static void deregisterScriptComponent(SlyScriptComponent target)
    {
        if(scriptComponents.Contains(target))
        {
            scriptComponents.Remove(target);
        }
    }
}
