﻿using UnityEngine;
using UnityEditor;

public class uteVisitWebsite : EditorWindow {

	[MenuItem ("Window/proTileMapEditor/Other/Visit Website",false,8)]
    static void Init ()
	{
		Application.OpenURL("http://www.protilemapeditor.com");
    }
}
