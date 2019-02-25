using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class uteSettings : EditorWindow
{
	private string globalYsize = "";
	private string globalYsizeOld;
	private float newGlobalYsize;
	private string rightClick = "";
	private string yType = "";
	private string cameraType = "";
	private GUIContent[] rightClickOptions;
	private GUIContent[] yTypesOptions;
	private GUIContent[] cameraTypeOptions;
	private ArrayList rTypes = new ArrayList();
	private ArrayList yTypes = new ArrayList();
	private ArrayList cameraTypes = new ArrayList();
	private uteComboBox comboBoxControl = new uteComboBox();
	private uteComboBox comboBoxControl2 = new uteComboBox();
	private uteComboBox comboBoxControl3 = new uteComboBox();
	private GUIStyle listStyle = new GUIStyle();
	private string filepathstring;
	private string catfilepath;
	private string tcfilepath;
	private bool firstTime = true;
	private string gridSizeX;
	private string gridSizeZ;
	private int isConfirming = 0;
	
    [MenuItem ("Window/proTileMapEditor/Settings-Controls",false,5)]
    static void Init ()
	{
        uteSettings window = (uteSettings)EditorWindow.GetWindow (typeof (uteSettings));
		window.Show();
    }
    
    private void OnFocus()
    {
    	filepathstring = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteSettingstxt);
    	catfilepath = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteCategoryInfotxt);
    	tcfilepath = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteTileConnectionstxt);
    }

	private void OnGUI()
	{
		if(isConfirming!=0)
		{
			int addY = 0;

			GUI.Label(new Rect(25,25,100,25),"ARE YOU SURE?");

			if(isConfirming.Equals(1))
			{
				GUI.Label(new Rect(25,50,200,200),"This will reset only this Settings\nwindow to default values.");
			}
			else if(isConfirming.Equals(2))
			{
				addY = 40;
				GUI.Label(new Rect(25,50,200,200),"This will reset and delete:\n - Settings Window\n - Delete and Clear all Categories\n - Delete and Clear all Tile-Connections\n - Delete and Clear all maps\n - Delete and Clear all Patterns");
			}

			if(GUI.Button(new Rect(25,100+addY,70,30),"NO"))
			{
				isConfirming = 0;
			}

			if(GUI.Button(new Rect(100,100+addY,50,25),"YES"))
			{
				if(isConfirming.Equals(1))
				{
					globalYsize = "1.0";
					gridSizeX = "100";
					gridSizeZ = "100";
					saveAllSettings(globalYsize,0,0,0);
				}
				else if(isConfirming.Equals(2))
				{
					gridSizeX = "100";
					gridSizeZ = "100";
					globalYsize = "1.0";
					saveAllSettings(globalYsize,0,0,0);
					ClearCategories();
					ClearTileConnections();
					DeleteAllMapsAndPatterns();
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}

				isConfirming = 0;
			}
			return;
		}

		if(firstTime)
		{
			LoadSettings();
		}
		
		ReloadComboBoxes();

		if(!globalYsize.Equals("err")&&!rightClick.Equals("err"))
		{
			int selectedItemIndex = comboBoxControl.GetSelectedItemIndex();
			int selectedItemIndex2 = comboBoxControl2.GetSelectedItemIndex();
			int selectedItemIndex3 = comboBoxControl3.GetSelectedItemIndex();

			if(firstTime)
			{
				setIndexes();
				firstTime = false;
			}
			
			GUI.Box(new Rect(0,20,330,260),"proTile Map Editor Settings");
			
			GUI.Label (new Rect(10,45,100,25),"Grid Size X/Z: ");
			gridSizeX = GUI.TextField (new Rect(120,45,90,20),gridSizeX);
			gridSizeZ = GUI.TextField (new Rect(220,45,90,20),gridSizeZ);

			if(!comboBoxControl.isClickedComboButton&&!comboBoxControl2.isClickedComboButton&&!comboBoxControl3.isClickedComboButton)
			{
				GUI.Label (new Rect(10,150,300,25),"Grid Y Move Distance: ");
				globalYsize = GUI.TextField (new Rect(140,150,100,20),globalYsize);

				int buttonsOffsetY = 50;
				if(GUI.Button (new Rect(10,140+buttonsOffsetY,300,25),"Save Settings & Exit"))
				{
					saveAllSettings(globalYsize, selectedItemIndex, selectedItemIndex2, selectedItemIndex3);
					this.Close();
				}
				
				if(GUI.Button (new Rect(10,170+buttonsOffsetY,150,25),"Reset Settings"))
				{
					isConfirming = 1;
				}
				
				if(GUI.Button (new Rect(160,170+buttonsOffsetY,150,25),"Reset Everything"))
				{
					isConfirming = 2;
				}

				if(GUI.Button(new Rect(10,200+buttonsOffsetY,300,25),"Don't Save & Close"))
				{
					this.Close();
				}
			}
			
			GUI.Label(new Rect(10,70,300,25),"Right Click Action: ");
			selectedItemIndex = comboBoxControl.List(new Rect(120, 68, 200, 20), rightClickOptions[selectedItemIndex].text+" ^", rightClickOptions, listStyle );
			
			if(!comboBoxControl.isClickedComboButton)
			{
				GUI.Label(new Rect(10,90,300,25),"Y Snap Type: ");
				selectedItemIndex2 = comboBoxControl2.List(new Rect(120, 88, 200, 20), yTypesOptions[selectedItemIndex2].text+" ^", yTypesOptions, listStyle );
			}

			if(!comboBoxControl.isClickedComboButton&&!comboBoxControl2.isClickedComboButton)
			{
				GUI.Label(new Rect(10,110,300,25),"Camera Type: ");
				selectedItemIndex3 = comboBoxControl3.List(new Rect(120,108,200,20), cameraTypeOptions[selectedItemIndex3].text+" ^", cameraTypeOptions, listStyle );
			}
		}
		else
		{
			Debug.Log ("Error: Failed to load settings file, try to reset all.");
			GUI.Label(new Rect(30,30,300,25),"Error loading settings, try to reset all");
		}
	}
	
	private void ClearCategories()
	{
		StreamWriter rw = new StreamWriter(catfilepath);
		rw.Write("");
		rw.Flush();
		rw.Close();
	}
	
	private void ClearTileConnections()
	{
		StreamWriter rw = new StreamWriter(tcfilepath);
		rw.Write("");
		rw.Flush();
		rw.Close();
	}

	private void ReloadComboBoxes()
	{
		yTypes.Clear();
		yTypes.Add("auto");
		yTypes.Add("fixed");
		yTypes.Add("nosnap");
		yTypesOptions = new GUIContent[3];
		yTypesOptions[0] = new GUIContent((string)"Auto");
		yTypesOptions[1] = new GUIContent((string)"Fixed Snap");
		yTypesOptions[2] = new GUIContent((string)"Don't Snap");

		rTypes.Clear();
		rTypes.Add("rotL");
		rTypes.Add("rotR");
		rTypes.Add("rotU");
		rTypes.Add("rotD");
		rTypes.Add("rotI");
		rTypes.Add("erase");
		rightClickOptions = new GUIContent[6];
		rightClickOptions[0] = new GUIContent((string)"Rotate Left");
		rightClickOptions[1] = new GUIContent((string)"Rotate Right");
		rightClickOptions[2] = new GUIContent((string)"Rotate Up");
		rightClickOptions[3] = new GUIContent((string)"Rotate Down");
		rightClickOptions[4] = new GUIContent((string)"Rotate Invert");
		rightClickOptions[5] = new GUIContent((string)"Eraser");

		cameraTypes.Clear();
		cameraTypes.Add("isometric-perspective");
		cameraTypes.Add("isometric-ortho");
		cameraTypes.Add("2d-perspective");
		cameraTypes.Add("2d-ortho");
		cameraTypeOptions = new GUIContent[4];
		cameraTypeOptions[0] = new GUIContent((string)"Isometric-Perspective");
		cameraTypeOptions[1] = new GUIContent((string)"Isometric-Ortho");
		cameraTypeOptions[2] = new GUIContent((string)"2D-Perspective");
		cameraTypeOptions[3] = new GUIContent((string)"2D-Ortho");

		listStyle.normal.textColor = Color.white;
		listStyle.normal.background = new Texture2D(0,0);
		listStyle.onHover.background = new Texture2D(2, 2);
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.padding.bottom = 4;
	}
	
	private void LoadSettings()
	{	
		string pth = filepathstring;

		if(!File.Exists(pth))
		{
			File.Create(pth).Dispose();

			StreamWriter sw = new StreamWriter(pth);
			sw.Write("1:erase:auto:1000:1000:isometric-perspective");
			sw.Flush();
			sw.Close();
			
			AssetDatabase.Refresh();
		}

		TextAsset tx = (TextAsset) Resources.Load ("uteForEditor/uteSettings");
		string info = tx.text;
		string[] splinfo = info.Split(':');
		
		globalYsize = splinfo[0].ToString();
		rightClick = splinfo[1].ToString();
		yType = splinfo[2].ToString();
		gridSizeX = splinfo[3].ToString();
		gridSizeZ = splinfo[4].ToString();
		cameraType = splinfo[5].ToString();
	}
	
	private void setIndexes()
	{
		for(int i=0;i<rTypes.Count;i++)
		{
			if(rTypes[i].ToString().Equals(rightClick))
			{
				comboBoxControl.selectedItemIndex = i;
				break;
			}
		}

		for(int i=0;i<yTypes.Count;i++)
		{
			if(yTypes[i].ToString().Equals(yType))
			{
				comboBoxControl2.selectedItemIndex = i;
				break;
			}
		}

		for(int i=0;i<cameraTypes.Count;i++)
		{
			if(cameraTypes[i].ToString().Equals(cameraType))
			{
				comboBoxControl3.selectedItemIndex = i;
				break;
			}
		}
	}
	
	private void saveAllSettings(string newGlobalYsize, int index, int index2, int index3)
	{
		float gyS;
		
		if(!newGlobalYsize.Contains("."))
			newGlobalYsize+=".0";
		
		try{
			gyS = System.Convert.ToSingle(newGlobalYsize);
			
			string[] _newGlobalYsize = newGlobalYsize.Split('.');

			if(_newGlobalYsize[1].Length>1)
			{
				Debug.Log ("Error: Save failed because Map Y Global Siz float value must contain only 1 number after seperator");
				return;
			}
			
			if(gyS<=0.0f||gyS>100000.0f)
			{
				Debug.Log ("Error: Save failed because Map Y Global Size value is less than 0 or more than 100000.");
				return;
			}
		}
		catch
		{
			Debug.Log ("Error: Save failed because Map Y Global Size value is not float type.");
			return;
		}
		
		string rCtp = rTypes[index].ToString();
		string yTpc = yTypes[index2].ToString();
		string cTpc = cameraTypes[index3].ToString();

		if(gridSizeX=="") gridSizeX = "1000";
		if(gridSizeZ=="") gridSizeZ = "1000";

		string info = gyS.ToString()+":"+rCtp+":"+yTpc+":"+gridSizeX+":"+gridSizeZ+":"+cTpc;
		StreamWriter rw = new StreamWriter(filepathstring);
		rw.Write("");
		rw.Write(info);
		rw.Close();
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		LoadSettings();
		ReloadComboBoxes();
		setIndexes();
		
		Debug.Log ("Settings Saved.");
	}

	private void DeleteAllMapsAndPatterns()
	{
		string pthMaps = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteMyMapstxt); 
		string cMap = uteGLOBAL3dMapEditor.getMapsDir();
		string pthPats = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteMyPatternstxt);
		string cPat = uteGLOBAL3dMapEditor.getPatternsDir();

		StreamReader sr = new StreamReader(pthMaps);
		string myMapsInfo = sr.ReadToEnd();
		sr.Close();

		string[] myMapsParts = (string[]) myMapsInfo.Split(":"[0]);

		for(int i=0;i<myMapsParts.Length;i++)
		{
			string mapName = myMapsParts[i];

			if(!mapName.Equals(""))
			{
				if(File.Exists(cMap+mapName+".txt"))
				{
					File.Delete(cMap+mapName+".txt");
				}
			}
		}

		StreamWriter sw = new StreamWriter(pthMaps);
		sw.Write("");
		sw.Flush();
		sw.Close();

		StreamReader sr2 = new StreamReader(pthPats);
		string myPatsInfo = sr2.ReadToEnd();
		sr2.Close();

		string[] myPatsParts = (string[]) myPatsInfo.Split(":"[0]);

		for(int i=0;i<myPatsParts.Length;i++)
		{
			string patName = myPatsParts[i];

			if(!patName.Equals(""))
			{
				if(File.Exists(cPat+patName+".txt"))
				{
					File.Delete(cPat+patName+".txt");
				}
			}
		}

		StreamWriter sw2 = new StreamWriter(pthMaps);
		sw2.Write("");
		sw2.Flush();
		sw2.Close();
	}
}
