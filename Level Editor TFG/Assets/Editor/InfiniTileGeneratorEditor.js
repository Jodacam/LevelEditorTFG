#pragma strict
#pragma downcast
import System.Collections.Generic;

/*
		T I L E  E D I T O R   F O R   U N I T Y 3 D
				  Made by Wojciech Gabel
				  	wgabel34@gmail.com
						
					dev. state : 0.67

*/


@CustomEditor (InfiniTileGenerator)

class InfiniTileGeneratorEditor extends Editor {
	#if UNITY_EDITOR
	private var editSwitch : boolean;
	private var styleCopy : GUIStyle;
	private var st: GUIStyle;
	private var st2: GUIStyle;
	
	private var tarS : InfiniTileGenerator;	
	
	private var tileSize : float;
	
	private var lvlHeight : float;
	
	private var absoluteLvlHeight : float;
	
	private var tgleGrid : boolean;
	
	private var waitForUpdate : boolean;
	
	private var iconTextures : Texture2D[]; //8 length
	private var logo : Texture2D;
	private var logoB : Texture2D;
	private var logoC : Texture2D;
	private var deleteTrigger : boolean;
	private var deleterPos : int;
	
	private var tMergeTex : Texture2D;

	private var rowsTemp : int;
	private var rows : int;
	var mm : Material;
	
	var mainState : int;
	
	var helpMessa: String;
	
	var scrollPos : Vector2; //scroll view for wallSets
	var scrollPos2 : Vector2; // scroll view for level control
	
