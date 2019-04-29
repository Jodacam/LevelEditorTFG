
    using UnityEngine;
    using UnityEditor;
    using System;
    
    public static class LevelLoader
    {
        public static Level GetLevel(string name)
        {

          Level l = (Level) Resources.Load(name,typeof(Level));
          l.LoadVariable();
          return l;

        }

    }
