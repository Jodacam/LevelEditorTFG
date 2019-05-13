using UnityEngine;
using UnityEditor;
using LevelEditor.Editor;

[CustomEditor(typeof(RegionTerrain))]
public class RegionTerrainCustomInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var terrain = Selection.activeGameObject.GetComponent<RegionTerrain>();
        if(GUILayout.Button("Edit"))
        {
            RegionEditorWindow.OpenRegionSceneEditor(terrain.owner);
        }
        
    }
}