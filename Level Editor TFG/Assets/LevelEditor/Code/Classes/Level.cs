using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level Editor/Level", order = 0)]
public class Level : ScriptableObject {
    //Clase para guardar todos los string necesarios para la serialización del nivel. Suele ser mucho más sencillo si tengo que cambiar una variable.
    public static class LevelProperties{
        public const string NAME = "name";
    }
    public string name;

    public Vector2Int mapSize;
    public float xcellSize;

    public float ycellSize;

}