	var e : Event;
	
#endif 
	
// Initialisation when selected:----------------------------------------------------
	function Awake() {
		#if UNITY_EDITOR	
		//Load textures for gui:
		mm = EditorGUIUtility.Load("InfiniTile/mmMat.mat");

		iconTextures = new Texture2D[14];
		iconTextures[0] = EditorGUIUtility.Load("InfiniTile/ic0.png");
		iconTextures[1] = EditorGUIUtility.Load("InfiniTile/ic1.png");
		iconTextures[2] = EditorGUIUtility.Load("InfiniTile/ic2.png");
		iconTextures[3] = EditorGUIUtility.Load("InfiniTile/ic3.png");
		iconTextures[4] = EditorGUIUtility.Load("InfiniTile/ic4.png");
		iconTextures[5] = EditorGUIUtility.Load("InfiniTile/ic5.png");
		iconTextures[6] = EditorGUIUtility.Load("InfiniTile/ic6.png");
		iconTextures[7] = EditorGUIUtility.Load("InfiniTile/ic7.png");
		iconTextures[12] = EditorGUIUtility.Load("InfiniTile/ic8.png");
		iconTextures[13] = EditorGUIUtility.Load("InfiniTile/ic9.png");
		iconTextures[8] = EditorGUIUtility.Load("InfiniTile/iconRd1.png"); // this and next textures are for button icons
		iconTextures[9] = EditorGUIUtility.Load("InfiniTile/iconRd1Off.png");
		iconTextures[10] = EditorGUIUtility.Load("InfiniTile/iconRd2.png");
		iconTextures[11] = EditorGUIUtility.Load("InfiniTile/iconCon1.png");
		logo = EditorGUIUtility.Load("InfiniTile/logo.gif");
		logoB = EditorGUIUtility.Load("InfiniTile/logoBack.gif");
		logoC = EditorGUIUtility.Load("InfiniTile/ShSmLo.png");
		
		tMergeTex  = EditorGUIUtility.Load("InfiniTile/modules/InfiniTile_TextureMerge/template1.png");
		
		deleterPos = -1;
		// Load script:	 
		if(!tarS) {
			
			tarS = target as InfiniTileGenerator;
			
			if(tarS.levels.Count <=0  ) {
				
				var tl: TGLevel = new TGLevel();
				
				tl.AddLevel( 0 , tarS.levels , tarS.transform, "0" );		
				
				tarS.levels[0].setActiveLevel(0, tarS.levels);
				tarS.activeLevel = 0;
					
				UpdateVariables(tarS.activeLevel);
				EditorUtility.SetDirty(tarS);
				
			} 
		}
		else {
			//Debug.Log("somehow, tarS is already here...");
			
			tarS.levels[0].setActiveLevel(0, tarS.levels);
			tarS.activeLevel = 0;
					
			UpdateVariables(tarS.activeLevel);
			EditorUtility.SetDirty(tarS);
			
		}
		
		tarS.tMergeTex = tMergeTex;
		#endif  
	}
	
	
	function UpdateVariables(lvlNr : int) {
	
		tileSize = tarS.levels[tarS.activeLevel].tileSize;
		lvlHeight = tarS.levels[tarS.activeLevel].lvlHeight;
		tarS.levels[tarS.activeLevel].CheckModules();
	}
	
	
	function SendVariablesToScript(lvlNr : int){
	
		tarS.levels[tarS.activeLevel].tileSize = tileSize;
		tarS.levels[tarS.activeLevel].lvlHeight = lvlHeight;
	}
//----------------------------------------------------------------------------------
//Draw the Thing!:------------------------------------------------------------------
	override function OnInspectorGUI () {
		#if UNITY_EDITOR	
		//handle for deleting levels:
		if(deleterPos == 0 && deleteTrigger == false) {
			
			deleterPos = -1;
			
		}
		//load custom style:
//		if(styleCopy == null) {
//
//			styleCopy = GUI.skin.customStyles[256];
//			styleCopy.overflow = new RectOffset(2,0,2,0);
//			
//
//		} if(!st) {
//			st= new GUIStyle (EditorStyles.miniButton);
//			st.contentOffset.x = -2;
//			st.clipping = TextClipping.Overflow;
//			tarS.tStyle = st;
//			
//			st2 = new GUIStyle (GUI.skin.customStyles[142]);
//			st2.contentOffset.x = -2;
//			st2.clipping = TextClipping.Overflow;
//			st2.padding = RectOffset(2,2,2,2);
//			st2.margin = RectOffset(4,4,2,2);
//			st2.font = st.font;
//			tarS.t2Style = st2;
//
//		}
		
		if(tMergeTex == null || tarS.tMergeTex == null){
			
			tMergeTex  = EditorGUIUtility.Load("InfiniTile/modules/InfiniTile_TextureMerge/template1.png");

			tarS.tMergeTex = tMergeTex;
		
		}
		 
		DrawDefaultInspector();
		
// check if generator is at 0 y:

	if(tarS.gameObject.transform.localPosition.y !=0 ){
		
		tarS.gameObject.transform.localPosition.y = 0;
		
	}
			
//display help Box if something is not right!
		
		
//Info and main buttons:
		var rectWSLo0 : Rect = EditorGUILayout.BeginHorizontal ("Button",GUILayout.Height(27)); // bh2
		
			EditorGUI.DrawPreviewTexture(Rect(rectWSLo0.x,rectWSLo0.y-2,logoC.width,logoC.height),logoC,mm);
			EditorGUILayout.LabelField("Tools:",EditorStyles.boldLabel);
			
		EditorGUILayout.EndHorizontal (); //end bh2
		
//Main Buttons Area:----------------------------------------------------
		
		var rMain : Rect = EditorGUILayout.BeginHorizontal (); // bh3
			
			EditorGUILayout.LabelField(" ",GUILayout.MinWidth(32));
			
			var rMainV : Rect = EditorGUILayout.BeginHorizontal (); // bh4

			    if(tarS){
					
			    	tarS.editing = GUILayout.Toggle(tarS.editing,"Paint","Button",GUILayout.Height(40),GUILayout.Width(40) );
			    		if(tarS.editing){
							
								tarS.cleaning = !tarS.editing;
								
							}
			    	EditorGUILayout.Space();
			    	
			    	tarS.cleaning = GUILayout.Toggle(tarS.cleaning,"delete","Button",GUILayout.Height(40),GUILayout.Width(50) );
					
							if(tarS.cleaning){
							
								tarS.editing = !tarS.cleaning;
								
							}
			    	
			    }
			EditorGUILayout.EndHorizontal(); // end bh4

			EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));
		
		EditorGUILayout.EndHorizontal(); // end bh3
		EditorGUILayout.Space();
		EditorGUILayout.Space();

//Levels Control:----------------------------------------------------------

	var rectWSLo3 : Rect = EditorGUILayout.BeginHorizontal ("Button",GUILayout.Height(27)); // bh5
	
		EditorGUI.DrawPreviewTexture(Rect(rectWSLo3.x,rectWSLo3.y-2,logoC.width,logoC.height),logoC,mm);
		EditorGUILayout.LabelField("Elevation:",EditorStyles.boldLabel);
	
	EditorGUILayout.EndHorizontal (); // end bh5
	
