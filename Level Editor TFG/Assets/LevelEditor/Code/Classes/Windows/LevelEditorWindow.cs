using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using static Level;
using UnityEditor.SceneManagement;
using System;

namespace LevelEditor.Editor
{
    public class LevelEditorWindow : EditorWindow
    {

        [MenuItem(Paths.MENU_LEVEL_SCENE,false,0)]
        private static void OpenLevelEditor()
        {
            OpenEditor();
        }

        public static void OpenEditor(Level editLevel = null)
        {
            string pre = GUIAuxiliar.OpenNewScene("Level Editor", out var scene);
            if (String.IsNullOrEmpty(pre))
            {
                return;
            }

            previousScene = pre;
            var window = LevelEditorWindow.GetWindow<LevelEditorWindow>();
            if (editLevel != null)
            {
                actualLevel = editLevel;
                actualLevel.LoadLevel(Vector3.zero, null);
                Selection.activeGameObject = actualLevel.runTimeTerrain;
                SceneView.lastActiveSceneView.FrameSelected();
            }

            else
            {
                actualLevel = null;
                window.InitWindow();
            }

            window.titleContent = Style.TITLE_LEVEL_EDITOR_WINDOW;
            Selection.activeGameObject = actualLevel.runTimeTerrain;
            SceneView.lastActiveSceneView.FrameSelected();

            window.maxSize = new Vector2(300, 700);
            window.minSize = new Vector2(300, 300);

            window.Show();

            //We create the folders here.
            if (!AssetDatabase.IsValidFolder(Paths.FOLDER_LEVELS))
            {
                AssetDatabase.CreateFolder(Paths.FOLDER_PREFABS, Paths.NAME_LEVELS);

            }
            if (!AssetDatabase.IsValidFolder(Paths.FOLDER_RESOURCE_LEVEL))
            {
                AssetDatabase.CreateFolder(Paths.FOLDER_RESOURCES_LEVEL_EDITOR, Paths.NAME_LEVELS);
            }
        }

        bool isPicking = false;
        static Vector2 scrollPosition;
        static Level actualLevel;

        static string previousScene;
        static SerializedObject levelSerialized;
        public static ReorderableList list;
        private void OnGUI()
        {
            InitWindow();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            levelSerialized.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(levelSerialized.FindProperty(Level.Properties.NAME));
            DrawGridProperties();
            list.DoLayoutList();
            levelSerialized.ApplyModifiedProperties();
            GUILayout.EndScrollView();
            DrawSaveAndLoad();
            CheckPicker();
        }

        private void DrawGridProperties()
        {
            Vector2Int mapSize = actualLevel.cellCount;
            Vector2 mapScale = actualLevel.cellSize;
            mapSize = EditorGUILayout.Vector2IntField(Style.LABLE_MAP_SIZE, mapSize);
            mapScale = EditorGUILayout.Vector2Field(Style.LABLE_MAP_SCALE, mapScale);

            if (!mapSize.Equals(actualLevel.cellCount) || !mapScale.Equals(actualLevel.cellSize))
            {
                actualLevel.cellCount = mapSize;
                actualLevel.cellSize = mapScale;
                ScaleGrid();
            }
        }

        private void ScaleGrid()
        {
            actualLevel.ScaleGrid();
        }

        private void DrawSaveAndLoad()
        {
            if (GUILayout.Button(Style.BUTTON_TEXT_SAVE))
            {
                Save();
            }

            if (GUILayout.Button(Style.BUTTON_TEXT_SAVE_AND_EXIT))
            {
                SaveAndExit();
            }

            if (GUILayout.Button(Style.BUTTON_TEXT_LOAD))
            {
                Load();
            }

            if (GUILayout.Button(Style.BUTTON_TEXT_NEW))
            {
                NewMap();
            }


            if (GUILayout.Button("Exit"))
            {
                Exit();
            }
        }

        private void CheckPicker()
        {
            string commandName = Event.current.commandName;
            if (commandName == RegionEditorWindow.ON_PICK_COMMAND)
            {
                if (isPicking)
                {
                    Level data = (Level)EditorGUIUtility.GetObjectPickerObject();
                    if (data != null)
                    {
                        GUIAuxiliar.Destroy(actualLevel.runTimeTerrain);
                        actualLevel = data;
                        levelSerialized = new SerializedObject(actualLevel);
                        CreateReorderableList();
                        Selection.activeGameObject = actualLevel.runTimeTerrain;
                        SceneView.lastActiveSceneView.FrameSelected();

                    }
                    isPicking = false;
                }
            }
        }
        private void NewMap()
        {
            GUIAuxiliar.Destroy(actualLevel.runTimeTerrain);
            actualLevel = null;
            InitWindow();
            levelSerialized = new SerializedObject(actualLevel);
            CreateReorderableList();
            Selection.activeGameObject = actualLevel.runTimeTerrain;
            SceneView.lastActiveSceneView.FrameSelected();

        }

        private void Load()
        {
            int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
            isPicking = true;
            EditorGUIUtility.ShowObjectPicker<Level>(null, false, "", controlID);
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

        private void Save()
        {

            actualLevel.SaveVars();
            string exist = AssetDatabase.GetAssetPath(actualLevel);
            if (string.IsNullOrEmpty(exist))
            {
                AssetDatabase.CreateAsset(actualLevel, Paths.PATH_RESOURCE_LEVELS + actualLevel.levelName + ".asset");
            }
            actualLevel.SaveItself(Paths.FOLDER_LEVELS);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        /// <summary>
        ///  Check if something is null. If it is inicialice it.
        /// </summary>
        private void InitWindow()
        {
            if (actualLevel == null)
            {
                var inSceneObject = FindObjectOfType<LevelScript>();
                if (inSceneObject == null)
                {
                    actualLevel = Level.CreateInstance<Level>();
                    actualLevel.Init("New Level");
                    levelSerialized = new SerializedObject(actualLevel);
                }
                else
                {
                    actualLevel = inSceneObject.owner;
                }

                CreateReorderableList();
            }
            if (levelSerialized == null)
            {
                levelSerialized = new SerializedObject(actualLevel);
            }

            if (list == null)
            {
                CreateReorderableList();
            }

        }

        private void CreateReorderableList()
        {
            if (actualLevel.varList == null)
            {
                actualLevel.LoadVars();
            }
            list = new ReorderableList(actualLevel.varList, typeof(IData));


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

            var variable = actualLevel.varList[index];
            variable.varName = EditorGUI.TextField(headerRect, variable.varName);

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel += 2;
            var e = (VariableTypes)EditorGUI.EnumPopup(propRect, "Type", variable.type);
            if (e != variable.type)
            {

                actualLevel.ChangeVariableType(variable, e);

            }
            propRect.y += 15;
            variable.ShowGUI(propRect);
            EditorGUI.indentLevel -= 2;
            EditorGUI.indentLevel++;
        }

        private void AddNewVariable()
        {
            var e = new VariableString();
            e.Init("new var");

            actualLevel.AddVariable(e);
        }
    }
}