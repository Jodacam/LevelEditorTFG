using UnityEditor;
using UnityEngine;

public static class Style
{

    //En esta clase guardo todos los objetos de GUI que hacen falta para crear la ventana.
    //Además guardo tambien todos los títulos
    #region Labels
    public const string TITLE_LEVEL_EDITOR_WINDOW = "Level Editor Window";
    public const string TITLE_PREFAB_COLLECTION_WINDOW = "Prefab Collection Window";
    public const string BUTTON_TEXT_SAVE = "Save Level";

    public const string BUTTON_TEXT_LOAD = "Load Level";

    public const string BUTTON_TEXT_NEW = "New Level";
    public const string BUTTON_TEXT_NEW_PREFAB = "Add Prefab";

    public const string BUTTON_TEXT_LOAD_DATABASE = "Load Prefab Collection";

    public const string BUTTON_TEXT_NEW_DATABASE = "Create Prefab Collection";
    public const string LABLE_MAP_SIZE = "Map Size";
    public const string LABLE_MAP_SCALE = "Map Scale";

    public const string LABLE_NO_DATABASE = "You Have to Load a Collection to add objects";
    public const string LABLE_DATABASE_TITLE = "Prefab Collection : {0}";

    public static readonly GUIContent LABLE_ENUM_EDIT_MODE = EditorGUIUtility.TrTextContent("Editing Mode", "Mode of editing");

    public static readonly GUIContent LABLE_AUTOSIZE = EditorGUIUtility.TrTextContent("Autosize","If this is marked, the object will have its first render as size of the objet");
    public const  string PREFAB_FIELD = "Prefab";
    #endregion
}