// selected level options:	

	EditorGUILayout.BeginVertical ("helpBox"); // bv5.5
		
		EditorGUILayout.LabelField("Selected level options:",EditorStyles.miniBoldLabel);
		
		EditorGUILayout.BeginHorizontal (); // bh6
		
			if(GUILayout.Button("add highier",EditorStyles.miniButton)) {
		
				tarS.levels[tarS.activeLevel].AddLevel( tarS.activeLevel , tarS.levels , tarS.transform, "lower" );		
				tarS.activeLevel++;		
			}
			
			if(GUILayout.Button("add lower",EditorStyles.miniButton)) {
	
				tarS.levels[tarS.activeLevel].AddLevel( tarS.activeLevel , tarS.levels , tarS.transform, "higher" );						
			}
			
			if(GUILayout.Button("regenerate",EditorStyles.miniButton)) {
			
				tarS.levels[tarS.activeLevel].regenerateLevel( tarS.levels[tarS.activeLevel] , tarS.gameObject);
			}
			
			if(GUILayout.Button("clean",EditorStyles.miniButton)) {
				
				tarS.CleanOneLevel(tarS.activeLevel);
				UpdateVariables(tarS.activeLevel);
				EditorUtility.SetDirty(tarS);							
			}
			
		EditorGUILayout.EndHorizontal (); // end bh6
		
		EditorGUILayout.Space();
		
		EditorGUILayout.BeginHorizontal(); // bh7
		
			EditorGUILayout.LabelField(" ",GUILayout.MinWidth(12));
				GUILayout.Label("TileSize:");
				tarS.levels[tarS.activeLevel].tileSize = EditorGUILayout.FloatField("", tarS.levels[tarS.activeLevel].tileSize,GUILayout.MaxWidth(35));
				GUILayout.Label("Level Height:");
				EditorGUI.BeginChangeCheck ();
				tarS.levels[tarS.activeLevel].lvlHeight = EditorGUILayout.FloatField("", tarS.levels[tarS.activeLevel].lvlHeight,GUILayout.MaxWidth(35));
				if (EditorGUI.EndChangeCheck ()) {
				
					//change all absolute heights
					tarS.levels[0].setAbsoluteHeights(tarS.levels);
					tarS.levels[0].moveLevels(tarS.activeLevel,tarS.levels,"change");
				
				}
			EditorGUILayout.LabelField(" ",GUILayout.MinWidth(32));
		
		EditorGUILayout.EndHorizontal(); // end bh7
		
	EditorGUILayout.EndVertical (); // end bv 5.5

// level buttons:
			
	scrollPos2 =  EditorGUILayout.BeginScrollView(scrollPos2,GUILayout.Height(125)); //sc1
	
		EditorGUILayout.Space();
		EditorGUILayout.Space();
	
		EditorGUILayout.BeginHorizontal (); // bh8
	
			EditorGUILayout.LabelField(" ",GUILayout.MinWidth(32));
	
			for(var ll : int = 0 ; ll< tarS.levels.Count ; ll++) { 
	
				var tgeLvll : boolean = tarS.levels[ll].isActive;
			
				EditorGUILayout.BeginVertical (); // bh9
			
					EditorGUI.BeginChangeCheck ();
				
						tgeLvll = GUILayout.Toggle(tgeLvll,tarS.levels[ll].lvlNr.ToString(),"button",GUILayout.MaxWidth(45) );
				
					if (EditorGUI.EndChangeCheck ()) {
				
						tarS.levels[ll].setActiveLevel(ll, tarS.levels);
						tarS.activeLevel = ll;
						
						if(tarS.levels[ll].brushSetList.Count > 0){
							
							tarS.levels[ll].changeActiveBrushSet(0,tarS.activeLevel,tarS);
						
						}
						
						UpdateVariables(tarS.activeLevel);
						EditorUtility.SetDirty(tarS);
						waitForUpdate = true;
					}
				
					if(GUILayout.Button("delete",EditorStyles.miniButton,GUILayout.MaxWidth(45))) {
					
						deleteTrigger = true;
						deleterPos = ll;	
					}
				
					if(deleteTrigger == true && deleterPos == ll) {
					
						if(GUILayout.Button("ok",EditorStyles.miniButton,GUILayout.MaxWidth(45))) {

			    			tarS.levels[tarS.activeLevel].DeleteLevel(ll,tarS.levels[ll].lvlNr, tarS.levels, tarS);

			    			EditorUtility.SetDirty(tarS);
			    			waitForUpdate = true;
			    			deleteTrigger = false;
			    			deleterPos = -1;
						}
						
						if(GUILayout.Button("cancel",EditorStyles.miniButton,GUILayout.MaxWidth(45))) {
						
							deleteTrigger = false;
							deleterPos = -1;
						}
					}
			
				EditorGUILayout.EndVertical (); // end bh9
			}
		
			EditorGUILayout.LabelField(" ",GUILayout.MinWidth(32));
		
		EditorGUILayout.EndHorizontal (); // end bh8
	
	EditorGUILayout.EndScrollView(); //end sv1
	
	EditorGUILayout.Space();

