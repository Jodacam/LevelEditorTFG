using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using static PrefabContainer;
using static Container;

[Serializable]
public class PrefabDataBase : ScriptableObject
{
#if UNITY_EDITOR
    //Pop Up to create the DataBase.
    private class ContainerWindowCreator : EditorWindow 
    {
        Container container;
        PrefabDataBase dataBase;
        EditorWindow Owner;
        bool edit;
        Action createObject;
        public static Func<EditorWindow, PrefabDataBase,Container,EditorWindow> create = CreateWindow;

        public static ContainerWindowCreator CreateWindow(EditorWindow owner, PrefabDataBase data,Container tipe)
        {
            
            ContainerWindowCreator window = CreateInstance<ContainerWindowCreator>();
            window.title = "Create Prefab";
            window.edit = false;
            window = DoStyle(owner,data,tipe,window);
            return window;
        }

      

        public static ContainerWindowCreator CreateEditWindow(EditorWindow owner, PrefabDataBase data, Container tipe)
        {

            ContainerWindowCreator window = CreateInstance<ContainerWindowCreator>();
            window.title = "Edit Prefab";
            window.edit = true;
            window = DoStyle(owner, data, tipe, window);
            return window;
        }

          private static ContainerWindowCreator DoStyle(EditorWindow owner, PrefabDataBase data, Container tipe, ContainerWindowCreator window)
        {
            window.maxSize = new Vector2(300, 300);
            window.minSize = window.maxSize;
            window.container = tipe;
            window.dataBase = data;
            window.createObject = window.CreateObject;
            window.Owner = owner;
            window.ShowUtility();
            return window;
        }

        private void OnGUI()
        {
            container.ShowGUIEdit(this);
            GUIAuxiliar.Button(createObject, !edit ? Style.BUTTON_TEXT_NEW_PREFAB : Style.BUTTON_TEXT_EDIT_PREFAB);
        }

        private void CreateObject()
        {
            if(!edit)
                dataBase.AddPrefab(container);
            container.Reload(Owner);
            Owner.Repaint();
            Close();
        }
    }

#endif

    //The ScriptableObjects in unity can not use a constructor so a Init Function is needed.
    public void Init()
    {
        prefabList = new List<PrefabContainer>();
        Walls = new List<WallContainer>();
    }
    public string dataBaseName;

    public List<PrefabContainer> prefabList;
    public List<WallContainer> Walls;
    

    //Dado que los Scripts de editor no se pueden referenciar en los scripts que no estan dentro de Editor, al menos no se como, pasaremos como parametro la funcion de recogida del prefab por
    //ShowGUI, asi podremos obtener el prefab cuando se pulse, sin perder la estructura que tenemos,
#if UNITY_EDITOR
    public void ShowGUI(Editor.PrefabCollectionWindow window)
    {
        GUILayout.Label(string.Format(Style.LABLE_DATABASE_TITLE, dataBaseName));


        DoPrefabs(window);
        DoWalls(window);
        DoAddButtons(window);
    }

    private void DoWalls(Editor.PrefabCollectionWindow window)
    {
        GUILayout.Label("Walls Prefabs");
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(window.minSize.x), GUILayout.MinWidth(50));
        int number = 0;
        try
        {
            foreach (var prefab in Walls)
            {
                number++;
                prefab.ShowGUI(window);
                if (number > 3)
                {
                    EditorGUILayout.EndHorizontal();
                    number = 0;
                    EditorGUILayout.BeginVertical();
                    Rect rect = EditorGUILayout.GetControlRect(false, 1);

                    rect.height = 1;

                    EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(window.minSize.x), GUILayout.MinWidth(50));
                }
            }
        }
        catch 
        {
            
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DoPrefabs(Editor.PrefabCollectionWindow window)
    {


        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(window.minSize.x), GUILayout.MinWidth(50));
        int number = 0;
        try
        {
            foreach (var prefab in prefabList)
            {
                number++;
                prefab.ShowGUI(window);
                if (number > 2)
                {
                    EditorGUILayout.EndHorizontal();
                    number = 0;
                    EditorGUILayout.BeginVertical();
                    Rect rect = EditorGUILayout.GetControlRect(false, 1);

                    rect.height = 1;

                    EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(window.minSize.x), GUILayout.MinWidth(50));
                }
            }
        }
        catch (Exception e)
        {

        }
        EditorGUILayout.EndHorizontal();
    }
    private void DoAddButtons(EditorWindow window)
    {

        GUIAuxiliar.Button<EditorWindow>(Style.BUTTON_TEXT_NEW_PREFAB, ContainerWindowCreator.create, window, this,new PrefabContainer());
        GUIAuxiliar.Button<EditorWindow>(Style.BUTTON_TEXT_NEW_WALL, ContainerWindowCreator.create, window, this,new WallContainer());
    }

    public void AddPrefab(PrefabContainer container)
    {
  
        //TODO vamos a hacer una imagen con el objeto, lo vamos a colocar en el 0,0 coger una camara virtual, renderizar solo ese objeto y pegar la textura.
        container.preview = AssetPreview.GetAssetPreview(container.prefab);
        while (AssetPreview.IsLoadingAssetPreview(container.prefab.GetInstanceID()))
        {
            container.preview = AssetPreview.GetAssetPreview(container.prefab);
        }
        prefabList.Add(container);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public void AddPrefab(Container container)
    {
        Type t = container.GetType();
        if(t == typeof(PrefabContainer))
        {
            AddPrefab((PrefabContainer)container);

        }else if(t == typeof(WallContainer))
        {
            AddPrefab((WallContainer)container);
        }
    }
    public void AddPrefab(WallContainer container)
    {
        //TODO vamos a hacer una imagen con el objeto, lo vamos a colocar en el 0,0 coger una camara virtual, renderizar solo ese objeto y pegar la textura.
        container.preview = AssetPreview.GetAssetPreview(container.prefab);
        Walls.Add(container);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void ShowEditWindow(EditorWindow editor, Container container)
    {

        ContainerWindowCreator.CreateEditWindow(editor, this, container);
    }

    public void Reload(EditorWindow window)
    {
        foreach(var obj in prefabList){
            obj.Reload(window);
        }
    }


#endif
}