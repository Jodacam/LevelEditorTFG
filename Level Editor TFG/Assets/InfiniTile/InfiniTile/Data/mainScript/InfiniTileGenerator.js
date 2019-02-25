#pragma strict
#pragma downcast
import System.Collections.Generic;
import System.Reflection;
import System.IO;
/*
		T I L E  E D I T O R   F O R   U N I T Y 3 D
				  Made by Wojciech Gabel
				  	wgabel34@gmail.com
						
					dev. state : 0.67

*/

//CLASSES:::::::::::::::::::::::::::::::::::::::::::::::::
public class TGLevel
{
	
	var lvlNr : int;
	var isActive : boolean;
	var tileSize : int;
	var lvlHeight : float;
	var absoluteLvlHeight : float;
	
	var unsortedTiles : List.<GameObject>;
	var unsortedGenerated  : List.<GameObject>;
	var unsortedContainer : GameObject;
	var sortedTiles : SortedTile[,];
	var sortedOrigin: Vector3; // position of 0,0 tile(even if there is no tile) in sorted array. Its for checking what is level lower or highier of given tile. 
	var sortedCountX:int;
	var sortedCountZ:int;
	var smallestSortedX: float;
	var smallestSortedZ: float;
	
	var generatedContainer : GameObject;
	
	var brushSetList : List.<BrushSet>; 
	var wallsSetList : List.<BrushSetWall>;
	//var allBrushTilesNr : int;
	var shuffleBags : List.<intBag>;
	
	var modules : SHModules;
	
	var poceduralTextures : List.<Material>;
	var procTexDictionary : Dictionary.<String,Material>;
	
	var activeBrushSet : int;
	var activeTile : int;
	
	//constructor
	function TGLevel(){
		
		try 
		{
		/*	
			Debug.Log("SHAZAM LOG: TGLevel construtor: was constructed. Check"	+":\n Level constructed: "+lvlNr
																				+"\n lvlNr: "+lvlNr
																				+"\n isActive: "+isActive 
																				+"\n tileSize: "+tileSize 
																				+"\n lvlHeight: "+lvlHeight 
																				+"\n absoluteLvlHeight: "+absoluteLvlHeight 
																				+"\n unsortedTiles[List]: "+unsortedTiles 
																				+"\n unsortedGenerated[List]: "+unsortedGenerated 
																				+"\n unsortedContainer[GameObject] :"+unsortedContainer 
																				+"\n sortedTiles[2d array]: "+sortedTiles 
																				+"\n sortedOrigin[Vector3]: "+sortedOrigin 
																				+"\n sortedCountX: "+sortedCountX 
																				+"\n sortedCountZ: "+sortedCountX 
																				+"\n smallestSortedX: "+smallestSortedX
																				+"\n smallestSortedZ: "+smallestSortedZ
																				+"\n generatedContainer[GameObject]: "+generatedContainer
																				+"\n brushSetList[List.<BrushSet>]: "+brushSetList
																				+"\n wallsSetList[List.<BrushSetWall>]: "+wallsSetList
																				+"\n allBrushTilesNr: "+allBrushTilesNr
																				//+"\n shuffleBag[List.<Bag>]: "+shuffleBag
																				//+"\n grups[List.<BagOfGOs>]: "+grups
																				+"\n"
																				);
																				*/
		}catch(err){
			Debug.Log("SHAZAM ERROR: x_xx : Problem when constructing level. ( "+ err+" )");
		}

	}
	
//	function findSequence( d : Dictionary.<String,int> , s:List.<float> ) {
//		
//		for( var x : int = 0 ; x < s.Count ; x++ ){
//		
//			
//		
//		}
//		
//	}
	
	function AddLevel(l: int, z : List.<TGLevel>, mother : Transform, w:String) {
		
		var i : TGLevel = new TGLevel();
		i.tileSize = 1;
		i.lvlHeight = 1;

		var name : String; 
		
		var tileContainer : GameObject = new GameObject();
		
		var activeLvNr : int;
		
		if(w == "0"){
			
			name = "TileContainerL:"+l.ToString();
			
			tileContainer.transform.name = name;
				
			z.Add(i);	
			
			i.absoluteLvlHeight = setAbsoluteHeight(l,activeLvNr,z);
				
		}
		
		if(w == "lower") {
			
			activeLvNr = z[l].lvlNr;
			
			rechangeLowerLevels( l , z , activeLvNr );
			
			i.lvlNr = z[l].lvlNr-1;	
					
			name = "TileContainerL:"+(z[l].lvlNr-1).ToString();
			
			tileContainer.transform.name = name;
			
			moveLevels( l , z , "add" );
			
			z.Insert( l, i );
			
		} if(w == "higher") {
			
			activeLvNr = z[l].lvlNr;

			rechangeHigherLevels( l , z , activeLvNr );
			
			i.lvlNr = z[l].lvlNr + 1;
			
			name = "TileContainerL:"+(z[l].lvlNr+1).ToString();
			
			tileContainer.transform.name = name;
			
			if(l < z.Count) {
			
				
				
				z.Insert( l+1, i );
				moveLevels( l+1 , z , "add" );
				
			} else {
			
				moveLevels( l , z , "add" );
			
				z.Add( i );
				
				i.absoluteLvlHeight = setAbsoluteHeight(l,activeLvNr,z);
				
			}
			
		} 
		
		setAbsoluteHeights( z );
		
		tileContainer.transform.position = mother.position;
		tileContainer.transform.parent = mother;
		i.unsortedContainer = tileContainer;
		
	}
	
	
	function rechangeLowerLevels( l:int , z : List.<TGLevel>, al:int){

		if( al <= 0) {
			for (var i :int = 0 ; i < l ; i++) {
			
				z[i].lvlNr--;
				z[i].unsortedContainer.transform.name = "TileContainerL:"+z[i].lvlNr.ToString();
		
			}
		} else {
			for (var ii :int = l ; ii < z.Count ; ii++) {
			
				z[ii].lvlNr++;
				z[ii].unsortedContainer.transform.name = "TileContainerL:"+z[ii].lvlNr.ToString();
				
			}
		}
	}
	
	
	function rechangeHigherLevels( l:int , z : List.<TGLevel>, al:int){
	
		if( al >= 0 ) {
		
			for (var i :int = l+1 ; i < z.Count ; i++) {

				z[i].lvlNr++;
				z[i].unsortedContainer.transform.name = "TileContainerL:"+z[i].lvlNr.ToString();
				
			}
		
		} else{
			
			for (var ii :int = 0 ; ii < l+1 ; ii++) {
			
				z[ii].lvlNr--;
				z[ii].unsortedContainer.transform.name = "TileContainerL:"+z[ii].lvlNr.ToString();
			
			}
		}
	}
	
	
	function DeleteLevel(l:int, al:int, z: List.<TGLevel>, x:InfiniTileGenerator){ // have to pass whole script instance to this
		
		var name:String;
		
		moveLevels( l , z , "delete" );
		
		if(al!=0) {
		
			if( al < 0 ){
				
				for(var i : int = 0 ; i < l ; i++){
				
					z[i].lvlNr++;
					
				}
				
			} else if( al > 0 ) {
			
				for(var ii : int = l ; ii < z.Count ; ii++){
				
					z[ii].lvlNr--;
				
				}
			}
			
			if( z[l].unsortedContainer != null && z[l].unsortedContainer != null ) {
				
				z[l].unsortedContainer.DestroyImmediate ( z[l].unsortedContainer );
			} 
			
			if( z[l].unsortedContainer != null && z[l].generatedContainer != null ){
				
				z[l].unsortedContainer.DestroyImmediate ( z[l].generatedContainer ); 
			}
			
			z.RemoveAt(l);
			
			try{
				
				if( l < z.Count ){
				
					//Debug.Log("setting to highier after deleting");
			    	setActiveLevel(l, z);
			    	x.activeLevel = l;
				
				} else {
				
					//Debug.Log("setting to lower after deleting");
			    	setActiveLevel(l-1, z);
			    	x.activeLevel = l-1;
				
				}
				
			} catch(err) {
				Debug.Log(err);
			}
			
			setAbsoluteHeights( z );
		
		} else {
		
			Debug.Log("Level 0 cannot be deleted!");
		
		}
	}
	
	
	function moveLevels(l: int, z : List.<TGLevel> , t:String ){
	
		//var h: float = z[l].lvlHeight;
		
		var m : int = 0;
		
		var e : int;
		
		var s : int;
		
		//when deleting: ( only two options )
		if(t == "delete") {
		
			if( z[l].lvlNr < 0 ) {
				 
				 s = 0;
				 
				 e = l;
				 
				 m = -1;
				 
			} else if( z[l].lvlNr > 0 ) {
			
				s = l;
				
				e = z.Count;
				
				m = 1;
			}
			//Debug.Log("moving l"+l+" lvlNr:"+z[l].lvlNr+" s"+s+" e"+e +" m"+m);
			for(var i : int = s ; i < e ; i++) {
				try{
					if( m == -1 ) {
						
						if( z[i].generatedContainer ) {
						
							for(var x : Transform in  z[i].generatedContainer.transform) {
	
								x.localPosition.y -= z[l].lvlHeight;
							}
						}
						
						if( z[i].unsortedContainer ) {
						
							for(var x1 : Transform in   z[i].unsortedContainer.transform) {
	
								x1.localPosition.y -= z[l].lvlHeight;
							}
						}
	
					} else if( m == 1 ) {
					
						if( z[i].generatedContainer ) {
						
							for(var x2 : Transform in   z[i].generatedContainer.transform) {
	
								x2.localPosition.y += z[l].lvlHeight;
							}
						}
						
						if( z[i].unsortedContainer ) {
						
							for(var x3 : Transform in   z[i].unsortedContainer.transform) {
	
								x3.localPosition.y += z[l].lvlHeight;
							}
						}
					}
				} catch (err) {
					Debug.Log(err);
				}
			}
		}
		//end when adding: ( four options )
		
		if( t == "add" ) {
		
			if( z[l].lvlNr <= 0 ) {
			
				s = 0;
				
				e = l;
				
				m = -1;
			
			} else if( z[l].lvlNr > 0 ) {
			
				s = l;
				
				e = z.Count;
				
				m = 1;
			
			}
			
			for(var i3 : int = s ; i3 < e ; i3++) {
			
				if( m == -1 ) {
				
					if( z[i3].generatedContainer ) {
					
						for(var x6 : Transform in   z[i3].generatedContainer.transform) {
						
							x6.localPosition.y += 1.0f;
						
						}
					}
					
					if( z[i3].unsortedContainer ) {
					
						for(var x7 : Transform in   z[i3].unsortedContainer.transform) {
						
							x7.localPosition.y += 1.0f;
						
						}
					}
				
				} else if( m == 1 ){
				
					if( z[i3].generatedContainer ) {
					
						for(var x8 : Transform in   z[i3].generatedContainer.transform) {
						
							x8.localPosition.y -= 1.0f;
						
						}

					}
					if( z[i3].unsortedContainer ) {
					
						for(var x9 : Transform in   z[i3].unsortedContainer.transform) {
						
							x9.localPosition.y -= 1.0f;
						
						}
					
					}
					
				}
			
			}
			
		}
		
		if(t == "change") {
		
			//var lnr : int = ( z[l].lvlNr * (-1) ) + l; // zero level index
			
			
			
			if(z[l].lvlNr <= 0 ) { // change with 0 level
			
				s = 0;
				
				e = l;
			
			
			} else if(z[l].lvlNr > 0) {
			
				s = l;
				
				e = z.Count-1;
			
			}
			
			for(var i2 : int = s ; i2 < e+1 ; i2++) {
			
				if( z[i2].generatedContainer ) {
			
					for(var x4 : Transform in   z[i2].generatedContainer.transform) {
			
						x4.localPosition.y = z[i2].absoluteLvlHeight;
				
					}
				}
				
				if( z[i2].unsortedContainer ) {
				
					for(var x5 : Transform in   z[i2].unsortedContainer.transform) {
	
						x5.localPosition.y = z[i2].absoluteLvlHeight;
					
					}
				}
			}
		}
	}
	
	
	function setAbsoluteHeight( l : int , al : int , z : List.<TGLevel> ) : float {
		
		//Debug.Log("setting absolute level height. l:"+l+" al: "+al);
		
		var i : float;
		
		var zeroL : int = (al*(-1))+l;
		
		if( al < 0 ){
		
			for( var b : int = zeroL ; b >= l ; b-- ){
				
				i += z[b].lvlHeight;
				
			}
		
		} else if( al > 0 ){
		
			for( var bb : int = zeroL+1 ; bb < l ; bb++ ){
				
				i -= z[bb].lvlHeight;
				
			}
		}else if( al == 0 ){
		
			i = z[l].lvlHeight;
		
		}
		
		return i;
		
	}
	
	
	function setAbsoluteHeights(z : List.<TGLevel> ){
	
		//var zeroL : int = (z[0].lvlNr*-1);
		//Debug.Log("Setting absolute heights!");
		
		for( var i : int = 0 ; i < z.Count ; i++ ){
		
			z[i].absoluteLvlHeight = setAbsoluteHeight(i,z[i].lvlNr,z);
		
		}
	
	}
	
	
	function setActiveLevel( l : int , z : List.<TGLevel> ){
		
		//Debug.Log("setting active level");
		
		for( var i : int = 0 ; i < z.Count ; i++) {
		
			if( i != l ) {
				
				z[i].isActive = false;
				
			}	
		}
		
		z[l].isActive = true;
		
		z[l].absoluteLvlHeight = setAbsoluteHeight(l,z[l].lvlNr,z);
		
		
		
	}
	
	function regenerateLevel( z : TGLevel , scrG : GameObject){

		var scr : InfiniTileGenerator  = scrG.GetComponent(InfiniTileGenerator);
		var brushUsed : int;
		var tileGOUsed: int; 
	
		var a : boolean;
		//for checking numbers of brushes and tiles used:
		
		var e : String;
		
		if(!z.sortedTiles){
			//Debug.Log(scr.activeLevel);
			scr.ReGetTiles(scr.activeLevel);
			scr.RecolectGenereted(scr.activeLevel);
		}
		
		try{
			
			for( var x : int = 0 ; x < z.sortedTiles.GetLength(0) ; x++ ) {
			
				for( var y : int = 0 ; y < z.sortedTiles.GetLength(1) ; y++ ) {
				
					 //if there is a tile, delete that tile and insert a tile in its place
					 //how to find this tile pos(x,y) if there is no user?
				 	if( z.sortedTiles[x,y] != null && z.sortedTiles[x,y].tile !=null){
				 	
				 		for(var c : int = 0 ; c< 4 ; c++ ){
					 		
					 		for(var w : int = 0 ; w < z.sortedTiles[x,y].corners4[c].walls.Count ; w++){
								
								if(z.sortedTiles[x,y].corners4[c].walls[w] != null){
								
									z.sortedTiles[x,y].tile.DestroyImmediate( z.sortedTiles[x,y].corners4[c].walls[w] );
									
								}
					 			
					 		}
				 		}  
				 		
				 		//check brush and tile numbers:
				 		
				 		e = z.sortedTiles[x,y].tile.transform.name;
				 		
				 		brushUsed = parseInt( e.Substring( 0 , e.LastIndexOf(",") ) );
				 		
				 		tileGOUsed = parseInt( e.Substring( e.LastIndexOf(",")+1 , e.LastIndexOf(":") - ( e.LastIndexOf(",")+1 ) ) );
						
						//check if there are those tiles and brushes to use. if not then try to find a good one:
						if( brushUsed >= z.brushSetList.Count ){
						
							if( z.brushSetList[0] !=null ){
								
								brushUsed = 0;
								
								if( z.brushSetList[0].tilesGOs[0] != null ){
								
									tileGOUsed = 0;
								
								} else {
									
									Debug.Log("SHAZAM: Could not regenerate. There was no tile to use!");
									a = true;
								
								}
							} else {
							
								Debug.Log("SHAZAM: Could not regenerate. There was no brush set to use!");
								a = true;
								
							}
						}
						if(tileGOUsed >= z.brushSetList[brushUsed].tilesGOs.Count){
						
							if( z.brushSetList[brushUsed].tilesGOs[0] != null ){
							
								tileGOUsed = 0;
							
							} else {
							
								Debug.Log("SHAZAM: Could not regenerate. There was no tile to use!");
								a = true;
							}
						
						}
						
				 		//make the new tile:
				 		if(a == false){
				 		var newTile : GameObject = z.sortedTiles[x,y].tile.Instantiate(  z.brushSetList[ brushUsed ].tilesGOs[tileGOUsed] , z.sortedTiles[x,y].tile.transform.position , z.sortedTiles[x,y].tile.transform.rotation);
				 		
				 		newTile.transform.parent = z.unsortedContainer.transform;
	
						newTile.transform.name = brushUsed.ToString()+","+tileGOUsed.ToString()+":Tile";
				 		
				 		z.sortedTiles[x,y].tile.DestroyImmediate(z.sortedTiles[x,y].tile);
				 		
				 		z.sortedTiles[x,y].tile = newTile;
				 		
				 		scr.GenerateTileBorders(x,y, false, brushUsed, tileGOUsed);
						}
				 	} 

				}

			}
		}
		catch(err){
		
			Debug.Log("SHAZAM: Could not do level regeneration :" + err);
		
		}
	}
	
