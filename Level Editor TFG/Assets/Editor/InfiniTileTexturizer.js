#pragma strict
/*
		T I L E  E D I T O R   F O R   U N I T Y 3 D
				  Made by Wojciech Gabel
				  	wgabel34@gmail.com
						
					dev. state : 0.67

*/


class InfiniTileTexturizer extends EditorWindow {

	var obj : Transform;
	var t : String;
	
	var shId : int;
	
	var tarS : InfiniTileGenerator;
	
	var butNr : int;
	
	var dragging : boolean;
	
	var imgToDrag : Material;
	
	var startDragPos : Vector2;
	
	var dragPos : Vector2;
	
	var dragPoint : int;

	var areaOfOperation : Rect;
	
	var insertion : int;
	
	//@MenuItem ("Window/InfiniTile/Texturizer")
    static function Init () {

       var windowz : InfiniTileTexturizer = EditorWindow.GetWindow(InfiniTileTexturizer,false) as InfiniTileTexturizer;
       
       windowz.butNr = -1;
       windowz.Show();
       windowz.OnSelectionChange();
       windowz.SetSize(windowz);
       windowz.GetListOfTextures();
    }
	
	function SetSize(x: EditorWindow){
	
		if(tarS){
		
			x.maxSize = Vector2((tarS.levels[tarS.activeLevel].poceduralTextures.Count*45),200); 
			
			if(x.maxSize.x < 200){
				x.maxSize = Vector2(200,200); 
			}
			
			x.minSize = Vector2(x.maxSize.x-5,x.maxSize.y-5);
		}
	
	}
	
	function OpenFromButton(){
		
		butNr = -1;
		OnSelectionChange();
		SetSize(this);
		GetListOfTextures();
	}
   
