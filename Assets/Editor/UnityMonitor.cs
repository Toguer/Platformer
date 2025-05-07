using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public class UnityMonitor : Editor
{
    static Transform copiedTransform;

    static UnityMonitor()
    {
        EditorApplication.update += EditorUpdating;
    }

    static void EditorUpdating()
    {
        if (Keyboard.current.f4Key.ReadValue() == 1)
            ClearLog();

        if (Keyboard.current.leftCtrlKey.ReadValue() == 1 && Keyboard.current.rKey.ReadValue() == 1 && Selection.objects.Length == 1)
        {
            if (copiedTransform != Selection.activeTransform)
            {
                copiedTransform = Selection.activeTransform;
                Debug.Log("Copied transform: " + copiedTransform.position);
            }
        }
        if (Keyboard.current.leftCtrlKey.ReadValue() == 1 && Keyboard.current.tKey.ReadValue() == 1 && Selection.objects.Length == 1)
        {
            if (copiedTransform != null)
            {
                Selection.activeGameObject.transform.position = copiedTransform.position;
                Selection.activeGameObject.transform.rotation = copiedTransform.rotation;
                copiedTransform = null;
            }
        }
    }


    static void ClearLog() 
    {
        var assembly = Assembly.GetAssembly(typeof(Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
