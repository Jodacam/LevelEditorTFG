using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class LevelEditorWindow : EditorWindow
{


    #region Static Functions
    [MenuItem("LevelEditor/EditorWindow")]
    //Creates the main window.
    public static void OpenWindow()
    {
        LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
        window.name = Style.TITLE_LEVEL_EDITOR_WINDOW;
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
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
        levelSerialized.UpdateIfRequiredOrScript();
        EditorGUILayout.PropertyField(levelSerialized.FindProperty(Level.LevelProperties.NAME));
        
        DoGrid();
        
        levelSerialized.ApplyModifiedProperties();
        GUILayout.EndScrollView();

        DoSaveAndLoad();
        DoPicker();

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
        mapSize = EditorGUILayout.Vector2IntField(Style.LABLE_MAP_SIZE,mapSize);



        Editlevel.mapSize = mapSize;

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
        Level n = Editlevel;
        string exist = AssetDatabase.GetAssetPath(n);
        if (string.IsNullOrEmpty(exist))
        {
            AssetDatabase.CreateAsset(n, "Assets/Resources/" + Editlevel.name + ".asset");
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = n;
    }

    private void NewMap()
    {
         Editlevel = (Level)CreateInstance(typeof(Level));
         Editlevel.name = "New Level";
         Editlevel.mapSize = new Vector2Int(10,10);
         Editlevel.xcellSize = 1;
         Editlevel.ycellSize = 1;
         CreateGrid();
    }
    private void OnPick()
    {
        if (isPicking)
        {
            Level pickedObject = (Level)EditorGUIUtility.GetObjectPickerObject();
            if (pickedObject != null)
            {
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

    private void LoadGrid() { }

    private void CreateGrid()
    {
        GameObject plane = new GameObject("BaseTerrain",typeof(GridTerrain));
        plane.AddComponent<MeshFilter>();
        MeshRenderer e =  plane.AddComponent<MeshRenderer>();
        plane.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);
        plane.GetComponent<GridTerrain>().CreateGrid(Editlevel.xcellSize,Editlevel.ycellSize,Editlevel.mapSize);
        e.material = new Material(Shader.Find("Standard"));
        
    }
    #endregion

}
