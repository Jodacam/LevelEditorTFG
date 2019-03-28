using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using static PrefabContainer;
using static Container;

namespace Editor
{
    //Ventana para la colleción de prfab y poder añadir objetos o selccionarlos.
    public class PrefabCollectionWindow : EditorWindow
    {
        #region Static Functions
        [MenuItem("LevelEditor/PrefabCollection")]
        //Creates the main window.
        public static void OpenWindow()
        {
            PrefabCollectionWindow window = (PrefabCollectionWindow)GetWindow(typeof(PrefabCollectionWindow));
            window.getPrefab = window.GetPrefab;
            window.title = Style.TITLE_PREFAB_COLLECTION_WINDOW;
            window.minSize = new Vector2(350, 250);
            window.maxSize = new Vector2(350, 1000);
            window.selectObject = new SceneObjectContainer();
            window.Show();
        }

        #endregion


        public enum Mode
        {
            None,
            Add,

            Remove,
            Edit
        }


        private class PrefabCollectionCreatorWindow : EditorWindow
        {
            protected PrefabCollectionWindow owner;
            public static PrefabCollectionCreatorWindow CreateWindow(PrefabCollectionWindow owner)
            {
                var window = PrefabCollectionCreatorWindow.CreateInstance<PrefabCollectionCreatorWindow>();
                window.title = "Collection Creator";
                window.ShowUtility();
                window.dataBase = CreateInstance<PrefabDataBase>();
                window.dataBase.Init();
                window.dataBase.dataBaseName = "New";
                window.dataBase.prefabList = new List<PrefabContainer>();
                window.maxSize = new Vector2(300, 100);
                window.minSize = new Vector2(300, 100);
                window.owner = owner;
                return window;
            }

            PrefabDataBase dataBase;

            private void OnGUI()
            {
                GUILayout.Label(title);
                dataBase.dataBaseName = EditorGUILayout.TextField("Name", dataBase.dataBaseName);
                if (GUILayout.Button("Create"))
                {
                    AssetDatabase.CreateAsset(dataBase, "Assets/Resources/PrefabData/" + dataBase.dataBaseName + ".asset");
                    owner.OnCreateDataBase(dataBase);
                    this.Close();
                }
            }


        }


        static Mode actualMode;
        public SceneObjectContainer selectObject;

        bool isPicking = false;
        static PrefabDataBase dataBase;
        Vector2 scrollPosition = Vector2.zero;
        Action<Container, PrefabAction> getPrefab;
        RaycastHit hit;
        static float rotation;


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

                    break;
                case Mode.Add:
                    OnAdd();
                    break;
                case Mode.Edit:
                    OnSelect();
                    break;
                case Mode.Remove:
                    OnRemove();
                    break;
            }
            // Do your drawing here using GUI.
            Handles.EndGUI();
        }

        private void OnSelect()
        {
            Event e = Event.current;
            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Grid")))
                {
                    var terrain = hit.transform.GetComponent<GridTerrain>();
                    var selectedCell = terrain.GetCell(hit.triangleIndex);
                    selectedCell.Edit(this);

                }
            }
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
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
                    OnCreateDataBase(pickedObject);
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
            if (dataBase != null)
            {
                dataBase.ShowGUI(this, getPrefab);
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

        void OnCreateDataBase(PrefabDataBase data)
        {

            dataBase = data;
            Repaint();
        }
        void OnAdd()
        {
            Event e = Event.current;


            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            if (selectObject.HasObject)
            {
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Grid")))
                {
                    var t = hit.transform.GetComponent<GridTerrain>();
                    Vector3 c = t.GetClampPosition(hit);
                    selectObject.preview.transform.position = c;
                    if (e.button == 0 && e.type == EventType.MouseDown)
                    {
                        t.SetObjetIntoCell(selectObject, hit.triangleIndex);
                    }
                }

                if (e.control && e.type == EventType.KeyDown)
                {
                    selectObject.preview.transform.Rotate(0, 90, 0, Space.World);
                }

            }

            else
            {
                Debug.Log("No hay objeto seleccionado");
            }

        }
        private void OnRemove()
        {
            Event e = Event.current;



            Vector2 guiPosition = Event.current.mousePosition;

            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);

                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Grid")))
                {
                    GridTerrain ter = hit.transform.GetComponent<GridTerrain>();
                    //ter.SetIntoCell(null,hit.triangleIndex);
                    ter.Remove(hit.triangleIndex);
                }

            }
        }
        public void AddPrefabToDataBase(PrefabContainer container)
        {
            dataBase.AddPrefab(container);
        }


        private void GetPrefab(Container cont, PrefabAction action)
        {


            Type t = cont.GetType();

            if (t == typeof(PrefabContainer))
            {
                PrefabContainer container = (PrefabContainer)cont;
                switch (action)
                {
                    case PrefabAction.Select:
                        SelectPrefab(container);
                        break;
                    case PrefabAction.Delete:
                        DeletePrefab(container);
                        break;
                    case PrefabAction.Reload:
                        Reload(container);
                        break;
                }

            }
            else if (t == typeof(WallContainer))
            {

            }


        }

        private void SelectPrefab(PrefabContainer container)
        {
            selectObject.SetObjectInfo(container);
        }

        private void DeletePrefab(PrefabContainer container)
        {
            dataBase.prefabList.Remove(container);
            if (selectObject.realObject.GetInstanceID() == container.prefab.GetInstanceID())
            {
                selectObject.SetToNull();
            }

            EditorUtility.SetDirty(dataBase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Reload(PrefabContainer container)
        {
            container.preview = AssetPreview.GetAssetPreview(container.prefab);
            EditorUtility.SetDirty(dataBase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}