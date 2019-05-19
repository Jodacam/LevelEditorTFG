
    using UnityEngine;
    using UnityEditor;
    using System;
    
    public static class LevelLoader
    {
        public static LevelRegion GetLevel(string name)
        {

          LevelRegion l = (LevelRegion) Resources.Load(name,typeof(LevelRegion));
          l.LoadVariable();
          return l;

        }

    }
