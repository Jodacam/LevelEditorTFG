﻿using UnityEngine;
using UnityEditor;

public class uteDocumentation : MonoBehaviour {

	[MenuItem ("Window/proTileMapEditor/Other/Documentation",false,7)]
    static void Init ()
	{
        Application.OpenURL("http://protilemapeditor.com/?page_id=5");
    }
}