	function cleanLevel(z : TGLevel){
	
		
	
	}
	
	function changeActiveBrushSet(l : int , a: int , x : InfiniTileGenerator){ // l - brushSet number , a : activeLevel
	
		if(x.levels[a].brushSetList.Count <=0){
		
			//Debug.Log("No brush. Add one");
			return;
			
		}
		
		if(x.levels[a].brushSetList[l].tilesGOs.Count <=0) {
		
			Debug.Log("No Tiles in brush. Add one");
			return;
			
		}
		
		for(var i : int = 0 ; i<x.levels[a].brushSetList.Count ; i++) {
			
			x.levels[a].brushSetList[i].isActive = false;
			
		}
		
		x.levels[a].brushSetList[l].isActive = true;
		
		x.activeBrushSet = l;
		
		x.levels[a].activeBrushSet = l;
		
		//Debug.Log("number of tiles in this set("+l+") = "+ x.levels[a].brushSetList[l].tilesGOs.Count+"and active tile is:"+x.levels[a].activeTile);
		
		if(x.levels[a].activeTile >= x.levels[a].brushSetList[l].tilesGOs.Count ){
			//Debug.Log("changing active tile to 0");
			x.levels[a].activeTile = 0;
		
		}
	}
	
	function changeActiveTile(x:int,y:int, z : TGLevel){
	
		

		// brushSet  activeTileGO : int is the one to change
		// savfe changes to TGLevel activeTile
	
	}
/*
	Checking if any modules are present. Bought modules have the 'boughtCheck' method in them. If this method is present,
	the 'exist' variable is set to true.
	This class method is run every recompile and load/save.
	run by: GeneratorEditor.js ( updateVariables )
*/
	
	function CheckModules(){
		
		
		try
		{

			/* Piece for variable checking inside a class:
			
			var myType : System.Type = typeof(Sh_modTexMerge);
			
			var myField : System.Reflection.FieldInfo[] = myType.GetFields();
			
			for( var i : int = 0; i < myField.Length; i++){
				Debug.Log("Field: "+ myField[i].Name);
			}
			
			*/
			var myType : System.Type = typeof(Sh_modTexMerge);
			
			var info : System.Reflection.MethodInfo[] = myType.GetMethods();
			
			var tcheck : boolean = false;
			
			for (var iz :int = 0; iz < info.Length; iz++)

    		{
    		
				if( info[iz].Name == "boughtCheck" ){
					
					tcheck = true;
					
				}
    		}
    		
    		if(tcheck == true) {
    		
    			modules.mod1.exist = true;
    			
    		} else {
    			
    			modules.mod1.exist = false;
    			
    		}

			
		} catch (err){ 
			Debug.Log("Error while trying to make modules. "+ err);
		}
	}
	
	
	function createBrushSet(){

		//Debug.Log("Adding brushSet. Count before:"+brushSetList.Count );
		
		try
		{
			var i : BrushSet = new BrushSet();
			
			if(!i.tilesGOs) {
				var t: List.<GameObject> = new List.<GameObject>();
				//var b : GameObject[] = new GameObject[1];
				//t.AddRange(b);
				
				i.tilesGOs = t;
				
				i.name = "";
				
			}
			brushSetList.Add(i);
			
			//create specials in all wallSets here
			for(var x : int = 0 ; x < wallsSetList.Count ; x++) {
				
				var o : specials = new specials();
					
				wallsSetList[x].specialsList.Add(o);
					
			}
			/*			
			Debug.Log("SHAZAM LOG: TGLevel createWall.Created. Check:" 	+"\n Last brushSetList item: "+ brushSetList[brushSetList.Count-1] 
																		+"\n brushSetList Count: "+brushSetList.Count
																		+"\n tileGOs: "+brushSetList[brushSetList.Count-1].tilesGOs
																		+"\n tileGOs Count: "+brushSetList[brushSetList.Count-1].tilesGOs.Count +" ( should be 1 (empty))"
																		+"\n note, that specialsList should be recreated in here to reflect any changes in brushes"
																		+"\n"
																		);
			*/
			
		}catch(err){
			Debug.Log("SHAZAM ERROR: x_xx : Problem when creating a brushSet: ( "+ err+" )");
		}
	}
	
	function changeBrushSetContents(x:int, y:int, g : GameObject){
		
		
		if( g != null){
			try
			{
				//add
				if(y >= brushSetList[x].tilesGOs.Count){
				
					brushSetList[x].tilesGOs.Add(g);
					
					//Add to specials:
					
					for(var ba1 : int = 0 ; ba1 < wallsSetList.Count ; ba1 ++) {
							
						var bb1 : boolean = true;
							
						wallsSetList[ba1].specialsList[x].sPlist.Insert(y,bb1);
					}
					
				} else {
				
					//delete:
					brushSetList[x].tilesGOs.RemoveAt(y);
					
					try
					{
					
						for(var a : int = 0 ; a < wallsSetList.Count ; a ++) {
							
							wallsSetList[a].specialsList[x].sPlist.RemoveAt(y);
						
						}						
					} 
					catch(err){
						
						Debug.Log("SHAZAM ERROR: x_xx : can't remove from specials: ( "+ err+" )");
					
					}
					
				
				}
			
			}
			catch(err){
			
				Debug.Log("SHAZAM ERROR: x_xx : Problem when recreating walls specials: ( "+ err+" )");
			
			}
		}
		if(g == null){
			try
			{
				if(brushSetList[x].tilesGOs[y] == null) {
				
				//remove	
				
					if(y < brushSetList[x].tilesGOs.Count-1) { //is it last? no, then remove
						
						brushSetList[x].tilesGOs.RemoveAt(y);
						
						//remove from specials:
						try
						{
						
							for(var a1 : int = 0 ; a1 < wallsSetList.Count ; a1 ++) {
								
								wallsSetList[a1].specialsList[x].sPlist.RemoveAt(y);
							
							}						
						} 
						catch(err){
							
							Debug.Log("SHAZAM ERROR: x_xx : can't remove from specials: ( "+ err+" )");
						
						}
					}
	
				} else {
				
				//Add empty to tilesGOs and add to specials
				
					if(brushSetList[x].tilesGOs[brushSetList[x].tilesGOs.Count-1] != null) { // if last is not empty
						
						var b : GameObject[] = new GameObject[1];
						
						brushSetList[x].tilesGOs.AddRange(b);
						
						//Add to specials:
						
						for(var ba : int = 0 ; ba < wallsSetList.Count ; ba ++) {
							
							var bb : boolean = true;
							
							wallsSetList[ba].specialsList[x].sPlist.Insert(y,bb);
						}
					}
				}
				
			}catch(err){
				
				Debug.Log("SHAZAM ERROR: x_xx : Problem when recreating walls specials: ( "+ err+" )");
			}
		}
	}
	
	

	function deleteBrushSet(n:int){
		
		//Debug.Log("Deleting brushSet at:"+n);
		
		try
		{
			brushSetList.RemoveAt(n);
			
			//remove from specials in here
			for(var x : int = 0 ; x < wallsSetList.Count ; x++) {
				
				wallsSetList[x].specialsList.RemoveAt(n);
			}
			
		}catch(err){
			
			Debug.Log("SHAZAM ERROR: x_xx : Problem when deleting a brushSet: ( "+ err+" )");
		}
	}
	

	function createWall(){
		try
		{
			//create empty wall
			var i : BrushSetWall = new BrushSetWall();
			 
			//fill tilesGOs with dummies:
			var s : List.<GameObject> = new List.<GameObject>(); //tileGOs list
			
			var b : GameObject[] = new GameObject[10]; //dummies
			
			s.AddRange(b);
			
			i.tilesGOs = s;
			
			i.strength = 1;
			
			wallsSetList.Add(i);
			
			//fill specialsList:
			if(!wallsSetList[wallsSetList.Count-1].specialsList) {
				
				wallsSetList[wallsSetList.Count-1].specialsList = new List.<specials>();
			}
			
			for(var x : int = 0 ; x<brushSetList.Count; x++) {
			
				var o2 : specials = new specials();
				
				if(!o2.sPlist) {
					
					o2.sPlist = new List.<boolean>();
				
				}
				
				if(brushSetList[x].tilesGOs !=null) {
						
					for(var y : int = 0 ; y < brushSetList[x].tilesGOs.Count ; y++) {
						
						if(brushSetList[x].tilesGOs[y] != null) {
							
							var v : boolean = true;
							
							o2.sPlist.Add(v);
							
						}
					}
				}
				
				wallsSetList[wallsSetList.Count-1].specialsList.Add(o2);
				
			} 
			var bb : boolean[] = new boolean[wallsSetList.Count];
			
			if(!wallsSetList[wallsSetList.Count-1].conBools){
			
				//Debug.Log("was empty");
				wallsSetList[wallsSetList.Count-1].conBools = new List.<boolean>();
				wallsSetList[wallsSetList.Count-1].conBools.AddRange(bb);
				//Debug.Log("is "+wallsSetList[wallsSetList.Count-1].conBools.Count +" now");
			
			}else if(wallsSetList[wallsSetList.Count-1].conBools.Count != wallsSetList.Count-1){
				//Debug.Log("was different");
				wallsSetList[wallsSetList.Count-1].conBools = new List.<boolean>();
				wallsSetList[wallsSetList.Count-1].conBools.AddRange(bb);
				//Debug.Log("is "+wallsSetList[wallsSetList.Count-1].conBools.Count +" now");
			}
			
			for(var w : int = 0 ; w<wallsSetList.Count-1; w++) {
//			
				var o3 : boolean = new boolean();
//				
				wallsSetList[w].conBools.Add(o3);
//				
//				for(var w2 : int = 0 ; w2 < wallsSetList.Count ; w2++) {
//				
//					var v2 : boolean = false;
//							
//							o3.sPlist.Add(v2);
//					
//				}
//				wallsSetList[wallsSetList.Count-1].conBools.Add(o3);
			}
			
			repopulateShuffleBags();
			/*
			Debug.Log("SHAZAM LOG: TGLevel createWall.Created. Check:" 	+"\n Last wallSetList item: "+ wallsSetList[wallsSetList.Count-1] 
																		+"\n WallSetList Count: "+wallsSetList.Count
																		+"\n tileGOs: "+wallsSetList[wallsSetList.Count-1].tilesGOs
																		+"\n tileGOs Count: "+wallsSetList[wallsSetList.Count-1].tilesGOs.Count
																		//+"\n specialsList: "+wallsSetList[wallsSetList.Count-1].specialsList
																		//+"\n specialsList Count: "+wallsSetList[wallsSetList.Count-1].specialsList.Count
																		+"\n Note that specialsList should be same as BrushSetList, but with booleans. Also, all wallSet list items should have the same specials"
																		//+"\n brushSetList.Count: "+brushSetList.Count+" , specialsList.Count:"+wallsSetList[wallsSetList.Count-1].specialsList.Count
																		+"\n"
																	);
			
			*/
		}
		catch(err){
			Debug.Log("SHAZAM ERROR: x_xx : Problem when creating a wall: ( "+ err+" )");
		}
	}
	
	
	function deleteWall(n:int){
		//delete last wall. Maybe set which one later
		try
		{
			wallsSetList.RemoveAt(n);
			
			for(var x : int = 0 ; x < wallsSetList.Count ; x++){
			
				wallsSetList[x].conBools.RemoveAt(wallsSetList[x].conBools.Count-1);
			
			}
			
			repopulateShuffleBags();
			
		}catch(err){
			Debug.Log("SHAZAM ERROR: x_xx : Problem when deleting wall: ( "+ err+" )");
		}
	} 
	
