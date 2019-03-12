using UnityEngine;
using UnityEditor;
using System;

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


    private class PrefabCollectionCreatorWindow : EditorWindow
    {
        public static PrefabCollectionCreatorWindow CreateWindow(PrefabCollectionWindow owner)
        {
            var window = PrefabCollectionCreatorWindow.CreateInstance<PrefabCollectionCreatorWindow>();
            window.title = "Hello there";
            window.ShowUtility();
            window.dataBase = CreateInstance<PrefabDataBase>();
            window.dataBase.name = "New";
            return window;
        }

        PrefabDataBase dataBase;

        private void OnGUI()
        {        
            GUILayout.Label(title);
            dataBase.name = EditorGUILayout.TextField("Name",dataBase.name);
            if(GUILayout.Button("Save"))
            {          
                this.Close();
            }
        }

        
    }


    static Mode actualMode;
    static GameObject selectObject;

    bool isPicking = false;
    static PrefabDataBase dataBase;
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
        DoOptions();
        DoPrefabSelector();
        GUILayout.EndScrollView();
        DoPicker();
    }

    private void DoPicker()
    {
        string commandName = Event.current.commandName;
        if (commandName == "ObjectSelectorClosed")
        {
            OnPick();
        }

    }

     private void OnPick()
    {
        if (isPicking)
        {
            PrefabDataBase pickedObject = (PrefabDataBase)EditorGUIUtility.GetObjectPickerObject();
            if (pickedObject != null)
            {

                isPicking = false;
                Repaint();
            }
        }
    }

    private void DoPrefabSelector()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(Style.BUTTON_TEXT_LOAD_DATABASE))
        {
            Load();
        }

        if (GUILayout.Button(Style.BUTTON_TEXT_NEW_DATABASE))
        {
            CreateNew();
        }
        EditorGUILayout.EndHorizontal();
        if(dataBase != null)
        {

        }
        else
        {
            GUILayout.Label(Style.LABLE_NO_DATABASE);
        }

    }

    private void CreateNew()
    {
        var window = PrefabCollectionCreatorWindow.CreateWindow(this);
    }

    private void Load()
    {
        int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
        isPicking = true;
        EditorGUIUtility.ShowObjectPicker<PrefabDataBase>(null, false, "", controlID);
    }

    private void DoOptions()
    {

    }

    void OnCreateDatBase(PrefabDataBase data)
    {
        dataBase = data;
    }
    void OnAdd()
    {
        RaycastHit hit;

        Vector2 guiPosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
        if (Physics.Raycast(ray, out hit))
        {
            var t = hit.transform.GetComponent<GridTerrain>();
            t.GetClampPosition(selectObject, hit);

        }

    }


}