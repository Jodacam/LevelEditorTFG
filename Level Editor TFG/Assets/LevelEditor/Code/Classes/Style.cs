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
    public const string LABLE_MAP_SIZE = "Map Size";
    public const string LABLE_MAP_SCALE = "Map Scale";

    public static readonly GUIContent LABLE_ENUM_EDIT_MODE = EditorGUIUtility.TrTextContent("Editing Mode", "Mode of editing");
    #endregion
}