	function createShuffleBags() {
		try
		{
			var o : intBag = new intBag();
		
			if (!o.tokenBag){
				
				var o2 : List.<int> = new List.<int>();
				
				o.tokenBag = o2;
			}
			
			shuffleBags.Add(o);

		} catch (err){
			Debug.Log("SHAZAM ERROR: x_xx : Problem when creating a shuffleBag: ( "+ err+" )");
		}
		
	}
	
	
	function populateShuffleBag(){
		
		//populate based on wallsSetList 
		
		//get all grups:
		
		var tgrups : List.<int> = new List.<int>();
		
		for(var i : int = 0 ; i < wallsSetList.Count ; i++) {
			
			var n: int;
			
			if(tgrups.Contains(wallsSetList[i].grNr)) {
				
				n = tgrups.IndexOf(wallsSetList[i].grNr);
				
				for(var f0 : int = 0 ; f0 < (wallsSetList[i].strength*10) ; f0++ ) {
						shuffleBags[n].tokenBag.Add(i);
					}

				//Debug.Log("i:"+i+"temp grup has wallsSetList[i] grup nr:( "+wallsSetList[i].grNr+" ). added to shuffleBag["+n+"].tokenBag");
				
			} else {
				tgrups.Add(wallsSetList[i].grNr);
				
				n = tgrups.IndexOf(wallsSetList[i].grNr);
				
				if(n < shuffleBags.Count) {
					for(var f1 : int = 0 ; f1 < (wallsSetList[i].strength*10) ; f1++ ) {
						shuffleBags[n].tokenBag.Add(i);
					}
					
				} else {
					//create one shuffleBag entry
					
					createShuffleBags();
					
					for(var f2 : int = 0 ; f2 < (wallsSetList[i].strength*10) ; f2++ ) {
						shuffleBags[n].tokenBag.Add(i);
					}
				}
			}
		}
		
		//Fisher-Yates algorithm for shuffling tokens inside a bag(i hope its not copyright protected (murica...) ):
		for(var s : int = 0 ; s < shuffleBags.Count ; s++) {
			
			var m : int = shuffleBags[s].tokenBag.Count-1;
			if(m < 0) {
				shuffleBags.RemoveAt(shuffleBags.Count-1);
				m = 0;
			}
			while(m){
				var z : int = Random.Range(0,m);
			
				var t : int = shuffleBags[s].tokenBag[m];
				
				shuffleBags[s].tokenBag[m] = shuffleBags[s].tokenBag[z];
				
				shuffleBags[s].tokenBag[z] = t;
				m--;
				
			}
		}
	}
	
	
	function repopulateShuffleBags(){
		
		shuffleBags = new List.<intBag>();
		
		populateShuffleBag();
			
	}
	
	
	function repopulateOneBag(x: int){ // w is wallsetlist nr
		

		if(shuffleBags[x].tokenBag.Count > 0) {
		
			var t : int = wallsSetList[shuffleBags[x].tokenBag[0]].grNr ; //nr of wallSetList wall:
			
			var tgrups : List.<int> = new List.<int>();
			
			for(var i : int = 0 ; i < wallsSetList.Count ; i++) {
				
				if(wallsSetList[i].grNr == t) {
					
					for(var a : int = 0 ; a < (wallsSetList[i].strength*10) ; a++ ) {
						tgrups.Add(i);
					}
	
				}
			}
			shuffleBags[x].tokenBag = tgrups;
			
			//and shuffle:
			var m : int = shuffleBags[x].tokenBag.Count-1;
			
			while(m){
					
					var z : int = Random.Range(0,m);
				
					var ts : int = shuffleBags[x].tokenBag[m];
					
					shuffleBags[x].tokenBag[m] = shuffleBags[x].tokenBag[z];
					
					shuffleBags[x].tokenBag[z] = ts;
					m--;	
			}
		}
	}	
	
	
//	function repopulateTileCornerTypes( xp : int , yp : int , z : TGLevel ) {
//		
//		var s: SortedTile = z.sortedTiles[xp,yp];
//		//go through all corners, and walls list and from those fill the corners
//		//if there are 0 walls, then check neighbour tiles corners.
//		
//		var t: int[,] = new int[3,2];
//		
//		t[0,0] = -1;
//		t[0,1] = -1;
//		
//		t[1,0] = -1;
//		t[1,1] =  0;
//		
//		t[2,0] =  0;
//		t[2,1] = -1;
//		
//		var t1 : int = 2;
//		
//		for( var d : int = 0 ; d < 4 ; d++ ) { // every corner of a tile
//		
//			if( s.corners4[d].walls.Count > 0 ) {
//				
//				for( var y : int = 0 ; y < s.corners4[d].walls.Count ; y++ ) { // every wall in a corner
//
//					if(!s.corners4[d].whatWallSet) {
//					
//						var xT: boolean[] = new boolean[z.wallsSetList.Count];
//			
//						s.corners4[d].whatWallSet = new List.<boolean>(xT);
//					}
//					
//					if(!s.corners4[d].neighbourWalls) {
//					
//						var xTT: boolean[] = new boolean[z.wallsSetList.Count];
//						
//						s.corners4[d].neighbourWalls = new List.<boolean>(xTT);
//					}
//				
//					//go through all walls of this tile and check what wallSet they are:
//					
//					var e : String = s.corners4[d].walls[y].transform.name;
//					
//					var idx : int = e.LastIndexOf("s");
//					
//					var idxe : int = e.LastIndexOf(".");
//					
//					e = e.Substring( idx+1, ( idxe )-( idx+1 ) );
//					
//					var nr : int = parseInt( e );
//					
//					Debug.Log(s.corners4[d].whatWallSet.Count+" "+nr);
//					
//					if( s.corners4[d].whatWallSet.Count  < nr+1 ){
//					
//						var q : int = nr - s.corners4[d].whatWallSet.Count;
//					
//						for(var i : int = 0 ; i < q ; i++) {
//							
//							var sb: boolean;
//						
//							s.corners4[d].whatWallSet.Add(sb);
//						
//						}
//					
//					}
//					Debug.Log(s.corners4[d].whatWallSet.Count + " and nr:" +nr);
//					
//					s.corners4[d].whatWallSet[nr] = true;
//					
//					//and set the same wall nr's to all neghbouring tiles neighbouring corners
//					
//					var t2 : int = t1;
//					
//					for( var c : int = 0 ; c < 3 ; c++) {
//						
//						//check if tile is inside array
//						if( ( xp + t[c,0] ) >= 0 || ( xp + t[c,0] ) < z.sortedTiles.GetLength(0) || ( yp + t[c,1] ) >= 0 || ( yp + t[c,1] ) < z.sortedTiles.GetLength(1) ) {
//							
//							if( z.sortedTiles[ (xp + t[c,0] ) , ( yp + t[c,1] ) ].tile !=null ) {
//								
//								if(z.sortedTiles[ (xp + t[c,0] ) , ( yp + t[c,1] ) ].corners4[t1].neighbourWalls.Count != s.corners4[d].neighbourWalls.Count) {
//								
//									var xTT3: boolean[] = new boolean[z.wallsSetList.Count];
//									
//									z.sortedTiles[ (xp + t[c,0] ) , ( yp + t[c,1] ) ].corners4[t1].neighbourWalls = new List.<boolean>(xTT3);
//								
//								} else if( z.sortedTiles[ (xp + t[c,0] ) , ( yp + t[c,1] ) ].corners4[t1].neighbourWalls.Count == s.corners4[d].neighbourWalls.Count ) {
//								
//									z.sortedTiles[ (xp + t[c,0] ) , ( yp + t[c,1] ) ].corners4[t1].neighbourWalls[nr] = true;
//									
//									Debug.Log("setting type of wall"+" x "+" in tile"+" y "+" in corner"+" z ");
//								
//								}
//							}
//						}
//						
//						var ttemp: int = t[c,0];
//						
//						t[c,0] = t[c,1];
//					
//						t[c,1] = -ttemp;
//						
//						t2++;
//						
//						if( t2 == 4) {
//						
//							t2 = 0;
//						
//						}
//					
//					}
//					
//					t1++; 
//					
//					if( t1 == 4) {
//					
//						t1 = 0;
//					
//					}
//				}
//			} 
//		}
//	}
	
	// int = 0 if there is no same connector. 1 if the first connector is true. 2 if the second connector is true, and 3 if both of the connectors are true
	function checkForCorner( d : int , p : int , c : int , xx: int , zz : int , z : TGLevel) : intBag { // d=d , p=passValue , c = p( choosed wallSet in generateBorders )
		//returns true if one of four tiles has a corner with the same p as choosed p
		
		var xt: int[] = new int[2];
		
		var i : intBag = new intBag();
		
		i.tokenBag = new List.<int>(xt);
		
		i.tokenBag[0] = 0;
		i.tokenBag[1] = 0;
		
		
		
		var t : int[,] = new int[4,4]; 	
		//case d = 0
		t[0,0] = -1;	
		t[0,1] = -2;
		t[0,2] = -2;
		t[0,3] = -1;
		//case d = 1
		t[1,0] = -2;	
		t[1,1] = 0;
		t[1,2] = -1;
		t[1,3] = 1;
		//case d = 2
		t[2,0] = 0;		
		t[2,1] = 1;
		t[2,2] = 1;
		t[2,3] = 0;
		//case d = 3
		t[3,0] = 1;		//x	
		t[3,1] = -1;	//z
		t[3,2] = 0;		//x ( corner 2 )
		t[3,3] = -2;	//z ( corner 2 )
		
		var l : int[] = new int[4];
		
		l[0] = 2;
		l[1] = 3;
		l[2] = 0;
		l[3] = 1;
		
		
		var doubleC :int = 1;
		
		var sx : int;
		var sz : int;
		
		switch ( p ) {

			case 1:
				
				sx = 0;
				sz = 1;
				
				doubleC = 2;
			
			break;
			case 2:
			
				sx = 2;
				sz = 3;
				
			break;
			case 3:
			
				sx = 0;
				sz = 1;
			
			break;
			case 4:
				
				sx = 0;
				sz = 1;
			
			break;
			case 5:
				
				sx = 2;
				sz = 3;
			
			break;
			case 6:
				
				sx = 0;
				sz = 1;
			
				doubleC = 2;
			
			break;

			default:
				
				if( p == 0 || p == 7 ){
					
					return i;
				}
				
			break;
		
		}
		
		//Debug.Log("Checking tile: "+xx+","+zz+" passValue:"+p);
		
		var e: String;
		
		var idx : int;
		
		var idxe : int;
		
		var nr : int;
		
		var tx : int = 0;
		
		var tz : int = 1;
		
		
		var trigger : boolean;
		//Debug.Log("Checking tile:"+xx+","+zz+". passed values: d:"+d+" passValue:"+p+" sx:"+sx+"("+t[d,sx]+") sz:"+sz+"("+t[d,sz]+"). Begin:");
		//if there are two connectors(special passValues), do two times:
		for(var dd : int = 0 ; dd < doubleC ; dd++){
//		Debug.Log(trigger);
		//Debug.Log("dd="+dd);
		// check 4 tiles:
			for(var x : int = 0 ; x < 4 ; x++) { 
				
				//Debug.Log("x:"+x+" tile: "+xx+","+zz+" start tile :"+(xx+t[d,sx])+","+(zz+t[d,sz])+" tx,tz:"+tx+","+tz+" length of sortedTiles:"+z.sortedTiles.GetLength(0)+","+z.sortedTiles.GetLength(1));
				
				//try{  
		//check if checked tile is inside the array of tiles:
				if(  (xx + t[d,sx]) >= 0 &&  (xx + t[d,sx]) < z.sortedTiles.GetLength(0) && ( zz + t[d,sz]) >= 0 && ( zz + t[d,sz]) < z.sortedTiles.GetLength(1)  ) {
					
					//Debug.Log("checked tile is inside array. Will try to check name of walls: (Count:"+z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls.Count+")");
					
					//Debug.Log("check tile "+x+" ("+(xx + t[d,sx])+","+(zz + t[d,sz])+")");
		//if this tile is not null:			
					if(z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ]){
						
						var xc : int = z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls.Count;
						
						//Debug.Log("there are "+xc+" walls in this corner(x,z:"+(xx + t[d,sx])+","+(zz + t[d,sz] )+", corner:"+l[x]+",d:"+d);
						
						
		// check all walls in a corner in one of every 4 tiles:					
						for(var w : int = 0 ; w < xc ; w++){ 
					
							if(z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[w] != null) {
							
								e 		= z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[w].transform.name;
								
								idx 	= e.LastIndexOf("s");
								 
								idxe 	= e.LastIndexOf(".");
								
								if(idx > 0 && idxe > 0){
								 
									e 		= e.Substring( idx+1, ( idxe )-( idx+1 ) );
									nr 		= parseInt( e );
								
								} else {
									 
									nr = -1;
								 
								}
								
								if( nr == c ) { // if number of this checked wallset wall is the same as choosed wallset
									
									
									
									w = xc;
									
									//Debug.Log("wallSet is the same(nr:"+nr+", c:"+c+") d:"+d+" corner:"+l[x]);
									trigger = true;
									//Debug.Log("dd:"+dd+" d:"+d+" corner:"+ l[x] +"in tile:"+(xx + t[d,sx])+","+ (zz + t[d,sz])   +" nr:"+nr +"c(wallset number):"+c +"  - this wall is the same as you are trying to build. Not building(idx:"+idx+", idxe:"+ idxe+"). Name of checed corner: "+z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[w].transform.name);

		//the wall number in this corner is the same as the one you are trying to build. There might be a connector allready in this corner!
									
									var e1 : String;
									
									var nr1 : int;
									
									var nr2 : int;
									
									for(var s : int = 0 ; s < xc ; s++){
										if(z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ] && z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[s] !=null){
									
											e1 = z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[s].transform.name;
											
											if(e1.Substring(0,1) == "c"){
												
												//Debug.Log("Found connector object");
												
												nr1 = parseInt( e1.Substring(12,1) ); // [0 or 1 . 0 means its empty(should never happen)] 0 or 1 means that it was  passValue 1 or 6
												
												nr2 = parseInt( e1.Substring(16,1) ); //passValue of the made corner
												
												var sp : Vector3 = z.sortedTiles[ xx  , zz  ].tile.transform.localPosition;
									
												var bPos: Vector3;
												
												switch(d){
												
												case 0:
													sp.x = sp.x - (  z.tileSize/2.0f );
													sp.z = sp.z - ( z.tileSize/2.0f );
												break;
												case 1:
													sp.x = sp.x - (  z.tileSize/2.0f );
													sp.z = sp.z + (   z.tileSize/2.0f );
												break;
												case 2:
													sp.x = sp.x + (  z.tileSize/2.0f );
													sp.z = sp.z + (  z.tileSize/2.0f );
												break;
												case 3:
													sp.x = sp.x + (   z.tileSize/2.0f );
													sp.z = sp.z - (  z.tileSize/2.0f );
												break;
												default:
												break;
												}
												 
												
												
												// every corner is a child of empty container object, so I need to get the child position of that container:
											    for (var child : Transform in z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[s].transform ) 
											    {
											       bPos = z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[s].transform.localPosition + child.transform.localPosition;
											    }
												
												//Debug.Log("sp.x+ bPos.x="+sp.x+"+"+bPos.x +"="+(sp.x+ bPos.x) );
												
												//var childPos : Vector3 = new Vector3 (sp.x + bPos.x ,sp.y, sp.z + bPos.z );					
												
												var dist : float = Vector3.Distance(sp, bPos);
												
												//Debug.Log("d:"+d+" sp:"+sp+" bPos:"+bPos+" dist:"+dist);
												
												if(  dist < z.tileSize+(0.1*z.tileSize)  ){
													
													//Debug.Log("destroying corner: " +s+"("+z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[s].transform.name+")");
													z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[s].DestroyImmediate ( z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls[s] );
															
													z.sortedTiles[ xx + t[d,sx] , zz + t[d,sz] ].corners4[ l[x] ].walls.RemoveAt(s);
												} else {
												
													//Debug.Log("Done");
												
												}
											} 
										} 
										else {
										
											//Debug.Log("No walls in corner"+x);
										
										} 
									}
									
									
								} else if(nr != c && nr >= 0 && trigger == false) {
								
									//Debug.Log("No wallSet in this corner is the same as choosen("+c+") corner: "+d);
									//if there is no connectors in here, make new connectors:
									if( dd == 0 ) {
										if( p == 6 || p == 4 || p == 5){
											
											i.tokenBag[0] = 1;
												
										} else {
											
											i.tokenBag[0] = 2;
										
										}
									} else if( dd == 1 ) {
									
										if( p  == 6 ) {
											
											i.tokenBag[1] = 2;
											
										} else if( p == 1 ) {
										
											i.tokenBag[1] = 1;
										
										}
									}
									if(z.wallsSetList[c].conBools[nr] == false  ){
										//Debug.Log("turning off");
										i.tokenBag[0] = 0;
										i.tokenBag[1] = 0;
							
									}
								}
							}    
						}    
						if(xc == 0){
						
						//no walls in this corner, but tile exists. Automatic conector!
							//Debug.Log("No walls in corner"+l[x]+" in tile! But there is a tile.Shoud make connector regardles...");
//							if(i.tokenBag[dd] == 0){
//							
//								if( dd == 0 ) {
//									if( p == 1 || p == 5 || p == 4 ){
//										
//										i.tokenBag[dd] = 1;
//											
//									} else {
//										
//										i.tokenBag[dd] = 2;
//									
//									}
//								} else if( dd == 1 ) {
//									
//									if( p  == 6 ) {
//										
//										i.tokenBag[dd] = 1;
//										
//									} else if( p == 1 ) {
//									
//										i.tokenBag[dd] = 2;
//									
//									}
//								}
//							}
						}
					}
				}  
					
				//}catch(err){
				
					//Debug.Log(err);
				
				//}
				
				t[d,sx] += tx;
				t[d,sz] += tz;
				
				
					var tt : int = tx;
					tx = tz;
					tz = -tt;
				
			
			}
			tx = 0;
			tz = 1;
			
