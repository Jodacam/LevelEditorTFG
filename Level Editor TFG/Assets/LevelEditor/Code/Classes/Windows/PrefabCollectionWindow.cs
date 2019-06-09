#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using static LevelEditor.PrefabContainer;
using static LevelEditor.Container;
using UnityEditor.SceneManagement;
using LevelEditor;
namespace LevelEditor.EditorScripts
{
    //Ventana para la colleción de prfab y poder añadir objetos o selccionarlos.
    public class PrefabCollectionWindow : EditorWindow
    {
        #region Static Functions
        [MenuItem("LevelEditor/Prefab Collection Window", false, 1)]
        //Creates the main window.
        public static void OpenWindow()
        {
            PrefabCollectionWindow window = GUIAuxiliar.OpenEditorWindow<PrefabCollectionWindow>(Style.TITLE_PREFAB_COLLECTION_WINDOW);
            window.minSize = new Vector2(350, 250);
            window.maxSize = new Vector2(350, 1000);
            PrefabCollectionWindow.selectObject = new SceneObjectContainer();
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


                var window = GUIAuxiliar.OpenEditorWindow<PrefabCollectionCreatorWindow>("Collection Creator");

                Paths.CreateFolderIfNotExist(Paths.FOLDER_RESOURCES_LEVEL_EDITOR, Paths.NAME_DATA_BASE);

                /* if (!AssetDatabase.IsValidFolder(Paths.FOLDER_DATA_BASE))
                {
                    AssetDatabase.CreateFolder(Paths.FOLDER_RESOURCES_LEVEL_EDITOR, Paths.NAME_DATA_BASE);
                }
                */
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
                GUILayout.Label(titleContent);
                dataBase.dataBaseName = EditorGUILayout.TextField("Name", dataBase.dataBaseName);
                if (GUILayout.Button("Create"))
                {
                    if (!AssetDatabase.IsValidFolder(Paths.FOLDER_DATA_BASE))
                    {
                        AssetDatabase.CreateFolder(Paths.PATH_RESOURCES_LEVEL_EDITOR, Paths.NAME_DATA_BASE);
                    }
                    AssetDatabase.CreateAsset(dataBase, Paths.PATH_DATA_BASE + dataBase.dataBaseName + ".asset");
                    owner.OnCreateDataBase(dataBase);
                    Close();
                }
            }


        }

        #region Variables
        static Sprite Cancel;
        static Mode actualMode;
        public static SceneObjectContainer selectObject;

        bool isPicking = false;
        static PrefabDataBase dataBase;
        Vector2 scrollPosition = Vector2.zero;

        RaycastHit hit;
        static Vector3 offset;
        static float rotation;
        static bool allowOffset = false;
        static bool usingWalls = false;

        public bool showPrefabs = true;

        public bool showWalls = true;

        public bool showRegions = true;

        public bool canUseRegions = true;
        static int rotationSide = 0;
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

