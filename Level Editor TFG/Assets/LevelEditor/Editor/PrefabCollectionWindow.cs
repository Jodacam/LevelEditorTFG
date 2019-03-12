using UnityEngine;
using UnityEditor;

public class PrefabCollectionWindow : EditorWindow
{
    #region Static Functions
    [MenuItem("LevelEditor/PrefabCollection")]
    //Creates the main window.
    public static void OpenWindow()
    {
        PrefabCollectionWindow window = (PrefabCollectionWindow)GetWindow(typeof(PrefabCollectionWindow));
        window.name = Style.TITLE_PREFAB_COLLECTION_WINDOW;
        window.Show();
    }
    #endregion


    public enum Mode
    {
        None,
        Add,
        Select
    }

    static Mode actualMode;
    static GameObject selectObject;
    Vector2 scrollPosition = Vector2.zero;
    void OnFocus()
    {
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        
        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;

    }
    



    void OnDestroy()
    {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        // Do your drawing here using Handles.
        Handles.BeginGUI();
        switch (actualMode)
        {
            case Mode.None:
                Debug.Log("None");
                break;
            case Mode.Add:
                OnAdd();
                break;
            case Mode.Select:
                break;
        }
        // Do your drawing here using GUI.
        Handles.EndGUI();
    }




    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
        actualMode = (Mode)EditorGUILayout.EnumPopup(Style.LABLE_ENUM_EDIT_MODE, actualMode);
        





        GUILayout.EndScrollView();
    }


    void OnAdd()
    {
        RaycastHit hit;

        Vector2 guiPosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
        if (Physics.Raycast(ray, out hit))
        {
            var t = hit.transform.GetComponent<GridTerrain>();
            t.GetClampPosition(selectObject,hit);
            
        }

    }


}