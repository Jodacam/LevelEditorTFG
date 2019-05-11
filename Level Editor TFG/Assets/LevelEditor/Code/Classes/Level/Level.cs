using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
[CreateAssetMenu(fileName = "Level", menuName = "Level Editor/Level", order = 0)]
public class Level : ScriptableObject
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


    public GridTerrain terrainGrid;
    public GameObject terrainGameObject;
    public Mesh terrainMesh;
    public string name;
    public Vector2Int mapSize;
    public Vector2 mapScale;
    [SerializeField]
    private string jsonData;

    //Tengo que crear un diccionario para poder guardar los diferentes valores personalizados, Booleans, Strings, Floats e Ints.
    [SerializeField]
    public List<IData> stringList;
    

    public float xcellSize { get { return mapScale.x; } set { mapScale.x = value; } }
    public float ycellSize { get { return mapScale.y; } set { mapScale.y = value; } }

    public int xSize { get { return mapSize.x; } set { mapSize.x = value; } }
    public int ySize { get { return mapSize.y; } set { mapSize.y = value; } }

    public void LoadGrid()
    {
        if(terrainGrid == null){
            terrainGrid = Instantiate(terrainGameObject, Vector3.zero, Quaternion.identity).GetComponent<GridTerrain>();
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
        GameObject plane = new GameObject("BaseTerrain", typeof(GridTerrain));
        plane.AddComponent<MeshFilter>();
        MeshRenderer e = plane.AddComponent<MeshRenderer>();
        plane.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        //plane.GetComponent<GridTerrain>().CreateGrid(xcellSize, ycellSize, mapSize);
        terrainGrid = plane.GetComponent<GridTerrain>();
        terrainMesh = terrainGrid.Init(xcellSize, ycellSize, mapSize, this);


    }

    public void AddVariable(IData e)
    {
        stringList.Add(e);
    }

    public void InitLevel(Vector3 position, Transform levelParent)
    {
        terrainGrid = Instantiate(terrainGameObject, position, Quaternion.identity, levelParent).GetComponent<GridTerrain>();
        terrainGrid.ReDoDictionary();


    }

    private void OnDestroy() {
        Debug.Log(name + "Destroy");
    }


    #region Variables
    public void ChangeVariableType(IData v, VariableTypes e)
    {
        int index = stringList.IndexOf(v);
        stringList.RemoveAt(index);
        switch (e)

        {
            case VariableTypes.String:
                var newString = new VariableString();
                newString.Init(v.varName);
                stringList.Insert(index, newString);
                break;
            case VariableTypes.Boolean:
                var newBool = new VariableBool();
                newBool.Init(v.varName);
                stringList.Insert(index, newBool);
                break;
            case VariableTypes.Int:
                var newInt = new VariableInt();
                newInt.Init(v.varName);
                stringList.Insert(index, newInt);
                break;
            case VariableTypes.Float:
                var newFloat = new VariableFloat();
                newFloat.Init(v.varName);
                stringList.Insert(index, newFloat);
                break;
        }
    }

    public void SaveVars()
    {
        jsonData = GUIAuxiliar.Serialize(new VariableContainer() { value = stringList });
    }

    internal void LoadVariable()
    {
        if (!string.IsNullOrEmpty(jsonData))
        {
            var container = (VariableContainer)GUIAuxiliar.Deserialize<VariableContainer>(jsonData);
            stringList = container.value;
        }
        else
        {
            stringList = new List<IData>();
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
        VariableBool e = (VariableBool)getData(name, VariableTypes.String);
        return e != null ? e.value : false;
    }
    public int GetInt(string name)
    {
        VariableInt e = (VariableInt)getData(name, VariableTypes.String);
        return e != null ? e.value : int.MinValue;
    }
    public float GetFloat(string name)
    {
        VariableFloat e = (VariableFloat)getData(name, VariableTypes.String);
        return e != null ? e.value : float.MinValue;
    }

    private IData getData(string name, VariableTypes type)
    {
        return stringList.Find((value) => value.varName == name && value.type == type);
    }
    #endregion
}