        private void Awake()
        {
            selectObject = new SceneObjectContainer();
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
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Grid")))
                {
                    var terrain = hit.transform.GetComponent<RegionTerrain>();
                    var selectedCell = terrain.GetCell(hit.point);
                    selectedCell.Edit(this);

                }
            }
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);


            EditorGUILayout.LabelField(Style.LABLE_ENUM_EDIT_MODE, Style.boldCenterText);


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
            var rect = EditorGUILayout.GetControlRect(Style.maxHButton, Style.maxWButton);
            actualMode = GUI.Toggle(rect, toMode == actualMode, icon, EditorStyles.miniButton) ? toMode : actualMode;
        }


        public void DoModeButton(Mode toMode, GUIContent icon)
        {
            var rect = EditorGUILayout.GetControlRect(Style.maxHButton, Style.maxWButton, Style.minHButton);
            rect.x += (position.width - (5 * 100 / 3)) / 2.4f;
            actualMode = GUI.Toggle(rect, toMode == actualMode, icon, EditorStyles.miniButton) ? toMode : actualMode;
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
                    case KeyCode.LeftControl:
                        if (actualMode == Mode.Add || actualMode == Mode.AddInstancing)
                        {
                            selectObject.preview.transform.RotateAround(selectObject.WorldPivot, new Vector3(0, 1, 0), 90);
                            rotationSide = (rotationSide + 1) % 4;
                            selectObject.RecalculatePivot(rotationSide);
                        }
                        break;
                }
                Repaint();

            }


        }

        internal void ChangeToNone()
        {
            actualMode = Mode.None;
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

                dataBase.ShowGUI(this);
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
            if (selectObject == null)
            {
                selectObject = new SceneObjectContainer();
            }
            if (selectObject.HasObject)
            {
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Grid", "LevelTerrain")))
                {
                    if (EditorSceneManager.GetActiveScene().name == "Level Editor")
                    {
                        AddIntoLevel(e, offset, instancing, ray);
                    }
                    else
                    {

                        if (hit.transform.GetComponent<RegionTerrain>())
                        {
                            if (!usingWalls)
                            {
                                AddingObject(e, off, instancing);
                            }
                            else
                            {
                                AddingWall(e, off, instancing);
                            }
                        }

                    }
                }
            }

        }


        private void AddIntoLevel(Event e, Vector3 off, bool instancing, Ray ray)
        {
            var terrain = hit.transform.GetComponent<LevelScript>();
            if (terrain)
            {
                if (!usingWalls)
                {

                    Vector3 position = terrain.GetClampPositon(hit.point, ray, selectObject.CellSize);
                    Vector2 cellScale = terrain.owner.cellSize;
                    selectObject.preview.transform.position = position - selectObject.Pivot + off;
                    selectObject.SetAutoScale(cellScale);
                    if (e.button == 0 && e.type == EventType.MouseDown)
                    {
                        Undo.RegisterFullObjectHierarchyUndo(terrain, "Add Object");
                        terrain.SetObject(selectObject, position - selectObject.Pivot + off, instancing);
                    }

                }
                else
                {
                    Vector3 position = terrain.GetWallClampPosition(hit.point, ray, rotationSide);
                    selectObject.preview.transform.position = position - selectObject.Pivot + off;
                    if (e.button == 0 && e.type == EventType.MouseDown)
                    {
                        Undo.RegisterFullObjectHierarchyUndo(terrain, "Add Wall");
                        terrain.SetObject(selectObject, position - selectObject.Pivot + off, instancing);
                    }
                }


            }
        }
        private void AddingWall(Event e, Vector3 off, bool instancing)
        {
            var t = hit.transform.GetComponent<RegionTerrain>();

            Vector3 position = t.GetWallClampPosition(hit, rotationSide);

            selectObject.preview.transform.position = position - selectObject.Pivot + off;
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                Undo.RegisterFullObjectHierarchyUndo(t, "Add Wall");
                t.SetWallIntoCell(selectObject, hit.point, rotationSide, off, instancing);
            }
        }

        private void AddingObject(Event e, Vector3 off, bool instancing)
        {
            var t = hit.transform.GetComponent<RegionTerrain>();
            Vector3 c = t.GetClampPosition(hit, selectObject.CellSize);

            selectObject.preview.transform.position = c - selectObject.Pivot + off;
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                Undo.RegisterFullObjectHierarchyUndo(t, "AddObject");

                t.SetObjetIntoCell(selectObject, hit.point, off, instancing);
            }
        }

        private void OnRemove()
        {
            Event e = Event.current;



            Vector2 guiPosition = Event.current.mousePosition;

            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);

                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Grid")))
                {
                    RegionTerrain ter = hit.transform.GetComponent<RegionTerrain>();
                    //ter.SetIntoCell(null,hit.triangleIndex);
                    ter.Remove(hit.point);
                }
                else
                {
                    var levels = FindObjectsOfType<LevelScript>();
                    bool find = false;
                    for (int i = 0; i < levels.Length && !find; i++)
                    {
                        find = levels[i].RemoveAtRay(ray);
                    }
                }

            }

        }
        public void AddPrefabToDataBase(PrefabContainer container)
        {
            dataBase.AddPrefab(container);
        }

        #region Prefabs Functions

        // Calls to the edit window of the prefab Container.
        public void Edit(PrefabContainer container)
        {
            dataBase.ShowEditWindow(this, container);
        }

        public void SelectPrefab(PrefabContainer container)
        {
            string sceneName = EditorSceneManager.GetActiveScene().name;
            if (sceneName.Equals("Level Editor") || sceneName.Equals("Region Editor"))
            {
                if (selectObject == null)
                    selectObject = new SceneObjectContainer();
                selectObject.SetObjectInfo(container);
                usingWalls = false;
                rotationSide = 0;
            }
            else
            {
                Debug.Log("You can only edit level in at the level editor scenes");
            }
        }

        public void DeletePrefab(PrefabContainer container)
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

        public void Reload(PrefabContainer container)
        {
            container.preview = AssetPreview.GetAssetPreview(container.prefab);
            EditorUtility.SetDirty(dataBase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }




        public void Edit(WallContainer container)
        {
            dataBase.ShowEditWindow(this, container);
        }

        public void SelectPrefab(WallContainer container)
        {
            selectObject.SetObjectInfo(container);

            usingWalls = true;
            rotationSide = 0;
        }

        public void DeletePrefab(WallContainer container)
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

        public void Reload(WallContainer container)
        {
            container.Reload(this);
            EditorUtility.SetDirty(dataBase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }



        public void SelectPrefab(RegionContainer regionContainer)
        {
            regionContainer.prefab.GetComponent<MeshCollider>().enabled = false;
            selectObject.SetObjectInfo(regionContainer);
            regionContainer.prefab.GetComponent<MeshCollider>().enabled = true;
            usingWalls = false;
            rotationSide = 0;
        }

        public void Reload(RegionContainer regionContainer)
        {
            regionContainer.Reload(this);
        }

        public void DeletePrefab(RegionContainer regionContainer)
        {
            dataBase.regions.Remove(regionContainer);
            if (selectObject.realObject.GetInstanceID() == regionContainer.prefab.GetInstanceID())
            {
                selectObject.SetToNull();
            }

            EditorUtility.SetDirty(dataBase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void Edit(RegionContainer regionContainer)
        {
            dataBase.ShowEditWindow(this, regionContainer);
        }
        #endregion

    }
}
#endif