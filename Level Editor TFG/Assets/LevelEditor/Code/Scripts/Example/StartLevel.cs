﻿using System.Collections;
using System.Collections.Generic;
using LevelEditor;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelRegion l = LevelLoader.GetLevel("Nivel 1");
        l.InitLevel(Vector3.zero,null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
