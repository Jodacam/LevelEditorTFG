

using System;
using UnityEngine;
[Serializable]
public class Cell {
    //Posición en cordenadas de la malla, dado que el nivel se puede instanciar en un lugar que no sea el 0,0 se tendra que acceder mediante el nivel.
    public Vector3 position;
    public GameObject cellObject;

    //TODO Posiblemente otra clase que contenga algo de información, como el tipo de suelo etc
    public int cellInfo;
    

    public Cell(Vector3 middlePoint)
    {
        position = middlePoint;
    }
}