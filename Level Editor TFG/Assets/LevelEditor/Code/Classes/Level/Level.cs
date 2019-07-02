using UnityEditor;
using UnityEngine;


using System.Collections.Generic;


namespace LevelEditor
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level Editor TFG/Level", order = 0)]
    public class Level : ScriptableObject
    {


        public enum VariableTypes
        {
            String,
            Int,

            Float,

            Boolean,
            None

        }


        public static class Properties
        {
            public const string NAME = "levelName";
            public const string SIZE = "cellSize";

            public const string EXTENSION = "cellCount";
        }
        [SerializeField]
        [HideInInspector]
        public List<IData> varList;

        [SerializeField]
        [HideInInspector]
        string jsonData;

        [SerializeField]
        public string levelName;

        [HideInInspector]
        public GameObject runTimeTerrain;

        [SerializeField]
        [HideInInspector]
        public Vector2Int cellCount;

        [SerializeField]
        [HideInInspector]
        public Vector2 cellSize;

        [SerializeField]
        public GameObject terrainPrefab;

        public void Init(string name)
        {
            levelName = name;
            LoadVars();
            runTimeTerrain = new GameObject("Base Level Terrain", typeof(LevelScript), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
            cellCount = new Vector2Int(10, 10);
            cellSize = Vector2.one;
            runTimeTerrain.GetComponent<LevelScript>().InitTerrain(cellSize, cellCount, this);
            runTimeTerrain.layer = LayerMask.NameToLayer("LevelTerrain");
        }

        public void LoadLevel(Vector3 position, Transform parent)
        {
            runTimeTerrain = Instantiate(terrainPrefab, position, Quaternion.identity, parent);
            LoadVars();
        }

        #region Variables

        public void AddVariable(IData e)
        {
            varList.Add(e);
        }
        public void ChangeVariableType(IData v, VariableTypes e)
        {
            int index = varList.IndexOf(v);
            varList.RemoveAt(index);
            switch (e)

            {
                case VariableTypes.String:
                    var newString = new VariableString();
                    newString.Init(v.varName);
                    varList.Insert(index, newString);
                    break;
                case VariableTypes.Boolean:
                    var newBool = new VariableBool();
                    newBool.Init(v.varName);
                    varList.Insert(index, newBool);
                    break;
                case VariableTypes.Int:
                    var newInt = new VariableInt();
                    newInt.Init(v.varName);
                    varList.Insert(index, newInt);
                    break;
                case VariableTypes.Float:
                    var newFloat = new VariableFloat();
                    newFloat.Init(v.varName);
                    varList.Insert(index, newFloat);
                    break;
            }
        }
#if UNITY_EDITOR
        public void ScaleGrid()
        {
            runTimeTerrain.GetComponent<LevelScript>().CreateMesh(cellSize, cellCount);
        }

        public void SaveVars()
        {
            jsonData = GUIAuxiliar.Serialize(new VariableContainer() { value = varList });
        }

        /// <summary>
        /// Save the level into the disk
        /// </summary>
        /// <param name="path">path were to save</param>
        public void SaveItself(string folder)
        {
            Paths.CreateFolderIfNotExist(folder, levelName);
            /* if (!AssetDatabase.IsValidFolder(folder + "/" + levelName))
            {
                AssetDatabase.CreateFolder(folder, levelName);
            }
           */
            SaveVars();
           
            var levelScript = runTimeTerrain.GetComponent<LevelScript>();
            levelScript.SaveItself(folder + "/" + levelName + "/");
            terrainPrefab = PrefabUtility.SaveAsPrefabAsset(runTimeTerrain, folder + "/" + levelName + "/" + levelName + ".prefab");
        }
        #endif
        public void LoadVars()
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                var container = (VariableContainer)GUIAuxiliar.Deserialize<VariableContainer>(jsonData);
                varList = container.value;
            }
            else
            {
                varList = new List<IData>();
            }
        }

        //Funciones que devuelven el valor guardado en el nivel.
        // Si el valor no existe se avisará mediante consola y devolvera un valor predeterminado. 
        //Tambien puedo hacer que saquen una excepcion si el valor no existe, como en unity.
        public string GetString(string name)
        {
            VariableString e = (VariableString)getData(name, VariableTypes.String);
            return e != null ? e.value : null;
        }



        public bool GetBool(string name)
        {
            VariableBool e = (VariableBool)getData(name, VariableTypes.Boolean);
            return e != null ? e.value : false;
        }
        public int GetInt(string name)
        {
            VariableInt e = (VariableInt)getData(name, VariableTypes.Int);
            return e != null ? e.value : int.MinValue;
        }
        public float GetFloat(string name)
        {
            VariableFloat e = (VariableFloat)getData(name, VariableTypes.Float);
            return e != null ? e.value : float.MinValue;
        }

        private IData getData(string name, VariableTypes type)
        {
            return varList.Find((value) => value.varName == name && value.type == type);
        }
        #endregion



    }
}