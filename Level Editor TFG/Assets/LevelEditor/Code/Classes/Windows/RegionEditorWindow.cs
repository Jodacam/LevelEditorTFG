﻿#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using static LevelEditor.LevelRegion;
using UnityEngine.SceneManagement;

namespace LevelEditor.EditorScripts
{
    public class RegionEditorWindow : EditorWindow
    {


        #region Static Functions
        //[MenuItem(Paths.MENU_REGION_EDITOR, false, 3)]
        //Creates the main window.
        public static void OpenWindow()
        {
            if (!AssetDatabase.IsValidFolder(Paths.FOLDER_MAPS))
                AssetDatabase.CreateFolder(Paths.FOLDER_PREFABS, Paths.NAME_MAPS);

            RegionEditorWindow window = (RegionEditorWindow)EditorWindow.GetWindow(typeof(RegionEditorWindow));
            window.titleContent = Style.TITLE_LEVEL_EDITOR_WINDOW;
            window.Show();
        }

        static Vector3 cameraPos = new Vector3(-0.5f, 11, -9);
        static Vector3 cameraRotation = new Vector3(30, 25);



        public static void OpenRegionSceneEditor(LevelRegion region = null)
        {

            previousScene = GUIAuxiliar.OpenNewScene("Region Editor", out var actualScene);
            RegionEditorWindow window = GUIAuxiliar.OpenEditorWindow<RegionEditorWindow>(Style.TITLE_REGION_EDITOR_WINDOW);
            if (String.IsNullOrEmpty(previousScene))
            {
                return;
            }


            //RegionEditorWindow window = (RegionEditorWindow)EditorWindow.GetWindow(typeof(RegionEditorWindow));

            if (!AssetDatabase.IsValidFolder(Paths.FOLDER_MAPS))
                AssetDatabase.CreateFolder(Paths.FOLDER_PREFABS, Paths.NAME_MAPS);
            window.ReStart();
           
            if (region != null)
            {
                Editlevel = region;
                window.LoadGrid();
            }

            window.FindLevel();
            Selection.activeObject = Editlevel.terrainGrid.gameObject;
            SceneView.lastActiveSceneView.FrameSelected();
            window.Show();
            //OpenWindow();
            PrefabCollectionWindow.OpenWindow();
        }



        [MenuItem(Paths.MENU_REGION_SCENE, false, 0)]
        public static void OpenRegionScene()
        {
            //GUIAuxiliar.SetPickerCallBack(new Action<LevelRegion>((value) => Debug.Log(value)));
            OpenRegionSceneEditor();

        }
        #endregion
        #region Constants
        public const string ON_PICK_COMMAND = "ObjectSelectorClosed";
        #endregion
        #region Variables
        Vector2 scrollPosition = Vector2.zero;

        static string previousScene;
        static LevelRegion Editlevel;
        static bool isPicking;

        static SerializedObject levelSerialized;

        #endregion


        /* This functions draw the UI of the window. Unity uses an inmediate GUI system so some buttons
            have theirs functionality here.
        */
        #region GUIFunctions
        private void OnGUI()
        {

            EditorGUI.BeginChangeCheck();
            try
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
                if (levelSerialized == null || Editlevel == null)
                {
                    FindLevel();
                }
                levelSerialized.UpdateIfRequiredOrScript();
                EditorGUILayout.PropertyField(levelSerialized.FindProperty(LevelRegion.LevelProperties.NAME));

                DoGrid();
                //DoVariables();
                levelSerialized.ApplyModifiedProperties();

            }
            catch (Exception e)
            {
                EditorGUILayout.LabelField("Please Load a Valid Level");
                Debug.Log(e.ToString());
            }
            GUILayout.EndScrollView();
            DoSaveAndLoad();
            DoPicker();

        }

        private void Awake()
        {
            FindLevel();
        }

        private void FindLevel()
        {
            if (Editlevel == null)
            {
                var actualTerrain = FindObjectOfType<RegionTerrain>();
                if (actualTerrain != null)
                {
                    Editlevel = actualTerrain.owner;
                    Editlevel.LoadGrid();
                    if (Editlevel.varList == null)
                    {
                        Editlevel.varList = new List<IData>();
                    }
                    //CreateReorderableList();
                }
                else

                {
                    NewMap();
                }
            }
            levelSerialized = new SerializedObject(Editlevel);



        }

        private void ReStart()
        {
            Editlevel = null;
            levelSerialized = null;

        }







        private void DoGrid()
        {
            Vector2Int mapSize = Editlevel.mapSize;
            Vector2 mapScale = Editlevel.mapScale;
            mapSize = EditorGUILayout.Vector2IntField(Style.LABLE_MAP_SIZE, mapSize);
            mapScale = EditorGUILayout.Vector2Field(Style.LABLE_MAP_SCALE, mapScale);
            mapSize.x = Mathf.Clamp(mapSize.x, 1, 100);
            mapSize.y = Mathf.Clamp(mapSize.y, 1, 100);

            if (!mapSize.Equals(Editlevel.mapSize) || !mapScale.Equals(Editlevel.mapScale))
            {
                Editlevel.mapSize = mapSize;
                Editlevel.mapScale = mapScale;
                CreateGrid();
            }




        }

