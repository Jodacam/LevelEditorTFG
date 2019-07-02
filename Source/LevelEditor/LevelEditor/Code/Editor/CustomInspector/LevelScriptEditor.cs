using UnityEngine;
using UnityEditor;

namespace LevelEditor.EditorScripts
{
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
}