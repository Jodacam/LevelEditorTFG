using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class LevelEditorWindow : EditorWindow
{

    #region Static Functions
    [MenuItem("LevelEditor/EditorWindow")]
    //Creates the main window.
    public static void OpenWindow(){
            LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
            window.name = Style.TITLE_LEVEL_EDITOR_WINDOW;
            window.Show();
    }   
    #endregion

    #region Variables
    Vector2 scrollPosition = Vector2.zero;
    static Level Editlevel;


    SerializedObject levelSerialized;


    #endregion



    #region GUIFunctions
    private void OnGUI() {
        
        EditorGUI.BeginChangeCheck();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);   
        levelSerialized.UpdateIfRequiredOrScript();
        EditorGUILayout.PropertyField(levelSerialized.FindProperty("name"));
        levelSerialized.ApplyModifiedProperties();
        GUILayout.EndScrollView();
        if (GUILayout.Button("Save Level"))
        {
            Save();
        }

        if(GUILayout.Button("Load Level"))
        {
            //Load();
        }

    }
    private void Awake() {

        if(Editlevel == null){
            Editlevel = (Level) CreateInstance(typeof(Level));
        }
        levelSerialized = new SerializedObject(Editlevel);
    }
    
    #endregion

    #region EditorFuntions

    void Save(){
        Level n = Editlevel;
        AssetDatabase.CreateAsset(n,"Assets/Resources/"+Editlevel.name+".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = n;
    }
    #endregion

}
