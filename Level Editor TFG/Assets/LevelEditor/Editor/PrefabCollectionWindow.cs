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

            window.titleContent = Style.TITLE_PREFAB_COLLECTION_WINDOW;
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
            AddInstancing,
            Remove,
            Edit
        }
        bool active;

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

        #region Variables
        static Sprite Cancel;
        static Mode actualMode;
        public SceneObjectContainer selectObject;

        bool isPicking = false;
        static PrefabDataBase dataBase;
        Vector2 scrollPosition = Vector2.zero;
        Action<Container, PrefabAction> getPrefab;
        RaycastHit hit;
        static Vector3 offset;
        static float rotation;
        static bool allowOffset = false;
        static bool usingWalls = false;
        static int wallPos = 0;
        #endregion

        #region Unity events
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



        #endregion
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
                case Mode.AddInstancing:
                    OnAdd(true);
                    break;
                case Mode.Edit:
                    OnSelect();
                    break;
                case Mode.Remove:
                    OnRemove();
                    break;
            }
            // Do your drawing here using GUI.
            DoCommands();
            Handles.EndGUI();
        }
      

        #region GUIFunctions
        private void OnSelect()
        {
            Event e = Event.current;
            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity,LayerMask.GetMask("Grid")))
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
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);


            EditorGUILayout.LabelField(Style.LABLE_ENUM_EDIT_MODE,Style.boldCenterText);
            EditorGUILayout.BeginHorizontal();
            DoModeButton(Mode.Add, Style.GUI_ICON_ADD_MODE);
            DoModeButton(Mode.AddInstancing, Style.GUI_ICON_ADD_INSTANCING_MODE);
            DoModeButton(Mode.Edit, Style.GUI_ICON_EDIT_MODE);
            DoModeButton(Mode.Remove, Style.GUI_ICON_REMOVE_MODE);
            DoModeButton(Mode.None, Style.GUI_ICON_NONE_MODE);
            EditorGUILayout.EndHorizontal();
            DoOptions();
            DoPrefabSelector();
            GUILayout.EndScrollView();
            DoPicker();
            DoCommands();
        }

        //Make a toggle button to change the mode of the editor.
        public void DoModeButton(Mode toMode, Texture icon)
        {   
            actualMode = GUI.Toggle(EditorGUILayout.GetControlRect(Style.maxHButton, Style.maxWButton), toMode == actualMode, icon, EditorStyles.miniButton) ? toMode:actualMode;
        }


        public void DoModeButton(Mode toMode, GUIContent icon)
        {
            actualMode = GUI.Toggle(EditorGUILayout.GetControlRect(Style.maxHButton, Style.maxWButton), toMode == actualMode, icon, EditorStyles.miniButton) ? toMode : actualMode;
        }
        private void DoCommands()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                var key = e.keyCode;
                switch (key)
                {
                    case KeyCode.LeftAlt:
                        actualMode = Mode.Add;
                        break;
                    case KeyCode.RightAlt:
                        actualMode = Mode.AddInstancing;
                        break;
                    case KeyCode.E:
                        actualMode = Mode.Edit;
                        break;
                    case KeyCode.R:
                        actualMode = Mode.Remove;
                        break;
                    case KeyCode.N:
                        actualMode = Mode.None;
                        break;
                }
                Repaint();
                
            }

            
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
#endregion

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
            allowOffset = EditorGUILayout.Toggle("Use Offset", allowOffset);
            if (allowOffset)
                offset = EditorGUILayout.Vector3Field("Offset", offset);
        }

        void OnCreateDataBase(PrefabDataBase data)
        {

            dataBase = data;
            dataBase.Reload(this);
            Repaint();
        }
        void OnAdd(bool instancing = false)
        {
            Event e = Event.current;

            Vector3 off = allowOffset ? offset : Vector3.zero;
            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            if (selectObject.HasObject)
            {
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity,LayerMask.GetMask("Grid")))
                {

                    if (!usingWalls)
                    {
                        AddingObject(e, off,instancing);
                    }
                    else
                    {
                        AddingWall(e, off,instancing);
                    }
                }
            }

        }



        private void AddingWall(Event e, Vector3 off,bool instancing)
        {
            var t = hit.transform.GetComponent<GridTerrain>();
            Vector3 position = t.GetWallClampPosition(hit, wallPos);

            selectObject.preview.transform.position = position - selectObject.Pivot + off;
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                t.SetWallIntoCell(selectObject, hit.triangleIndex, wallPos,off,instancing);
            }
            if (e.control && e.type == EventType.KeyDown)
            {
                selectObject.preview.transform.RotateAround(selectObject.WorldPivot, new Vector3(0, 1, 0), 90);
                wallPos = (wallPos + 1) % 4;
                selectObject.RecalculatePivot(wallPos);
                
            }
        }

        private void AddingObject(Event e,Vector3 off, bool instancing)
        {
            var t = hit.transform.GetComponent<GridTerrain>();
            Vector3 c = t.GetClampPosition(hit,selectObject.CellSize);

            selectObject.preview.transform.position = c - selectObject.Pivot + off;
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                Undo.RegisterFullObjectHierarchyUndo(t,"AddObject");
                
                t.SetObjetIntoCell(selectObject, hit.triangleIndex, off,instancing);
            }


            if (e.control && e.type == EventType.KeyDown)
            {
                selectObject.preview.transform.RotateAround(selectObject.WorldPivot, new Vector3(0, 1, 0), 90);
                wallPos = (wallPos + 1) % 4;
                selectObject.RecalculatePivot(wallPos);
                
            }
        }

        private void OnRemove()
        {
            Event e = Event.current;



            Vector2 guiPosition = Event.current.mousePosition;

            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);

                if (Physics.Raycast(ray, out hit, float.PositiveInfinity,LayerMask.GetMask("Grid")))
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

        #region Prefabs Functions
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
                    case PrefabAction.Edit:
                        Edit(container);
                        break;
                }

            }
            else if (t == typeof(WallContainer))
            {
                WallContainer container = (WallContainer)cont;
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
                    case PrefabAction.Edit:
                        Edit(container);
                        break;
                }
            }


        }

        private void Edit(PrefabContainer container)
        {
            dataBase.ShowEditWindow(this, container);
        }

        private void SelectPrefab(PrefabContainer container)
        {
            selectObject.SetObjectInfo(container);
            usingWalls = false;
            wallPos = 0;
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




        private void Edit(WallContainer container)
        {
            dataBase.ShowEditWindow(this, container);
        }

        private void SelectPrefab(WallContainer container)
        {
            selectObject.SetObjectInfo(container);
            usingWalls = true;
            wallPos = 0;
        }

        private void DeletePrefab(WallContainer container)
        {
            dataBase.Walls.Remove(container);
            if (selectObject.realObject.GetInstanceID() == container.prefab.GetInstanceID())
            {
                selectObject.SetToNull();
            }

            EditorUtility.SetDirty(dataBase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Reload(WallContainer container)
        {
            container.Reload(this);
            EditorUtility.SetDirty(dataBase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion
    }
}