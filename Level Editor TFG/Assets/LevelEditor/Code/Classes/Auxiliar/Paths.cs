//Static class to store all the path that the plugins needs. Also Store the name of the folders used.
public static class Paths {
    public const string PATH_LEVEL_EDITOR = "Assets/LevelEditor/";

    public const string PATH_PREFABS = PATH_LEVEL_EDITOR + "Prefabs/";
    public const string PATH_MAPS = PATH_LEVEL_EDITOR+ "Prefabs/Regions/";

    public const string PATH_RESOURCES = "Assets/Resources/";

    

    public const string PATH_REGIONS = PATH_RESOURCES + "LevelEditor/Regions/";

    public const string PATH_RESOURCE_LEVELS = PATH_RESOURCES_LEVEL_EDITOR + "Levels/";

    public const string PATH_LEVELS = PATH_LEVEL_EDITOR + "Prefabs/Levels/";
    

    public const string PATH_DATA_BASE = PATH_RESOURCES_LEVEL_EDITOR + "PrefabDataBases/";

    public const string PATH_RESOURCES_LEVEL_EDITOR = PATH_RESOURCES + "LevelEditor/";
    public const string NAME_REGIONS = "Regions";

    public const string NAME_MAPS = "Regions";

    

    public const string NAME_LEVELS = "Levels";
    

    public const string NAME_LEVEL_EDITOR = "LevelEditor";

    public const string NAME_DATA_BASE = "PrefabDataBases";

    public const string FOLDER_LEVEL_EDITOR = "Assets/LevelEditor";
    public const string FOLDER_RESOURCES = "Assets/Resources";
    public const string FOLDER_LEVELS = PATH_PREFABS + NAME_LEVELS;
   
    public const string FOLDER_DATA_BASE = PATH_RESOURCES_LEVEL_EDITOR + NAME_DATA_BASE; 
    public const string FOLDER_RESOURCES_LEVEL_EDITOR = PATH_RESOURCES+ NAME_LEVEL_EDITOR;

    public const string FOLDER_MAPS = PATH_PREFABS + NAME_MAPS;

    public const string FOLDER_RESOURCE_LEVEL = PATH_RESOURCES_LEVEL_EDITOR + NAME_LEVELS;

    public const string FOLDER_PREFABS = PATH_LEVEL_EDITOR + "Prefabs";
    


    public  const string MENU_EDITOR = "LevelEditor/";
    
    public const string MENU_REGION_EDITOR = "LevelEditor/RegionEditorWindow";

    public   const string MENU_REGION_SCENE = MENU_EDITOR + "Open Region Editor";
    

    public  const string MENU_LEVEL_SCENE = MENU_EDITOR + "Open Level Editor";
    
    
    }