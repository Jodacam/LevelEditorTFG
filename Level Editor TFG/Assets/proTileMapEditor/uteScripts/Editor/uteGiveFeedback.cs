﻿using UnityEngine;
using UnityEditor;

public class uteGiveFeedback : EditorWindow {

	[MenuItem ("Window/proTileMapEditor/Other/Give Feedback",false,6)]
    static void Init ()
	{
        Application.OpenURL("http://protilemapeditor.com/?page_id=19");
    }
}