			sx = 2;
			sz = 3;
		}
		if(trigger == true){
			
			i.tokenBag[0] = 0;
			i.tokenBag[1] = 0;
		
		
		}
		return i;
		
	}
	
	
	function basicTextureMerge (pX : int , pZ : int, z : TGLevel, t : Texture2D ) { 
	
		//check tiles around me:
		//correct for first tile: start from -1,-1
		if(pX <0){
			pX = 0;
		}
		
		if(pZ < 0){
			pZ = 0;
		}
		
		if( z.sortedTiles.GetLength(0) == 1 && z.sortedTiles.GetLength(1) == 1 ){
			pX = 0;
			pZ = 0;
		}
		
		//Initiate base values for search:
		
		var ticker : int = 1;
		
		var pXs : int;
		var pZs : int;
	
			pXs = pX -1;
			pZs = pZ -1;
	
		var pXc : int = 0;
		var pZc : int = 1;

		var tileName : String;
		
		//var tileRefList : List.<Material> = new List.<Material>();
		var tileRefList : Material[] = new Material[8];
		
		for( var x : int = 0 ; x < 8 ; x++){
		
			//check if tile is inside array of tiles:
			if( (pXs) >= 0 && (pZs) >= 0 && (pXs) < z.sortedTiles.GetLength(0) && (pZs) < z.sortedTiles.GetLength(1) ){
				
				if(z.sortedTiles[pXs,pZs]){
					
					var c2: String = z.sortedTiles[ pXs,pZs ].tile.transform.name;
					
					var p :int = c2.LastIndexOf(":");
					
					tileName = tileName + c2.Substring(0,p);
					
					tileRefList[x] = z.sortedTiles[pXs,pZs].tile.GetComponent.<Renderer>().sharedMaterial;

					var c1: String = z.sortedTiles[ pX,pZ ].tile.transform.name;
					
				}
			}
			
			tileName = tileName + "I";
			
			//change coordinates of next checked tile:
			if(x%2==0 && x != 0){ // x is even ((0),2,4,6)
				var pt: int = pXc;
				pXc = pZc; 
				pZc = -pt;
			}
			pXs += pXc;
			pZs += pZc;
		}
		
		if(tileName == "IIIIIIII"){
		
			return;
		
		}
		
		var tempMaterial : Material;
			
		var tex : Texture2D; 
		
		//if material is not in the array, create new material and add it to the array( dictionary)
//		if( z.procTexDictionaryCheck(tileName, z) == false ){
//		
//			//create material and add to assets and dictionary:
//			tempMaterial = new Material(z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial);
//			
//			AssetDatabase.CreateAsset(tempMaterial, "Assets/SHAZAM/data/textureMergeTextures/"+tileName+".mat");
//			
//			AssetDatabase.Refresh();
//			
//			//create texture and add to assets and dictionary:
//			tex = new Texture2D(tempMaterial.mainTexture.width, tempMaterial.mainTexture.height, TextureFormat.ARGB32, false);
//			
//			tex = z.generateTexture( z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial , tileRefList , t , z , tex);
//			
//			if(tex != null){
//			
//				var bytes : byte[] = tex.EncodeToPNG();
//				
//				File.WriteAllBytes("Assets/SHAZAM/data/textureMergeTextures/"+tileName+".png", bytes);
//				
//				AssetDatabase.Refresh();
//				
//				tempMaterial.mainTexture = null;
//				
//				tempMaterial.mainTexture = AssetDatabase.LoadAssetAtPath("Assets/SHAZAM/data/textureMergeTextures/"+tileName+".png", Texture2D);
//				
//				tex.DestroyImmediate(tex);
//				
//				
//			}
//			
//			z.procTexDictionary.Add(tileName,tempMaterial);
//			
//			z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial = tempMaterial;
//			
//		} else{
//		
//			Debug.Log("Retrieved asset from dictionary!");
//			
//			tempMaterial = z.procTexDictionary[tileName];
//			
//			z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial = tempMaterial;
//		}
		
		
//		if(z.procTexDictionaryCheck(tileName, z) == false && tileName !="........"){
//
//			tempMaterial = new Material(z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial);
//			
//			tex = new Texture2D(tempMaterial.mainTexture.width, tempMaterial.mainTexture.height, TextureFormat.ARGB32, false);
//			//create texture here. Add material to dictionary
//			//Debug.Log(tileName+" oryginal tex:"+tempMaterial.mainTexture);
//			tex = z.generateTexture( z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial , tileRefList , t , z , tex);
//			//...
//			
//			//also add material to aseets:
//			                               
//			//...
//			if(tex != null){
//			
//				var bytes : byte[] = tex.EncodeToPNG();
//				File.WriteAllBytes("Assets/Editor/SHAZAM/"+tileName+".png", bytes);
//				tex.DestroyImmediate(tex);
//				AssetDatabase.Refresh();
//				tempMaterial.mainTexture = AssetDatabase.LoadAssetAtPath("Assets/Editor/SHAZAM/"+tileName+".png", Texture2D);
//				//Debug.Log(tempMaterial.mainTexture.name);
//				z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial = tempMaterial;
//			} else {
//			
//				//tempMaterial.DestroyImmediate(tempMaterial);
//			
//			}
//			z.procTexDictionary.Add(tileName,tempMaterial);
//			
//		}
//		
//		else if(z.procTexDictionaryCheck(tileName, z) == true && tileName !="........") {
//			Debug.Log(tileName);
//			//assign material and texture from the dictionary to the tile
//			tempMaterial = procTexDictionaryGet(tileName, z, tempMaterial);
//			
//			z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial = tempMaterial;
//		}
//		var tex : Texture2D = z.procTexDictionaryCheck(tileName, z);
//		
//		if( tex == null ){
//		
//			tex = z.generateTexture( z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial , tileRefList , t , z );
//			
//			z.procTexDictionary.Add(tileName,tex);
//			
//			var material = new Material (Shader.Find("Diffuse"));
//			
//			AssetDatabase.CreateAsset(material, "Assets/" + Selection.activeGameObject.name + ".mat");
//			
//			material.mainTexture = tex;
//			
//			z.sortedTiles[pX,pZ].tile.renderer.sharedMaterial = material;
//		
//		} else {
//		
//			//get that texture to this new material?
//		
//		}
		
	}
	
	
	function procTexDictionaryGet(t : String , z : TGLevel, tex: Material ) : Material {
	
		if(!z.procTexDictionary){
		
			z.procTexDictionary = new Dictionary.<String, Material>();
			
		}
		if(z.procTexDictionary.ContainsKey(t)){
		
			tex = z.procTexDictionary[t];
		
		}
		return tex;
	}
	
	
	function procTexDictionaryCheck(t : String , z : TGLevel ) : boolean {
		
		var b : boolean;
		
		//var tTex : Texture2D;
		
		if(!z.procTexDictionary){
		
			z.procTexDictionary = new Dictionary.<String, Material>();
			
		}
		
		if(z.procTexDictionary.ContainsKey(t)){
		
			//tTex = z.procTexDictionary[t];
			Debug.Log("Dictionary contains the same texture as that needed to be made here ("+ t+")");
			b = true;
		
		} else {
		}
		
		return b;
	
	}
	
//	
//	function checkAssets(){
//	}
//	
//	
//	function saveToAsset(){
//	}
	
	
	function generateTexture(t: Material , t2: Material[] , t3 : Texture2D , z : TGLevel , textureToChange : Texture2D) : Texture2D {
	
		var brake : boolean;
		
		var virgin : boolean;
		
		for(var x : int = 0 ; x < z.poceduralTextures.Count  ; x++){
			
			// stop, if texture from procedural is highier than the texture from original texture
			if(z.poceduralTextures[x].name == t.name){
				
				brake = true;
				
				if(virgin == false){
				
					textureToChange = null;
				
				}
				break;
				//Debug.Log("brake at "+x);
			}
			
			for(var y : int = 0 ; y < t2.Length ; y++){
				
				if(t2[y] != null && brake == false){
				
					//if( t2[y].mainTexture.name == z.poceduralTextures[x].name || t2[y].mainTexture.name+"Clone" == z.poceduralTextures[x].name ){
					
						//if(t != t2[y]){
						var tex : Texture2D;
							for(var d: int = 0 ; d < z.brushSetList.Count ; d++){
							
								for(var d1 : int = 0 ; d1 < z.brushSetList[d].tilesGOs.Count ; d1++){
								
									if(z.brushSetList[d].tilesGOs[d1] != null && z.brushSetList[d].tilesGOs[d1].GetComponent.<Renderer>() !=null && z.brushSetList[d].tilesGOs[d1].GetComponent.<Renderer>().sharedMaterial != null){ //&& z.brushSetList[d].tilesGOs[d1]renderer != null && z.brushSetList[d].tilesGOs[d1].renderer.sharedMaterial != null
									
										if( z.brushSetList[d].tilesGOs[d1].GetComponent.<Renderer>().sharedMaterial.name ==  t2[y].name){
											tex = z.brushSetList[d].tilesGOs[d1].GetComponent.<Renderer>().sharedMaterial.mainTexture as Texture2D;
										
										}
									
									}
								
								}
							}
							
							//Debug.Log("Joining"+t2[y].name);
							if(tex !=null){
								textureToChange = z.algoTextures(y,t.mainTexture,tex,t3,textureToChange);
							virgin = true;
							}
						//}
					//}
				}
			}
		}
		if(virgin == false){
			
			//textureToChange = z.algoTextures(y,t.mainTexture,textureToChange,t3,textureToChange);
		
		}
		//Debug.Log(textureToChange.name+" and main texture is:"+t.mainTexture.name);
		return textureToChange;
	}
	
	
	function algoTextures( nr:int , t: Texture2D , t2: Texture2D , t3 : Texture2D, textureToChange : Texture2D ) : Texture2D { // t - texture of made tile ; t2 - texture of checked(bordering) tile ; nr - number of tile checked ; t3 - alpha blend texture

		if(t3 == null){
			
			Debug.Log("No alpha texture.Aborting merge.");
			return textureToChange;
		
		}
		//var t0 : Texture2D = new Texture2D (t.width, t.height);
		
		//resize blend texture ONLY if it is bigger than the source texture.
		//otherwise multiply choosen part of it to fit bigger texture(only straight parts)
		
		//var texture : Texture2D = Instantiate(renderer.material.mainTexture); - THIS IS FOR DUPLICATING TEXTURE(ALPHA one)
		
		
		//set y and yEnd as needed(based on nr value)
		var xStart:int;
		var xWidth: int;
		var yStart : int;
		var yHeight : int;
		
		var setX : int;
		var setY : int;
		var setW : int;
		var setH : int;
		
		var setWs: int;
		var setHs: int;
		
		var cols : Color[]; // my tile (get part)
		var cols2 : Color[]; // border tile(get part)
		var colsA : Color[]; // alpha tile (get part)
		
		var multiplier: int = 1;
		
		var cols3 : Color[] = t.GetPixels();
		
		textureToChange.SetPixels( cols3 );
		
		switch ( nr ){
		
			case 0:
				cols  = t.GetPixels(0,384,128,128,0);
				cols2 = t2.GetPixels(384,0,128,128,0);
				colsA = t3.GetPixels(0,384,128,128,0);
				
				xStart = 0; 	setX = 0;
				yStart = 384;	setY = 384;	
				xWidth = 384;	setW = 128;	
				yHeight = 128;	setH = 128;	
				
				setWs = 128;
				setHs = 128;
				
				cols2.Reverse(cols2);
				
			break;
			
			case 1:
				cols  = t.GetPixels(0,384,512,128,0);
				cols2 = t2.GetPixels(0,0,512,128,0);
				colsA = t3.GetPixels(128,384,256,128,0);
				
				xStart = 0;					setX = 0;
				yStart = 384; 				setY = 384;
				xWidth = 256;				setW = 512;
				yHeight = 128;				setH = 128;
				multiplier = 2;
				
				setWs = 512;
				setHs = 128;

			break;
			
			case 2:
				cols  = t.GetPixels(384,384,128,128,0);
				cols2 = t2.GetPixels(0,0,128,128,0);
				colsA = t3.GetPixels(384,384,128,128,0);
				
				xStart = 384;	setX = 384;
				yStart = 384;	setY = 384;
				xWidth = 128;	setW = 128;
				yHeight = 128;	setH = 128;
				
				setWs = 128;
				setHs = 128;
				
				cols2.Reverse(cols2);
			
			break;
			
			case 3:
				cols  = t.GetPixels(384,0,128,512,0);
				cols2 = t2.GetPixels(0,0,128,512,0);
				colsA = t3.GetPixels(384,128,128,256,0);
				
				xStart = 384;	setX = 384;
				yStart = 128;	setY = 0;
				xWidth = 128;	setW = 128;
				yHeight = 256;	setH = 512;
				multiplier = 2;
				
				setWs = 128;
				setHs = 512;
				
				cols2.Reverse(cols2);
			break;
			
			case 4:
				cols  = t.GetPixels(0,384,128,128,0);
				cols2 = t2.GetPixels(0,384,128,128,0);
				colsA = t3.GetPixels(384,0,128,128,0);
				
				xStart = 384;	setX = 384;
				yStart = 0;		setY = 0;
				xWidth = 128;	setW = 128;
				yHeight = 128;	setH = 128;
				
				setWs = 128;
				setHs = 128;
				
				cols2.Reverse(cols2);
				
			break;
			
			case 5:
				cols  = t.GetPixels(0,0,512,128,0);
				cols2 = t2.GetPixels(0,384,512,128,0);
				colsA = t3.GetPixels(128,0,256,128,0);
				
				xStart = 128;	setX = 0;
				yStart = 0;		setY = 0;
				xWidth = 256;	setW = 512;
				yHeight = 128;	setH = 128;
				multiplier = 2;
				
				setWs = 512;
				setHs = 128;
				
			break;
			
			case 6:
				cols  = t.GetPixels(0,0,128,128,0);
				cols2 = t2.GetPixels(384,384,128,128,0);
				colsA = t3.GetPixels(0,0,128,128,0);
				
				xStart = 0;		setX = 0;
				yStart = 0;		setY = 0;
				xWidth = 128;	setW = 128;
				yHeight = 128;	setH = 128;
				
				setWs = 128;
				setHs = 128;
				
				cols2.Reverse(cols2);
				
			break;
			
			case 7:
				cols  = t.GetPixels(0,0,128,512,0);
				cols2 = t2.GetPixels(384,0,128,512,0);
				colsA = t3.GetPixels(0,128,128,256,0);
				
				xStart = 0;		setX = 0;
				yStart = 128;	setY = 0;
				xWidth = 128;	setW = 128;
				yHeight = 256;	setH = 512;
				multiplier = 2;
				
				setWs = 128;
				setHs = 512;
				
				cols2.Reverse(cols2);
			break;
						
			default:
	
			break;
		}
		
		//reversing ( one side revedrse, not full reverse) the adjacent tile texture block( mirror image )

		var colsTr0 : Color[] = new Color[cols2.Length];
		
		for(var r0 : int = 0 ; r0 < setHs ; r0++){
		
			colsTr0.Copy(cols2,r0*setWs,colsTr0,( ((setHs-1)*setWs)-(setWs*r0) ),setWs);
		
		}
		
		cols2 = colsTr0;

		if(multiplier == 2){
		
			var colsT : Color[] = new Color[colsA.Length*2];
			
			for(var c : int = 0 ; c < yHeight; c++){
	
				colsT.Copy(colsA,(c)*xWidth,colsT,((c*2)*xWidth),xWidth);
				colsT.Copy(colsA,(c)*xWidth,colsT,(((c*2)+1)*xWidth),xWidth);
			
			}

			colsA = colsT;

		}
		
		for (var y : int = 0; y < colsA.Length; ++y) {
	
			cols3[y] =  Color.Lerp( cols[y], cols2[y], colsA[y].a );
		
		}
		
		textureToChange.SetPixels(setX,setY,setW,setH, cols3 , 0);
		
		textureToChange.Apply( false );
		
		//Debug.Log("finished:"+nr+" cols.l:"+cols.Length+" cols2.l:"+cols2.Length+ " colsA.l:"+colsA.Length+" cols3.l:"+cols3.Length);
		
		//t0.filterMode = FilterMode.Point;
		
		return textureToChange;
	}
	