//-------------------------------------------------------------------------
//Brush Sets Main Control:-------------------------------------------------	

	var rectWSLo1 : Rect = EditorGUILayout.BeginHorizontal ("Button",GUILayout.Height(16)); //bh10
		EditorGUI.DrawPreviewTexture(Rect(rectWSLo1.x,rectWSLo1.y-2,logoC.width,logoC.height),logoC,mm);
		EditorGUILayout.BeginHorizontal (GUILayout.Height(24),GUILayout.Height(16)); // bh11
			GUILayout.Label("Ground tiles:",EditorStyles.boldLabel,GUILayout.Height(16));
	
	//open brusheditor:
	
			if (GUILayout.Button("open brush editor"))
			    {  
			       var windowz : InfiniTileBrushEditor = EditorWindow.GetWindow(InfiniTileBrushEditor,false);
			       windowz.OpenFromButton();
			    }
	
//	//Add A brush set:
//			if(GUILayout.Button("+",GUILayout.Width(24),GUILayout.Height(16))) {	
//				if(tarS) {
//					tarS.levels[tarS.activeLevel].createBrushSet();
//					waitForUpdate = true;
//					EditorUtility.SetDirty(tarS);
//				}
//			}
//	//Delete A brush set:
//			if(GUILayout.Button("-",GUILayout.Width(24),GUILayout.Height(16))) {
//				if(tarS) {
//					tarS.levels[tarS.activeLevel].deleteBrushSet( tarS.levels[tarS.activeLevel].brushSetList.Count-1);
//					waitForUpdate = true;
//					EditorUtility.SetDirty(tarS);
//				}
//			}	
		EditorGUILayout.EndHorizontal (); // end bh11	
		
		
	EditorGUILayout.EndHorizontal (); //end bh10
		
		//Draw the brush sets, and check what is done with them:
	if(tarS && tarS.levels[tarS.activeLevel].brushSetList.Count == 0){
		
		
		EditorGUILayout.BeginHorizontal ();
			GUILayout.Label(" ",EditorStyles.boldLabel,GUILayout.Height(16));
				GUILayout.Box("No brush set. Add a set in brush editor",GUILayout.Height(20));
			GUILayout.Label(" ",EditorStyles.boldLabel,GUILayout.Height(16));
		EditorGUILayout.EndHorizontal ();
		
	
	}
	
	if(waitForUpdate == false) {

		//var nrTiles : int = 0;
		
		for(var t : int = 0 ; t< tarS.levels[tarS.activeLevel].brushSetList.Count; t++) {	
			
			var mainT : Rect = EditorGUILayout.BeginHorizontal ("box");//whole container: //bh 12		
				
				var name : String;
				
				var tgleTile : boolean;	
				
				var state : String;
				
				if(tarS.levels[tarS.activeLevel].brushSetList[t]) {	

					if(tarS.levels[tarS.activeLevel].brushSetList[t].name != ""){
						
						name = tarS.levels[tarS.activeLevel].brushSetList[t].name;
						
					} else {
							
						name = "no name";
						
					}
					
					if(tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count > 0) {
						
						tgleTile = tarS.levels[tarS.activeLevel].brushSetList[t].isActive; 
						
					} 
					
					EditorGUILayout.BeginVertical (GUILayout.MaxWidth(104)); //bv13
						
						EditorGUI.BeginChangeCheck ();	
							tgleTile = GUILayout.Toggle(tgleTile,name,"Button",GUILayout.MaxWidth(104),GUILayout.Height(45));
						if (EditorGUI.EndChangeCheck ()) {
							tarS.ChangeBrushSet(t);
							tarS.levels[tarS.activeLevel].changeActiveBrushSet(t,tarS.activeLevel,tarS);
						}
						
						var rIco1 : Rect = EditorGUILayout.BeginHorizontal();// bh14
							tarS.levels[tarS.activeLevel].brushSetList[t].random = GUILayout.Toggle( tarS.levels[tarS.activeLevel].brushSetList[t].random," ",tarS.t2Style,GUILayout.Width(32),GUILayout.Height(32),GUILayout.MaxWidth(32));
							tarS.levels[tarS.activeLevel].brushSetList[t].randomTileRot = GUILayout.Toggle( tarS.levels[tarS.activeLevel].brushSetList[t].randomTileRot," ",tarS.t2Style,GUILayout.Width(32),GUILayout.Height(32),GUILayout.MaxWidth(32));
							//tarS.levels[tarS.activeLevel].brushSetList[t].conTex = GUILayout.Toggle( tarS.levels[tarS.activeLevel].brushSetList[t].conTex," ",tarS.t2Style,GUILayout.Width(32),GUILayout.Height(32),GUILayout.MaxWidth(32));
							var nr : int; 
							if(tarS.levels[tarS.activeLevel].brushSetList[t].random == true) {
								nr = 8;
							} else {
								nr = 9;
							}
							EditorGUI.DrawPreviewTexture(Rect(rIco1.x,rIco1.y,32,32),iconTextures[nr],mm);
							EditorGUI.DrawPreviewTexture(Rect(rIco1.x+32+4,rIco1.y,32,32),iconTextures[10],mm);
							//EditorGUI.DrawPreviewTexture(Rect(rIco1.x+64+7,rIco1.y-1,32,32),iconTextures[11],mm);	
						EditorGUILayout.EndHorizontal(); // end bh14	
					
					EditorGUILayout.EndVertical (); //end bv13
					
					if(Event.current.type == EventType.Repaint && tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count >0){
					
						var rowsTempT = Mathf.Ceil (tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count/5.0f);
						
						if(rowsTempT > rowsTemp){
						
							rowsTemp = rowsTempT;
						}
					}
					if(Event.current.type == EventType.Layout) {

							rows = rowsTemp;
					}
					
					EditorGUILayout.BeginVertical();
					
						for(var t0 : int = 0 ; t0 < rows ; t0++){
							
							EditorGUILayout.BeginHorizontal();
								
								for(var t1 : int = 0 ; t1 < 5; t1++){
								
									if((t0*5)+t1 < tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count){
									
										if(tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs[(t0*5)+t1] !=null && tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs[(t0*5)+t1].GetComponent.<Renderer>()){
											
											if(tarS.levels[tarS.activeLevel].activeBrushSet == t && tarS.levels[tarS.activeLevel].activeTile == ((t0*5)+t1) ){
											
												state = "box";
												
											} else{
											
												state = "button";
											
											}
											
											if(GUILayout.Button(tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs[(t0*5)+t1].GetComponent.<Renderer>().sharedMaterial.mainTexture,state,GUILayout.Width(40),GUILayout.Height(40), GUILayout.MaxWidth(40), GUILayout.MaxHeight(40))){

												tarS.levels[tarS.activeLevel].activeTile = ((t0*5)+t1);
												
												if(tarS.levels[tarS.activeLevel].activeBrushSet != t){
												
													tarS.levels[tarS.activeLevel].changeActiveBrushSet(t,tarS.activeLevel,tarS);
												
												}
											}
										}										
									}							
								}
								
							EditorGUILayout.EndHorizontal();
						}
						
					EditorGUILayout.EndVertical();
					
//					EditorGUILayout.BeginVertical ("Button"); //small buttons container: // bv15
//						//adding/deleting tiles:
//						if(tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs !=null) {
//							var tgCount: int = tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count;
//							switch (tgCount) {
//								case 0: // empty , add one empty
//									tgCount = tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count;
//									break;
//								default:
//									break;
//							}
//							for(var tg : int = 0 ; tg < tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count ; tg++) {//tg < tGOL[tarS.activeLevel][t].Count
//								//switch here. If there is empty , check if last, if last then leave, if not, then remove from list.
//								switch(tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs[tg]) {
//									case null:
//										if(tg < tgCount-1) { //this tile is not last:
//											tgCount = tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count;
//										}
//										break;
//									default:
//										if(tg == tgCount-1){ // this tile is last:
//											if(tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs[tg] != null) {
//												tgCount = tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs.Count;
//											}
//										}	
//										nrTiles ++;
//										break;
//								}
//									EditorGUI.BeginChangeCheck ();
//										tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs[tg] = EditorGUILayout.ObjectField(tarS.levels[tarS.activeLevel].brushSetList[t].tilesGOs[tg],GameObject,true);
//									if (EditorGUI.EndChangeCheck ()) {
//										tarS.levels[tarS.activeLevel].changeBrushSetContents(t,tg, null);
//									}
//							}
//						}
//						EditorGUILayout.EndVertical(); // end bv15
					
				
				}  
				EditorGUILayout.EndHorizontal(); // end bh 12
			} // for t
			
//			if(nrTiles != tarS.levels[tarS.activeLevel].allBrushTilesNr) {
//				tarS.levels[tarS.activeLevel].allBrushTilesNr = nrTiles;
//			}
			//EditorGUILayout.EndVertical();
		} // waitforupdate==false
		
