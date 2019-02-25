using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;

public class uteCategoryEditor : EditorWindow
{
    private string catName = "newCategoryName";
    private GUIContent[] comboBoxList;
    private GUIContent[] comboBoxList_TilesType;
	private uteComboBox comboBoxControl = new uteComboBox();
	private uteComboBox comboBoxTilesType = new uteComboBox();
	private GUIStyle listStyle = new GUIStyle();
	private string filepathstring;
	private ArrayList catNames = new ArrayList();
	private ArrayList catColls = new ArrayList();
	private ArrayList catObjs = new ArrayList();
	private ArrayList catState = new ArrayList();
	private Vector2 scrollPosition;
	private string selItemText = "";
	[SerializeField]
	private int lastTileTypeIndex = -1;
	private int lastCategoryIndex = -1;
	private int boxSize = 125;
	
	public int lastTileTypeIndexP
    {
        get { return lastTileTypeIndex; }
        set
        {
            if (lastTileTypeIndex == value) return;
 
            lastTileTypeIndex = value;
        }
    }

    [MenuItem ("Window/proTileMapEditor/Tiles Editor",false,3)]
    static void Init () {
        uteCategoryEditor window = (uteCategoryEditor)EditorWindow.GetWindow (typeof (uteCategoryEditor));
		window.Show();
    }
	
	private void LoadMain()
	{
		ReadCategoriesFromFile();
	}
	
	private void ReloadComboBox()
	{
		comboBoxList = new GUIContent[catNames.Count];
		for(int i=0;i<catNames.Count;i++)
		{
			comboBoxList[i] = new GUIContent((string)catNames[i].ToString());
		}

		comboBoxList_TilesType = new GUIContent[2];
		comboBoxList_TilesType[0] = new GUIContent((string)"Static");
		comboBoxList_TilesType[1] = new GUIContent((string)"Dynamic");

		listStyle.normal.textColor = Color.white;
		listStyle.normal.background = new Texture2D(0,0);
		listStyle.onHover.background = new Texture2D(2, 2);
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.padding.bottom = 4;
	}

