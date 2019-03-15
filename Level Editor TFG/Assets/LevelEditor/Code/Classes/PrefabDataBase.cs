using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using static PrefabContainer;

[Serializable]
public class PrefabDataBase : ScriptableObject
{
#if UNITY_EDITOR
    //Pop Up para crear el objeto
    private class PrefabContinerWindowCreator : EditorWindow
    {
        PrefabContainer container;
        PrefabDataBase dataBase;
        EditorWindow Owner;
        Action createObject;
        public static Func<EditorWindow, PrefabDataBase, EditorWindow> create = CreateWindow;

        public static PrefabContinerWindowCreator CreateWindow(EditorWindow owner, PrefabDataBase data)
        {
            PrefabContinerWindowCreator window = CreateInstance<PrefabContinerWindowCreator>();
            window.title = "Create Prefab";
            window.maxSize = new Vector2(300, 100);
            window.minSize = window.maxSize;
            window.container = new PrefabContainer();
           
            window.dataBase = data;
            window.createObject = window.CreateObject;
            window.Owner = owner;
            window.ShowUtility();
            return window;
        }


        private void OnGUI()
        {
            container.showGUIEdit(this);
            GUIAuxiliar.Button(createObject, Style.BUTTON_TEXT_NEW_PREFAB);
        }

        private void CreateObject()
        {
            dataBase.AddPrefab(container);
            Owner.Repaint();
            Close();
        }
    }
#endif

    public void Init()
    {
        prefabList = new List<PrefabContainer>();
    }
    public string dataBaseName;

    public List<PrefabContainer> prefabList;

    //Dado que los Scripts de editor no se pueden referenciar en los scripts que no estan dentro de Editor, al menos no se como, pasaremos como parametro la funcion de recogida del prefab por
    //ShowGUI, asi podremos obtener el prefab cuando se pulse, sin perder la estructura que tenemos,
#if UNITY_EDITOR
    public void ShowGUI(EditorWindow window, Action<PrefabContainer, PrefabAction> getPrefab)
    {
        GUILayout.Label(string.Format(Style.LABLE_DATABASE_TITLE, dataBaseName));


        DoPrefabs(window, getPrefab);
        DoAddButtons(window);
    }

    private void DoPrefabs(EditorWindow window, Action<PrefabContainer, PrefabAction> getPrefab)
    {


        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(window.minSize.x), GUILayout.MinWidth(50));
        int number = 0;
        try
        {
            foreach (var prefab in prefabList)
            {
                number++;
                prefab.ShowGUI(window, getPrefab);
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
        catch (Exception e)
        {

        }
        EditorGUILayout.EndHorizontal();
    }
    private void DoAddButtons(EditorWindow window)
    {

        GUIAuxiliar.Button<EditorWindow>(Style.BUTTON_TEXT_NEW_PREFAB, PrefabContinerWindowCreator.create, window, this);
    }

    public void AddPrefab(PrefabContainer container)
    {
        //TODO vamos a hacer una imagen con el objeto, lo vamos a colocar en el 0,0 coger una camara virtual, renderizar solo ese objeto y pegar la textura.


        container.preview = AssetPreview.GetAssetPreview(container.prefab);

        prefabList.Add(container);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


    }
#endif
}