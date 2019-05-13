using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using static Level;
using UnityEditor.SceneManagement;
using System;

public class LevelEditorWindow : EditorWindow
{

    [MenuItem(Paths.MENU_LEVEL_SCENE)]
    private static void OpenLevelEditor()
    {
        OpenEditor();
    }

    private static void OpenEditor(Level editLevel = null)
    {
        previousScene = GUIAuxiliar.OpenNewScene("Level Editor", out var scene);
        if (editLevel != null)
        {
            actualLevel = editLevel;
        }

        else
        {

        }
        var window = LevelEditorWindow.GetWindow<LevelEditorWindow>();
        window.titleContent = Style.TITLE_LEVEL_EDITOR_WINDOW;
        window.maxSize = new Vector2(300, 700);
        window.minSize = new Vector2(300, 300);
        window.Show();

    }


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
        list.DoLayoutList();
        levelSerialized.ApplyModifiedProperties();
        GUILayout.EndScrollView();
        DrawSaveAndLoad();
    }

    private void DrawGridProperties()
    {
        Vector2Int mapSize = actualLevel.cellCount;
        Vector2 mapScale = actualLevel.cellSize;
        mapSize = EditorGUILayout.Vector2IntField(Style.LABLE_MAP_SIZE, mapSize);
        mapScale = EditorGUILayout.Vector2Field(Style.LABLE_MAP_SCALE, mapScale);
        mapSize.x = Mathf.Clamp(mapSize.x, 1, 100);
        mapSize.y = Mathf.Clamp(mapSize.y, 1, 100);

        if (!mapSize.Equals(actualLevel.cellCount) || !mapScale.Equals(actualLevel.cellSize))
        {
            actualLevel.cellCount = mapSize;
            actualLevel.cellSize = mapScale;
            ScaleGrid();
        }
    }

    private void ScaleGrid()
    {

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
    }

    private void NewMap()
    {
        throw new NotImplementedException();
    }

    private void Load()
    {
        throw new NotImplementedException();
    }

    private void SaveAndExit()
    {
        throw new NotImplementedException();
    }

    private void Save()
    {
        throw new NotImplementedException();
    }

    //Check if something is null. If it is inicialice it.
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

        var e = (VariableTypes)EditorGUI.EnumPopup(propRect, "Type", variable.type);
        if (e != variable.type)
        {

            actualLevel.ChangeVariableType(variable, e);

        }
        propRect.y += 15;
        variable.ShowGUI(propRect);

        EditorGUI.indentLevel++;
    }

    private void AddNewVariable()
    {
        var e = new VariableString();
        e.Init("new var");

        actualLevel.AddVariable(e);
    }
}