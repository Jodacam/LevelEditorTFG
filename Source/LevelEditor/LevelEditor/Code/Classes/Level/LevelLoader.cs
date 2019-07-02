
using UnityEngine;
using UnityEditor;
using System;
using LevelEditor;
public static class LevelLoader
{
    public static Level GetLevel(string name)
    {

        if (String.IsNullOrEmpty(name))
        {
            return null;
        }
        Level l = (Level)Resources.Load(Paths.RESOURCES_PATH_LEVELS + name, typeof(Level));
        l?.LoadVars();
        return l;

    }

}
