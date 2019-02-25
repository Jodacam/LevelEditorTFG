using UnityEngine;
using System.Collections;

public class uteOptionsBox : MonoBehaviour {
#if UNITY_EDITOR
	[HideInInspector]
	public bool isShow;
	private GUISkin ui;
	private uteMapEditorEngine MapEngine;

	//options
	private bool isEditorLightOn;
	private bool isShowGrid;
	private bool isCastShadows;
	private bool snapOnTop;

	private void Start()
	{
		isCastShadows = false;
		isEditorLightOn = true;
		isShowGrid = true;
		ui = (GUISkin) Resources.Load("uteForEditor/uteUI");
		isShow = false;
		MapEngine = (uteMapEditorEngine) this.gameObject.GetComponent<uteMapEditorEngine>();

		if(MapEngine.yTypeOption.Equals("auto"))
		{
			snapOnTop = true;
		}
		else
		{
			snapOnTop = false;
		}
	}

	private void OnGUI()
	{
		if(isShow)
		{
			GUI.skin = ui;
			GUI.Box(new Rect(Screen.width-300,40,200,320),"OPTIONS");
			
			GUI.Label(new Rect(Screen.width-294,70,120,30),"Editor Light ");
			if(GUI.Button(new Rect(Screen.width-170,70,60,25),ReturnCondition(isEditorLightOn)))
			{
				if(isEditorLightOn)
				{
					isEditorLightOn = false;
					MapEngine.mapLightGO.SetActive(false);
				}
				else
				{
					isEditorLightOn = true;
					MapEngine.mapLightGO.SetActive(true);
				}
			}

			GUI.Label(new Rect(Screen.width-294,100,120,30),"Shadows ");
			if(GUI.Button(new Rect(Screen.width-170,100,60,25),ReturnCondition(isCastShadows)))
			{
				if(isEditorLightOn)
				{
					if(isCastShadows)
					{
						isCastShadows = false;
						MapEngine.mapLightGO.GetComponent<Light>().shadows = LightShadows.None;
					}
					else
					{
						MapEngine.mapLightGO.GetComponent<Light>().shadows = LightShadows.Soft;
						MapEngine.mapLightGO.GetComponent<Light>().shadowStrength = 0.7f;
						isCastShadows = true;
					}
				}
				else
				{
					Debug.Log("Enable lights first!");
				}
			}

			GUI.Label(new Rect(Screen.width-294,130,120,30),"XZ Snapping ");
			if(GUI.Button(new Rect(Screen.width-170,130,60,25),ReturnCondition(uteGLOBAL3dMapEditor.XZsnapping)))
			{
				if(uteGLOBAL3dMapEditor.XZsnapping)
				{
					uteGLOBAL3dMapEditor.XZsnapping = false;
				}
				else
				{
					uteGLOBAL3dMapEditor.XZsnapping = true;
				}
			}

			GUI.Label(new Rect(Screen.width-294,160,130,30),"Overlap Detection ");
			if(GUI.Button(new Rect(Screen.width-170,160,60,25),ReturnCondition(uteGLOBAL3dMapEditor.OverlapDetection)))
			{
				if(uteGLOBAL3dMapEditor.OverlapDetection)
				{
					uteGLOBAL3dMapEditor.OverlapDetection = false;
				}
				else
				{
					uteGLOBAL3dMapEditor.OverlapDetection = true;
				}
			}

			GUI.Label(new Rect(Screen.width-294,190,130,30),"Show Grid");
			if(GUI.Button(new Rect(Screen.width-170,190,60,25),ReturnCondition(isShowGrid)))
			{
				if(isShowGrid)
				{
					isShowGrid = false;
					MapEngine.grid.SetActive(false);
				}
				else
				{
					isShowGrid = true;
					MapEngine.grid.SetActive(true);
				}
			}

			GUI.Label(new Rect(Screen.width-294,220,130,30),"Calculate XZ Pivot");
			if(GUI.Button(new Rect(Screen.width-170,220,60,25),ReturnCondition(uteGLOBAL3dMapEditor.CalculateXZPivot)))
			{
				if(uteGLOBAL3dMapEditor.CalculateXZPivot)
				{
					uteGLOBAL3dMapEditor.CalculateXZPivot = false;
				}
				else
				{
					uteGLOBAL3dMapEditor.CalculateXZPivot = true;
				}
			}

			GUI.Label(new Rect(Screen.width-294,250,130,30),"Snap on TOP");
			if(GUI.Button(new Rect(Screen.width-170,250,60,25),ReturnCondition(snapOnTop)))
			{
				if(snapOnTop)
				{
					snapOnTop = false;
					MapEngine.yTypeOption = "fixed";
				}
				else
				{
					snapOnTop = true;
					MapEngine.yTypeOption = "auto";
				}
			}

			if(GUI.Button(new Rect(Screen.width-280,300,160,40),"CLOSE"))
			{
				isShow = false;
			}
		}
	}

	private string ReturnCondition(bool isTrue)
	{
		if(isTrue)
		{
			return "ON";
		}
		else
		{
			return "OFF";
		}
	}

#endif
}
