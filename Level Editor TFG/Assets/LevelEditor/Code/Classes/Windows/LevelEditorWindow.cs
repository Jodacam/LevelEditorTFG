#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using static Level;



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
        #region Constants
        const string ON_PICK_COMMAND = "ObjectSelectorClosed";
        #endregion
        #region Variables
        Vector2 scrollPosition = Vector2.zero;
        static Level Editlevel;
        static bool isPicking;

        static SerializedObject levelSerialized;
        public static ReorderableList list;

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
                EditorGUILayout.PropertyField(levelSerialized.FindProperty(Level.LevelProperties.NAME));

                DoGrid();
                DoVariables();
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

        /**
        Draw the Variable list
        */
        private void DoVariables()
        {
            list.DoLayoutList();
        }

        private void AddNewVariable()
        {
            var e = new VariableString();
            e.Init("new var");

            Editlevel.AddVariable(e);
        }

        private void Awake()
        {

            FindLevel();


        }

        private void FindLevel()
        {
            if (Editlevel == null)
            {
                var actualTerrain = FindObjectOfType<GridTerrain>();
                if (actualTerrain != null)
                {
                    Editlevel = actualTerrain.owner;
                    Editlevel.LoadGrid();
                    if (Editlevel.stringList == null)
                    {
                        Editlevel.stringList = new List<IData>();
                    }
                    CreateReorderableList();
                }
                else

                {
                    NewMap();
                }
            }
            levelSerialized = new SerializedObject(Editlevel);



        }


        private void CreateReorderableList()
        {
            list = new ReorderableList(Editlevel.stringList, typeof(IData));


            list.drawElementCallback = this.onDrawReorderlist;

            list.elementHeightCallback = (index) =>
            {
                return 60;
            };

            list.onAddCallback += (list) =>
            {
                AddNewVariable();
            };

            list.drawHeaderCallback = (Rect) => GUI.Label(Rect, "Variables");
        }

        private void onDrawReorderlist(Rect rect, int index, bool isActive, bool isFocused)
        {
            var propRect = new Rect(rect.x,
                                     rect.y + EditorGUIUtility.standardVerticalSpacing + 15,
                                     rect.width - 2 * Style.defaultIndentWidth,
                                     EditorGUIUtility.singleLineHeight);

            var headerRect = new Rect(rect.x + Style.defaultIndentWidth,
                                        rect.y + EditorGUIUtility.standardVerticalSpacing,
                                        rect.width - Style.defaultIndentWidth,
                                        EditorGUIUtility.singleLineHeight);

            var variable = Editlevel.stringList[index];
            variable.varName = EditorGUI.TextField(headerRect, variable.varName);

            EditorGUI.indentLevel--;

            var e = (VariableTypes)EditorGUI.EnumPopup(propRect, "Type", variable.type);
            if (e != variable.type)
            {

                Editlevel.ChangeVariableType(variable, e);

            }
            propRect.y += 15;
            variable.ShowGUI(propRect);

            EditorGUI.indentLevel++;
        }


        #endregion

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
            if (commandName == ON_PICK_COMMAND)
            {
                OnPick();
            }

        }

        //Button functionality and some Auxiliar functions.
        #region EditorFuntions
        /**
        Function to Save the level. If the folders have not been created yet, they will be created.

         */
        void Save()
        {
            if (!AssetDatabase.IsValidFolder(Paths.FOLDER_RESOURCES_LEVEL_EDITOR))
            {

                AssetDatabase.CreateFolder(Paths.FOLDER_RESOURCES, Paths.NAME_LEVEL_EDITOR);
                AssetDatabase.CreateFolder(Paths.FOLDER_RESOURCES_LEVEL_EDITOR, Paths.NAME_LEVELS);
            }



            Editlevel.SaveVars();
            string exist = AssetDatabase.GetAssetPath(Editlevel);
            if (string.IsNullOrEmpty(exist))
            {
                AssetDatabase.CreateAsset(Editlevel, Paths.PATH_LEVELS + Editlevel.name + ".asset");
            }

            if (!AssetDatabase.IsValidFolder(Paths.PATH_MAPS + Editlevel.name))
                AssetDatabase.CreateFolder(Paths.FOLDER_MAPS, Editlevel.name);


            exist = AssetDatabase.GetAssetPath(Editlevel.terrainMesh);
            if (string.IsNullOrEmpty(exist))
            {
                AssetDatabase.CreateAsset(Editlevel.terrainMesh, Paths.PATH_MAPS + Editlevel.name + "/" + Editlevel.terrainMesh.name + ".mesh");
                AssetDatabase.CreateAsset(Editlevel.terrainGrid.meshRenderer.sharedMaterial, Paths.PATH_MAPS + Editlevel.name + "/Material.mat");
            }


            Editlevel.terrainGameObject = PrefabUtility.SaveAsPrefabAsset(Editlevel.terrainGrid.gameObject, Paths.PATH_MAPS + Editlevel.name + "/" + Editlevel.name + ".prefab");
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
            CreateReorderableList();
            CreateGrid();
        }
        private void OnPick()
        {
            if (isPicking)
            {
                Level pickedObject = (Level)EditorGUIUtility.GetObjectPickerObject();
                if (pickedObject != null)
                {
                    if (Editlevel.terrainGrid != null)
                    {
                        DestroyImmediate(Editlevel.terrainGrid.gameObject);
                    }
                    Editlevel = pickedObject;

                    levelSerialized = new SerializedObject(Editlevel);
                    LoadGrid();
                    CreateReorderableList();
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
#endif