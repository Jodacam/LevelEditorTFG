using UnityEngine;
using System.Collections;

public class uteHelpBox : MonoBehaviour {
#if UNITY_EDITOR
	[HideInInspector]
	public bool isShow;
	private string[] keys;
	private string[] keysInfo;
	private int keysCount;
	private GUISkin ui;

	private void Start()
	{
		ui = (GUISkin) Resources.Load("uteForEditor/uteUI");
		keysCount = 14;
		keys = new string[keysCount];
		keysInfo = new string[keysCount];
		isShow = false;

		SetInfo();
	}
	
	private void OnGUI()
	{
		if(isShow)
		{
			GUI.skin = ui;
			GUI.Box(new Rect((Screen.width/2)-200,50,500,530),"HELP");

			for(int i=0;i<keysCount;i++)
			{
				GUI.Label(new Rect((Screen.width/2)-170,80+(i*30),100,30),keys[i]);
				GUI.Label(new Rect((Screen.width/2)-50,80+(i*30),400,30),keysInfo[i]);
			}

			if(GUI.Button(new Rect((Screen.width/2)-180,500,460,40),"For more information visit www.protilemapeditor.com"))
			{
				Application.OpenURL("http://www.protilemapeditor.com");
			}

			if(GUI.Button(new Rect((Screen.width/2)+130,545,150,30),"Close"))
			{
				isShow = false;
			}
		}
	}

	private void SetInfo()
	{
		keys[0] = "Key W";
		keysInfo[0] = "Move camera forward.";
		keys[1] = "Key S";
		keysInfo[1] = "Move camera backwards.";
		keys[2] = "Key A";
		keysInfo[2] = "Move camera left.";
		keys[3] = "Key D";
		keysInfo[3] = "Move camera right.";
		keys[4] = "Key Q";
		keysInfo[4] = "Rotate camera left (around your current object).";
		keys[5] = "Key E";
		keysInfo[5] = "Rotate camera right (around your current object).";
		keys[6] = "Key C";
		keysInfo[6] = "Show/Hide object line helpers.";
		keys[7] = "Key Z";
		keysInfo[7] = "Move grid down (distance is defined in settings).";
		keys[8] = "Key X";
		keysInfo[8] = "Move grid up (defined in settings).";
		keys[9] = "Key R";
		keysInfo[9] = "Reset camera rotation to default.";
		keys[10] = "Mouse left";
		keysInfo[10] = "Place object (drag for mass or tile connections build).";
		keys[11] = "Mouse right";
		keysInfo[11] = "Rotate object left (or can be defined in settings).";
		keys[12] = "Mouse scroll";
		keysInfo[12] = "Scroll down/up will zoom in/out camera.";
		keys[13] = "Hold Key ALT";
		keysInfo[13] = "Enabled/Disabled orbit camera rotation.";
	}
#endif
}
