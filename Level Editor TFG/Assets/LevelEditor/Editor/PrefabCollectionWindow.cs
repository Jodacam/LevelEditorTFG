using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using static PrefabContainer;

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
            Select
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
        Action<PrefabContainer,PrefabAction> getPrefab;
       

        

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
                case Mode.Select:
                    break;
            }
            // Do your drawing here using GUI.
            Handles.EndGUI();
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
                dataBase.ShowGUI(this,getPrefab);
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
            RaycastHit hit;

            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            if (selectObject.HasObject)
            {
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Grid")))
                {
                    var t = hit.transform.GetComponent<GridTerrain>();
                    Vector3 position = t.GetClampPosition(selectObject, hit);

                }
            }
            else
            {
                Debug.Log("No hay objeto seleccionado");
            }

        }

        public void AddPrefabToDataBase(PrefabContainer container)
        {
            dataBase.AddPrefab(container);
        }


        private void GetPrefab(PrefabContainer container,PrefabAction action)
        {
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

        private void SelectPrefab(PrefabContainer container)
        {
            selectObject.SetObjectInfo(container);
        }

        private void DeletePrefab(PrefabContainer container)
        {
            dataBase.prefabList.Remove(container);
            if(selectObject.realObject.GetInstanceID() == container.prefab.GetInstanceID())
            {
                selectObject.SetToNull();
            }
        }

        private void Reload(PrefabContainer container)
        {
            container.preview = AssetPreview.GetAssetPreview(container.prefab);
        }

    }
}