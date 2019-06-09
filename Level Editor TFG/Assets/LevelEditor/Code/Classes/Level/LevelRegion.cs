using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace LevelEditor
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level Editor/Level", order = 0)]
    public class LevelRegion : ScriptableObject
    {
        //Clase para guardar todos los string necesarios para la serialización del nivel. Suele ser mucho más sencillo si tengo que cambiar una variable.
        public static class LevelProperties
        {
            public const string NAME = "name";

        }

        public enum VariableTypes
        {
            String,
            Int,

            Float,

            Boolean,
            None

        }


        public RegionTerrain terrainGrid;
        public GameObject terrainGameObject;
        public Mesh terrainMesh;
        public string regionName;
        public Vector2Int mapSize;
        public Vector2 mapScale;
        [SerializeField]
        private string jsonData;

        //Tengo que crear un diccionario para poder guardar los diferentes valores personalizados, Booleans, Strings, Floats e Ints.
        [SerializeField]
        public List<IData> varList;


        public float xcellSize { get { return mapScale.x; } set { mapScale.x = value; } }
        public float ycellSize { get { return mapScale.y; } set { mapScale.y = value; } }

        public int xSize { get { return mapSize.x; } set { mapSize.x = value; } }
        public int ySize { get { return mapSize.y; } set { mapSize.y = value; } }

        public void LoadGrid()
        {
            if (terrainGrid == null)
            {
                terrainGrid = Instantiate(terrainGameObject, Vector3.zero, Quaternion.identity).GetComponent<RegionTerrain>();
            }
            terrainGrid.ReDoDictionary();
            LoadVariable();
        }



        public void ReCreateGrid()
        {

            terrainMesh = terrainGrid.ChangeSize(xcellSize, ycellSize, mapSize);

        }

        public void CreateGrid()
        {
            GameObject plane = new GameObject("BaseTerrain", typeof(RegionTerrain));
            plane.AddComponent<MeshFilter>();
            MeshRenderer e = plane.AddComponent<MeshRenderer>();
            plane.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            //plane.GetComponent<GridTerrain>().CreateGrid(xcellSize, ycellSize, mapSize);
            terrainGrid = plane.GetComponent<RegionTerrain>();
            terrainMesh = terrainGrid.Init(xcellSize, ycellSize, mapSize, this);


        }

        public void AddVariable(IData e)
        {
            varList.Add(e);
        }

        public void InitLevel(Vector3 position, Transform levelParent)
        {
            terrainGrid = Instantiate(terrainGameObject, position, Quaternion.identity, levelParent).GetComponent<RegionTerrain>();
            terrainGrid.ReDoDictionary();


        }

        private void OnDestroy()
        {
            Debug.Log(regionName + "Destroy");
        }


        #region Variables
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

        public void SaveVars()
        {
            jsonData = GUIAuxiliar.Serialize(new VariableContainer() { value = varList });
        }

        internal void LoadVariable()
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


        #endregion
    }
}