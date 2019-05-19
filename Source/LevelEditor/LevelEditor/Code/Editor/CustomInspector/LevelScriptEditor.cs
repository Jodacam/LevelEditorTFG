#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using LevelEditor.Editor;
[CustomEditor(typeof(LevelScript))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var terrain = Selection.activeGameObject.GetComponent<LevelScript>();
        if (GUILayout.Button("Edit"))
        {
            LevelEditorWindow.OpenEditor(terrain.owner);
        }
    }
}
 #endif