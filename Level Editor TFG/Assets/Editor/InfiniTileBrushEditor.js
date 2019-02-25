#pragma strict
/*
		T I L E  E D I T O R   F O R   U N I T Y 3 D
				  Made by Wojciech Gabel
				  	wgabel34@gmail.com
						
					dev. state : 0.67

*/

class InfiniTileBrushEditor extends EditorWindow {

	var obj : Transform;
	
	var tarS : InfiniTileGenerator;

	var mytext : String;
	
	var objt : Object;
	
	var draggedObj : GameObject;

	var dragging : boolean;
	
	var mousePos : Vector2;
	
	var startDragPos : Vector2;
	
	var tex : Texture2D;
	
	var lastRect : Rect;
	
	var lastSet : int;
	
	var waitForUpdate : boolean;
	
	var stopDrag :boolean;
	
	var send : boolean;
	
	@MenuItem ("Window/InfiniTile/Brush Editor")
	
	static function Init () {
		
		var windowz : InfiniTileBrushEditor = EditorWindow.GetWindow(InfiniTileBrushEditor,false) as InfiniTileBrushEditor;
		windowz.Show();
		windowz.OnSelectionChange();
		
	}
	
	function OpenFromButton(){
	
		OnSelectionChange();
		//SetSize(this);
		//GetListOfTextures();
	}
	
	function OnSelectionChange(){
   		
   		if(Selection.activeTransform){
    	
	    	var script : InfiniTileGenerator = Selection.activeTransform.GetComponent(InfiniTileGenerator);
	    	
	    	if(script){
	    	
	    		obj = Selection.activeTransform;
	    	
	    		tarS = script;
	    		
	    	} else {
	    	
	    		obj = null;
	    		
	    		tarS = null;
	    	}
    	}
    	
    	else{
    	
    		obj = null;
    		
	    	tarS = null;
    	
    	}
    	
    	this.Repaint();
    }
	
	function OnInspectorUpdate()
	{
    	// This will only get called 10 times per second.
   	 	this.Repaint();
   	 	
	}
	
