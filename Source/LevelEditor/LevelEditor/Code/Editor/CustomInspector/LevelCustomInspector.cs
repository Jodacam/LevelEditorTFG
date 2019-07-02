using UnityEngine;
using UnityEditor;
using LevelEditor;

namespace LevelEditor.EditorScripts
{
    [CustomEditor(typeof(Level))]
    public class LevelCustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Edit"))
            {
                LevelEditorWindow.OpenEditor((Level)Selection.activeObject);
            }
        }
    }
}