//	function changeMainState(e: int , x : SHAZAMTileGenerator){ // e is state: 0 -nothing 1 - editing 2 - cleaning
//	
//		
//		if(e == 1){
//		
//			x.editing = true;
//			x.cleaning = false;
//		
//		} 
//		if(e == 2){
//		
//			x.editing = false;
//			x.cleaning = true;
//		
//		}
//
//	}
} 


public class BrushSet
{
	var name : String;
	var isActive : boolean;
	var activeTileGO : int;
	var random : boolean;
	var randomTileRot : boolean;
	var conTex : boolean;
	var tilesGOs: List.<GameObject>;
	//options for wallSets:
	var strength : float; 
	var grNr : int;
	var doUpper: boolean;
	var doLower:boolean;
	
	var opt1 : int;
	var opt2 : int;
	var fold : boolean;
	var fold2 : boolean;

}




public class BrushSetWall extends BrushSet 
{
	var specialsList : List.<specials>; // this is for selecting for what tile create this wall.
	//var conBools : List.<specials>; //list of other wallSets for connector checking
	var conBools : List.<boolean>;
}

public class specials
{
	var sPlist: List.<boolean>;
}

public class intBag
{
	var tokenBag : List.<int>;
}

public class SortedTile 
{
	var tile : GameObject;
	var corners4 : Corners[]; //always 4
}

public class Corners 
{ 
	var walls : List.<GameObject>;
	
	var whatWallSet : List.<boolean>; // set to true if something is inside( if walls list Count is >0 
	
	var neighbourWalls : List.<boolean>;
}

//public class Bag 
//{
//	var bagNr : int;
//	var tokens : List.<int>;
//	var alwaysList : List.<int>; // nrs of wallsets with str = 1. Doesn't matter if in grup or not. If added in here, dont add in tokens when in grup
//	var tempTokens : List.<int>;
//}

//public class BagOfGOs
//{ 
//	var bagNr : int;
//	var tokens : List.<BrushSet>;
//}

public class SHModules
{
	var mod1 : Sh_modTexMerge;
}


//VARIABLES:::::::::::::::::::::::::::::::::::::::::::::::
//var testCube : GameObject;
 @SerializeField @HideInInspector
var levels : List.<TGLevel>;


@HideInInspector @SerializeField
var activeLevel: int;

@HideInInspector
var sortedLengthX: int;

@HideInInspector
var sortedLengthZ: int;

@HideInInspector
var placementPoint : Vector3;

@SerializeField @HideInInspector
var activeBrushSet : int;

@SerializeField @HideInInspector
var lastActiveWallSet : int;

@SerializeField @HideInInspector
var wallSetClipBoard : BrushSet[];

@HideInInspector
var oldSortedTiles : SortedTile[,];

@HideInInspector
var sortedOffsetX : int;

@HideInInspector
var sortedOffsetZ : int;

@HideInInspector
var nPos : Vector3;

@HideInInspector
var zxc : Vector3;

@HideInInspector
var ghostTilePos : Vector3;

@SerializeField @HideInInspector
var editing: boolean;

@SerializeField @HideInInspector 
var cleaning : boolean;

@HideInInspector
var smallestFluctChangeX : int; //this is for helping repopulate new sortedTiles array when a tile was deleted. Its for when a tile is deleted and array is smaller than the previous one.
@HideInInspector
var smallestFluctChangeZ : int;
#if UNITY_EDITOR
@HideInInspector
var tStyle : GUIStyle; // small buttons(minus) style
@HideInInspector
var t2Style : GUIStyle; // normal button style;
#endif
@HideInInspector
var tMergeTex : Texture2D;



function Awake(){

	activeBrushSet = 900;
	if(levels.Count <=0) {
		var i: TGLevel;
		levels.Add(i);
	}
}

//		-------------------------------------				//
//				FUNCTIONS LAND BELOW						//
//		-------------------------------------				//
//			 (should be in main class later)		
/*----------------------------------------------------


				LEVELS OPERATIONS:					
					
*/													
//called when clicking on the level toggle.lvlNr is the t from loop, because "levels" list should always be sorted
//function SetActiveLevel(lvlNr : int) {
//
//	if(levels[lvlNr].isActive == true) {
//		
//		Debug.Log("Level "+lvlNr+" is already active!");
//	}
//	else {
//		
//		levels[lvlNr].isActive = !levels[lvlNr].isActive;
//	}
//	for(var x : int = 0 ; x<levels.Count ; x++) {
//		
//		if(x!=lvlNr) {
//			
//			levels[x].isActive = false;
//		}
//	}
//	activeLevel = lvlNr;
//	//count the absoluteLvlHeight:
//	levels[lvlNr].absoluteLvlHeight = FindAbsoluteHeight(lvlNr);
//	Debug.Log("Setting level. Absolute height = "+levels[lvlNr].absoluteLvlHeight);
//	try{
//		ReGetTiles(lvlNr);
//	} catch (err){
//	
//		Debug.Log(err);
//	
//	}
//	
//}

//returns absolute height of this level from the 0 level:
function FindAbsoluteHeight(actveLvlNr : int) : float {
	var absHeight : float;
	var zeroLvlIndex : int;
	//find 0 level:
	for(var y : int = 0 ; y<levels.Count ; y++) {
		if(levels[y].lvlNr == 0) {
			zeroLvlIndex = y;
			Debug.Log("FINDING ABSOLUTE HEIGHT:::: level '0' is at:"+y+" and active is at:"+actveLvlNr);
		}
	}
	//witch way to cout?:
	var start : int;
	var finish : int;
	var naliczanie : int;
	
	if(actveLvlNr > zeroLvlIndex) { 
		start = zeroLvlIndex;
		finish = actveLvlNr;
		for(var x : int = start ; x<finish ; x++) {
			absHeight += levels[x].lvlHeight;
			naliczanie++;
			Debug.Log("Dodaje height levelu:"+ levels[x].lvlNr +":"+levels[x].lvlHeight +" do elementu"+actveLvlNr+" listy");
		}
	}
	else if(actveLvlNr < zeroLvlIndex) {
		start = actveLvlNr;
		finish = zeroLvlIndex;
		for(var x1 : int = start ; x1<finish ; x1++) {
			absHeight -= levels[x1].lvlHeight;
			naliczanie++;
			Debug.Log("Dodaje height levelu:"+ levels[x1].lvlNr);
		}
	}
	else if(actveLvlNr == zeroLvlIndex){
		Debug.Log("This level is level 0");
		return 0;
	}	
	return absHeight;
}

//function AddLowerLevel(active : int, lvlNr : int){ //this number is actual index in the levels array passed from the controller(active one)
////check how to add this level:
//	var l : TGLevel = new TGLevel();
//	var name:String;
//	//check if selected LEVEL NUMBER is smaller than 0: 
//	if(levels[active].lvlNr<=0) {
//		
//		//if selected index is not 0(out of range error otherwise):
//		if(active!=0) {
//			//change numbers of previous ones:
//			for(var i : int = 0 ; i<active ; i++) {
//				levels[i].lvlNr -=1;
//				//and change names of previous containers:
//				name = "TileContainerL:"+levels[i].lvlNr.ToString();
//				levels[i].unsortedContainer.name = name;
//				//and change their container(and everything that is inside) Y Position:
//			}
//			//add:
//			l.lvlNr = lvlNr-1;
//			l.tileSize = 1;
//			l.lvlHeight = 1;
//			levels.Insert(active,l);
//			MakeLVLContainer(active,levels[active].lvlNr);
//			
//		
//		//if there is nothing before in the List(INDEX = 0):
//		} else {
//			l.lvlNr = lvlNr-1;
//			l.tileSize = 1;
//			l.lvlHeight = 1;
//			levels.Insert(active,l);
//			MakeLVLContainer(active,levels[active].lvlNr);
//		}
//	} else {
//		if(active+1 != levels.Count) {
//			for(var i2 : int = active ; i2<levels.Count ; i2++) {
//				levels[i2].lvlNr +=1;
//				name = "TileContainerL:"+levels[i].lvlNr.ToString();
//				levels[i].unsortedContainer.name = name;
//			}
//			l.lvlNr = lvlNr;
//			l.tileSize = 1;
//			l.lvlHeight = 1;
//			levels.Insert(active,l);
//			MakeLVLContainer(active,levels[active].lvlNr);
//		}
//		else {
//			levels[active].lvlNr +=1;
//			
//			l.lvlNr = lvlNr;
//			l.tileSize = 1;
//			l.lvlHeight = 1;
//			levels.Insert(active,l);
//			MakeLVLContainer(active,levels[active].lvlNr);
//		}
//	}
//}

//function AddHighierLevel(active : int, lvlNr : int){
//	var l : TGLevel = new TGLevel();
//	var name:String;
//	
//	if(levels[active].lvlNr>=0) {// if adding a level highier than active after 0 level:
//		if(active+1!=levels.Count) {//if there is something after this one:
//			for(var i : int = active+1 ; i<levels.Count ; i++) {
//				levels[i].lvlNr +=1;
//				name = "TileContainerL:"+levels[i].lvlNr.ToString();
//				levels[i].unsortedContainer.name = name;
//			}
//			l.lvlNr = lvlNr+1;
//			l.tileSize = 1;
//			l.lvlHeight = 1;
//			levels.Insert(active+1,l);
//			MakeLVLContainer(active+1,levels[active].lvlNr+1);
//		} else {
//			l.lvlNr = lvlNr+1;
//			l.tileSize = 1;
//			l.lvlHeight = 1;
//			levels.Add(l);
//			MakeLVLContainer(active+1,levels[active].lvlNr+1);
//		}
//	} else 
//	//if active button nr is lower than 0(-1 and lower) change next buttons numbers to from this one to button with button nr -1(lower than 0)
//		if(active!=0) { //if it is not the last one:
//			for(var i2 : int = 0 ; i2<active+1 ; i2++) {
//				levels[i2].lvlNr -=1;
//				name = "TileContainerL:"+levels[i2].lvlNr.ToString();
//				levels[i2].unsortedContainer.name = name;
//			}
//			l.lvlNr = lvlNr;
//			l.tileSize = 1;
//			l.lvlHeight = 1;
//			levels.Insert(active+1,l);
//			MakeLVLContainer(active+1,levels[active].lvlNr+1);
//		}
//		else {
//			levels[active].lvlNr--;
//			name = "TileContainerL:"+levels[active].lvlNr.ToString();
//			levels[active].unsortedContainer.name = name;
//			l.lvlNr = lvlNr;
//			l.tileSize = 1;
//			l.lvlHeight = 1;
//			levels.Insert(active+1,l);
//			MakeLVLContainer(active+1,levels[active].lvlNr+1);
//		}
//}

//function RemoveLevel(active : int, lvlNr : int) {
//	var name:String;
//	
//	if(lvlNr == 0) {
//		Debug.Log("Level 0 cannot be deleted! Try adding empty in here or editing 0 level tiles for desired effect.");
//	}
//	if(lvlNr < 0) {
//	
//		for(var i : int = 0 ; i<active+1 ; i++) {
//			levels[i].lvlNr +=1;
//			name = "TileContainerL:"+levels[i].lvlNr.ToString();
//			levels[i].unsortedContainer.name = name;
//		}
//		//and tiles? Where will they go? Delete them all!!!
//		DestroyImmediate(levels[active].unsortedContainer);
//		DestroyImmediate(levels[active].generatedContainer);
//		levels.RemoveAt(active);
//		SetActiveLevel(active);
//	}
//	if(lvlNr > 0) {
//		for(var i2 : int = active ; i2<levels.Count ; i2++) {
//			levels[i2].lvlNr -=1;
//			name = "TileContainerL:"+levels[i].lvlNr.ToString();
//			levels[i].unsortedContainer.name = name;
//		}
//		DestroyImmediate(levels[active].unsortedContainer);
//		DestroyImmediate(levels[active].generatedContainer);
//		levels.RemoveAt(active);
//		SetActiveLevel(active-1);
//	}
//}

function MakeLVLContainer(lvlNr: int, nameNr:int) {
	var name : String = "TileContainerL:"+nameNr.ToString();
	var tileContainer : GameObject = new GameObject(name);
	tileContainer.transform.position = transform.position;
	tileContainer.transform.parent = transform;
	levels[lvlNr].unsortedContainer = tileContainer;
	
}

function ChangeLVLContents() {

}

/*----------------------------------------------------


				PRE GATHERING CHECKS:					
					


*/
	
/*----------------------------------------------------


				GATHERING:					
					


*/
// Get all children in this level and add to unsorted array if number doeasnt match last gathered:
function ReGetTiles(levelNr:int){
	
	var children : List.<GameObject> = new List.<GameObject>();
	
	for(var child : Transform in levels[levelNr].unsortedContainer.transform) {
		
		children.Add(child.gameObject);
		  
	}
	
	//check if nr of last gathered tiles is different than those gathered here as children. If yes, then recreate:
	var oldNr: int = levels[levelNr].unsortedTiles.Count;
	
	if(oldNr != children.Count) {

		levels[levelNr].unsortedTiles = new List.<GameObject>();
		levels[levelNr].unsortedTiles = children;

	} else {

		levels[levelNr].unsortedTiles = children;
		
	}
	
	
	ReSortTiles(levelNr);
	//Debug.Log("RegetTiles complete. nr of children in unsortedContainer:"+children.Count+" nr of children in unsorted tiles:"+levels[levelNr].unsortedTiles.Count);
} 


function RecolectGenereted(levelNr:int){

	var children : List.<GameObject> = new List.<GameObject>();

	if(!levels[levelNr].generatedContainer){
	//first check if the container is already in the scene!
	
	//...
	 
	//if not, then make one!
		//Debug.Log("There is no generatedContainer. Why?");
		var name : String;
		var genContainer : GameObject;
		if(!levels[levelNr].generatedContainer) {
			name = "Generated.L"+levelNr.ToString();
			genContainer = new GameObject(name);
			genContainer.transform.position = transform.position;
			genContainer.transform.parent = transform;
			levels[levelNr].generatedContainer = genContainer;
		}
	}  
	
	levels[levelNr].unsortedGenerated = new List.<GameObject>();
	
	for(var child : Transform in levels[levelNr].generatedContainer.transform) {
		
		levels[levelNr].unsortedGenerated.Add(child.gameObject);
		 
	} 
	
//	var oldNr: int = levels[levelNr].unsortedGenerated.Count;
//	
//	Debug.Log("there is "+children.Count+" children in generatedContainer(walls and connectors) oldNr is"+oldNr);
//	
//	
//	
//	if(oldNr < children.Count) {
//		  
//		levels[levelNr].unsortedGenerated = new List.<GameObject>();
//		
//		for(var c : int = 0 ; c < children.Count ; c++){
//		
//			if( c < levels[levelNr].unsortedGenerated.Count ){
//				
//				levels[levelNr].unsortedGenerated[c] = children[c];
//			
//			} else {
//			    
//				levels[levelNr].unsortedGenerated.Add( children[c] );
//			
//			}
//			
//			Debug.Log(levels[levelNr].unsortedGenerated[c]);
//		}  
//	}  
	if(!levels[levelNr].unsortedGenerated){
	  
		//Debug.Log("No 'unsortedGeneraited' container");
	
	}   
	//now sort those and fill the sortedTiles corner walls lists with those.
	for(var y : int = 0 ; y<  levels[levelNr].unsortedGenerated.Count ; y++) {
		
		if(levels[levelNr].unsortedGenerated[y] == null){
		
			levels[levelNr].unsortedGenerated[y] =  children[y];
		 
		}
		
		var iXN: float = Mathf.Abs(levels[levelNr].unsortedGenerated[y].transform.localPosition.x-levels[levelNr].smallestSortedX)/levels[levelNr].tileSize;
		
		var iZN: float = Mathf.Abs(levels[levelNr].unsortedGenerated[y].transform.localPosition.z-levels[levelNr].smallestSortedZ)/levels[levelNr].tileSize;
			
			iXN = Mathf.Floor(iXN * 10) / 10;
			
			iZN = Mathf.Floor(iZN * 10) / 10;  
		 
		var iX : int = Mathf.RoundToInt(iXN); 
		 
		var iZ : int = Mathf.RoundToInt(iZN); 
		
// collect walls based on their name
	//d value(one of 4 corners of a tile):	
		var dIndicator : String = levels[levelNr].unsortedGenerated[y].transform.name;
		
		var idx : int = dIndicator.LastIndexOf("d");
		
		if( idx != 0){
		
			dIndicator = dIndicator.Substring(idx+1);

			var nr: int = parseInt(dIndicator);

			levels[levelNr].sortedTiles[iX,iZ].corners4[nr].walls.Add(levels[levelNr].unsortedGenerated[y]);
			
			//Debug.Log("added a wall to sortedTiles"+iX+","+iZ+" corner"+nr+"("+levels[levelNr].unsortedGenerated[y].transform.name+")");
		
		} else {
		
			Debug.Log("IDX in gatering is 0! Some unknown objects are in the generated container.");
		  
		}
	}   
	//Debug.Log("finished resorting. ");
} 
 