//// VERTICAL TILES control--------------------------------------------:
		EditorGUILayout.Space();	
		
		var rectWSLo4 : Rect = EditorGUILayout.BeginHorizontal ("Button",GUILayout.Height(27));
		EditorGUI.DrawPreviewTexture(Rect(rectWSLo4.x,rectWSLo4.y-2,logoC.width,logoC.height),logoC,mm);
		var rWS : Rect = EditorGUILayout.BeginHorizontal ("Label");
		EditorGUILayout.LabelField("Wall tiles:",EditorStyles.boldLabel);

	//add one wall:	
		if(GUILayout.Button("+",GUILayout.Width(24),GUILayout.Height(16))) {
			if(tarS) {
			
				tarS.levels[tarS.activeLevel].createWall();
				EditorUtility.SetDirty(tarS);
				waitForUpdate = true;
				
			}
		}
	//delete one wall:
		if(GUILayout.Button("-",GUILayout.Width(24),GUILayout.Height(16))) {
			if(tarS) {
			
				tarS.levels[tarS.activeLevel].deleteWall(tarS.levels[tarS.activeLevel].wallsSetList.Count-1);
				EditorUtility.SetDirty(tarS);
				waitForUpdate = true;
			
			}
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndHorizontal ();
		
		if(tarS && tarS.levels[tarS.activeLevel].wallsSetList.Count == 0){

			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label(" ",EditorStyles.boldLabel,GUILayout.Height(16));
					GUILayout.Box("No wall set. Add one",GUILayout.Height(20));
				GUILayout.Label(" ",EditorStyles.boldLabel,GUILayout.Height(16));
			EditorGUILayout.EndHorizontal ();

		}
		
		
		var tgleSet : boolean;
		if(waitForUpdate == false) {
		
		if(tarS.levels[tarS.activeLevel].wallsSetList.Count > 0) {
		
		scrollPos =  EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));
		
		var rectWS : Rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(370),GUILayout.MaxHeight(370));
			//icons first:
			
