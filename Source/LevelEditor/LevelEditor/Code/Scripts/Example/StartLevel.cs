using System.Collections;
using System.Collections.Generic;
using LevelEditor;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Level l = LevelLoader.GetLevel("Cruce");
        l.LoadLevel(Vector3.zero,null);
        Debug.Log(l.GetInt("Personas"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
