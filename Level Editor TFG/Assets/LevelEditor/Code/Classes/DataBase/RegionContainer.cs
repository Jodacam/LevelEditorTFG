using LevelEditor.EditorScripts;
using System;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    [Serializable]
    public class RegionContainer : Container
    {



        [SerializeField]
        public LevelRegion region;





#if UNITY_EDITOR
        private GUILayoutOption maxW = GUILayout.MaxWidth(100);
        private GUILayoutOption maxH = GUILayout.MaxHeight(50);
        private GUILayoutOption maxWButton = GUILayout.MaxWidth(100 / 3);
        private GUILayoutOption maxHButton = GUILayout.MaxHeight(25);


        public override void ShowGUI(PrefabCollectionWindow window)
        {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button(preview, maxW, maxH))
            {
                window.SelectPrefab(this);
            }
            EditorGUILayout.BeginHorizontal(maxW, maxHButton);
            GUIStyle style = new GUIStyle(GUI.skin.button);

            if (GUILayout.Button(Style.ICON_EDIT, style, maxWButton, maxHButton))
            {
                window.Edit(this);
            }
            if (GUILayout.Button(Style.ICON_RELOAD, style, maxWButton, maxHButton))
            {
                window.Reload(this);
            }

            if (GUILayout.Button(Style.ICON_CLOSE, style, maxWButton, maxHButton))
            {
                window.DeletePrefab(this);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        public override void ShowGUIEdit(EditorWindow window)
        {
            region = (LevelRegion)EditorGUILayout.ObjectField(Style.LABLE_REGION_FIELD, region, typeof(LevelRegion), false);
            if (region != null)
            {
                prefab = region.terrainGameObject;
                //prefab.GetComponent<RegionTerrain>().enabled = false;

                preview = preview = AssetPreview.GetAssetPreview(prefab);
                cellSize = region.mapSize;

                pivot = new Vector3((region.mapSize.x * region.mapScale.x) / 2, 0, (region.mapSize.y * region.mapScale.y) / 2);
            }
        }
#endif
    }
}