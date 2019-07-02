//Static class to store all the path that the plugins needs. Also Store the name of the folders used.
using UnityEditor;

public static class Paths
{
    #region Constant
    public const string PATH_LEVEL_EDITOR = "Assets/LevelEditor/";

    public const string PATH_PREFABS = PATH_LEVEL_EDITOR + "Prefabs/";
    public const string PATH_MAPS = PATH_LEVEL_EDITOR + "Prefabs/Regions/";

    public const string PATH_RESOURCES = "Assets/Resources/";

    public const string PATH_LEVEL_EDITOR_ICON = PATH_LEVEL_EDITOR + "UI/Icons/";

    public const string PATH_REGIONS = PATH_RESOURCES + "LevelEditor/Regions/";

    public const string PATH_RESOURCE_LEVELS = PATH_RESOURCES_LEVEL_EDITOR + "Levels/";

    public const string PATH_LEVELS = PATH_LEVEL_EDITOR + "Prefabs/Levels/";


    public const string PATH_DATA_BASE = PATH_RESOURCES_LEVEL_EDITOR + "PrefabDataBases/";

    public const string PATH_RESOURCES_LEVEL_EDITOR = PATH_RESOURCES + "LevelEditor/";
    public const string NAME_REGIONS = "Regionss";

    public const string NAME_MAPS = "Regions";



    public const string NAME_LEVELS = "Levels";


    public const string NAME_LEVEL_EDITOR = "LevelEditor";

    public const string NAME_DATA_BASE = "PrefabDataBases";

    public const string FOLDER_LEVEL_EDITOR = "Assets/LevelEditor";
    public const string FOLDER_RESOURCES = "Assets/Resources";
    public const string FOLDER_LEVELS = PATH_PREFABS + NAME_LEVELS;

    public const string FOLDER_DATA_BASE = PATH_RESOURCES_LEVEL_EDITOR + NAME_DATA_BASE;
    public const string FOLDER_RESOURCES_LEVEL_EDITOR = PATH_RESOURCES + NAME_LEVEL_EDITOR;

    public const string FOLDER_MAPS = PATH_PREFABS + NAME_MAPS;

    public const string FOLDER_RESOURCE_LEVEL = PATH_RESOURCES_LEVEL_EDITOR + NAME_LEVELS;

    public const string FOLDER_PREFABS = PATH_LEVEL_EDITOR + "Prefabs";



    public const string MENU_EDITOR = "LevelEditor/";

    public  const string RESOURCES_PATH_LEVELS = "LevelEditor/Levels/";
    

    public const string MENU_REGION_EDITOR = "LevelEditor/RegionEditorWindow";

    public const string MENU_REGION_SCENE = MENU_EDITOR + "Open Region Editor";


    public const string MENU_LEVEL_SCENE = MENU_EDITOR + "Open Level Editor";
    #endregion

    #region Auxiliar Functions
    #if UNITY_EDITOR
    /// <summary>
    /// Check if a folder exist, if it exist return that folder.
    /// If do not exist, firts check if the parent folder exist, a recursively create the folders.
    /// </summary>
    /// <param name="parentFolder">parent folder</param>
    /// <param name="name">new folder name</param>
    /// <returns>The path to the folder</returns>
    public static string CreateFolderIfNotExist(string parentFolder, string name)
    {

        string folder = parentFolder + "/" + name;
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder(CreateFolderIfNotExist(parentFolder), name);
        }
        return folder;
    }
    /// <summary>
    /// Check if a folder exist, if it exist return that folder.
    /// If do not exist, firts check if the parent folder exist, a recursively create the folders.
    /// </summary>
    /// <param name="folderPath">new folder Path</param>
    /// <returns>The Path to the folder</returns>
    public static string CreateFolderIfNotExist(string folderPath)
    {

        string folder = folderPath;
        if (!AssetDatabase.IsValidFolder(folder))
        {
            string[] split = folderPath.Split('/');
            if (split.Length <= 1)
            {
                folder = split[0];
            }
            else
            {
                string folderName = split[split.Length-1];
                string parentFolder = "";
                for (int i = 0; i<split.Length-2; i++) 
                {
                    parentFolder += split[i] + "/";
                }
                parentFolder += split[split.Length-2];

                AssetDatabase.CreateFolder(CreateFolderIfNotExist(parentFolder), folderName);
            }
        }
        return folderPath;
    }
#endif
    #endregion


}