   function OnSelectionChange(){
   		
   		if(Selection.activeTransform){
    	
	    	var script : InfiniTileGenerator = Selection.activeTransform.GetComponent(InfiniTileGenerator);
	    	
	    	if(script){
	    	
	    		obj = Selection.activeTransform;
	    		t 	= Selection.activeTransform.gameObject.name;
	    		tarS = script;
	    	} else {
	    	
	    		obj = null;
	    		tarS = null;
	    		t = "";
	    	
	    	}
    	}
    	else{
    	
    		obj = null;
	    	tarS = null;
    	
    	}
    	Repaint();
    	//GetListOfTextures();
    }
    

	
    function OnGUI () {
		
		
		
		if(obj && tarS){
		
			GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
			if (GUILayout.Button("Reset precedence")){
			
				GetListOfTextures();
			
			}
			GUILayout.Label ("Precedence:", EditorStyles.boldLabel);
			GUILayout.Label ("Drag textures to set precedence:", EditorStyles.miniLabel);
			GUILayout.Space (10);
			//var rect1: Rect = GUILayout.BeginHorizontal("box");
			var rect1 : Rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(70),GUILayout.MaxHeight(70),GUILayout.Width(tarS.levels[tarS.activeLevel].poceduralTextures.Count*42),GUILayout.MaxWidth(tarS.levels[tarS.activeLevel].poceduralTextures.Count*42));
			//GUI.Box( rect1, "") ;
			for(var x : int = 0 ; x < tarS.levels[tarS.activeLevel].poceduralTextures.Count ; x++){
				
				//GUI.DrawTexture(Rect(rect1.x,rect1.y+(60*x)+2,60,60), tarS.levels[tarS.activeLevel].poceduralTextures[x], ScaleMode.ScaleToFit, true, rect1.width/rect1.height);
				//EditorGUI.DrawPreviewTexture(Rect(rect1.x+(45*x)+2,rect1.y,40,40),tarS.levels[tarS.activeLevel].poceduralTextures[x]);
				if(x!=butNr){
					
					GUILayout.Label(tarS.levels[tarS.activeLevel].poceduralTextures[x].mainTexture, GUILayout.MaxWidth(40), GUILayout.MaxHeight(40));
				
				} 
				else {
					
					GUILayout.Label(" ", GUILayout.Width(40), GUILayout.Height(40), GUILayout.MaxWidth(40), GUILayout.MaxHeight(40));
				
				}
				
				if(Event.current.type != EventType.Layout){
				
					if (Event.current.type == EventType.MouseDrag && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && !dragging)  {
						
						butNr = x;
						areaOfOperation = rect1;
						dragging = true;
						//Debug.Log(Event.current.mousePosition+"sdfsdf     "+GUILayoutUtility.GetLastRect());
						startDragPos = Event.current.mousePosition;
						imgToDrag = tarS.levels[tarS.activeLevel].poceduralTextures[x];
						
						dragPoint = x;
						Repaint();
					}
				}
			}
			GUILayout.EndHorizontal();
			
			if(Event.current.type != EventType.Layout){
			
				if(dragging && imgToDrag !=null ){
					startDragPos.x += Event.current.delta.x;
					startDragPos.y += Event.current.delta.y;
					//Debug.Log("asdfadsf"+Event.current.delta+" x,y:"+(startDragPos.x+Event.current.delta.x));
					
					EditorGUI.DrawPreviewTexture(Rect(startDragPos.x,startDragPos.y,40,40),imgToDrag.mainTexture); //startDragPos.x+Event.current.delta.x,startDragPos.y+Event.current.delta.y
					//dragging = false;
					insertion = ((startDragPos.x-(4*(startDragPos.x/40))) / 40);
					
					Repaint();
				}
				if(Event.current.type == EventType.MouseUp){
					
					if(dragging && areaOfOperation.Contains(Event.current.mousePosition)){
						tarS.levels[tarS.activeLevel].poceduralTextures.RemoveAt(butNr);
						tarS.levels[tarS.activeLevel].poceduralTextures.Insert(insertion,imgToDrag);
					
					}
					
					butNr = -1;
					dragging = false;
					imgToDrag = null;
					areaOfOperation = Rect(0,0,0,0);
					Repaint();
				
				}
				if(dragging && !areaOfOperation.Contains(Event.current.mousePosition)){
				
					//Debug.Log("outside!");
					tarS.levels[tarS.activeLevel].poceduralTextures.RemoveAt(butNr);
					tarS.levels[tarS.activeLevel].poceduralTextures.Add(imgToDrag);
					butNr = -1;
					dragging = false;
					Repaint();
				
				}

			}
			
			if (GUILayout.Button("Update procedural textures")){
			
				
			
			}
			
			if (GUILayout.Button("reset dictionary")){

				var c : Dictionary.<String, Material> = tarS.levels[tarS.activeLevel].procTexDictionary;
				var s : String;
				if( c ){
					Debug.Log("Dictionary Count:"+tarS.levels[tarS.activeLevel].procTexDictionary.Count);
					
					for(var cKey : Material in c.Values) {
					
						//s = AssetDatabase.GetAssetPath(cKey.mainTexture);
						
						//AssetDatabase.DeleteAsset (s);
						
						//s = AssetDatabase.GetAssetPath(cKey);
						
						//AssetDatabase.DeleteAsset (s);
						
						//tarS.levels[tarS.activeLevel].procTexDictionary[d].DestroyImmediate(tarS.levels[tarS.activeLevel].procTexDictionary[d]);
					
					}
				}
				tarS.levels[tarS.activeLevel].procTexDictionary = new Dictionary.<String, Material>();
				
				Debug.Log("Dictionary cleaned. Ne Count:"+tarS.levels[tarS.activeLevel].procTexDictionary.Count);
			
			}
			
		} else {
		
			GUILayout.Label ("Select generator form scene", EditorStyles.boldLabel);
		}
	}

	
	function GetListOfTextures(){
	
		if(obj && tarS){
		
			//if(!tarS.levels[tarS.activeLevel].poceduralTextures){
			
				tarS.levels[tarS.activeLevel].poceduralTextures = new List.<Material>();
			
			//}
			//if(tarS.levels[tarS.activeLevel].poceduralTextures.Count == 0){
				
				for(var x : int = 0 ; x < tarS.levels[tarS.activeLevel].brushSetList.Count ; x++){
				
					for(var y : int = 0 ; y < tarS.levels[tarS.activeLevel].brushSetList[x].tilesGOs.Count ; y++){
					
						if(tarS.levels[tarS.activeLevel].brushSetList[x].tilesGOs[y] !=null){
							
							if(tarS.levels[tarS.activeLevel].brushSetList[x].tilesGOs[y].GetComponent.<Renderer>() && tarS.levels[tarS.activeLevel].brushSetList[x].tilesGOs[y].GetComponent.<Renderer>().sharedMaterial.mainTexture){
								
								var ind : boolean;
								
								var tx : Material = tarS.levels[tarS.activeLevel].brushSetList[x].tilesGOs[y].GetComponent.<Renderer>().sharedMaterial as Material;
								
								for(var z : int = 0 ; z < tarS.levels[tarS.activeLevel].poceduralTextures.Count ; z++){
								
									if(tx == tarS.levels[tarS.activeLevel].poceduralTextures[z]){
									
										ind = true;
									
									}
								
								}
								if(ind == false){
								
									tarS.levels[tarS.activeLevel].poceduralTextures.Add(tx);
								
								}
							}
						}
					}
				
				}
			//} 
		
		}
	
	}
	
}