        private void DoSaveAndLoad()
        {
            if (GUILayout.Button(Style.BUTTON_TEXT_SAVE_REGION))
            {
                Save();
            }

            if (GUILayout.Button(Style.BUTTON_TEXT_SAVE_AND_EXIT))
            {
                SaveAndExit();
            }

            if (GUILayout.Button(Style.BUTTON_TEXT_LOAD_REGION))
            {
                Load();
            }

            if (GUILayout.Button(Style.BUTTON_TEXT_NEW_REGION))
            {
                NewMap();
            }

            if (GUILayout.Button("Exit"))
            {
                Exit();
            }
        }



        private void DoPicker()
        {
            string commandName = Event.current.commandName;
            if (commandName == ON_PICK_COMMAND)
            {
                OnPick();
            }

        }
        #endregion

        //Button functionality and some Auxiliar functions.
        #region EditorFuntions
        /**
        Function to Save the level. If the folders have not been created yet, they will be created.

         */
        void Save()
        {
            Paths.CreateFolderIfNotExist(Paths.FOLDER_RESOURCES_LEVEL_EDITOR, Paths.NAME_REGIONS);
            /* if (!AssetDatabase.IsValidFolder(Paths.FOLDER_RESOURCES_LEVEL_EDITOR))
            {

                AssetDatabase.CreateFolder(Paths.FOLDER_RESOURCES, Paths.NAME_LEVEL_EDITOR);
                AssetDatabase.CreateFolder(Paths.FOLDER_RESOURCES_LEVEL_EDITOR, Paths.NAME_REGIONS);
            }
            */


            Editlevel.SaveVars();
            string exist = AssetDatabase.GetAssetPath(Editlevel);
            if (string.IsNullOrEmpty(exist))
            {
                AssetDatabase.CreateAsset(Editlevel, Paths.PATH_REGIONS + Editlevel.regionName + ".asset");
            }


            Paths.CreateFolderIfNotExist(Paths.FOLDER_MAPS, Editlevel.regionName);

            /* if (!AssetDatabase.IsValidFolder(Paths.PATH_MAPS + Editlevel.name))
                AssetDatabase.CreateFolder(Paths.FOLDER_MAPS, Editlevel.name);
            */

            exist = AssetDatabase.GetAssetPath(Editlevel.terrainMesh);
            if (string.IsNullOrEmpty(exist))
            {
                AssetDatabase.CreateAsset(Editlevel.terrainMesh, Paths.PATH_MAPS + Editlevel.regionName + "/" + Editlevel.terrainMesh.name + ".mesh");
                AssetDatabase.CreateAsset(Editlevel.terrainGrid.meshRenderer.sharedMaterial, Paths.PATH_MAPS + Editlevel.regionName + "/Material.mat");
            }


            Editlevel.terrainGameObject = PrefabUtility.SaveAsPrefabAsset(Editlevel.terrainGrid.gameObject, Paths.PATH_MAPS + Editlevel.regionName + "/" + Editlevel.regionName + ".prefab");
            EditorUtility.SetDirty(Editlevel);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = Editlevel;
        }

        private void SaveAndExit()
        {
            Save();
            Exit();

        }

        private void Exit()
        {
            this.Close();
            var actual = PrefabCollectionWindow.GetWindow<PrefabCollectionWindow>();
            actual.ChangeToNone();
            EditorSceneManager.OpenScene(previousScene);

        }

        private void NewMap()
        {
            if (Editlevel != null && Editlevel.terrainGrid.gameObject)
                DestroyImmediate(Editlevel.terrainGrid.gameObject);
            Editlevel = (LevelRegion)CreateInstance(typeof(LevelRegion));
            levelSerialized = new SerializedObject(Editlevel);
            Editlevel.regionName = "New Level";
            Editlevel.mapSize = new Vector2Int(10, 10);
            Editlevel.xcellSize = 1;
            Editlevel.ycellSize = 1;
            Editlevel.varList = new List<IData>();
            //CreateReorderableList();
            CreateGrid();
        }
        private void OnPick()
        {
            if (isPicking)
            {
                LevelRegion pickedObject = (LevelRegion)EditorGUIUtility.GetObjectPickerObject();
                if (pickedObject != null)
                {
                    if (Editlevel.terrainGrid != null)
                    {
                        DestroyImmediate(Editlevel.terrainGrid.gameObject);
                    }
                    Editlevel = pickedObject;

                    levelSerialized = new SerializedObject(Editlevel);
                    LoadGrid();
                    //CreateReorderableList();
                    isPicking = false;
                    Repaint();
                }
            }
        }

        private void Load()
        {
            int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
            isPicking = true;
            EditorGUIUtility.ShowObjectPicker<LevelRegion>(null, false, "", controlID);
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
#endif