	function OnGUI () {
	//Debug.Log(Event.current.type);
	

			//get what are we dragging. If this thing is something, then start drag:
			if (Event.current.type == EventType.DragUpdated || Event.current.type  == EventType.DragPerform )  {
			
				if(!draggedObj &&  DragAndDrop.objectReferences[0] && DragAndDrop.objectReferences[0].GetType() == GameObject && !dragging){
			 							
		 			draggedObj = DragAndDrop.objectReferences[0] as GameObject;
		 			startDragPos = Event.current.mousePosition;
		 			
		 			if(draggedObj.transform.GetComponent.<Renderer>() && draggedObj.transform.GetComponent.<Renderer>().sharedMaterial.mainTexture){
		 			
		 				dragging = true;
		 			}
		 		} 
			} 
			
			// adjust position of dragged object:
			if(dragging){
				if(Event.current.mousePosition.x != (-position.x) ){
					
					startDragPos = Event.current.mousePosition; 
				
				}
				
			} else {
			
				stopDrag = false;
				draggedObj = null;
			
			}
			//if drag has stopped, update:
			if(dragging){
				if( Event.current.type == EventType.DragExited || Event.current.type == EventType.MouseUp){
				
					if(stopDrag == true){
						//Debug.Log("Ending drag");
						send = true;
						
					} else {
						//Debug.Log("Ending drag. stopDrag is false. Aborting");
						stopDrag = false;
						draggedObj = null;
						dragging = false;
						//reset everything
					}
				
				}
			}
		
			if(Event.current.type == EventType.Layout && stopDrag == true && dragging == true && send == true){
				//update brushset and reset:
				tarS.levels[tarS.activeLevel].changeBrushSetContents(lastSet,tarS.levels[tarS.activeLevel].brushSetList[lastSet].tilesGOs.Count,draggedObj);
				dragging = false;
				stopDrag = false;
				send = false;
				draggedObj = null;
			
			}

		if(tarS){
		
			if(!tarS.levels[tarS.activeLevel].brushSetList || tarS.levels[tarS.activeLevel].brushSetList.Count  <= 0 ){
			
				var message : Rect = EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));

							GUILayout.Box("No brushes in this level",GUILayout.MinWidth(120),GUILayout.MaxWidth(120));

					EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));

				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));
						if(GUILayout.Button("Add set",GUILayout.MinWidth(70),GUILayout.MaxWidth(70))){
						
							//add a brush to brushset
							tarS.levels[tarS.activeLevel].createBrushSet();
							EditorUtility.SetDirty(tarS);
						
						}
				EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));
				EditorGUILayout.EndHorizontal();
			}
			
			else 
			
			{
			EditorGUILayout.BeginVertical ();
				var mainArea : Rect = EditorGUILayout.BeginHorizontal ();
					
				EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));
					
					for(var b : int = 0 ; b < tarS.levels[tarS.activeLevel].brushSetList.Count ; b++){
						
						var Main : Rect = EditorGUILayout.BeginVertical(GUILayout.MaxWidth(120),GUILayout.MaxHeight(120));
						
							EditorGUILayout.BeginHorizontal( GUILayout.MaxWidth(Main.width));
								GUILayout.Label("Name:",GUILayout.MaxWidth(45));
								tarS.levels[tarS.activeLevel].brushSetList[b].name = EditorGUILayout.TextField(tarS.levels[tarS.activeLevel].brushSetList[b].name);
							EditorGUILayout.EndHorizontal();
						
						var mainBrush : Rect = EditorGUILayout.BeginVertical ("box",GUILayout.MinHeight(120),GUILayout.MaxHeight(120));	
						
						EditorGUILayout.LabelField(" ",GUILayout.MinWidth(1));
						
								if(tarS.levels[tarS.activeLevel].brushSetList[b].tilesGOs.Count == 0 && Event.current.type == EventType.Repaint ){
									
									 GUI.Box(Rect(mainBrush.x+5,(mainBrush.y)+5,mainBrush.width-10,20),"empty");
									
								}
								
								

									for(var t : int = 0 ; t < Mathf.Ceil (tarS.levels[tarS.activeLevel].brushSetList[b].tilesGOs.Count/4.0f) ; t++){
										
										EditorGUILayout.BeginHorizontal();
										
										for(var t1 : int = 0 ; t1 < 4; t1++){
											
											if((t*4)+t1 < tarS.levels[tarS.activeLevel].brushSetList[b].tilesGOs.Count){
											
												if(tarS.levels[tarS.activeLevel].brushSetList[b].tilesGOs[(t*4)+t1] !=null && tarS.levels[tarS.activeLevel].brushSetList[b].tilesGOs[(t*4)+t1].GetComponent.<Renderer>()){
												
													GUILayout.Label(tarS.levels[tarS.activeLevel].brushSetList[b].tilesGOs[(t*4)+t1].GetComponent.<Renderer>().sharedMaterial.mainTexture, GUILayout.MaxWidth(40), GUILayout.MaxHeight(40));
		
												}
												//add a delete button
												if(GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)){
													
													if(GUI.Button(GUILayoutUtility.GetLastRect(),"delete",EditorStyles.textField)){
														
														tarS.levels[tarS.activeLevel].changeBrushSetContents(b,(t*4)+t1,tarS.levels[tarS.activeLevel].brushSetList[b].tilesGOs[(t*4)+t1]);

													}
												}
											
											}
										}
										
										EditorGUILayout.EndHorizontal();
									
									}
									GUILayout.Space (10);
									
									EditorGUILayout.HelpBox ("\n Drag a tile object here to add \n",MessageType.None);
								
									if(Event.current.type == EventType.Repaint || Event.current.type == EventType.DragUpdated){
										if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && dragging )  {
													
													GUI.Box(GUILayoutUtility.GetLastRect(), "release here",EditorStyles.objectFieldThumb );
													DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
													
													if(stopDrag == false && dragging == true){
														
														lastSet = b;
						 							
						 								lastRect = GUILayoutUtility.GetLastRect();
						 							
						 								stopDrag = true;
						 							
						 							} else if( lastSet != b){
						 							
						 								stopDrag = false;
						 							
						 							}
	
										} else if(lastSet == b)  {
											
											stopDrag = false;

										}
									
									}
									
									
									if(dragging){
									
										EditorGUI.DrawPreviewTexture(Rect(startDragPos.x,startDragPos.y,40,40),draggedObj.transform.GetComponent.<Renderer>().sharedMaterial.mainTexture);
										
										this.Repaint();

									}


						
						EditorGUILayout.EndVertical();
						
						EditorGUILayout.EndVertical();
						
					}
					
				EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));
				
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));
				if(GUILayout.Button("add set",GUILayout.MaxWidth(70))){
						
					tarS.levels[tarS.activeLevel].createBrushSet();
				}
				if(GUILayout.Button("delete set",GUILayout.MaxWidth(70))){
				
					tarS.levels[tarS.activeLevel].deleteBrushSet( tarS.levels[tarS.activeLevel].brushSetList.Count-1);
					tarS.levels[tarS.activeLevel].changeActiveBrushSet(0,tarS.activeLevel,tarS);
				
				}
				EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));
				EditorGUILayout.EndHorizontal();
				
			EditorGUILayout.EndVertical ();
			}
			
		}
		
//		if(GUILayout.Button("reset")){
//			
//			dragging = false;
//			draggedObj = null;
//			stopDrag = false;
//			lastRect = Rect(0,0,0,0);
//			
//		}

	}
	
//	 function OnInspectorUpdate() {
//        if(EditorWindow.mouseOverWindow){ 
//        	EditorWindow.mouseOverWindow.Focus();
//        	counter++;
//        }
//       
//        //this.Repaint();
//    }
 
}
