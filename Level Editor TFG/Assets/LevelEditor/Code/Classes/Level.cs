using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "Level", menuName = "Level Editor/Level", order = 0)]
public class Level : ScriptableObject {
    //Clase para guardar todos los string necesarios para la serialización del nivel. Suele ser mucho más sencillo si tengo que cambiar una variable.
    public static class LevelProperties{
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
    public GameObject terrainGameObjec;
    public Mesh terrainMesh;
    public string name;
    public Vector2Int mapSize;
    public Vector2 mapScale;

    //Tengo que crear un diccionario para poder guardar los diferentes valores personalizados, Booleans, Strings, Floats e Ints.
    [SerializeField]
    public List<IData> stringList;

    public float xcellSize { get { return mapScale.x; } set { mapScale.x = value; } }
    public float ycellSize { get { return mapScale.y; } set { mapScale.y = value; } }

    public int xSize {get { return mapSize.x; } set { mapSize.x = value; } }
    public int ySize {get { return mapSize.y; } set { mapSize.y = value; }}

    public void LoadGrid()
    {
        terrainGrid = Instantiate(terrainGameObjec,Vector3.zero,Quaternion.identity).GetComponent<GridTerrain>();
        terrainGrid.ReDoDictionary();
    }

    public void ReCreateGrid()
    {
       terrainMesh = terrainGrid.ChangeSize(xcellSize,ycellSize,mapSize);
    }

    public void CreateGrid()
    {
            GameObject plane = new GameObject("BaseTerrain", typeof(GridTerrain));
            plane.AddComponent<MeshFilter>();
            MeshRenderer e = plane.AddComponent<MeshRenderer>();
            plane.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);      
            //plane.GetComponent<GridTerrain>().CreateGrid(xcellSize, ycellSize, mapSize);
            terrainGrid = plane.GetComponent<GridTerrain>();
            terrainMesh = terrainGrid.Init(xcellSize,ycellSize,mapSize,this);
            
            
    }

    public void AddVariable(IData e)
    {
        stringList.Add(e);
    }

    public void InitLevel(Vector3 position, Transform levelParent)
    {
        terrainGrid = Instantiate(terrainGameObjec,position,Quaternion.identity,levelParent).GetComponent<GridTerrain>();
        terrainGrid.ReDoDictionary();
    }



    public void EditVariable(EditorWindow levelEditorWindow)
    {

        GUILayout.Label("Variables");
        foreach(var v in stringList)
        {
            GUIStyle s = new GUIStyle( GUI.skin.label);
            s.alignment = TextAnchor.MiddleCenter;
            s.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField(v.varName,s);
            v.varName = EditorGUILayout.TextField("Name", v.varName);
            var e = (VariableTypes) EditorGUILayout.EnumPopup("Type",v.type);
            if(e != v.type)
            {
                
                ChangeVariableType(v,e);
                
            }
            else
            {


                v.ShowGUI();
              
               
            }
        }
    }



    private void ChangeVariableType(IData v, VariableTypes e)
    {   
        int index = stringList.IndexOf(v);
        stringList.RemoveAt(index);
        switch(e)

        {
            case VariableTypes.String: 
            var newString = new VariableString(); 
            newString.Init(v.varName);
            stringList.Insert(index,newString);
            break;
            case VariableTypes.Boolean:
            var newBool = new VariableBool();
            newBool.Init(v.varName);  
            stringList.Insert(index,newBool);
            break;
            case VariableTypes.Int:
             var newInt = new VariableInt();
            newInt.Init( v.varName);   
            stringList.Insert(index,newInt);
            break;
            case VariableTypes.Float:
            var newFloat =new VariableFloat();
            newFloat.Init( v.varName);   
            stringList.Insert(index,newFloat);
            break;
        }
    }
}