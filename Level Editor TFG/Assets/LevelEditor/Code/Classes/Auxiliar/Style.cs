using System;
using UnityEditor;
using UnityEngine;

public static class Style
{

    //This class serves to store all the style constants, icons, and labels.
    #region Labels
    

    public static readonly GUIContent TITLE_LEVEL_EDITOR_WINDOW = EditorGUIUtility.TrTextContentWithIcon("Level Editor Window",MessageType.Info);

    public static readonly GUIContent TITLE_REGION_EDITOR_WINDOW = EditorGUIUtility.TrTextContentWithIcon("Region Editor Window",MessageType.Info);
    public static readonly GUIContent TITLE_PREFAB_COLLECTION_WINDOW = EditorGUIUtility.TrTextContentWithIcon("Prefab Collection Window",MessageType.Info);
    
    public const string BUTTON_TEXT_SAVE = "Save Level";

    public const string BUTTON_TEXT_LOAD = "Load Level";

    public const string BUTTON_TEXT_NEW = "New Level";
    public const string BUTTON_TEXT_NEW_PREFAB = "Add Prefab";
    public const string BUTTON_TEXT_NEW_WALL = "Add new wall prefab";

    public const string BUTTON_TEXT_LOAD_DATABASE = "Load Prefab Collection";

    public const string BUTTON_TEXT_NEW_DATABASE = "Create Prefab Collection";
    public const string LABLE_MAP_SIZE = "Map Size";
    public const string LABLE_MAP_SCALE = "Map Scale";

    public const string LABLE_NO_DATABASE = "You Have to Load a Collection to add objects";
    public const string LABLE_DATABASE_TITLE = "Prefab Collection : {0}";

    public static readonly GUIContent LABLE_ENUM_EDIT_MODE = EditorGUIUtility.TrTextContent("Editing Mode", "Mode of editing");

    public static readonly GUIContent LABLE_AUTOSIZE = EditorGUIUtility.TrTextContent("AutoBounds","If this is marked, the object will have its first render as size of the objet");
    public static readonly GUIContent LABLE_AUTOPIVOT = EditorGUIUtility.TrTextContent("Automatic Pivot", "If this is marked, the object will get the pivot in to the center of the mesh");
    public static readonly GUIContent LABLE_AUTO_SCALE = EditorGUIUtility.TrTextContent("Automatic Scale", "If this is marked, the object will Scale to fit its cellSize");
    public const string BUTTON_TEXT_EDIT_PREFAB = "Save Prefab";
    public const  string PREFAB_FIELD = "Prefab";

    #endregion
    #region Styles
    public static readonly GUILayoutOption maxW = GUILayout.MaxWidth(100);
    public static readonly GUILayoutOption maxH = GUILayout.MaxHeight(50);
    public static readonly GUILayoutOption maxWButton = GUILayout.MaxWidth(100/3);
    public static readonly GUILayoutOption maxHButton = GUILayout.MaxHeight(25);
    public static readonly GUILayoutOption maxWWalls = GUILayout.MaxWidth(100 / 3);
    public static readonly GUILayoutOption maxWCompleteWall = GUILayout.MaxWidth(100);
    public static readonly GUILayoutOption maxHWalls = GUILayout.MaxHeight(25);

    public static readonly GUILayoutOption minWButton = GUILayout.MinWidth(100/3);

    public static readonly GUILayoutOption minHButton = GUILayout.MinWidth(25);

    public static readonly GUIStyle boldCenterText = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };

    public static readonly GUIStyle centerText = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };


    public static float defaultLineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    public static float defaultIndentWidth = 12;
    #endregion
    #region Icons
    public static readonly Texture2D ICON_CLOSE = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "cross.png");

    public static readonly Texture2D ICON_RELOAD = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "return.png");

    public static readonly Texture2D ICON_EDIT = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "gear.png");

    public static readonly Texture2D ICON_ADD_MODE = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "plus.png");

    public static readonly Texture2D ICON_EDIT_MODE             = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "wrench.png");
    public static readonly Texture2D ICON_REMOVE_MODE           = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "minus.png");
    public static readonly Texture2D ICON_ADD_INSTANCING_MODE    = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "plus.png");
    public static readonly Texture2D ICON_NONE_MODE              = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "locked.png");

    public static readonly GUIContent GUI_ICON_ADD_MODE = EditorGUIUtility.TrIconContent(ICON_ADD_MODE, "Add Objects into the scene");
    public static readonly GUIContent GUI_ICON_EDIT_MODE           = EditorGUIUtility.TrIconContent(ICON_EDIT_MODE , "Edit the Cell of the scene");
    public static readonly GUIContent GUI_ICON_REMOVE_MODE         = EditorGUIUtility.TrIconContent(ICON_REMOVE_MODE, "Remove the object into the scene");
    public static readonly GUIContent GUI_ICON_ADD_INSTANCING_MODE = EditorGUIUtility.TrIconContent(ICON_ADD_INSTANCING_MODE, "Add Object into the scene, the object are instances of the prefab");
    public static readonly GUIContent GUI_ICON_NONE_MODE =           EditorGUIUtility.TrIconContent(ICON_NONE_MODE, "Lock the scene, so it can't be edited");

    //TODO
    public static readonly string ICON_ADD = "Añadir";

    public static readonly GUIContent BUTTON_TEXT_SAVE_AND_EXIT = EditorGUIUtility.TrTextContent("Save and Exit");

    public static readonly GUIContent LAYOUT_PREFABS = EditorGUIUtility.TrTextContent("Prefabs");
    public static readonly GUIContent LAYOUT_WALLS = EditorGUIUtility.TrTextContent("Walls");

    public const string LABLE_REGION_FIELD  = "Region";

    public const string BUTTON_TEXT_NEW_REGION_CONTAINER = "Add new Region";

    public const string LABEL_CANT_USE_REGIONS  = "You can not use regions while editing other regions";



    #endregion
}