/*----------------------------------------------------
   

				SORTING:					
					 


*/
function ReSortTiles(levelNr:int) {
	
	var unsortedTilesCount : int = levels[levelNr].unsortedTiles.Count;
	if(unsortedTilesCount <= 0) {
		//Debug.Log("ReSortTiles::::::No tiles to sort!");
		levels[levelNr].sortedTiles = new SortedTile[0,0];
		return;
	}
	var biggestX : float = Mathf.NegativeInfinity;
	var biggestZ : float = Mathf.NegativeInfinity;
	var smallestX : float = Mathf.Infinity;
	var smallestZ : float = Mathf.Infinity;
	
	var newX : float;
	var newZ : float;
	
	
	
	for(var x : int = 0 ; x< unsortedTilesCount ; x++) {

			if(levels[levelNr].unsortedTiles[x].transform.localPosition.x < smallestX) {
				smallestX = levels[levelNr].unsortedTiles[x].transform.localPosition.x;
			}
			if(levels[levelNr].unsortedTiles[x].transform.localPosition.z < smallestZ) {
				smallestZ = levels[levelNr].unsortedTiles[x].transform.localPosition.z;
			}
			if(levels[levelNr].unsortedTiles[x].transform.localPosition.z > biggestZ) {
				biggestZ = levels[levelNr].unsortedTiles[x].transform.localPosition.z;
			}
			if(levels[levelNr].unsortedTiles[x].transform.localPosition.x > biggestX) {
				biggestX = levels[levelNr].unsortedTiles[x].transform.localPosition.x;
			}
		
	}
	
	if(smallestX<=0) { 
		
		newX = Mathf.Abs((Mathf.Abs(smallestX)+biggestX)/levels[levelNr].tileSize )+1.0f;
		//Debug.Log("ReSortTiles::::::COUNTING SMALLEST::::Found smallestX<=0:"+smallestX+", biggestX:"+biggestX+", sx/bx:"+(Mathf.Abs(smallestX)+biggestX)+ "; smX+bsX/tilesize:"+((Mathf.Abs(smallestX)+biggestX)/levels[levelNr].tileSize)+"+1. And newX:"+newX);
		//Debug.Log(xx+" Sorting... Found smallestX<=0: sX: "+smallestX+",bx: "+biggestX+ "sx/bx="+(Mathf.Abs(smallestX)+biggestX)+ "; smX+bsX/tilesize: "+ ((Mathf.Abs(smallestX)+biggestX)/tileSize)  );
	} else {
		newX = (Mathf.Abs( smallestX-biggestX )/levels[levelNr].tileSize)+1.0f;
	}
	if(smallestZ<=0) {
		newZ = Mathf.Abs((Mathf.Abs(smallestZ)+biggestZ)/levels[levelNr].tileSize )+1.0f;
	} else {
		newZ = (Mathf.Abs( smallestZ-biggestZ )/levels[levelNr].tileSize)+1.0f;
	}
	//Debug.Log("newX before changing to int:"+newX+" type:"+newX.GetType());
	//var xxInt : int = Mathf.CeilToInt(newX);
	//var zzInt : int = Mathf.CeilToInt(newZ);
	
	newX = Mathf.Floor(newX * 10) / 10;
	newZ = Mathf.Floor(newZ * 10) / 10; 
	//var xxInt : int;
	//var zzInt : int;
	var xxInt : int = Mathf.RoundToInt(newX);
	var zzInt : int = Mathf.RoundToInt(newZ);
	
	if(levels[levelNr].smallestSortedX< smallestX){
		//Debug.Log("smallest x is changed! from:"+levels[levelNr].smallestSortedX+" to "+smallestX+" x/z are: "+xxInt+","+zzInt);
		smallestFluctChangeX = Mathf.Abs(levels[levelNr].smallestSortedX-Mathf.RoundToInt(smallestX))/levels[levelNr].tileSize;
		
	}
	if(levels[levelNr].smallestSortedZ< smallestZ) {
		//Debug.Log("smallest z is changed! from:"+levels[levelNr].smallestSortedZ+" to "+smallestZ+" x/z are: "+xxInt+","+zzInt);
		smallestFluctChangeZ = Mathf.Abs(levels[levelNr].smallestSortedZ-Mathf.RoundToInt(smallestZ))/levels[levelNr].tileSize;
		
	}
	
	//Debug.Log("newX after changing to int:"+newX+" type:"+newX.GetType());
	//newX = xxInt;
	//newZ = zzInt;
	
	levels[levelNr].smallestSortedX = Mathf.RoundToInt(smallestX);
	levels[levelNr].smallestSortedZ = Mathf.RoundToInt(smallestZ);
	//this should not be needed. Use GetLength() instead. But, ERRORS ! :(
	
	levels[levelNr].sortedCountX = xxInt;
	levels[levelNr].sortedCountZ = zzInt;
	 
	//Debug.Log("Sorted tiles lengths: "+levels[levelNr].sortedTiles.GetLength(0)+","+levels[levelNr].sortedTiles.GetLength(1));
	
	oldSortedTiles = new SortedTile[0,0];
	//oldSortedTiles = ScriptableObject.CreateInstance(SortedTile[,]);
	
	if(levels[levelNr].sortedTiles){
		//Debug.Log(levels[levelNr].sortedTiles.GetLength(0)+" "+levels[levelNr].sortedTiles.GetLength(1));
		//Debug.Log("ReSortTiles::::::Sorted lengths:"+levels[levelNr].sortedTiles.GetLength(0)+","+levels[levelNr].sortedTiles.GetLength(1));
		oldSortedTiles = levels[levelNr].sortedTiles.Clone();
	} else {
		//Debug.Log("ReSortTiles::::::No sortedTiles! Aborting cloning ");
		
	}
	
	//levels[activeLevel].filledCorners = new GameObject[xxInt,zzInt,4,10];
	levels[levelNr].sortedTiles = new SortedTile[xxInt,zzInt];
	
	
	FillSortedList(unsortedTilesCount,smallestX,smallestZ,levelNr);
}

function FillSortedList(unsortedTilesCount : int, _smallestX : int, _smallestZ : int,levelNr:int) {
	for(var y : int = 0 ; y< unsortedTilesCount ; y++) {

		var iXN: float = Mathf.Abs(levels[levelNr].unsortedTiles[y].transform.localPosition.x-_smallestX)/levels[levelNr].tileSize;
		var iZN: float = Mathf.Abs(levels[levelNr].unsortedTiles[y].transform.localPosition.z-_smallestZ)/levels[levelNr].tileSize;
		
		iXN = Mathf.Floor(iXN * 10) / 10;
		iZN = Mathf.Floor(iZN * 10) / 10; 
		var iX : int = Mathf.RoundToInt(iXN);
		var iZ : int = Mathf.RoundToInt(iZN);

		
		if(iX < 0 || iX >= levels[levelNr].sortedTiles.GetLength(0) ) {
			Debug.Log("ReSortTiles::::::ERROR! iX:" + iX+"(xx:"+levels[levelNr].sortedCountX+")");
		}
		if(iZ < 0 || iZ >= levels[levelNr].sortedTiles.GetLength(1) ) {
			Debug.Log("ReSortTiles::::::ERROR! iZ:" + iZ+"(xx:"+levels[levelNr].sortedCountZ+")");
		}
		
		//Debug.Log("Recreating......................");
		var tST : SortedTile = new SortedTile();
		
			tST.tile = levels[levelNr].unsortedTiles[y];
			
		var tArr : Corners[] = new Corners[4];
				
		for (var co : int = 0 ; co < 4 ; co++) {
		
			var c1 : Corners = new Corners();
			
			c1.walls = new List.<GameObject>();
			
			c1.whatWallSet = new List.<boolean>();
			
			tArr[co] = c1;
		}
			tST.corners4 = tArr;
		
		
		levels[levelNr].sortedTiles[iX,iZ] = tST;
		//levels[levelNr].sortedTiles[iX,iZ].tile = levels[levelNr].unsortedTiles[y];
	}
	//find position of first(o,0) tile.
	var endsearch:boolean;
	for(var x:int = 0 ; x<levels[levelNr].sortedTiles.GetLength(0); x++) {
		for(var z:int = 0 ; z<levels[levelNr].sortedTiles.GetLength(1); z++) {
			if(endsearch == false) {
				if(levels[levelNr].sortedTiles[x,z] !=null) {
					levels[levelNr].sortedOrigin.x = levels[levelNr].sortedTiles[x,z].tile.transform.localPosition.x - (levels[levelNr].tileSize * x);
					levels[levelNr].sortedOrigin.z = levels[levelNr].sortedTiles[x,z].tile.transform.localPosition.z - (levels[levelNr].tileSize * z);
					//Debug.Log("Found origin of level"+levelNr+"at"+x+","+z+" 0,0 tile pos:"+levels[levelNr].sortedOrigin.x+","+levels[levelNr].sortedOrigin.z+" lp.z:"+levels[levelNr].sortedTiles[x,z].transform.localPosition.x+","+levels[levelNr].sortedTiles[x,z].transform.localPosition.z);
					endsearch = true;
					//var xxxxx : int = (-5)-(5);
					//Debug.Log("xxxxx="+xxxxx);
				}
			}
		}
	}
	//REPOPULATE ARRAY:
	
	var sortedOffsetXN : int = sortedOffsetX;
	var sortedOffsetZN : int = sortedOffsetZ;

	var sEndX : int;
	var sEndZ : int;
	
	var delOfstX : int;
	var delOfstZ : int;
	
	if(sortedOffsetX<0) {
		sEndX = sortedOffsetX*(-1);

	}
	if(sortedOffsetZ<0) {
		sEndZ = sortedOffsetZ*(-1);
	}
	if(sortedOffsetX>0) {
		sortedOffsetXN = 0;
	}
	if(sortedOffsetZ>0) {
		sortedOffsetZN = 0;
	}
	if(sortedOffsetX == 0) {
	}
	if(sortedOffsetZ == 0) {
	}
	
	if(smallestFluctChangeX >0) {
		//delOfstX = 1;
		sortedOffsetXN = smallestFluctChangeX;
	}
	if(smallestFluctChangeZ >0) {
		//delOfstZ = 1;
		sortedOffsetZN = smallestFluctChangeZ;
	}
	
	//Debug.Log("oryginal array length:"+oldSortedTiles.GetLength(0)+","+oldSortedTiles.GetLength(1)+". New array lengths:"+levels[levelNr].sortedTiles.GetLength(0)+","+levels[levelNr].sortedTiles.GetLength(1)+" offsets: "+sortedOffsetXN+","+sortedOffsetZN );
	for(var r1 : int = 0 ; r1 < levels[levelNr].sortedTiles.GetLength(0) ; r1++) {
		
		for(var r2 : int = 0 ; r2 < levels[levelNr].sortedTiles.GetLength(1) ; r2++){
			
			if( r1+sortedOffsetXN < 0 || r2+sortedOffsetZN < 0 || r1 > (oldSortedTiles.GetLength(0)-1)+sEndX || r2 > (oldSortedTiles.GetLength(1)-1)+sEndZ ) {

			} else if(oldSortedTiles[r1+sortedOffsetXN,r2+sortedOffsetZN] && oldSortedTiles[r1+sortedOffsetXN,r2+sortedOffsetZN].tile !=null) {
				
				levels[levelNr].sortedTiles[r1,r2] = oldSortedTiles[(r1+sortedOffsetXN),(r2+sortedOffsetZN)];
				//Debug.Log("made walls count for this tile:"+oldSortedTiles[(r1+sortedOffsetXN),(r2+sortedOffsetZN)].corners4[0].walls.Count+","+oldSortedTiles[(r1+sortedOffsetXN),(r2+sortedOffsetZN)].corners4[1].walls.Count+","+oldSortedTiles[(r1+sortedOffsetXN),(r2+sortedOffsetZN)].corners4[2].walls.Count+","+oldSortedTiles[(r1+sortedOffsetXN),(r2+sortedOffsetZN)].corners4[3].walls.Count+"");
			}
		}
		
	}
	
	sortedOffsetX = 0;
	sortedOffsetZ = 0;
	
	smallestFluctChangeX = 0;
	smallestFluctChangeZ = 0;
}

//return true if passed position is already filled with a tile.
function CheckSortedIfEmpty(px: int, pz:int) : boolean {
	
	var result : boolean;
	
	if(!levels[activeLevel].sortedTiles) {
		
		result = false;
		return result;
	}

	var xxxN : float = Mathf.Floor(levels[activeLevel].sortedCountX * 10) / 10;
	var zzzN : float = Mathf.Floor(levels[activeLevel].sortedCountZ * 10) / 10;
	 
	var xxx : int = Mathf.RoundToInt(xxxN);
	var zzz : int = Mathf.RoundToInt(zzzN);

	if(px<0 || px>xxx-1 || pz<0 || pz>zzz-1) {
		
		result = false;
		return result;
		
	} else {
		
		try{
			if(levels[activeLevel].sortedTiles[px,pz]) {
		
				if(levels[activeLevel].sortedTiles[px,pz].tile) {
				 
					result = true;
				}
			}
		} catch (err) {
		
		}
		
	}
	
	return result;
}
/*----------------------------------------------------


				ADDING, DELETING AND CLEANING TILES:					
					


*/
/*							ADDING

*/