	private void OnFocus()
	{
		filepathstring = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteCategoryInfotxt);
		LoadMain();
		ReloadComboBox();
		SetComboBoxes();
		this.Repaint();
	}
	
    private void OnGUI()
	{
		if(lastCategoryIndex!=comboBoxControl.selectedItemIndex)
		{
			lastCategoryIndex = comboBoxControl.selectedItemIndex;
			LoadMain();
			SetComboBoxes();
			
			this.Repaint();
		}
		
		if(lastTileTypeIndex!=comboBoxTilesType.selectedItemIndex)
		{
			lastTileTypeIndex = comboBoxTilesType.selectedItemIndex;
			ChangeTileTypeInCategory();
		}

        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        catName = EditorGUILayout.TextField ("Category Name", catName);
		
		if(GUILayout.Button("Create New Category"))
		{
			AddNewCategory(catName);
			this.Repaint();
		}
		
		HandleAndDrawAllObjects();
		
		if(catNames.Count>0)
		{
			GUI.Box(new Rect(10,70,540,50), "Category: " + selItemText);
			GUI.Box(new Rect(10,120,540,20),"Drag Prefabs to add them to ["+selItemText+"] category!");
			GUI.Box(new Rect(560,70,300,50),"Category Settings");
			GUI.Label(new Rect(570,95,100,22),"Tiles Type: ");

			int selectedItemIndex = comboBoxControl.GetSelectedItemIndex();
			int selectedItemIndex_tilestype = comboBoxTilesType.GetSelectedItemIndex();

			selItemText = comboBoxList[selectedItemIndex].text;

			if(selectedItemIndex<catNames.Count)
			{
				if(catNames[selectedItemIndex].ToString().Equals(""))
				{
					comboBoxControl.selectedItemIndex = 0;
					selectedItemIndex = 0;
				}
			}
			else
			{
				comboBoxControl.selectedItemIndex = 0;
				selectedItemIndex = 0;
			}
			
			selectedItemIndex = comboBoxControl.List(new Rect(20,90,200,20), comboBoxList[selectedItemIndex].text+" ^", comboBoxList, listStyle );
			selectedItemIndex_tilestype = comboBoxTilesType.List(new Rect(640,95,150,20),comboBoxList_TilesType[selectedItemIndex_tilestype].text+" ^",comboBoxList_TilesType,listStyle);
			
			if(GUI.Button(new Rect(230,90,150,20),"Remove this Category"))
			{
				RemoveCategory(catNames[selectedItemIndex].ToString());
				this.Repaint();
			}
			
			if(GUI.Button(new Rect(390,90,150,20),"Clear all objects"))
			{
				ClearAllObjectsInCat(catNames[selectedItemIndex].ToString());
				this.Repaint();
			}
		}
		else
		{
			GUI.Label(new Rect(20,70,400,21), "No Categories, Please Create.");
		}
		
		HandleDragContent();
	}
	
	private void ClearAllObjectsInCat(string cname)
	{
		StreamReader rd = new StreamReader(filepathstring);
		string allinfo = rd.ReadToEnd();
		rd.Close();
		string allnewinfo = "";
		string[] allinfobycat = (string[]) allinfo.Split('|');
		
		for(int i=0;i<allinfobycat.Length;i++)
		{
			string[] splitedinfo = (string[]) allinfobycat[i].ToString().Split('$');
			
			if(!allinfobycat[i].Equals(""))
			{				
				if(splitedinfo[0].ToString().Equals(cname))
				{
					allnewinfo += cname+"$"+splitedinfo[1].ToString()+"$:$"+splitedinfo[3].ToString()+"$|";
				}
				else
				{
					allnewinfo += allinfobycat[i].ToString() + "|";
				}
			}
		}
		
		StreamWriter rw = new StreamWriter(filepathstring);
		rw.Write("");
		rw.Write(allnewinfo);
		rw.Close();
			
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		LoadMain();
		ReloadComboBox();
	}
	
	private void HandleAndDrawAllObjects()
	{
		if(comboBoxControl.selectedItemIndex>=0&&comboBoxControl.selectedItemIndex<catNames.Count)
		{
			string[] _objs = (string[]) catObjs[comboBoxControl.selectedItemIndex].ToString().Split(':');

			int x = 0;
			int y = 0;
			
			scrollPosition = GUI.BeginScrollView(new Rect(20, 140, 770, 250), scrollPosition, new Rect(0, 0, 650, boxSize));
			
			for(int j=0;j<_objs.Length;j++)
			{
				if(!_objs[j].ToString().Equals(""))
				{
					string objpath = AssetDatabase.GUIDToAssetPath(_objs[j].ToString());
					Object _obj = (Object) AssetDatabase.LoadMainAssetAtPath(objpath);
					Texture2D previewT = new Texture2D(2,2);
					//Texture icontexture = AssetDatabase.GetCachedIcon(objpath);
					if(_obj)
					{
						previewT = AssetPreview.GetAssetPreview(_obj);
					}
					
					if(_obj)
					{
						GUI.Box(new Rect(10+(105*x),10+(120*y),100,114),"");
						
						if(previewT)
						{
							GUI.DrawTexture(new Rect(11+(105*x),11+(120*y),99,99),previewT,ScaleMode.ScaleToFit);
						}
						else
						{
						//	GUI.DrawTexture(new Rect(56+(125*x),166+(120*y),50,50),icontexture,ScaleMode.ScaleToFit);
							GUI.Label(new Rect(25+(105*x),56+(120*y),80,50),"NO PREVIEW");
						}
						
						GUI.Label(new Rect(20+(105*x),13+(120*y),100,20),_obj.name);
						
						if(!comboBoxControl.isClickedComboButton&&!comboBoxTilesType.isClickedComboButton)
						{
							if(GUI.Button(new Rect(11+(105*x),100+(120*y),98,20),"Remove"))
							{
								RemoveObjectFromCat(catNames[comboBoxControl.selectedItemIndex].ToString(),_objs[j].ToString());
							}
						}
						
						x++;
						if(x==7)
						{
							x=0;
							y++;
							boxSize = (y*125)+125;
						}
					}
				}
			}
			
			GUI.EndScrollView();
		}
	}
	
	private void RemoveObjectFromCat(string cname, string objguid)
	{
		StreamReader rd = new StreamReader(filepathstring);
		string allinfo = rd.ReadToEnd();
		rd.Close();
		
		string allnewinfo = "";
		string[] allinfobycat = (string[]) allinfo.Split('|');
		
		for(int i=0;i<allinfobycat.Length;i++)
		{
			string[] splitedinfo = (string[]) allinfobycat[i].ToString().Split('$');
			
			if(splitedinfo[0].ToString().Equals(cname))
			{
				string[] objs = (string[]) splitedinfo[2].ToString().Split(':');
				string newobjs = "";
				
				for(int j=0;j<objs.Length;j++)
				{
					if(!objs[j].ToString().Equals(objguid) && !objs[j].ToString().Equals(""))
					{
						newobjs += objs[j].ToString() + ":";
					}
				}
				
				allnewinfo += cname+"$"+splitedinfo[1].ToString()+"$"+newobjs+"$"+splitedinfo[3].ToString()+"$|";
			}
			else
			{
				allnewinfo += allinfobycat[i].ToString() + "|";
			}
		}
		
		StreamWriter rw = new StreamWriter(filepathstring);
		rw.Write("");
		rw.Write(allnewinfo);
		rw.Close();
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		LoadMain();
		ReloadComboBox();
	}
	
	private void HandleDragContent()
	{	
	    DragAndDrop.AcceptDrag();

		if (Event.current.type == EventType.DragUpdated)	
		{
			DragAndDrop.visualMode = DragAndDropVisualMode.Link;
		}
		
		if (Event.current.type == EventType.DragPerform && catNames.Count>0)
		{
			string[] allpaths = (string[]) DragAndDrop.paths;
			//object[] allobjs = (object[]) DragAndDrop.objectReferences;
			ArrayList guids = new ArrayList();
			
			for(int i=0;i<allpaths.Length;i++)
			{
				string ftype = Path.GetExtension(allpaths[i].ToString());
				
				if(ftype.Equals(".prefab"))
				{
					string _guid = AssetDatabase.AssetPathToGUID(allpaths[i].ToString());
				
					if(!_guid.Equals(""))
					{
						guids.Add (_guid);
					}
				}
				else
				{
					string _guid = AssetDatabase.AssetPathToGUID(allpaths[i].ToString());
					Debug.Log(_guid);
					Debug.Log ("Warning: File ("+allpaths[i].ToString()+") was ignored because it's not a prefab");
				}
			}
			
			if(guids.Count>0)
			{
				AddGUIDStoCategory(guids);
			}

			this.Repaint();
		}
	}
	
	private void AddGUIDStoCategory(ArrayList guids)
	{
		string cname = catNames[comboBoxControl.selectedItemIndex].ToString();
		string _guids = "";
		//string collidertype = "";
		string allinfo = "";
		string newcatinfo = "";
		
		StreamReader rd = new StreamReader(filepathstring);
		allinfo = rd.ReadToEnd();
		rd.Close();
		
		string[] allinfobycat = (string[]) allinfo.Split('|');
		
		for(int i=0;i<allinfobycat.Length;i++)
		{
			string[] splitedinfo = (string[]) allinfobycat[i].ToString().Split('$');
			
			if(!allinfobycat[i].ToString().Equals(""))
			{
				if(splitedinfo[0].ToString().Equals(cname))
				{
					string[] oldguids = (string[]) splitedinfo[2].ToString().Split(':');
					
					for(int k=0;k<guids.Count;k++)
					{
						bool exist = false;
						
						for(int l=0;l<oldguids.Length;l++)
						{
							if(guids[k].ToString().Equals(oldguids[l].ToString()))
							{
								exist = true;
								break;
							}
						}
						
						if(!exist && !guids[k].ToString().Equals(""))
						{
							_guids += guids[k].ToString() + ":";
						}
					}	
					
					newcatinfo += cname+"$"+splitedinfo[1].ToString()+"$"+splitedinfo[2].ToString()+""+_guids+"$"+splitedinfo[3].ToString()+"$|";
				}
				else
				{
					newcatinfo += allinfobycat[i].ToString() + "|";
				}
			}
		}
		
		StreamWriter rw = new StreamWriter(filepathstring);
		rw.Write("");
		rw.Write(newcatinfo);
		rw.Close();
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		LoadMain();
		ReloadComboBox();
	}
	
	private void RemoveCategory(string catN)
	{
		StreamReader rd = new StreamReader(filepathstring);
		string allinfo = rd.ReadToEnd ();
		rd.Close();
		string[] infobycat = (string[]) allinfo.Split('|');
		string allnewinfo = "";
		
		for(int i=0;i<infobycat.Length;i++)
		{
			if(!infobycat[i].ToString().Equals(""))
			{
				string[] splitedinfo = (string[]) infobycat[i].ToString().Split('$');
				
				if(!splitedinfo[0].ToString().Equals(catN))
				{
					allnewinfo += infobycat[i].ToString() + "|";
				}
			}
		}
		
		StreamWriter rw = new StreamWriter(filepathstring);
		rw.Write("");
		rw.Write(allnewinfo);
		rw.Close();
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		LoadMain();
		ReloadComboBox();
		
		comboBoxControl.selectedItemIndex = catNames.Count-1;
	}
	
	private void AddNewCategory(string catN)
	{
		if(catN.Contains("$")||catN.Contains("|")||catN.Contains(":")||catN.Contains("/")||catN.Contains("\"")||catN.Contains(".")||catN.Contains(" "))
		{
			Debug.Log ("Warning: Can't containt symbols: /,\",$,:,|,.. They will be stripped.");
		}

		catN = catN.Replace(" ","");
		catN = catN.Replace(".","");
		catN = catN.Replace("\"","");
		catN = catN.Replace("/","");
		catN = catN.Replace(":","");
		catN = catN.Replace("$","");
		catN = catN.Replace("|","");
		
		if(CheckIfCategoryExists(catN))
		{
			return;
		}

		catName = "";
		
		StreamReader rd = new StreamReader(filepathstring);
		string info = rd.ReadToEnd();
		rd.Close();
		string addinfo = catN+"$boxcollider$:$Static$|";
		info += addinfo;
		StreamWriter rw = new StreamWriter(filepathstring);
		rw.Write("");
		rw.Write(info);
		rw.Close();
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		LoadMain();
		ReloadComboBox();
		
		comboBoxControl.selectedItemIndex = catNames.Count-1;
	}
	
	private bool CheckIfCategoryExists(string catN)
	{
		if(catN.Equals("") || catN.Length<3)
		{
			Debug.Log ("Error: Category name should contain at least 3 letters and can't be empty.");
			return true;
		}
		
		for(int i=0;i<catNames.Count;i++)	
		{
			if(catNames[i].ToString().Equals(catN))
			{
				Debug.Log ("Error: Category name already exists.");
				return true;
			}
		}

		return false;
	}
	
	private void ReadCategoriesFromFile()
	{
		if(!File.Exists(filepathstring))
		{
			File.Create(filepathstring).Dispose();
		}

		StreamReader rd = new StreamReader(filepathstring);
		string info = rd.ReadToEnd();
		rd.Close();
		string[] infobycat = (string[]) info.Split('|');
		
		catNames.Clear();
		catColls.Clear();
		catObjs.Clear();
		catState.Clear();

		for(int i=0;i<infobycat.Length;i++)
		{
			if(!infobycat[i].ToString().Equals(""))
			{
				string[] infoin = (string[]) (infobycat[i].ToString()).Split('$');
				catNames.Add(infoin[0].ToString());
				catColls.Add(infoin[1].ToString());
				catObjs.Add(infoin[2].ToString());
				catState.Add(infoin[3].ToString());
			}
		}
	}

	private void SetComboBoxes()
	{
		if(catState.Count>0)
		{
			if(catState[comboBoxControl.GetSelectedItemIndex()].ToString().Equals("Static"))
			{
				comboBoxTilesType.selectedItemIndex = 0;
			}
			else
			{
				comboBoxTilesType.selectedItemIndex = 1;
			}
		}
	}

	private void ChangeTileTypeInCategory()
	{
		string allinfo = "";

		for(int i=0;i<catNames.Count;i++)
		{
			if(!catNames[i].ToString().Equals(catNames[comboBoxControl.selectedItemIndex].ToString()))
			{
				allinfo += catNames[i].ToString()+"$boxcollider$"+catObjs[i].ToString()+"$"+catState[i].ToString()+"$|";
			}
			else
			{
				allinfo += catNames[i].ToString()+"$boxcollider$"+catObjs[i].ToString()+"$"+comboBoxList_TilesType[comboBoxTilesType.selectedItemIndex].text+"$|";
			}
		}

		StreamWriter sw = new StreamWriter(filepathstring);
		sw.Write("");
		sw.Write(allinfo);
		sw.Close();

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}