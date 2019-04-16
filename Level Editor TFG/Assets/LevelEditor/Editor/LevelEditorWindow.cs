using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelEditorWindow : EditorWindow
    {


        #region Static Functions
        [MenuItem("LevelEditor/EditorWindow")]
        //Creates the main window.
        public static void OpenWindow()
        {
            LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
            window.titleContent = Style.TITLE_LEVEL_EDITOR_WINDOW;
            window.Show();
        }
        #endregion

        #region Variables
        Vector2 scrollPosition = Vector2.zero;
        static Level Editlevel;
        static bool isPicking;

        SerializedObject levelSerialized;


        #endregion



        #region GUIFunctions
        private void OnGUI()
        {

            EditorGUI.BeginChangeCheck();
            try
            {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            levelSerialized.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(levelSerialized.FindProperty(Level.LevelProperties.NAME));
            
                DoGrid();
                DoVariables();
                levelSerialized.ApplyModifiedProperties();
               
            }
            catch(Exception e)
            {
                EditorGUILayout.LabelField("Please Load a Valid Level");
            }
            GUILayout.EndScrollView();
            DoSaveAndLoad();
            DoPicker();

        }

        private void DoVariables()
        {

            Editlevel.EditVariable(this);

            if (GUILayout.Button(Style.ICON_ADD))
            {
                AddNewVariable();
            }

        }

        private void AddNewVariable()
        {
            var e = new VariableString();
            e.Init("newName");
           
            Editlevel.AddVariable(e);
        }

        private void Awake()
        {

            if (Editlevel == null)
            {
                NewMap();
            }
            levelSerialized = new SerializedObject(Editlevel);
        }

        #endregion

        private void DoGrid()
        {
            Vector2Int mapSize = Editlevel.mapSize;
            Vector2 mapScale = Editlevel.mapScale;
            mapSize = EditorGUILayout.Vector2IntField(Style.LABLE_MAP_SIZE, mapSize);
            mapScale = EditorGUILayout.Vector2Field(Style.LABLE_MAP_SCALE, mapScale);
            if (mapSize.x <= 0)
            {
                mapSize.x = 1;
            }
            if (mapSize.y <= 0)
            {
                mapSize.y = 1;
            }
            if (!mapSize.Equals(Editlevel.mapSize) || !mapScale.Equals(Editlevel.mapScale))
            {
                Editlevel.mapSize = mapSize;
                Editlevel.mapScale = mapScale;
                CreateGrid();
            }




        }

        private void DoSaveAndLoad()
        {
            if (GUILayout.Button(Style.BUTTON_TEXT_SAVE))
            {
                Save();
            }

            if (GUILayout.Button(Style.BUTTON_TEXT_LOAD))
            {
                Load();
            }

            if (GUILayout.Button(Style.BUTTON_TEXT_NEW))
            {
                NewMap();
            }
        }

        private void DoPicker()
        {
            string commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorClosed")
            {
                OnPick();
            }

        }

        #region EditorFuntions

        void Save()
        {
            Editlevel.SaveVars();
            string exist = AssetDatabase.GetAssetPath(Editlevel);
            if (string.IsNullOrEmpty(exist))
            {
                AssetDatabase.CreateAsset(Editlevel, "Assets/Resources/" + Editlevel.name + ".asset");
            }

            if (!AssetDatabase.IsValidFolder("Assets/LevelEditor/Prefabs/Maps/" + Editlevel.name))
                AssetDatabase.CreateFolder("Assets/LevelEditor/Prefabs/Maps", Editlevel.name);


            exist = AssetDatabase.GetAssetPath(Editlevel.terrainMesh);
            if (string.IsNullOrEmpty(exist))
            {
                AssetDatabase.CreateAsset(Editlevel.terrainMesh, "Assets/LevelEditor/Prefabs/Maps/" + Editlevel.name + "/" + Editlevel.terrainMesh.name + ".mesh");
                AssetDatabase.CreateAsset(Editlevel.terrainGrid.meshRenderer.material, "Assets/LevelEditor/Prefabs/Maps/" + Editlevel.name + "/Material.mat");
            }


            Editlevel.terrainGameObjec = PrefabUtility.SaveAsPrefabAsset(Editlevel.terrainGrid.gameObject, "Assets/LevelEditor/Prefabs/Maps/" + Editlevel.name + "/" + Editlevel.name + ".prefab");
            EditorUtility.SetDirty(Editlevel);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = Editlevel;
        }

        private void NewMap()
        {
            if (Editlevel != null)
                DestroyImmediate(Editlevel.terrainGrid.gameObject);
            Editlevel = (Level)CreateInstance(typeof(Level));
            levelSerialized = new SerializedObject(Editlevel);
            Editlevel.name = "New Level";
            Editlevel.mapSize = new Vector2Int(10, 10);
            Editlevel.xcellSize = 1;
            Editlevel.ycellSize = 1;
            Editlevel.stringList = new List<IData>();
            CreateGrid();
        }
        private void OnPick()
        {
            if (isPicking)
            {
                Level pickedObject = (Level)EditorGUIUtility.GetObjectPickerObject();
                if (pickedObject != null)
                {
                    DestroyImmediate(Editlevel.terrainGrid.gameObject);
                    Editlevel = pickedObject;
                    levelSerialized = new SerializedObject(Editlevel);
                    LoadGrid();
                    isPicking = false;
                    Repaint();
                }
            }
        }

        private void Load()
        {
            int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
            isPicking = true;
            EditorGUIUtility.ShowObjectPicker<Level>(null, false, "", controlID);
        }

        private void LoadGrid()
        {

            Editlevel.LoadGrid();

        }

        private void CreateGrid()
        {
            if (Editlevel.terrainGrid != null)
            {
                Editlevel.ReCreateGrid();
            }
            else
            {
                Editlevel.CreateGrid();

            }

        }
        #endregion

    }
}