function AddATile() {
#if UNITY_EDITOR
	Undo.RegisterSceneUndo("Create a tile(shazam)");
#endif 
	//Debug.Log( levels[activeLevel].brushSetList[levels[activeLevel].activeBrushSet] );
	if(levels[activeLevel].brushSetList.Count == 0 ){
	
		Debug.LogWarning ("InfiniTile: Can't paint a tile: Add a tileSet first (err:01.1) ");
		return;
	}
	
	if( levels[activeLevel].brushSetList[levels[activeLevel].activeBrushSet] == null ){
	
		Debug.LogWarning ("InfiniTile: Can't paint a tile: Add a tileSet first (err:01.2)");
		return;
	
	}
	
	if(levels[activeLevel].brushSetList[levels[activeLevel].activeBrushSet].tilesGOs.Count == 0 ){
		Debug.LogWarning ("InfiniTile: No tile in this set.Add a tile in brush editor (err:01.3)");
		return;
	
	}
	
	var px : int;
	var pz : int;

	if(levels[activeLevel].smallestSortedX>placementPoint.x) {
		px = (placementPoint.x-levels[activeLevel].smallestSortedX)/levels[activeLevel].tileSize;

	} else {
		px = Mathf.Abs(placementPoint.x-levels[activeLevel].smallestSortedX)/levels[activeLevel].tileSize;

	}
	if(levels[activeLevel].smallestSortedZ>placementPoint.z) {
		pz = (placementPoint.z-levels[activeLevel].smallestSortedZ)/levels[activeLevel].tileSize;

		
	} else {
		pz = Mathf.Abs(placementPoint.z-levels[activeLevel].smallestSortedZ)/levels[activeLevel].tileSize;

	}
	
	if(CheckSortedIfEmpty(px,pz) == true) {
		//Debug.Log("ADDING TILE::::Sorted checked. There is something there.Cant do tile for "+px+","+pz);
		
		return;
	} 
	//Debug.Log("ADDING TILE::::CheckSorted passed");
	var pp : Vector3 = transform.TransformPoint (placementPoint);
	var tNr : int = 0;
	if(levels[activeLevel].brushSetList.Count <= 0) {
		//Debug.Log("No brushSet! Create brushSet first!");
		return;
	} 
	if(levels[activeLevel].brushSetList[levels[activeLevel].activeBrushSet].random == true) {
		
		//Debug.Log("Random tile chooser is true");
		tNr = Random.Range(0,levels[activeLevel].brushSetList[levels[activeLevel].activeBrushSet].tilesGOs.Count);
		
	} else {
		
		 tNr = levels[activeLevel].activeTile;
		
	}
	
	if(levels[activeLevel].brushSetList[levels[activeLevel].activeBrushSet].tilesGOs[tNr] != null ) {
	
		var nTile : GameObject = gameObject.Instantiate(levels[activeLevel].brushSetList[levels[activeLevel].activeBrushSet].tilesGOs[tNr],pp,transform.rotation);
			nTile.transform.parent = levels[activeLevel].unsortedContainer.transform;
	
			nTile.transform.name = levels[activeLevel].activeBrushSet.ToString()+","+tNr.ToString()+":Tile";
			if(levels[activeLevel].brushSetList[activeBrushSet].randomTileRot == true) {
				var rotVal : int = Random.Range(0,3);
				nTile.transform.Rotate(0,90*rotVal,0); 
			}
		  
		sortedOffsetX = px; 
		sortedOffsetZ = pz;
		
		var runGather : boolean;
		
		if(!levels[activeLevel].sortedTiles) {
			
			runGather = true;
			//Debug.Log("There is no sortedTiles. Regetting Tiles. And then recollecting generated");
		}   
		
		ReGetTiles(activeLevel);
		
		if(runGather == true) {
				
				//Debug.Log("SHAZAM ERROR (addATie):No sortedTile instance, but more than 1 tile. Will try to recolect!");
				
				RecolectGenereted(activeLevel);
				
		}
		
		//levels[activeLevel].basicTextureMerge (px , pz, levels[activeLevel] , tMergeTex);

		GenerateTileBorders(px,pz, false, levels[activeLevel].activeBrushSet, tNr);
	}
}

function DeleteATile() {
	var pxN : int;
	var pzN : int; 
	
	var px : float;
	var pz : float;
#if UNITY_EDITOR
	Undo.RegisterSceneUndo("Delete a tile(shazam)");
#endif
	if(levels[activeLevel].smallestSortedX>=placementPoint.x) {
		px = (placementPoint.x-levels[activeLevel].smallestSortedX)/levels[activeLevel].tileSize;
	} else {
		px = Mathf.Abs(placementPoint.x-levels[activeLevel].smallestSortedX)/levels[activeLevel].tileSize;
	}
	if(levels[activeLevel].smallestSortedZ>=placementPoint.z) {
		pz = (placementPoint.z-levels[activeLevel].smallestSortedZ)/levels[activeLevel].tileSize;
		
	} else {
		pz = Mathf.Abs(placementPoint.z-levels[activeLevel].smallestSortedZ)/levels[activeLevel].tileSize;
	}
	
	if(!levels[activeLevel].sortedTiles) {
		
		ReGetTiles(activeLevel);
		
		RecolectGenereted(activeLevel);
	} 
	
		if(CheckSortedIfEmpty(px,pz) == true ) {
			
			DestroyImmediate(levels[activeLevel].sortedTiles[px,pz].tile);
			
			for(var c1 : int = 0 ; c1 < 4 ; c1++ ) {
				
				for(var c2 : int = 0 ; c2 < levels[activeLevel].sortedTiles[px,pz].corners4[c1].walls.Count ; c2++ ) {
					
					if(levels[activeLevel].sortedTiles[px,pz].corners4[c1]) {
						
						//if(levels[activeLevel].sortedTiles[px,pz].corners4[c1].whatWallSet.Count > c2) {
					 							
					 		//levels[activeLevel].sortedTiles[px,pz].corners4[c1].whatWallSet.RemoveAt(c2);
					 		//Debug.Log("deleting from corners:"+c1+" from tile"+px+","+pz);
					 	//}
					 	
						DestroyImmediate(	levels[activeLevel].sortedTiles[px,pz].corners4[c1].walls[c2]);
					 											
					}
				}
			}
			
			ReGetTiles(activeLevel);
			
			var tAddX: int;
			var tAddZ: int;
			if(levels[activeLevel].sortedTiles.GetLength(0) == 1){
				tAddX = 0;
			} else {
				tAddX = -1;
			}
			if(levels[activeLevel].sortedTiles.GetLength(1) == 1) {
				tAddZ = 0;
			} else {
				tAddZ = -1;
			}
			
			for(var x : int = -1 ; x< 2 ; x++) {
				for(var z : int = -1 ; z < 2 ; z++) {
					
					if(px+x <0 || pz+z< 0 || px+x > (levels[activeLevel].sortedTiles.GetLength(0)-1) || pz+z > (levels[activeLevel].sortedTiles.GetLength(1)-1)) {
						
					} else if(levels[activeLevel].sortedTiles[px+x,pz+z]){
							
							 
							//var st: char;
							
							//var dt: char;
							
							var s : int;// = st.GetNumericValue(levels[activeLevel].sortedTiles[px+x,pz+z].tile.transform.name[0]);
							
							var d : int;// = dt.GetNumericValue(levels[activeLevel].sortedTiles[px+x,pz+z].tile.transform.name[2]);
							
							//var e = z.sortedTiles[x,y].tile.transform.name;
							
							var e : String = levels[activeLevel].sortedTiles[px+x,pz+z].tile.transform.name;
				 			
				 			s = parseInt( e.Substring( 0 , e.LastIndexOf(",") ) );
				 			
				 			d = parseInt( e.Substring( e.LastIndexOf(",")+1 , e.LastIndexOf(":") - ( e.LastIndexOf(",")+1 ) ) );
				 			
							
							for(var ci1 : int = 0 ; ci1 < 4 ; ci1++ ) {
								
								for(var ci2 : int = 0 ; ci2 < levels[activeLevel].sortedTiles[px+x,pz+z].corners4[ci1].walls.Count ; ci2++ ) {
								
									if(levels[activeLevel].sortedTiles[px+x,pz+z].corners4[ci1]) {
										
										//if(levels[activeLevel].sortedTiles[px+x,pz+z].corners4[ci1].whatWallSet.Count > ci2){
																
											//levels[activeLevel].sortedTiles[px+x,pz+z].corners4[ci1].whatWallSet.RemoveAt(ci2);
											//Debug.Log("deleting from corners:"+ci1+" from tile"+(px+x)+","+(pz+z));
										//}
										
										DestroyImmediate(	levels[activeLevel].sortedTiles[px+x,pz+z].corners4[ci1].walls[ci2]);
															
															
									}
								} 
							}
							GenerateTileBorders(px+x,pz+z,true, s, d);
					}
				}
			}
		}
}

function UpdateTilePosition() {

}


function ChangeBrushSet(x: int){
	//check if there are any brushes in this(passed) brushSet:

	if(levels[activeLevel].brushSetList.Count <= 0) {
		
		//send message to helpBox!
		
		Debug.Log("No brush. Add one");
		return;
	}
	if(levels[activeLevel].brushSetList[x].tilesGOs.Count <= 0) {
		
		//send message to helpBox!
		
		Debug.Log("No Tiles in brush. Add one");
		return;
	}
	//change toggle:
	levels[activeLevel].brushSetList[x].isActive = !levels[activeLevel].brushSetList[x].isActive;
	// set activeBrushSet number to this one. Otherwise set it to some high number, that will prevent running CreateTiles function!
	if(levels[activeLevel].brushSetList[x].isActive == true) {
		activeBrushSet = x;
	} else {
		activeBrushSet = 900;
	}
	//and change all other brushSets to not active:
	for(var i : int = 0 ; i<levels[activeLevel].brushSetList.Count ; i++) {
		if(i!=x) {
			levels[activeLevel].brushSetList[i].isActive = false;
		}
	}
}

function CleanLevels() {

	for (var l: int = 0 ; l<levels.Count ; l++) {
		DestroyImmediate(levels[l].unsortedContainer);
		levels[l].unsortedTiles = new List.<GameObject>();
		levels[l].sortedTiles = new SortedTile[0,0];
		//levels[l].filledCorners = new GameObject[0,0,0,0];
		levels[l].sortedCountX = 0;
		levels[l].sortedCountZ = 0;
		levels[l].smallestSortedX = 0;
		levels[l].smallestSortedZ = 0;
		DestroyImmediate(levels[l].generatedContainer);

		MakeLVLContainer(l, l);
	}

}

function CleanOneLevel(ln : int) {
#if UNITY_EDITOR
		Undo.RegisterSceneUndo("Clean level");
#endif	
		DestroyImmediate(levels[ln].unsortedContainer);
		levels[ln].unsortedTiles = new List.<GameObject>();
		levels[ln].sortedTiles = new SortedTile[0,0];
		//levels[ln].filledCorners = new GameObject[0,0,0,0];
		//levels[ln].generated = new GeneratedWalls[0,0];
		levels[ln].sortedCountX = 0;
		levels[ln].sortedCountZ = 0;
		levels[ln].smallestSortedX = 0;
		levels[ln].smallestSortedZ = 0;
		DestroyImmediate(levels[ln].generatedContainer);
		//levels[l].generated
		MakeLVLContainer(ln, ln);
		
		sortedOffsetX = 0;
		sortedOffsetZ = 0;
	
	
}

/*----------------------------------------------------


				GENERATING ADJACENT TILES:					
					


*/
function ChangeWallSet(setNr :int){
	levels[activeLevel].wallsSetList[setNr].isActive = !levels[activeLevel].wallsSetList[setNr].isActive;
	lastActiveWallSet = setNr;
}

function AutoFillWalls(ind:int,go1 : GameObject,go2 : GameObject,go3 : GameObject){
	Debug.Log("Autofill : "+ind+"["+go1+","+go2+","+go3+"]");
	if(go1 == null || go2 == null || go3 == null ) {
		Debug.Log("Nothing inside");
	}
}

function AddsWallSet(ind:int, t : List.<GameObject>){
	
	levels[activeLevel].wallsSetList[ind].tilesGOs = t;
}



function GenerateTileBorders(posX : int, posZ : int, withClean : boolean , tx : int , ty: int){

	//Debug.Log("Generating walls:"+posX+","+posZ);
	
	var startTime1: float = Time.realtimeSinceStartup;
	
	var l : int = activeLevel;

//Passed values check:	
	if(levels[l].sortedTiles.GetLength(0) == 0 || levels[l].sortedTiles.GetLength(1) == 0) {
		
		Debug.Log("No sortedTiles!");
		
		return;
	}
	
	if(levels[l].sortedTiles.GetLength(0) == 1 && levels[l].sortedTiles.GetLength(1) == 1) {
		
		posX = 0;
		
		posZ = 0;
	}
	
	if(posX <0) {

		posX = 0;
	}
	
	if(posZ < 0) {

		posZ = 0;
	}

	ShuffleBagFill ();

	var name : String;
	var genContainer : GameObject;
	if(!levels[l].generatedContainer) {
		//Debug.Log("There was no generated walls container object!");
		name = "Generated.L"+l.ToString();
		genContainer = new GameObject(name);
		genContainer.transform.position = transform.position;
		genContainer.transform.parent = transform;
		levels[l].generatedContainer = genContainer;

	}
	//variables for connector checks):
	var coX1 : float = -1;
	var coY1 : float = 0.5;
	var coX2 : float = -0.5;
	var coY2 : float = 1;
	var cornersList : List.<int>[] = new List.<int>[32]; 
	
	//rest variables:
	var n : int = -1;	//x
	var s : int = 0;	//y
	var v : int = 0;	//changer
	
	var aa : int;		//x for corner blocks
	var bb :int;		//y for corner blocks
	
	var tempCoords :int;
	
	var k0 : int = 2;
	var k1 : int = 3;

	for(var d : int = 0 ; d<4 ; d++) {

		var ka : int = n; //save for later
		var kb : int = s; //save for later
		
		aa = n+v;
		v=n;
		bb = s+v;
		var nT : int = n;
		n = s;
		s = -nT;
		
		var passValue : int;
		var tValue :int = 1;
		var rValue : int; 
		rValue = tValue;
		var ax : int = aa;
		var bx : int = bb; 
		
		var diffX: float;
		var diffY: float;
		var checkU: int;	
		var checkL: int;	
		
		var wallNr: int[] = new int[2];
			wallNr[0] = -1;
			wallNr[1] = -1;
			
		
		
		//check adjacent tiles if empty ( 1,2,4 ):
		for(var k : int = 0 ; k<3 ; k++) {
			
			//levels[l].repopulateTileCornerTypes( posX , posZ , levels[l] );
			
			if((posX+ax)<0 || (posX+ax)>levels[l].sortedTiles.GetLength(0)-1 || (posZ+bx)<0 || (posZ+bx)>levels[l].sortedTiles.GetLength(1)-1) {
				
				passValue += rValue;
			}
			
			else if(!levels[l].sortedTiles[posX+ax,posZ+bx]) {
				
				passValue += rValue;
				
			} 
			
			
			
			//now delete any walls that are in tile 1,2 or 4:
			else if(levels[l].sortedTiles[posX+ax,posZ+bx].tile){ 
				
				// this is filling the lookup array with lists of walls types in particular corner( in corners around this one tile)

				if( k == 0 || k == 1) {
				
					for(var x : int = 0 ; x < 4 ; x++) {
					
					//Debug.Log("d:"+d+", k:"+k+" :"+cornersList.Length+"  "+(   (d*4)+(4*(k+d))+x  ));
					
						//cornersList[  (d*4)+(4*(k+d))+x   ] = levels[l].sortedTiles[posX+ax,posZ+bx].corners4[x].whatWallSet;
					
					}
				}
				
				if(k==0) {
					//fill corners List:
					
					//delete 1 wall
					for(var dw : int = 0 ; dw < levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].walls.Count ; dw++) {
						
						DestroyImmediate(	levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].walls[dw] );
						
//						if(levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].whatWallSet.Count >dw){
//
//							//wallNr[0] = levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].whatWallSet[dw];
//							//Debug.Log("k0.0 /"+"passvalue: "+passValue+" d:"+d+" k0:"+k0+"    wallsetnr:"+levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].whatWallSet[dw]+", its count:"+levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].whatWallSet.Count);
//							//levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].whatWallSet.RemoveAt(dw);
//												
//						}
					}
				}
				
				if(k==1) {
					//Delete 2 walls
					for(var dw1 : int = 0 ; dw1 < levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].walls.Count ; dw1++) {
						
						DestroyImmediate(	levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].walls[dw1] );
						
//						if(levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].whatWallSet.Count > dw1 ) {
//
//							//wallNr[0] = levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].whatWallSet[dw1];
//							//Debug.Log("k0.1 /"+"passvalue: "+passValue+" d:"+d+" k0:"+k0+"    wallsetnr:"+levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].whatWallSet[dw1]+", its count:"+levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].whatWallSet.Count);
//							//levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k0].whatWallSet.RemoveAt(dw1);
//												
//						}
					}
					
					for(var dw2 : int = 0 ; dw2 < levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].walls.Count ; dw2++) {
						
						DestroyImmediate(	levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].walls[dw2] );
											
