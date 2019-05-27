using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using static LevelEditor.PrefabContainer;
using static LevelEditor.Container;
using LevelEditor.EditorScripts;


namespace LevelEditor
{
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
            public static Func<EditorWindow, PrefabDataBase, Container, EditorWindow> create = CreateWindow;

            public static ContainerWindowCreator CreateWindow(EditorWindow owner, PrefabDataBase data, Container tipe)
            {

                ContainerWindowCreator window = CreateInstance<ContainerWindowCreator>();
                window.title = "Create Prefab";
                window.edit = false;
                window = DoStyle(owner, data, tipe, window);
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
                if (!edit)
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

        public List<RegionContainer> regions;


        //Dado que los Scripts de editor no se pueden referenciar en los scripts que no estan dentro de Editor, al menos no se como, pasaremos como parametro la funcion de recogida del prefab por
        //ShowGUI, asi podremos obtener el prefab cuando se pulse, sin perder la estructura que tenemos,
#if UNITY_EDITOR
        public void ShowGUI(PrefabCollectionWindow window)
        {
            GUILayout.Label(string.Format(Style.LABLE_DATABASE_TITLE, dataBaseName));

            window.showPrefabs = EditorGUILayout.Foldout(window.showPrefabs, Style.LAYOUT_PREFABS);
            if (window.showPrefabs)
                DrawCollection(prefabList, window);

            window.showWalls = EditorGUILayout.Foldout(window.showWalls, Style.LAYOUT_WALLS);
            if (window.showWalls)
                DrawCollection(Walls, window);

            window.showRegions = EditorGUILayout.Foldout(window.showRegions, Style.LABLE_REGION_FIELD);
            if (window.showRegions)
            {
                window.canUseRegions = EditorGUILayout.Toggle(window.canUseRegions);
                if (window.canUseRegions)
                    DrawCollection(regions, window);
                else
                {
                    EditorGUILayout.LabelField(Style.LABEL_CANT_USE_REGIONS, Style.centerText);
                }
            }

            DoAddButtons(window);
        }

        private void DrawCollection<T>(List<T> collection, PrefabCollectionWindow window) where T : Container
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(window.minSize.x), GUILayout.MinWidth(50));
            int number = 0;
            try
            {
                foreach (var prefab in collection)
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
            catch
            {

            }
            EditorGUILayout.EndHorizontal();
        }

        private void DoAddButtons(EditorWindow window)
        {

            GUIAuxiliar.Button<EditorWindow>(Style.BUTTON_TEXT_NEW_PREFAB, ContainerWindowCreator.create, window, this, new PrefabContainer());
            GUIAuxiliar.Button<EditorWindow>(Style.BUTTON_TEXT_NEW_WALL, ContainerWindowCreator.create, window, this, new WallContainer());
            GUIAuxiliar.Button<EditorWindow>(Style.BUTTON_TEXT_NEW_REGION_CONTAINER, ContainerWindowCreator.create, window, this, new RegionContainer());
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
            if (t == typeof(PrefabContainer))
            {
                AddPrefab((PrefabContainer)container);

            }
            else if (t == typeof(WallContainer))
            {
                AddPrefab((WallContainer)container);
            }
            else if (t == typeof(RegionContainer))
            {
                AddPrefab((RegionContainer)container);
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

        public void AddPrefab(RegionContainer container)
        {
            container.preview = AssetPreview.GetAssetPreview(container.prefab);
            regions.Add(container);
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
            foreach (var obj in prefabList)
            {
                obj.Reload(window);
            }
            foreach (var obj in Walls)
            {
                obj.Reload(window);
            }

            foreach (var obj in regions)
            {
                obj.Reload(window);
            }
        }


#endif
    }
}