//			EditorGUILayout.BeginVertical("Button",GUILayout.Width(45),GUILayout.MinWidth(45),GUILayout.MaxWidth(45));
//				for(var ic : int = 0 ; ic<8 ; ic++) {
//					EditorGUI.DrawPreviewTexture(Rect(rectWS.x,rectWS.y+(45*ic)+2,40,40),iconTextures[ic]);
//				}
//			EditorGUILayout.EndVertical();
			
//			EditorGUILayout.BeginVertical (GUILayout.Width(30),GUILayout.MinWidth(30),GUILayout.MaxWidth(30));
//				EditorGUILayout.LabelField("",GUILayout.Width(30),GUILayout.MinWidth(30),GUILayout.MaxWidth(30));
//			EditorGUILayout.EndVertical();
			
				for(var ws : int = 0 ; ws < tarS.levels[tarS.activeLevel].wallsSetList.Count ; ws++) {
				EditorGUILayout.BeginVertical("Button",GUILayout.MinWidth(90),GUILayout.MaxWidth(350));

					for(var bs : int = 0 ; bs < 10 ; bs++) {
						
						EditorGUILayout.BeginVertical (GUILayout.Height(41.5));
							
							EditorGUILayout.BeginHorizontal();
								if(bs<8){
								
									GUILayout.Box(iconTextures[bs]);
									
								}
								else if(bs==8){
									GUILayout.Box(iconTextures[12]);
								}
								else if(bs==9){
									GUILayout.Box(iconTextures[13]);
								}
								tarS.levels[tarS.activeLevel].wallsSetList[ws].tilesGOs[bs] = EditorGUILayout.ObjectField(tarS.levels[tarS.activeLevel].wallsSetList[ws].tilesGOs[bs],GameObject,true);
							EditorGUILayout.EndHorizontal();	
							
							if(bs == 9){
							
								tarS.levels[tarS.activeLevel].wallsSetList[ws].fold2 = EditorGUILayout.Foldout(tarS.levels[tarS.activeLevel].wallsSetList[ws].fold2, "Connect to:");
					
								if(tarS.levels[tarS.activeLevel].wallsSetList[ws].fold2) {
						
									EditorGUILayout.BeginHorizontal (GUILayout.MaxHeight(20));
										
										for(var co : int = 0 ; co < tarS.levels[tarS.activeLevel].wallsSetList[ws].conBools.Count ; co++){
											
											if(co == ws && tarS.levels[tarS.activeLevel].wallsSetList[ws].conBools[co] == true){
											
												tarS.levels[tarS.activeLevel].wallsSetList[ws].conBools[co] = false;
												
											}
											
											tarS.levels[tarS.activeLevel].wallsSetList[ws].conBools[co] = EditorGUILayout.Toggle(tarS.levels[tarS.activeLevel].wallsSetList[ws].conBools[co],GUILayout.Width(10));
										}
										EditorGUILayout.LabelField("");
									EditorGUILayout.EndHorizontal ();
								}
							
							}
							
						EditorGUILayout.EndVertical();
					}
					
					
					
					EditorGUILayout.LabelField("Options:",EditorStyles.miniBoldLabel);
					EditorGUILayout.BeginHorizontal ("Button",GUILayout.MaxHeight(2));
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginVertical (GUILayout.Height(41.5));

						EditorGUILayout.BeginHorizontal();
							GUILayout.Label("group:");
							
							EditorGUI.BeginChangeCheck ();
							
								tarS.levels[tarS.activeLevel].wallsSetList[ws].grNr = EditorGUILayout.IntField("", tarS.levels[tarS.activeLevel].wallsSetList[ws].grNr,GUILayout.MaxWidth(35));
						
							
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.LabelField("Strength:",EditorStyles.miniLabel);
						
						 tarS.levels[tarS.activeLevel].wallsSetList[ws].strength =  GUILayout.HorizontalSlider(tarS.levels[tarS.activeLevel].wallsSetList[ws].strength, 0, 1);
						EditorGUILayout.BeginHorizontal("Label");
						
						if (EditorGUI.EndChangeCheck ()) {
								tarS.levels[tarS.activeLevel].repopulateShuffleBags();
							}
						
						if(GUILayout.Button("copy")) {
							tarS.wallSetClipBoard = new BrushSet[1];
							var nBS: BrushSet = new BrushSet();
							tarS.wallSetClipBoard[0] = nBS;
							tarS.wallSetClipBoard[0].isActive = tarS.levels[tarS.activeLevel].wallsSetList[ws].isActive;
							tarS.wallSetClipBoard[0].random = tarS.levels[tarS.activeLevel].wallsSetList[ws].random;
							
							var nGs = new List.<GameObject>();
							nGs.AddRange(tarS.levels[tarS.activeLevel].wallsSetList[ws].tilesGOs);
							
							tarS.wallSetClipBoard[0].tilesGOs = nGs;
							
							tarS.wallSetClipBoard[0].strength = tarS.levels[tarS.activeLevel].wallsSetList[ws].strength;
							EditorUtility.SetDirty(tarS);
							waitForUpdate = true;
						}
						if(GUILayout.Button("empty")) {
						
							Undo.RegisterSceneUndo("Clean a wallSet(InfiniTile)");
							
							for(var tge : int= 0 ; tge < tarS.levels[tarS.activeLevel].wallsSetList[ws].tilesGOs.Count ; tge++) {
									tarS.levels[tarS.activeLevel].wallsSetList[ws].tilesGOs[tge] = null;
							}
						}
						EditorGUILayout.EndHorizontal();
						//ADJACENT CHECK:

//	    				tarS.levels[tarS.activeLevel].wallsSetList[ws].doUpper = GUILayout.Toggle(tarS.levels[tarS.activeLevel].wallsSetList[ws].doUpper, "Don't generate if:","Button");
//	   					////tarS.levels[tarS.activeLevel].wallsSetList[ws].doLower = GUILayout.Toggle(tarS.levels[tarS.activeLevel].wallsSetList[ws].doLower, "do if lower","Button");
//						if(tarS.levels[tarS.activeLevel].wallsSetList[ws].doUpper == true){
//						var index : int = 1;
//						var options : String[] = ["lower", "highier"];
//						var options2 : String[] = ["full", "empty"];
//						EditorGUILayout.BeginHorizontal();
//							tarS.levels[tarS.activeLevel].wallsSetList[ws].opt1 = EditorGUILayout.Popup(tarS.levels[tarS.activeLevel].wallsSetList[ws].opt1, options,GUILayout.MaxWidth(70) );
//							EditorGUILayout.LabelField("=",GUILayout.MaxWidth(15));
//							tarS.levels[tarS.activeLevel].wallsSetList[ws].opt2 = EditorGUILayout.Popup(tarS.levels[tarS.activeLevel].wallsSetList[ws].opt2, options2 );
//						EditorGUILayout.EndHorizontal();
//						}
						// check tiles:
	   					tarS.levels[tarS.activeLevel].wallsSetList[ws].fold = EditorGUILayout.Foldout(tarS.levels[tarS.activeLevel].wallsSetList[ws].fold, "Only for tile:");
	   					
	   					if(tarS.levels[tarS.activeLevel].wallsSetList[ws].fold) {
	   						
	   						for(var wx : int = 0 ; wx < tarS.levels[tarS.activeLevel].wallsSetList[ws].specialsList.Count ; wx++) {
	   							
	   							EditorGUILayout.LabelField(""+wx);
	   							
	   							for(var wy : int = 0 ; wy < tarS.levels[tarS.activeLevel].wallsSetList[ws].specialsList[wx].sPlist.Count ; wy++) {
	   								
	   								EditorGUILayout.BeginHorizontal();
	   								
	   									EditorGUILayout.LabelField("["+wy+"] "+tarS.levels[tarS.activeLevel].brushSetList[wx].tilesGOs[wy].transform.name );
	   								
	   									tarS.levels[tarS.activeLevel].wallsSetList[ws].specialsList[wx].sPlist[wy] = EditorGUILayout.Toggle("",tarS.levels[tarS.activeLevel].wallsSetList[ws].specialsList[wx].sPlist[wy],GUILayout.MaxWidth(10));
	   								
	   								EditorGUILayout.EndHorizontal();
	   							}
	   							
	   						}
	   						
   							EditorGUILayout.Space();
   						
	   					}
	   					
					EditorGUILayout.EndVertical(); 
				EditorGUILayout.EndVertical();
				}
			
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.EndScrollView();
		}
		}

		
	