//						if(levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].whatWallSet.Count > dw2) {
//
//							//wallNr[1] = levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].whatWallSet[dw2];
//							//Debug.Log("k1.1 /"+"passvalue: "+passValue+" d:"+d+" k1:"+k1+"    wallsetnr:"+levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].whatWallSet[dw2]+", its count:"+levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].whatWallSet.Count);				
//							//levels[l].sortedTiles[posX+ax,posZ+bx].corners4[k1].whatWallSet.RemoveAt(dw2);
//						}
					}
				}
			}
			//UPPER/LOWER CHECK:
//			if(l-1 >= 0) {
//				diffX =((levels[l-1].sortedOrigin.x -levels[l].sortedOrigin.x )/levels[l].tileSize)/-1;
//				diffY =((levels[l-1].sortedOrigin.z -levels[l].sortedOrigin.z )/levels[l].tileSize)/-1;
//				diffX = Mathf.Floor(diffX* 10) / 10;
//				diffY = Mathf.Floor(diffY* 10) / 10;
//				
//				if( (posX+ax)+diffX < 0 || (posX+ax)+diffX>levels[l-1].sortedCountX-1 || (posZ+bx)+diffY<0 || (posZ+bx)+diffY>levels[l-1].sortedCountZ-1) {
//					//Debug.Log(k+"tile pos in highier array is out of bounds (l:"+l+"("+a+","+b+"))");
//					if(checkU == 0) {
//						checkU = 2; 
//					}
//					
//				}else if(levels[l-1].sortedTiles[(posX+ax)+diffX,(posZ+bx)+diffY] == null && checkU !=1) {
//					checkU = 2; 
//				} else if(levels[l-1].sortedTiles[(posX+ax)+diffX,(posZ+bx)+diffY] != null) {
//					checkU = 1;
//				}
//			} 
//			if(l+1 < levels.Count) {
//				diffX =((levels[l+1].sortedOrigin.x -levels[l].sortedOrigin.x )/levels[l].tileSize)/-1;
//				diffY =((levels[l+1].sortedOrigin.z -levels[l].sortedOrigin.z )/levels[l].tileSize)/-1;
//				diffX = Mathf.Floor(diffX* 10) / 10;
//				diffY = Mathf.Floor(diffY* 10) / 10;
//				//Debug.Log(k);
//				if((posX+ax)+diffX<0 || (posX+ax)+diffX>levels[l+1].sortedCountX-1 || (posZ+bx)+diffY<0 || (posZ+bx)+diffY>levels[l+1].sortedCountZ-1 ) { 
//					//Debug.Log(d+"tile pos in lower array is out of bounds (l:"+l+"("+a+","+b+"))");
//					if(checkL == 0) {
//						checkL = 2;
//					}
//					
//				} else if(levels[l+1].sortedTiles[(posX+ax)+diffX,(posZ+bx)+diffY] == null && checkL !=1) {
//					//Debug.Log(d+"empty");
//					checkL = 2;
//				}else if(levels[l+1].sortedTiles[(posX+ax)+diffX,(posZ+bx)+diffY] != null) {
//					//Debug.Log(d+"full");
//					checkL = 1;
//				}
//				
//			}
			
			rValue +=rValue;	
			ax = ka;
			bx = kb;
			var tka : int = ka;
			ka = -kb;
			kb = tka;
		}
		
		//switchers for walls checking:
		k0++;
		k1++;
		if(k0>3) {
			k0 = 0;
		}
		if(k1>3) {
			k1 = 0;
		}
		
		//switchers for connectors:
		//Debug.Log("DOING WALLS: d:"+d);
		
		
		if(levels[l].sortedTiles[posX,posZ] ) {
		
			var pos : Vector3 = Vector3(levels[l].sortedTiles[posX,posZ].tile.transform.position.x,levels[l].sortedTiles[posX,posZ].tile.transform.position.y,levels[l].sortedTiles[posX,posZ].tile.transform.position.z);						
			
			for(var sb : int = 0 ; sb < levels[activeLevel].shuffleBags.Count ; sb++) {
				
				
				
				if(levels[activeLevel].shuffleBags[sb].tokenBag.Count <= 1 ) {
					
					levels[activeLevel].repopulateOneBag(sb);
				}
				
				var p: int = levels[activeLevel].shuffleBags[sb].tokenBag[levels[activeLevel].shuffleBags[sb].tokenBag.Count-1];
		
				if(tx >=0 && ty>=0 && ty < levels[activeLevel].wallsSetList[p].specialsList[tx].sPlist.Count) {
					//Debug.Log(tx+","+ty+" / "+levels[activeLevel].wallsSetList.Count+":"+p+" / "+levels[activeLevel].wallsSetList[p].specialsList.Count+":"+tx+"/"+levels[activeLevel].wallsSetList[p].specialsList[tx].sPlist.Count+":"+ty);
					if(levels[activeLevel].wallsSetList[p].specialsList[tx].sPlist[ty] == true) {
						
						//Debug.Log("sb:"+sb+" p:"+p+", tx,ty: "+tx+","+ty+" "+levels[activeLevel].wallsSetList[p].specialsList[tx].sPlist[ty]);
						
						if(levels[l].wallsSetList[p].tilesGOs[passValue] != null) {
							
							//Debug.Log(levels[activeLevel].shuffleBags[sb]);
							
							var wall : GameObject = gameObject.Instantiate( levels[l].wallsSetList[p].tilesGOs[passValue] ,pos,transform.rotation);
							
							levels[activeLevel].shuffleBags[sb].tokenBag.RemoveAt(levels[activeLevel].shuffleBags[sb].tokenBag.Count-1);
					
							wall.transform.parent = levels[l].generatedContainer.transform;
					
							wall.transform.Rotate(Vector3.up*(90*d));
					 
							levels[l].sortedTiles[posX,posZ].corners4[d].walls.Add(wall);							
							
							wall.transform.name = posX.ToString()+","+posZ.ToString()+"ws"+p+".d"+d.ToString();	
							
							// make connectors:
							
							//Debug.Log("checking corner::::::: choosed wallSet: "+p);
							var xbag : intBag = levels[l].checkForCorner( d , passValue , p , posX , posZ , levels[l] );
							
							//Debug.Log( "d:"+d+"passvalue:"+passValue+ "xBag: "+xbag.tokenBag[0] +","+ xbag.tokenBag[1]);
							
							var cpX : float;
							var cpZ : float;  
							var rot : int[] = new int[2];
							var ts : int = levels[activeLevel].tileSize;
							
							
							
							for(var t : int = 0 ; t < 2 ; t++){
							
								if( xbag.tokenBag[t] != 0 && levels[l].wallsSetList[p].tilesGOs[ 10-xbag.tokenBag[t] ] !=null){
								
									switch( passValue ){
									
									case 1:
									
										if( t == 0 ){ // this corner always type = 1 
										
											cpX = -0.5 * ts;
											cpZ = -1 * ts;
											rot[0] = -180; //-90
										
										}
										if( t == 1 ){ // this corner always type = 2
										
											cpX = -1* ts;
											cpZ = -0.5* ts;
											rot[1] = -90; //0
										}
									
									break;
									
									case 6:
									
										if( t == 1 ){
										
											cpX = -1 * ts;
											cpZ = -0.5* ts;
											rot[1] = -90; //0
										
										}
										if( t == 0 ){
										
											cpX = -0.5* ts;
											cpZ = -1* ts;
											rot[0] = -180; //-90
										}
									
									break;
									
									case 2:
									
										if( t == 0 ){
										
											cpX = -1* ts;
											cpZ = -0.5* ts;
											rot[0] = -90; //0
										}
									break;
									
									case 3:
									
										if( t == 0 ){
										
											cpX = -0.5* ts;
											cpZ = -1* ts;
											rot[0] = -180;//-90
										}
									break;
									
									case 4:
									
										if( t == 0 ){
										
											cpX = -0.5* ts;
											cpZ = -1* ts;
											rot[0] = -180; //-90
										}
									break;
									
									case 5:
									
										if( t == 0 ){
										
											cpX = -1* ts;
											cpZ = -0.5* ts;
											rot[0] = -90; //0
										}
									break;
									
									default:
									break;
									
									}

								//coords are set. Now for every non zero int in xBag do the connector with 90*d rotation(rotate around the center tile position)
								var pos1 : Vector3 = pos;
								 	pos1.x +=cpX;
									pos1.z +=cpZ;
								//Debug.Log(xbag.tokenBag[t]);
								var cr0: GameObject = new GameObject();
								
								cr0.transform.localPosition = pos;
								
								cr0.transform.parent = levels[l].generatedContainer.transform;
								
								var cr1: GameObject = gameObject.Instantiate( levels[l].wallsSetList[p].tilesGOs[ 7+xbag.tokenBag[t] ] ,pos1,transform.rotation);
								
								cr1.transform.Rotate(Vector3.up*(rot[t]));
								
								cr1.transform.RotateAround (pos, Vector3.up, 90*d);
								
								cr1.transform.parent = cr0.transform;
								
								levels[l].sortedTiles[posX,posZ].corners4[d].walls.Add(cr0);
								
								cr0.transform.name = "connector:t:"+t.ToString()+"pv:"+passValue.ToString()+","+p.ToString()+".d"+d.ToString();
								
								} 
							}
						}
						
						else {
							
							levels[activeLevel].shuffleBags[sb].tokenBag.RemoveAt(levels[activeLevel].shuffleBags[sb].tokenBag.Count-1);
						}
								
					} else {
						
						levels[activeLevel].shuffleBags[sb].tokenBag.RemoveAt(levels[activeLevel].shuffleBags[sb].tokenBag.Count-1);
					}
				} 
			}
		}
	}
}

function ShuffleBagFill (){
	if(!levels[activeLevel].shuffleBags) {
		levels[activeLevel].shuffleBags = new List.<intBag>();
		
	}
	
	if(levels[activeLevel].shuffleBags.Count == 0) {
	//	levels[activeLevel].createShuffleBags();
		levels[activeLevel].populateShuffleBag();
	}
	
	//create shuufle bags based on wallsetgrups//populate
	
	//what grups are there? There is default 0 grup (unless user switched it to something else
	
	
	
	
	
	
	
	
//	var grupDictionary : Dictionary.<int,List.<BrushSet> > = new Dictionary.<int,List.<BrushSet> >();
//	//var tempGrupList : List.<List.<BrushSet> > = new List.<List.<BrushSet> >();
//	var tempGrupList : List.<BagOfGOs> = new List.<BagOfGOs>();
//	var l : int = activeLevel;
//	
//	for(var gl : int = 0 ; gl < levels[l].wallsSetList.Count ; gl++) {
//		
//		var grKey: int = levels[l].wallsSetList[gl].grNr;
//		
//		if(grupDictionary.ContainsKey(grKey)) {
//			grupDictionary[grKey].Add(levels[l].wallsSetList[gl]);
//		} else {
//			var myList : List.<BrushSet> = new List.<BrushSet>();
//				myList.Add(levels[l].wallsSetList[gl]);
//				grupDictionary.Add(grKey,myList);
//		}
//	} 
//	//var text : String;
//	for(var key : int in grupDictionary.Keys){
//		//text = text + "," + (grupDictionary[key].Count).ToString();
//		var tBag : BagOfGOs = new BagOfGOs();
//			tBag.bagNr = key;
//			var tTokens :  List.<BrushSet> = new List.<BrushSet>(grupDictionary[key]);
//			tBag.tokens = tTokens;
//		tempGrupList.Add(tBag);
//		//tempGrupList = new List.<List.<BrushSet> >(grupDictionary.Values);
//	}
//	levels[l].grups = tempGrupList;
//	
//	levels[l].shuffleBag = new List.<Bag>(); //always
//	
//	for(var c:int = 0 ; c<tempGrupList.Count ; c ++) { //przechodzimy przez wszystkie grupy i ustalamy ilosc tokenow(i nr tokenow)
//		
//		var nBag : Bag = new Bag();
//			nBag.bagNr = tempGrupList[c].bagNr; 
//			var tITokens : List.<int> = new List.<int>();
//			var nrTokens : int;
//			
//		for(var e : int = 0 ; e < tempGrupList[c].tokens.Count ; e++){ // przechodzi przez wszystkie tokeny tego Bag 
//			
//			if(tempGrupList[c].tokens[e].strength == 1) {
//				
//				if(!nBag.alwaysList) {
//					var alList : List.<int> = new List.<int>();
//					nBag.alwaysList = alList;
//				} 
//				nBag.alwaysList.Add(e);
//				
//				
//			} else if(tempGrupList[c].tokens[e].strength < 1 && tempGrupList[c].tokens[e].strength > 0) { // if this token (a BrushSet) in grup bag "c" has strength not 1, add its number to tokens list
//				
//				nrTokens =  Mathf.RoundToInt (tempGrupList[c].tokens[e].strength*10.0f);
//					
//				for(var nt : int = 0 ; nt < nrTokens ; nt++) {
//					tITokens.Add(e);
//				}
//
//				nBag.tokens = tITokens;
//	
//				
//			}
//			
//		}
//		levels[l].shuffleBag.Add(nBag);
//	}
}

function FillBagWithTokens(){

}



class MyList
{
	var GOList: List.<GameObject>;
	
	function ListOfLists(_GOList: List.<GameObject>) {
	
	GOList = _GOList;
	}
}

public class TileThings
{
	var active : boolean;
	var random : boolean;
	var tilesGOs: List.<GameObject>;
	
	function ListOfLists(_tilesGOs: List.<GameObject>) {
	
		tilesGOs = _tilesGOs;
	}
}


function OnDrawGizmos () {}

function OnDrawGizmosSelected () {
	if(levels.Count <=0) {
		Debug.Log("DRAW GIZMOS: error! No levels!");
		return;
	}
	Gizmos.matrix = transform.localToWorldMatrix;
	zxc = transform.InverseTransformPoint (ghostTilePos);
	//this nPos should not be here! Its for tile placement
	nPos = Vector3(
								Mathf.RoundToInt (zxc.x/levels[activeLevel].tileSize)*levels[activeLevel].tileSize
								,ghostTilePos.y
								,Mathf.RoundToInt (zxc.z/levels[activeLevel].tileSize)*levels[activeLevel].tileSize
	
								);
								
	//draw gizmo on the correct level and height
	
	var gizmoPos : Vector3 = nPos;
		//gizmoPos.y -= levels[activeLevel].lvlHeight/2.0f;
		//gizmoPos.y -= (levels[activeLevel].lvlHeight/2.0f);//poprawka, bo nPos jest srodkiem gizmosa wiec musimy odjac polowe(odjac, bo level height zawsze jest w dol)
		
		//gizmoPos.x += levels[activeLevel].tileSize/2;
		//gizmoPos.z += levels[activeLevel].tileSize/2;
		
		//gizmoPos.y -= levels[activeLevel].absoluteLvlHeight;
		nPos.y -=levels[activeLevel].lvlHeight/2;
	Gizmos.DrawWireCube (nPos, Vector3 (levels[activeLevel].tileSize,  levels[activeLevel].lvlHeight  ,levels[activeLevel].tileSize));
	
	//Gizmos.color = Color.yellow;
	
	//Gizmos.DrawWireSphere(zxc,levels[activeLevel].tileSize/15.0f);
	
		placementPoint = gizmoPos;
		//placementPoint.y -=(levels[activeLevel].lvlHeight);
}
