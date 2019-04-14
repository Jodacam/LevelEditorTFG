using UnityEditor;
using UnityEngine;

public static class Style
{

    //En esta clase guardo todos los objetos de GUI que hacen falta para crear ventanas y estilos.
    //Además guardo tambien todos los títulos
    #region Labels
    

    public static readonly GUIContent TITLE_LEVEL_EDITOR_WINDOW = EditorGUIUtility.TrTextContentWithIcon("Level Editor Window",MessageType.Info);
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

    public static readonly GUIContent LABLE_AUTOSIZE = EditorGUIUtility.TrTextContent("Autosize","If this is marked, the object will have its first render as size of the objet");
    public static readonly GUIContent LABLE_AUTOPIVOT = EditorGUIUtility.TrTextContent("Automatic Pivot", "If this is marked, the object will get the pivot in to the center of the mesh");
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
    #endregion
    #region Icons
    public static readonly Texture2D ICON_CLOSE = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "cross.png");

    public static readonly Texture2D ICON_RELOAD = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "return.png");


    public static readonly Texture2D ICON_EDIT = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "gear.png");

    //TODO
    public static readonly string ICON_ADD = "Añadir";
    
    #endregion
}