//MODULES:

//TextureMerge:
//		if(tarS.levels[tarS.activeLevel].modules && waitForUpdate == false ){
//			
//			if(tarS.levels[tarS.activeLevel].modules.mod1.exist == true) {
//				
//				EditorGUILayout.Space();	
//				
//				var rectWSLo5 : Rect = EditorGUILayout.BeginHorizontal ("Button",GUILayout.Height(27));
//					EditorGUI.DrawPreviewTexture(Rect(rectWSLo5.x,rectWSLo5.y-2,logoC.width,logoC.height),logoC,mm);
//					EditorGUILayout.LabelField("TextureMerge:",EditorStyles.boldLabel);
//				EditorGUILayout.EndHorizontal ();
//			}
//		}		
//
		waitForUpdate = false;
		
//		if (GUILayout.Button("Open Grid Window", GUILayout.Width(255)))
//	    {  
//	       var windowz : InfiniTileTexturizer = EditorWindow.GetWindow(InfiniTileTexturizer,false);
//	       windowz.OpenFromButton();
//	    }
	#endif 
	}
	
	
	
///GUI THINGS--------------------------------------------:
	function OnSceneGUI(){
	if(tarS){
		if(tarS.editing == true) {
			
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			
			e = Event.current;

			if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag ){
				
				if (Event.current.button == 0 && !e.control && !e.alt) {
					tarS.AddATile();
					EditorUtility.SetDirty(tarS);
				} else if(Event.current.button == 0 && e.control && !e.alt) {
					tarS.DeleteATile();
					EditorUtility.SetDirty(tarS);
				}
				
			}
		} else if(tarS.cleaning == true){
		
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		
			e = Event.current;
			
			if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag ){
			
				if(Event.current.button == 0 && !e.alt) {
				
					tarS.DeleteATile();
					EditorUtility.SetDirty(tarS);
				
				}
			}
		}
		
		var posM : Vector3 = Event.current.mousePosition;
			posM.y = Screen.height - (Event.current.mousePosition.y + 30); // flip the y of the mouse position
		var ray : Ray = Camera.current.ScreenPointToRay(posM);
		
		
			var pos : Vector3 = ray.origin - (ray.direction / ray.direction.y) * (ray.origin.y-(tarS.levels[tarS.activeLevel].absoluteLvlHeight)); //constrain  to y=0
			tarS.ghostTilePos = pos;

			//GUILayout.Label(EditorWindow.focusedWindow.ToString());
				
			if(Event.current.type == EventType.MouseMove ) {
				//SceneView.RepaintAll(); 
				HandleUtility.Repaint();
			}
		}
	}


}
