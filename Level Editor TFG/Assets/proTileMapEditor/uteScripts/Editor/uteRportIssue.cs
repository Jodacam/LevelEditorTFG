using UnityEngine;
using UnityEditor;

public class uteRportIssue : EditorWindow {

	[MenuItem ("Window/proTileMapEditor/Other/Report Issue",false,6)]
    static void Init ()
	{
        Application.OpenURL("http://protilemapeditor.com/?page_id=7");
    }
}
