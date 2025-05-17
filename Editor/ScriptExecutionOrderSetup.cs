using UnityEditor;
using UnityEngine;

namespace MiniDi.Editor
{
    [InitializeOnLoad]
    public static class ScriptExecutionOrderSetup
    {
        static ScriptExecutionOrderSetup()
        {
            SetExecutionOrder("ProjectContainer", -120);
            SetExecutionOrder("TestSceneContainer", -119);
            SetExecutionOrder("GameObjectContaonerTest", -118);
        }

        private static void SetExecutionOrder(string scriptName, int order)
        {
            var monoScript = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var script in monoScript)
            {
                if (script.name == scriptName)
                {
                    int currentOrder = MonoImporter.GetExecutionOrder(script);
                    if (currentOrder != order)
                    {
                        MonoImporter.SetExecutionOrder(script, order);
                        Debug.Log($"Set script execution order: {scriptName} to {order}");
                    }
                    break;
                }
            }
        }
    }
}