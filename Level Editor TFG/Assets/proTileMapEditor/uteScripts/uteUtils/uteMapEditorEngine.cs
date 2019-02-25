using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class uteMapEditorEngine : MonoBehaviour {
#if UNITY_EDITOR
	// states
	private bool editorIsLoading;
	private bool erase;
	private int isRotated;
	private int isRotatedH;
	private bool is2D;
	private bool isOrtho;
	private bool canBuild;
	private bool isCurrentStatic;
	private bool isCamGridMoving;
	private bool isBuildingTC;
	private bool isContinuesBuild;
	private bool isBuildingMass;
	private bool isMouseDown;
	private bool isInTopView;
	[HideInInspector]
	public bool isItNewPattern;
	[HideInInspector]
	public bool isItNewMap;
	[HideInInspector]
	public string newProjectName;
	[HideInInspector]
	public bool isItLoad;
	[HideInInspector]
	public GameObject mapLightGO;
	private bool isShowCameraOptions;

	// info
	private string catInfoPath;
	private string currentTcFamilyName;
	private string settingsInfoPath;
	private int lastSelectedIndex;
	private int lastSelectedIndexTC;
	private float globalYSize;
	private string cameraType;
	private int globalGridSizeX;
	private int globalGridSizeZ;
	private string rightClickOption;
	public string yTypeOption;
	private Vector3 gridsize;
	private static float ZERO_F = .0f;
	private float offsetZ;
	private float offsetX;
	private float offsetY;
	private float lastPosX;
	private float lastPosY;
	private float lastPosZ;
	private float rotationOffset;
	private string currentObjectID;
	private uteTileConnectionsEngine uTCE;
	private uteMassBuildEngine uMBE;
	private uteExporter exporter;
	private uteHelpBox uteHelp;
	private uteMouseOrbit mouseOrbit;
	private uteOptionsBox uteOptions;
	private uteLM _uteLM;
	private int currentLayer;
	private string currentObjectGUID;
	private GUISkin ui;
	private Vector3 startingCameraTransform;
	private int currentTCID;

	// map objects
	private GameObject MAP;
	private GameObject MAP_STATIC;
	private GameObject MAP_DYNAMIC;
	private GameObject MAIN;
	private GameObject cameraGO;
	private List<catInfo> allTiles = new List<catInfo>();
	private List<GameObject> catGoes = new List<GameObject>();
	private List<Texture2D> catPrevs = new List<Texture2D>();
	private List<string> catNames = new List<string>();
	private List<string> catGuids = new List<string>();
	private List<tcInfo> allTCTiles = new List<tcInfo>();
	private List<GameObject> tcGoes = new List<GameObject>();
	private List<Texture2D> tcPrevs = new List<Texture2D>();
	private List<string> tcNames = new List<string>();
	private List<string> tcGuids = new List<string>();
	private List<string> tcRots = new List<string>();
	private GameObject currentTile;
	private GameObject lastTile;
	[HideInInspector]
	public GameObject grid;
	private GameObject hitObject;

	// UI objects
	private Texture2D previewT;
	private Texture2D previewObjTexture;
	private Texture2D previewTTC;
	private Texture2D previewObjTextureTC;
	private uteComboBox comboBoxControl = new uteComboBox();
	private uteComboBox comboBoxForTileConnectionControl = new uteComboBox();
	private GUIContent[] comboBoxList;
	private GUIContent[] comboBoxList_TileConnections;
	private GUIStyle listStyle = new GUIStyle();
	private Rect GUIComboBoxCollider;
	private Rect GUIComboBoxColliderTC;
	private Rect GUIObjsCollider;
	private Rect GUIObjsColliderTC;
	private Rect checkGUIObjsCollider;
	private Rect checkGUIObjsColliderTC;
	private Rect cameraOptionsRect;
	private Vector2 scrollPosition = new Vector2(0,0);
	private Vector2 scrollPositionTC = new Vector3(0,0);

	// helpers
	private bool isShowLineHelpers;
	private LineRenderer[] helpers_LINES;
	private GameObject helpers_CANTBUILD;
	private GameObject helpers_CANBUILD;

	// other
	private uteCameraMove cameraMove;
	private uteSaveMap saveMap;

	public class catInfo
	{
		public string catName;
		public string catCollision;
		public List<GameObject> catObjs = new List<GameObject>();
		public List<Texture2D> catObjsPrevs = new List<Texture2D>();
		public List<string> catObjsNames = new List<string>();
		public List<string> catGuidNames = new List<string>();
		
		public catInfo(string _catName, List<string> _catObjsNames, string _catCollision, List<GameObject> _catObjs, List<Texture2D> _catObjsPrevs, List<string> _catGuidNames)
		{
			catName = _catName;
			catCollision = _catCollision;
			catObjs = _catObjs;
			catObjsPrevs = _catObjsPrevs;
			catObjsNames = _catObjsNames;
			catGuidNames = _catGuidNames;
		}
	}

	public class tcInfo
	{
		public string tcName;
		public List<GameObject> tcObjs = new List<GameObject>();
		public List<Texture2D> tcObjsPrevs = new List<Texture2D>();
		public List<string> tcObjsNames = new List<string>();
		public List<string> tcGuidNames = new List<string>();
		public List<string> tcRotsNames = new List<string>();

		public tcInfo(string _tcName, List<string> _tcObjsNames, List<GameObject> _tcObjs, List<Texture2D> _tcObjsPrevs, List<string> _tcGuidNames, List<string> _tcRotsNames)
		{
			tcName = _tcName;
			tcObjsNames = _tcObjsNames;
			tcObjs = _tcObjs;
			tcObjsPrevs = _tcObjsPrevs;
			tcGuidNames = _tcGuidNames;
			tcRotsNames = _tcRotsNames;
		}
	}

	public IEnumerator uteInitMapEditorEngine()
	{
		isInTopView = false;
		currentTCID = 0;
		lastTile = null;
		currentLayer = 0;
		isContinuesBuild = false;
		isMouseDown = false;
		isBuildingTC = false;
		isShowCameraOptions = false;
		isCamGridMoving = false;
		isBuildingMass = false;
		uteGLOBAL3dMapEditor.canBuild = true;
		editorIsLoading = true;
		canBuild = false;
		cameraGO = (GameObject) GameObject.Find("MapEditorCamera");
		uteGLOBAL3dMapEditor.isEditorRunning = true;
		cameraOptionsRect = new Rect(Screen.width-505,Screen.height-70,410,30);
		uteHelp = (uteHelpBox) this.gameObject.AddComponent<uteHelpBox>();
		ui = (GUISkin) Resources.Load("uteForEditor/uteUI");
		catInfoPath = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteCategoryInfotxt); //Application.dataPath + "/proTileMapEditor/Resources/uteForEditor/uteCategoryInfo.txt";
		settingsInfoPath = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteSettingstxt);//Application.dataPath + "/proTileMapEditor/Resources/uteForEditor/uteSettings.txt";
		GUIComboBoxCollider = new Rect(10, 0, 200, 30);
		GUIComboBoxColliderTC = new Rect(10,0,200,30);
	//	GUIToolsCollider = new Rect(Screen.width-95,3,90,210);
		gridsize = new Vector3(1000.0f,0.1f,1000.0f);
		previewObjTexture = new Texture2D(25,25);
		previewObjTextureTC = new Texture2D(25,25);
		erase = false;
		currentTcFamilyName = "";
		currentObjectGUID = "";
		
		MAP = new GameObject("MAP");
		MAP_DYNAMIC = new GameObject("MAP_DYNAMIC");
		MAP_DYNAMIC.transform.parent = MAP.transform;
		MAP_STATIC = new GameObject("MAP_STATIC");
		MAP_STATIC.transform.parent = MAP.transform;
		MAP_STATIC.isStatic = true;
		mapLightGO = (GameObject) Instantiate((GameObject) Resources.Load("uteForEditor/uteMapLight"));
		mapLightGO.name = "MapLight";
		GameObject tempGO = new GameObject();
		tempGO.name = "TEMP";

		previewT = new Texture2D(2,2);
		previewTTC = new Texture2D(2,2);

		isShowLineHelpers = false;
		GameObject help = (GameObject) Instantiate((GameObject)Resources.Load("uteForEditor/uteHELPERS"),Vector3.zero,Quaternion.identity);
		help.name = "uteHELPERS";
		helpers_CANTBUILD = GameObject.Find("uteHELPERS/CANTBUILD");
		helpers_CANBUILD = GameObject.Find("uteHELPERS/CANBUILD");
		InitHelperLines();

		uTCE = (uteTileConnectionsEngine) this.gameObject.AddComponent<uteTileConnectionsEngine>();
		uMBE = (uteMassBuildEngine) this.gameObject.AddComponent<uteMassBuildEngine>();

		yield return StartCoroutine(ReloadTileAssets());

		uteOptions = (uteOptionsBox) this.gameObject.AddComponent<uteOptionsBox>();

		if(isItLoad)
		{
			_uteLM = this.gameObject.AddComponent<uteLM>();
			_uteLM.uMEE = this;
			yield return StartCoroutine(_uteLM.LoadMap(newProjectName,MAP_STATIC,MAP_DYNAMIC,isItNewMap));
			editorIsLoading = false;
		}
		else
		{
			editorIsLoading = false;
		}

		if(!isItLoad)
		{
			yield return StartCoroutine(saveMap.SaveMap(newProjectName,isItNewMap));
		}

		yield return 0;
	}
	
	private void Update()
	{
		if(editorIsLoading)
			return;

		if(isItNewPattern)
		{
			if(exporter.isShow)
			{
				return;
			}
		}

		if(uteHelp.isShow)
		{
			return;
		}

		if(uteOptions.isShow)
		{
			return;
		}
		
		if(!CheckGUIPass())
		{
			return;
		}

		if(isItLoad)
		{
			if(!_uteLM.isMapLoaded)
			{
				return;
			}
		}

		if(isShowCameraOptions)
		{
			cameraOptionsRect = new Rect(Screen.width-505,Screen.height-70,410,30);
		}

		if(erase)
		{
			if(Input.GetMouseButtonDown(0))
			{
				Ray eraseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit eraseHit;
				
				if(Physics.Raycast(eraseRay, out eraseHit, 1000))
				{
					if(!eraseHit.collider.gameObject.name.Equals("Grid"))
					{
						Destroy (eraseHit.collider.gameObject);
						uteGLOBAL3dMapEditor.mapObjectCount--;
						hitObject = null;
						helpers_CANTBUILD.transform.position = new Vector3(-1000000.0f,0.0f,-1000000.0f);
					}
				}
			}
		}

		if(!isCamGridMoving)
		{
			if(Input.GetKeyUp (KeyCode.X))
			{
				StartCoroutine(gridSmoothMove(grid,true,cameraMove.gameObject));
			}
			else if(Input.GetKeyUp (KeyCode.Z))
			{
				StartCoroutine(gridSmoothMove(grid,false,cameraMove.gameObject));
			}

			if(!isInTopView)
			{
				if(Input.GetKeyDown(KeyCode.LeftAlt))
				{
					mouseOrbit.isEnabled = true;
				}

				if(Input.GetKeyUp(KeyCode.LeftAlt))
				{
					mouseOrbit.isEnabled = false;
				}
			}

			if(Input.GetKeyDown(KeyCode.C))
			{
				if(isShowLineHelpers)
				{
					ShowLineHelpers(isShowLineHelpers = false);
				}
				else
				{
					ShowLineHelpers(isShowLineHelpers = true);
				}
			}

			if(Input.GetKeyDown(KeyCode.R))
			{
				ResetCamera();
			}
		}

		if(isCamGridMoving)
			return;

		if(isShowLineHelpers&&currentTile)
		{
			CalculateLineHelpers();
		}

		if(Input.GetMouseButtonUp(1)&&!erase)
		{
			float rot3D = 10.0f;

			if(rightClickOption.Equals("rotL")&&currentTile)
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(0.0f,-rot3D,0.0f),true));
			}
			else if(rightClickOption.Equals("rotR")&&currentTile)
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(0.0f,rot3D,0.0f),true));
			}
			else if(rightClickOption.Equals("rotU")&&currentTile)
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(-rot3D,0.0f,0.0f),false));
			}
			else if(rightClickOption.Equals("rotD")&&currentTile)
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(rot3D,0.0f,0.0f),false));
			}
			else if(rightClickOption.Equals("rotI")&&currentTile)
			{
				rot3D *=2;

				StartCoroutine(smoothRotate(currentTile,new Vector3(0.0f,0.0f,rot3D),false));
			}
			else if(rightClickOption.Equals("erase"))
			{
				Ray eraseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit eraseHit;
				
				if(Physics.Raycast(eraseRay, out eraseHit, 1000))
				{
					if(!eraseHit.collider.gameObject.name.Equals("Grid"))
					{
						Destroy (eraseHit.collider.gameObject);
						uteGLOBAL3dMapEditor.mapObjectCount--;
					}
				}
			}
		}

		if(Input.GetMouseButtonUp(0))
		{
			lastTile = null;
			isMouseDown = false;

			if(!erase)
			{
				if(isBuildingMass)
				{
					uMBE.FinishUp();
				}
				else if(isBuildingTC)
				{
					uTCE.FinishUp();
				}
			}
		}

		if(erase&&hitObject&&!hitObject.name.Equals("Grid"))
		{
			helpers_CANTBUILD.transform.position = hitObject.transform.position+new Vector3(hitObject.GetComponent<BoxCollider>().center.x*hitObject.transform.localScale.x,hitObject.GetComponent<BoxCollider>().center.y*hitObject.transform.localScale.y,hitObject.GetComponent<BoxCollider>().center.z*hitObject.transform.localScale.z);
			helpers_CANTBUILD.transform.localScale = hitObject.GetComponent<Collider>().bounds.size + new Vector3(0.1f,0.1f,0.1f);
			helpers_CANBUILD.transform.position = new Vector3(-10000,0,-10000);
		}
		else if(hitObject&&!hitObject.name.Equals("Grid"))
		{
			helpers_CANBUILD.transform.position = hitObject.transform.position+new Vector3(hitObject.GetComponent<BoxCollider>().center.x*hitObject.transform.localScale.x,hitObject.GetComponent<BoxCollider>().center.y*hitObject.transform.localScale.y,hitObject.GetComponent<BoxCollider>().center.z*hitObject.transform.localScale.z);
			helpers_CANBUILD.transform.localScale = hitObject.GetComponent<Collider>().bounds.size + new Vector3(0.1f,0.1f,0.1f);
		}
		else
		{
			helpers_CANBUILD.transform.position = new Vector3(-10000,0,-10000);
		}

		if(currentTile||erase)
		{
			Ray buildRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    		RaycastHit buildHit;

    		if(Physics.Raycast(buildRay,out buildHit, 1000))
    		{
    			if(buildHit.collider)
    			{
    				GameObject hitObj = buildHit.collider.gameObject;
    				hitObject = hitObj;

    				if((isContinuesBuild||isBuildingMass)&&!erase)
    				{
    					if(!buildHit.collider.gameObject.name.Equals("Grid"))
    					{
    						canBuild = false;
							bool skipThisIsTc = false;

							if(isBuildingMass&&isMouseDown)
							{
								if(buildHit.collider.gameObject.name.Equals("uteTcDummy(Clone)"))
								{
									Destroy(buildHit.collider.gameObject);
								}
							}
    						else if(buildHit.collider.gameObject.GetComponent<uteTcTag>()&&isBuildingTC)
    						{
    							if(buildHit.collider.gameObject.GetComponent<uteTcTag>().tcFamilyName==currentTcFamilyName)
    							{
	    							if(Input.GetMouseButtonDown(0))
	    							{
	    								uTCE.tcBuildStart(tcGoes,tcNames,tcGuids,currentTcFamilyName,tcRots,currentTCID);
	    								isMouseDown = true;
	    								//uTCE.AddTile(buildHit.collider.gameObject);
	    								Destroy(buildHit.collider.gameObject);
	    								skipThisIsTc = true;
	    							}
	    							else if(isMouseDown)
	    							{
	    								Destroy(buildHit.collider.gameObject);
	    								skipThisIsTc = true;
	    							}
	    						}
    						}

    						if(!skipThisIsTc)
    						{
    							if(buildHit.normal!=Vector3.up)
    							{
    								return;
    							}
    						}
    						else
    						{
    							return;
    						}
    					}
    				}

    				if(erase)
    				{
    					return;
    				}

    				bool collZB = false;
					bool collZA = false;
					bool collXB = false;
					bool collXA = false;
					bool collYB = false;
					bool collYA = false;
    				float sizeX = currentTile.GetComponent<Collider>().bounds.size.x;//*currentTile.transform.localScale.x;
    				float sizeY = currentTile.GetComponent<Collider>().bounds.size.y;//*currentTile.transform.localScale.y;
	  				float sizeZ = currentTile.GetComponent<Collider>().bounds.size.z;//*currentTile.transform.localScale.z;
    				float centerX = ((float)sizeX)/2.0f;
    				float centerY = ((float)sizeY)/2.0f;
    				float centerZ = ((float)sizeZ)/2.0f;
					float centerPosX = centerX+(currentTile.transform.position.x-((float)sizeX/2.0f));
					float centerPosY = centerY+(currentTile.transform.position.y-((float)sizeY/2.0f));
					float centerPosZ = centerZ+(currentTile.transform.position.z-((float)sizeZ/2.0f));
					int castSizeX = (int) currentTile.GetComponent<Collider>().bounds.size.x;
					int castSizeZ = (int) currentTile.GetComponent<Collider>().bounds.size.z;
					int castSizeSide;

					if(castSizeX==castSizeZ)
					{
						castSizeSide = castSizeX;
					}
					else if(castSizeX>castSizeZ)
					{
						castSizeSide = castSizeX;
					}
					else
					{
						castSizeSide = castSizeZ;
					}

					float normalX = ZERO_F;
					float normalZ = ZERO_F;
					float normalY = ZERO_F;


					if(buildHit.normal.y==ZERO_F)
					{
						if((int)buildHit.normal.x>ZERO_F)
						{
							normalX = 0.5f;
						}
						else if((int)buildHit.normal.x<ZERO_F)
						{
							normalX = -0.5f;
						}

						if((int)buildHit.normal.z>ZERO_F)
						{
							normalZ = 0.5f;
						}
						else if((int)buildHit.normal.z<ZERO_F)
						{
							normalZ = -0.5f;
						}
					}


					if(buildHit.normal.y>ZERO_F)
					{
						normalY = 0.5f;
					}
					else if(buildHit.normal.y<ZERO_F)
					{
						normalY = -0.5f;
					}

					float internalOffsetX = ZERO_F;
					float internalOffsetZ = ZERO_F;
					float internalOffsetY = ZERO_F;

					if(Mathf.Round(currentTile.GetComponent<Collider>().bounds.size.z)%2==0)
					{
						internalOffsetZ = 0.5f;
					}

					if(Mathf.Round(currentTile.GetComponent<Collider>().bounds.size.x)%2==0)
					{
						internalOffsetX = 0.5f;
					}

					if(Mathf.Round(currentTile.GetComponent<Collider>().bounds.size.y)%2==0)
					{
						internalOffsetY = 0.5f;
					}

					float offsetFixX = 0.05f;//*currentTile.transform.localScale.x;
					float offsetFixZ = 0.05f;//*currentTile.transform.localScale.z;
					float offsetFixY = 0.05f;//*currentTile.transform.localScale.y;
					float castPosX = currentTile.GetComponent<Collider>().bounds.center.x;//centerPosX+(currentTile.GetComponent<BoxCollider>().center.x*currentTile.transform.localScale.x);
					float castPosZ = currentTile.GetComponent<Collider>().bounds.center.z;//centerPosZ+(currentTile.GetComponent<BoxCollider>().center.z*currentTile.transform.localScale.z);
					float castPosY = currentTile.GetComponent<Collider>().bounds.center.y;//centerPosY+(currentTile.GetComponent<BoxCollider>().center.y*currentTile.transform.localScale.y);

					Vector3 castFullPos = new Vector3(castPosX,castPosY,castPosZ);
					Vector3 checkXA = new Vector3(castPosX+centerX-offsetFixX,castPosY,castPosZ);
					Vector3 checkXB = new Vector3(castPosX-centerX+offsetFixX,castPosY,castPosZ);
					Vector3 checkZA = new Vector3(castPosX,castPosY,castPosZ-offsetFixZ+centerZ);
					Vector3 checkZB = new Vector3(castPosX,castPosY,castPosZ+offsetFixZ-centerZ);
					Vector3 checkYA = new Vector3(castPosX,castPosY+centerY-offsetFixY,castPosZ);
					Vector3 checkYB = new Vector3(castPosX,castPosY-centerY+offsetFixY,castPosZ);

					// debug
					Debug.DrawLine(castFullPos,checkXA,Color.red,ZERO_F,false);
					Debug.DrawLine(castFullPos,checkXB,Color.red,ZERO_F,false);
					Debug.DrawLine(castFullPos,checkZA,Color.green,ZERO_F,false);
					Debug.DrawLine(castFullPos,checkZB,Color.green,ZERO_F,false);
					Debug.DrawLine(castFullPos,checkYA,Color.blue,ZERO_F,false);
					Debug.DrawLine(castFullPos,checkYB,Color.blue,ZERO_F,false);
									
					collXB = false;
					collXA = false;
					collZB = false;
					collZA = false;
					collYB = false;
					collYA = false;

					float offsetToMoveXA = ZERO_F;
					float offsetToMoveXB = ZERO_F;
					float offsetToMoveZA = ZERO_F;
					float offsetToMoveZB = ZERO_F;
					float offsetToMoveYA = ZERO_F;
					float offsetToMoveYB = ZERO_F;

					if(uteGLOBAL3dMapEditor.OverlapDetection)
					{
						RaycastHit lineHit;
						if(Physics.Linecast(castFullPos+new Vector3(0,0,offsetFixZ), checkZB, out lineHit))
						{
							offsetToMoveZB = Mathf.Abs(Mathf.Round((Mathf.Abs(Mathf.Abs(lineHit.point.z)-Mathf.Abs(currentTile.transform.position.z)))-(currentTile.GetComponent<Collider>().bounds.size.z/2.0f)));
							collZB = true;
						}

						if(Physics.Linecast(castFullPos+new Vector3(0,0,-offsetFixZ), checkZA, out lineHit))
						{
							offsetToMoveZA = Mathf.Abs(Mathf.Round((Mathf.Abs(Mathf.Abs(lineHit.point.z)-Mathf.Abs(currentTile.transform.position.z)))-(currentTile.GetComponent<Collider>().bounds.size.z/2.0f)));
							collZA = true;
						}

						if(Physics.Linecast(castFullPos+new Vector3(0,offsetFixY,0), checkYB, out lineHit))
						{
							offsetToMoveYB = Mathf.Abs(Mathf.Round((Mathf.Abs(Mathf.Abs(lineHit.point.y)-Mathf.Abs(currentTile.transform.position.y)))-(currentTile.GetComponent<Collider>().bounds.size.y/2.0f)));
							collYB = true;
						}

						if(Physics.Linecast(castFullPos+new Vector3(0,-offsetFixY,0), checkYA, out lineHit))
						{
							offsetToMoveYA = Mathf.Abs(Mathf.Round((Mathf.Abs(Mathf.Abs(lineHit.point.y)-Mathf.Abs(currentTile.transform.position.y)))-(currentTile.GetComponent<Collider>().bounds.size.y/2.0f)));
							collYA = true;
						}

						if(Physics.Linecast(castFullPos+new Vector3(offsetFixX,0,0), checkXB, out lineHit))
						{
							offsetToMoveXB = Mathf.Abs(Mathf.Round((Mathf.Abs(Mathf.Abs(lineHit.point.x)-Mathf.Abs(currentTile.transform.position.x)))-(currentTile.GetComponent<Collider>().bounds.size.x/2.0f)));
							collXB = true;
						}

						if(Physics.Linecast(castFullPos+new Vector3(-offsetFixX,0,0), checkXA, out lineHit))
						{
							offsetToMoveXA = Mathf.Abs(Mathf.Round((Mathf.Abs(Mathf.Abs(lineHit.point.x)-Mathf.Abs(currentTile.transform.position.x)))-(currentTile.GetComponent<Collider>().bounds.size.x/2.0f)));
							collXA = true;
						}

						bool fixingY = false;

						if(collYA||collYB)
						{
							if(collYA&&!collYB)
							{
								offsetY-=offsetToMoveYA;
								fixingY = true;
							}
							else if(collYB&&!collYA)
							{
								offsetY+=offsetToMoveYB;
								fixingY = true;
							}
						}

						if(!fixingY)
						{
							if(collZA||collZB)
							{
								if(collZA&&!collZB)
								{
									offsetZ-=offsetToMoveZA;
								}
								else if(collZB&&!collZA)
								{
									offsetZ+=offsetToMoveZB;
								}
							}
							
							if(collXA||collXB)
							{
								if(collXA&&!collXB)
								{
									offsetX-=offsetToMoveXA;
								}
								else if(collXB&&!collXA)
								{
									offsetX+=offsetToMoveXB;
								}
							}
						}
					}

					float Xpivot = 0.0f; 
					float Zpivot = 0.0f;

					if(uteGLOBAL3dMapEditor.CalculateXZPivot)
					{
						Xpivot = -currentTile.GetComponent<BoxCollider>().center.x;
						Zpivot = -currentTile.GetComponent<BoxCollider>().center.z;

						if(isRotated==1||isRotated==3)
						{
							float _xpivot = Xpivot;
							Xpivot = Zpivot;
							Zpivot = _xpivot;
						}
					}

					float posX = (Mathf.Round(((buildHit.point.x+normalX)))+internalOffsetX+Xpivot);//-(currentTile.GetComponent<BoxCollider>().center.x*currentTile.transform.localScale.x)));
					float posZ = (Mathf.Round(((buildHit.point.z+normalZ)))+internalOffsetZ+Zpivot);//-(currentTile.GetComponent<BoxCollider>().center.z*currentTile.transform.localScale.z)));
					float posY = 0.0f;

					if(yTypeOption.Equals("auto"))
					{
						if(buildHit.normal==Vector3.up)
						{
							//Debug.Log("("+buildHit.point.y+"+("+currentTile.collider.bounds.size.y+"/2.0f))-"+currentTile.GetComponent<BoxCollider>().center.y+"0.0000001f");;
							posY = (buildHit.point.y+(currentTile.GetComponent<Collider>().bounds.size.y/2.0f))-(currentTile.GetComponent<BoxCollider>().center.y*currentTile.transform.localScale.y)+0.000001f;//posY = (Mathf.Round(buildHit.point.y+normalY)+internalOffsetY);
							//Debug.Log(posY);
						}
						else
						{
							if(currentTile.name.Equals(buildHit.collider.gameObject.name))
							{
								posY = buildHit.collider.gameObject.transform.position.y;
							}
							else
							{
								posY = 0.1f * ((Mathf.Round((buildHit.point.y+normalY)*10.0f))+internalOffsetY);
							}
						}
					}
					else if(yTypeOption.Equals("nosnap"))
					{
						posY = buildHit.point.y+(currentTile.GetComponent<Collider>().bounds.size.y/2.0f)-currentTile.GetComponent<BoxCollider>().center.y+0.000001f;
					}
					else if(yTypeOption.Equals("fixed"))
					{
						posY = 0.1f * ((Mathf.Round((buildHit.point.y+normalY)*10.0f))+internalOffsetY);//posY = (Mathf.Round(buildHit.point.y+normalY)+internalOffsetY);
					}

					if(Mathf.Abs(lastPosX-posX)>0.09f||Mathf.Abs(lastPosY-posY)>0.09f||Mathf.Abs(lastPosZ-posZ)>0.09f)
					{
						offsetZ = ZERO_F;
						offsetX = ZERO_F;
						offsetY = ZERO_F;
						offsetToMoveXA = ZERO_F;
						offsetToMoveXB = ZERO_F;
						offsetToMoveZA = ZERO_F;
						offsetToMoveZB = ZERO_F;
						offsetToMoveYA = ZERO_F;
						offsetToMoveYB = ZERO_F;
					}

					lastPosX = posX;
    				lastPosY = posY;
    				lastPosZ = posZ;
    				
    				float finalPosY = posY+offsetY;// * 100.0f);
    				posX = (posX+offsetX);// * 100.0f);
    				posZ = (posZ+offsetZ);// * 100.0f);
					float addOnTop = 0.0f;

					if(yTypeOption.Equals("auto"))
					{
						if(currentTile.GetComponent<Collider>().bounds.size.y<0.011f)
						{
							addOnTop = 0.002f;
						}
					}

    				Vector3 calculatedPosition = new Vector3(posX,finalPosY+addOnTop,posZ);

    				if(uteGLOBAL3dMapEditor.XZsnapping==false)
    				{
    					calculatedPosition = new Vector3(buildHit.point.x+Xpivot,finalPosY+addOnTop,buildHit.point.z+Zpivot);
    				}

    				cameraMove.sel = calculatedPosition;

    				if(((collZA&&collZB)||(collXA&&collXB)||(collYA&&collYB))||(!collZA&&!collZB&&!collXA&&!collXB&&!collYA&&!collYB))
    				{
    					currentTile.transform.position = calculatedPosition;
    				}

    				currentTile.transform.position = calculatedPosition;

    				if(((collZA&&collZB)||(collZA||collZB))||((collXA&&collXB)||(collXA||collXB))||((collYA&&collYB)||(collYA||collYB)))
					{
						canBuild = false;
					}
					else if(!uteGLOBAL3dMapEditor.canBuild)
					{
    					canBuild = true;
    				}
    				else
    				{	
    					canBuild = true;
    				}
    			}
    			else
    			{
    				canBuild = false;
    			}
    		}
    		else
    		{
    			canBuild = false;
    		}

    		if(canBuild&&!erase)
    		{
    			helpers_CANTBUILD.transform.position = new Vector3(-1000,0,-1000);

    			if(Input.GetMouseButtonDown(0))
    			{
    				isMouseDown = true;

    				if(!isContinuesBuild&&!isBuildingTC)
    					ApplyBuild();

    				if(isBuildingTC)
    				{
    					uTCE.tcBuildStart(tcGoes,tcNames,tcGuids,currentTcFamilyName,tcRots,currentTCID);
    				}

    				if(isBuildingMass)
    				{
    					uMBE.massBuildStart(currentTile,currentObjectID,currentObjectGUID);
    				}
	   			}
    		}
    		else
    		{
    			if(!isBuildingTC&&!erase)
    			{
    				helpers_CANTBUILD.transform.position = currentTile.transform.position+new Vector3(currentTile.GetComponent<BoxCollider>().center.x*currentTile.transform.localScale.x,currentTile.GetComponent<BoxCollider>().center.y*currentTile.transform.localScale.y,currentTile.GetComponent<BoxCollider>().center.z*currentTile.transform.localScale.z);
					helpers_CANTBUILD.transform.localScale = currentTile.GetComponent<Collider>().bounds.size + new Vector3(0.1f,0.1f,0.1f);
	    		}
    		}
		}
	}

	private void FixedUpdate()
	{
		if(isMouseDown&&(isContinuesBuild||isBuildingMass)&&canBuild)
		{
			if(isBuildingMass)
			{
				if(currentTile)
				{
					StartCoroutine(uMBE.AddTile(currentTile));
				}
			}
			else if(!isBuildingTC)
			{
				ApplyBuild();
			}
			else
			{
				if(currentTile)
				{
					uTCE.AddTile(currentTile);
				}
			}
		}
	}
	
	private IEnumerator SmoothObjInit(GameObject obj)
	{
		Vector3 wholeScale = obj.transform.localScale;
		float objSizeY = obj.transform.localScale.y;
		float objSizeYDiv = objSizeY / 5.0f;

		obj.transform.localScale = new Vector3(obj.transform.localScale.x,0,obj.transform.localScale.z);
		obj.transform.position -= new Vector3(0,objSizeY,0);

		for(int i=0;i<5;i++)
		{
			obj.transform.localScale += new Vector3(0,objSizeYDiv,0);
			obj.transform.position += new Vector3(0,objSizeYDiv,0);
			yield return 0;
		}

		obj.transform.localScale = wholeScale;

		yield return 0;
	}

	public void ApplyBuild(GameObject tcObj=null, Vector3 tcPos = default(Vector3), string tcName = default(string), string tcGuid = default(string), Vector3 tcRot = default(Vector3), string tcFamilyName = default(string), bool isMassBuild = false)
	{
		GameObject newObj = null;
		bool goodToGo = false;

		if(tcObj!=null)
		{
			Vector3 _pos = new Vector3(RoundTo(tcPos.x),tcPos.y,RoundTo(tcPos.z));

			if(!uteGLOBAL3dMapEditor.XZsnapping)
			{
				_pos = new Vector3(tcPos.x,tcPos.y,tcPos.z);
			}

			newObj = (GameObject) Instantiate(tcObj,_pos,tcObj.transform.rotation);

			if(!isMassBuild)
			{
				isCurrentStatic = true;
				uteTagObject utag = (uteTagObject) newObj.AddComponent<uteTagObject>();
				utag.objGUID = tcGuid;
				utag.isStatic = true;
				utag.isTC = true;
				uteTcTag uTT = (uteTcTag) newObj.AddComponent<uteTcTag>();
				newObj.transform.Rotate(tcRot);
				uTT.tcFamilyName = tcFamilyName;
			}
			else
			{
				newObj.transform.localEulerAngles = tcRot;
				newObj.GetComponent<uteTagObject>().isTC = false;
			}

			goodToGo = true;
		}
		else
		{
			if(currentTile)
			{
				float newTileDistance = 1000.0f;
				float newDistanceReq = 0.0f;

				if(lastTile!=null)
				{
					Vector3 lastTilePos = lastTile.transform.position;
					Vector3 currentTilePos = currentTile.transform.position;

					float tileDistanceX = Mathf.Floor(Mathf.Abs(lastTilePos.x-currentTilePos.x));
					float tileDistanceZ = Mathf.Floor(Mathf.Abs(lastTilePos.z-currentTilePos.z));
					bool skip = false;

					if(tileDistanceX!=0.0f)
					{
						newTileDistance = tileDistanceX;
						newDistanceReq = currentTile.GetComponent<Collider>().bounds.size.x;
					}
					else if(tileDistanceZ!=0.0f)
					{
						newTileDistance = tileDistanceZ;
						newDistanceReq = currentTile.GetComponent<Collider>().bounds.size.z;
					}
					else
					{
						skip = true;
						goodToGo = false;
					}

					if(!skip)
					{
						if((newTileDistance>newDistanceReq-0.01f)&&((Mathf.Abs(lastTile.transform.position.y-currentTile.transform.position.y)<0.01f)))
						{
							Vector3 _pos = new Vector3(RoundTo(currentTile.transform.position.x),currentTile.transform.position.y,RoundTo(currentTile.transform.position.z));

							if(!uteGLOBAL3dMapEditor.XZsnapping)
							{
								_pos = new Vector3(currentTile.transform.position.x,currentTile.transform.position.y,currentTile.transform.position.z);
							}

							newObj = (GameObject) Instantiate(currentTile,_pos,currentTile.transform.rotation);
							lastTile = newObj;
							goodToGo = true;
						}
						else
						{
							goodToGo = false;
						}
					}
				}
				else
				{
					Vector3 _pos = new Vector3(RoundTo(currentTile.transform.position.x),currentTile.transform.position.y,RoundTo(currentTile.transform.position.z));

					if(!uteGLOBAL3dMapEditor.XZsnapping)
					{
						_pos = new Vector3(currentTile.transform.position.x,currentTile.transform.position.y,currentTile.transform.position.z);
					}

					newObj = (GameObject) Instantiate(currentTile,_pos,currentTile.transform.rotation);
					
					goodToGo = true;
				}

				if(lastTile==null)
				{
					lastTile = newObj;
				}
			}
		}
	 	
	 	if((currentTile||tcObj)&&goodToGo)
	 	{
			newObj.layer = 0;
			Destroy(newObj.GetComponent<Rigidbody>());
			Destroy(newObj.GetComponent<uteDetectBuildCollision>());
			newObj.GetComponent<Collider>().isTrigger = false;
		}

		if(goodToGo)
		{
			RoundTo90(newObj);

			if(isCurrentStatic||isItNewPattern)
			{
				newObj.transform.parent = MAP_STATIC.transform;
				newObj.isStatic = true;
			}
			else
			{
				newObj.transform.parent = MAP_DYNAMIC.transform;
				newObj.isStatic = false;
			}

			if(tcObj!=null)
			{
				newObj.name = tcName;
			}
			else
			{
				newObj.name = currentObjectID;
			}
		}

		if(goodToGo)
		{
			uteGLOBAL3dMapEditor.mapObjectCount++;
		}
	}

	private IEnumerator ReloadTileAssets()
	{
		StreamReader rd = new StreamReader(catInfoPath);
		string rdinfo = rd.ReadToEnd();
		rd.Close();
		StreamWriter rw = new StreamWriter(catInfoPath);
		rw.Write("");
		rw.Write(rdinfo);
		rw.Close();
		
		yield return null;
		
		LoadGlobalSettings();
		
		yield return null;
		
		UnityEditor.AssetDatabase.SaveAssets();
		UnityEditor.AssetDatabase.Refresh();
		
		yield return null;
		
		LoadTools();
		yield return StartCoroutine(LoadTiles());
		LoadTilesIntoGUI();
		FinalizeGridAndCamera();
	}

	private IEnumerator LoadTiles()
	{	
		GameObject TEMP_STATIC = new GameObject("static_objs");
		TEMP_STATIC.isStatic = true;
		GameObject TEMP_DYNAMIC = new GameObject("dynamic_objs");
		GameObject TEMP = (GameObject) GameObject.Find("TEMP");
		GameObject TEMP_TC = new GameObject("TC");
		TEMP_TC.transform.parent = TEMP.transform;
		TEMP_STATIC.transform.parent = TEMP.transform;
		TEMP_DYNAMIC.transform.parent = TEMP.transform;
		TEMP.transform.position -= new Vector3(-1000000000.0f,100000000.0f,-1000000000.0f);

		TextAsset _allTilesConnectionInfo = (TextAsset) Resources.Load("uteForEditor/uteTileConnections");
		string allTilesConnectionInfo = _allTilesConnectionInfo.text;
		string[] allTilesConnectionbycat = (string[]) allTilesConnectionInfo.Split('|');

		for(int i=0;i<allTilesConnectionbycat.Length;i++)
		{
			if(!allTilesConnectionbycat[i].ToString().Equals(""))
			{
				string[] splitedtcinfo = (string[]) allTilesConnectionbycat[i].ToString().Split('$');
				string[] splitedtcguids = (string[]) splitedtcinfo[1].Split(':');
				string[] splitedtcrots = (string[]) splitedtcinfo[2].Split(':');
				string tcName = splitedtcinfo[0];
				GameObject tcDIR = new GameObject(tcName);
				tcDIR.transform.parent = TEMP_TC.transform;
				List<GameObject> tcObjs = new List<GameObject>();
				List<Texture2D> tcObjsP = new List<Texture2D>();
				List<string> tcObjsNames = new List<string>();
				List<string> tcGuidNames = new List<string>();
				List<string> tcRotsNames = new List<string>();

				for(int k=0;k<splitedtcrots.Length;k++)
				{
					if(!splitedtcrots[k].Equals(""))
					{
						tcRotsNames.Add(splitedtcrots[k]);
					}
				}

				for(int j=0;j<splitedtcguids.Length;j++)
				{
					if(!splitedtcguids[j].ToString().Equals(""))
					{
						string opath = UnityEditor.AssetDatabase.GUIDToAssetPath(splitedtcguids[j].ToString());
						GameObject tGO = (GameObject) UnityEditor.AssetDatabase.LoadMainAssetAtPath(opath);

						if(tGO)
						{
							if((Object)tGO)
							{
								for(int w=0;w<10;w++)
								{
									previewTTC = UnityEditor.AssetPreview.GetAssetPreview((Object)tGO);

									if(previewTTC)
									{
										break;
									}
									else
									{
										yield return 0;
									}
								}
							}

							tcGuidNames.Add(splitedtcguids[j].ToString());

							tcObjsNames.Add(tGO.name);
							GameObject tmp_tGO = (GameObject) Instantiate(tGO,Vector3.zero,tGO.transform.rotation);
							tmp_tGO.name = splitedtcguids[j].ToString();
							List<GameObject> twoGO = new List<GameObject>();
							twoGO = createColliderToObject(tmp_tGO,tGO);
							GameObject behindGO = (GameObject) twoGO[0];
							behindGO.name = tmp_tGO.name;
							GameObject objGO = (GameObject) twoGO[1];
							tGO = objGO;
							tGO.transform.parent = behindGO.transform;
							behindGO.layer = 2;
							behindGO.transform.parent = tcDIR.transform;
							tcObjs.Add(behindGO);

							if(previewTTC)
							{
								tcObjsP.Add(previewTTC);
							}
							else
							{
								tcObjsP.Add(new Texture2D(20,20));
							}
						}
					}
				}

				if(!tcName.Equals("")&&tcObjs.Count>0)
				{
					allTCTiles.Add(new tcInfo(tcName,tcObjsNames,tcObjs,tcObjsP,tcGuidNames,tcRotsNames));
				}
				else
				{
					if(tcObjs.Count<=0)
					{
						Debug.Log ("Warning: Tile-Connection ["+tcName+"] was ignored because there are no objects inside");
					}
					else
					{
						Debug.Log ("Something is Wrong (TC)");
					}
				}
			}
		}

		TextAsset _alltilesinfo = (TextAsset) Resources.Load ("uteForEditor/uteCategoryInfo");
		string alltilesinfo = _alltilesinfo.text;
		string[] allinfobycat = (string[]) alltilesinfo.Split('|');

		for(int i=0;i<allinfobycat.Length;i++)
		{
			if(!allinfobycat[i].ToString().Equals(""))
			{
				string[] splitedinfo = (string[]) allinfobycat[i].ToString().Split('$');
				string[] splitedguids = (string[]) splitedinfo[2].ToString().Split(':');
				string cName = splitedinfo[0].ToString();
				string cColl = splitedinfo[1].ToString();
				string cType = splitedinfo[3].ToString();
				List<GameObject> cObjs = new List<GameObject>();
				List<Texture2D> cObjsP = new List<Texture2D>();
				List<string> cObjsNames = new List<string>();
				List<string> cObjsGuids = new List<string>();
				
				for(int j=0;j<splitedguids.Length;j++)
				{
					if(!splitedguids[j].ToString().Equals(""))
					{
						string opath = UnityEditor.AssetDatabase.GUIDToAssetPath(splitedguids[j].ToString());
						GameObject tGO = (GameObject) UnityEditor.AssetDatabase.LoadMainAssetAtPath(opath);
						
						if(tGO)
						{
							if((Object)tGO)
							{
								for(int w=0;w<10;w++)
								{
									previewT = UnityEditor.AssetPreview.GetAssetPreview((Object)tGO);

									if(previewT)
									{
										break;
									}
									else
									{
										yield return 0;
									}
								}
							}
							
							cObjsNames.Add(tGO.name);
							cObjsGuids.Add(splitedguids[j].ToString());
							GameObject tmp_tGO = (GameObject) Instantiate(tGO,Vector3.zero,tGO.transform.rotation);
							tmp_tGO.name = splitedguids[j].ToString();
							List<GameObject> twoGO = new List<GameObject>();
							twoGO = createColliderToObject(tmp_tGO,tGO);
							GameObject behindGO = (GameObject) twoGO[0];
							GameObject objGO = (GameObject) twoGO[1];
							tGO = objGO;
							tGO.transform.parent = behindGO.transform;
							behindGO.layer = 2;
							//behindGO.transform.parent = cDIR.transform;
							cObjs.Add(behindGO);
							
							if(cType.Equals("Static"))
							{
								behindGO.transform.parent = TEMP_STATIC.transform;
								tmp_tGO.isStatic = true;
							}
							else if(cType.Equals("Dynamic"))
							{
								behindGO.transform.parent = TEMP_DYNAMIC.transform;
								tmp_tGO.isStatic = false;
							}
							//GameObject.Find("TEMP").transform.position -= new Vector3(-1000000000.0f,100000000.0f,-1000000000.0f);
							
							if(previewT)
							{
								cObjsP.Add(previewT);
							}
							else
							{
								cObjsP.Add(new Texture2D(20,20));
							}
						}
					}
				}
				
				if(!cName.Equals("")&&!cColl.Equals("")&&cObjs.Count>0)
				{
					allTiles.Add(new catInfo(cName,cObjsNames,cColl,cObjs,cObjsP,cObjsGuids));
				}
				else
				{
					if(cObjs.Count<=0)
					{
						Debug.Log ("Warning: Category ["+cName+"] was ignored because there are no objects inside");
					}
					else
					{
						Debug.Log ("Something is Wrong (CE)");
					}
				}
			}
		}

		yield return 0;
	}
	
	private void LoadTools()
	{
		MAIN = new GameObject("MAIN");
		cameraMove = MAIN.AddComponent<uteCameraMove>();
		saveMap = this.gameObject.AddComponent<uteSaveMap>();

	//	cameraMove = (CameraMove) ((GameObject) GameObject.Find ("MapEditorCamera")).AddComponent<CameraMove>();
		GameObject _grid = (GameObject) Resources.Load("uteForEditor/uteLayer");
		grid = (GameObject) Instantiate(_grid,new Vector3((gridsize.x/2)+0.5f,0.0f,(gridsize.z/2)+0.5f),_grid.transform.rotation);
		grid.name = "Grid";

		if(globalGridSizeX%2.0f!=0.0f)
		{
			grid.transform.position -= new Vector3(0.5f,0,0);
		}

		if(globalGridSizeZ%2.0f!=0.0f)
		{
			grid.transform.position -= new Vector3(0,0,0.5f);
		}

		cameraMove.gameObject.transform.position = Vector3.zero;
		MAP = (GameObject) GameObject.Find ("MAP");
		GameObject.Find("MapEditorCamera").transform.parent = MAIN.transform;
		GameObject.Find("MapEditorCamera").transform.position = Vector3.zero;	

		if(isItNewPattern)
		{
			exporter = (uteExporter) cameraMove.gameObject.AddComponent<uteExporter>();
			exporter.MAP_STATIC = MAP_STATIC;
			exporter.mapName = newProjectName;
		}
		
		lastSelectedIndex = -10;
		lastSelectedIndexTC = -10;

		SetGrid(globalGridSizeX,globalGridSizeZ);
		SetCamera(cameraType);
		mouseOrbit.target = grid.transform;
	}
	
	private void LoadTilesIntoGUI()
	{
		comboBoxList = new GUIContent[allTiles.Count];
		
		for(int i=0;i<allTiles.Count;i++)
		{
			catInfo cI = (catInfo) allTiles[i];
			
			comboBoxList[i] = new GUIContent((string)cI.catName);
		}
		
		comboBoxList_TileConnections = new GUIContent[allTCTiles.Count];

		for(int i=0;i<allTCTiles.Count;i++)
		{
			tcInfo tcI = (tcInfo) allTCTiles[i];
			comboBoxList_TileConnections[i] = new GUIContent((string)tcI.tcName);
		}

		listStyle.normal.textColor = Color.white;
		listStyle.normal.background = new Texture2D(0,0);
		listStyle.onHover.background = new Texture2D(2, 2);
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.padding.bottom = 4;
	}

	private void LoadGlobalSettings()
	{
		StreamReader rd = new StreamReader(settingsInfoPath);
		string info = rd.ReadToEnd();
		rd.Close();
		StreamWriter rw = new StreamWriter(settingsInfoPath);
		rw.Write("");
		rw.Write(info);
		rw.Close();
		
		if(!info.Equals(""))
		{
			string[] infoSplited = info.Split(':');
			
			globalYSize = System.Convert.ToSingle(infoSplited[0].ToString());

			rightClickOption = infoSplited[1].ToString();
			yTypeOption = infoSplited[2].ToString();
			globalGridSizeX = System.Convert.ToInt32(infoSplited[3]);
			globalGridSizeZ = System.Convert.ToInt32(infoSplited[4]);
			cameraType = infoSplited[5].ToString();
		}
		else
		{
			Debug.Log ("Error: Failed to load settings. Loading default settings.");
			globalYSize = 1.0f;

			rightClickOption = "rotL";
			globalGridSizeZ = 1000;
			globalGridSizeX = 1000;
			cameraType = "isometric-perspective";
		}
	}
	
	private void SetCamera(string camType)
	{
		GameObject rotationArea = new GameObject("YArea");
		cameraGO.transform.parent = rotationArea.transform;
		rotationArea.transform.parent = MAIN.transform;
		cameraGO.transform.position = Vector3.zero;
		mouseOrbit = (uteMouseOrbit) rotationArea.AddComponent<uteMouseOrbit>();
		Camera camTemp = cameraGO.GetComponent<Camera>();

		if(camType.Equals("isometric-perspective"))
		{
			MAIN.transform.localEulerAngles = new Vector3(0,45,0);
			cameraGO.transform.localEulerAngles = new Vector3(30,0,0);
			camTemp.orthographic = false;
			camTemp.fieldOfView = 60;
			camTemp.farClipPlane = 1000.0f;
			isOrtho = false;
			is2D = false;
		}
		else if(camType.Equals("isometric-ortho"))
		{
			MAIN.transform.localEulerAngles = new Vector3(0,45,0);
			cameraGO.transform.localEulerAngles = new Vector3(30,0,0);
			camTemp.orthographic = true;
			camTemp.orthographicSize = 5;
			camTemp.nearClipPlane = -100.0f;
			camTemp.farClipPlane = 1000.0f;
			is2D = false;
			isOrtho = true;
		}
		else if(camType.Equals("2d-perspective"))
		{
			MAIN.transform.localEulerAngles = new Vector3(0,0,0);
			camTemp.orthographic = false;
			camTemp.nearClipPlane = 0.1f;
			camTemp.farClipPlane = 1000.0f;
			isOrtho = false;
			is2D = true;
		}
		else if(camType.Equals("2d-ortho"))
		{
			MAIN.transform.localEulerAngles = new Vector3(0,0,0);
			camTemp.orthographic = true;
			camTemp.orthographicSize = 5;
			camTemp.nearClipPlane = -10.0f;
			camTemp.farClipPlane = 300.0f;
			isOrtho = true;
			is2D = true;
		}
	}

	private void FinalizeGridAndCamera()
	{
		if(is2D)
		{
			cameraMove.is2D = true;
			cameraGO.transform.Rotate(new Vector3(90,0,0));
			MAIN.transform.position = new Vector3(500,14,490);
		}
		else
		{
			cameraMove.is2D = false;
			
			if(isOrtho)
			{
				MAIN.transform.position = new Vector3(492,8,492);
			}
			else
			{
				MAIN.transform.position = new Vector3(493,8,493);
			}
		}
	}

	private void SetGrid(int x, int z)
	{
		grid.transform.localScale = new Vector3((float)x,0.01f,(float)z);
		grid.GetComponent<Renderer>().material.mainTextureScale = new Vector2((float)x,(float)z);
	}

	private void ReloadCatPrevs()
	{
		for(int i=0;i<allTiles.Count;i++)
		{
			catInfo ct = (catInfo) allTiles[i];
			
			ct.catObjsPrevs.Clear();
			
			for(int j=0;j<ct.catObjs.Count;j++)
			{
				GameObject tGO = (GameObject) ct.catObjs[j];
				
				if((Object)tGO)
				{
					previewT = UnityEditor.AssetPreview.GetAssetPreview((Object)tGO);
				}
				
				if(previewT)
				{
					ct.catObjsPrevs.Add(previewT);
				}
				else
				{
					ct.catObjsPrevs.Add(new Texture2D(20,20));
				}
			}
		}
	}

	public List<GameObject> createColliderToObject(GameObject obj, GameObject obj_rs)
	{
		float lowestPointY = 10000.0f;
		float highestPointY = -10000.0f;
		float lowestPointZ = 10000.0f;
		float highestPointZ = -10000.0f;
		float lowestPointX = 10000.0f;
		float highestPointX = -10000.0f;
		float finalYSize = 1.0f;
		float finalZSize = 1.0f;
		float finalXSize = 1.0f;
		float divX = 2.0f;
		float divY = 2.0f;
		float divZ = 2.0f;
		
		Vector3 objScale = obj.transform.localScale;
		obj.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
		
		MeshFilter mfs = (MeshFilter) obj.GetComponent<MeshFilter>();
		MeshFilter[] mfs_arr = (MeshFilter[]) obj.GetComponentsInChildren<MeshFilter>();
		SkinnedMeshRenderer smfs = (SkinnedMeshRenderer) obj.GetComponent(typeof(SkinnedMeshRenderer));
		SkinnedMeshRenderer[] smfs_arr = (SkinnedMeshRenderer[]) obj.GetComponentsInChildren<SkinnedMeshRenderer>();
		Transform[] trms = (Transform[]) obj.GetComponentsInChildren<Transform>();
		
		if(mfs&&mfs.GetComponent<Renderer>())
		{
			lowestPointY = mfs.GetComponent<Renderer>().bounds.min.y;
			highestPointY = mfs.GetComponent<Renderer>().bounds.max.y;
		}
		
		if(mfs_arr.Length>0)
		{
			for(int i=0;i<mfs_arr.Length;i++)
			{
				MeshFilter mf_c = (MeshFilter) mfs_arr[i];
				
				if(mf_c&&mf_c.GetComponent<Renderer>())
				{
					if(mf_c.GetComponent<Renderer>().bounds.min.y<lowestPointY)
					{
						lowestPointY = mf_c.GetComponent<Renderer>().bounds.min.y;
					}
					
					if(mf_c.GetComponent<Renderer>().bounds.max.y>highestPointY)
					{
						highestPointY = mf_c.GetComponent<Renderer>().bounds.max.y;
					}
					
					if(mf_c.GetComponent<Renderer>().bounds.min.x<lowestPointX)
					{
						lowestPointX = mf_c.GetComponent<Renderer>().bounds.min.x;
					}
					
					if(mf_c.GetComponent<Renderer>().bounds.max.x>highestPointX)
					{
						highestPointX = mf_c.GetComponent<Renderer>().bounds.max.x;
					}
		
					if(mf_c.GetComponent<Renderer>().bounds.min.z<lowestPointZ)
					{
						lowestPointZ = mf_c.GetComponent<Renderer>().bounds.min.z;
					}
					
					if(mf_c.GetComponent<Renderer>().bounds.max.z>highestPointZ)
					{
						highestPointZ = mf_c.GetComponent<Renderer>().bounds.max.z;
					}
				}
			}
		}
		
		if(smfs)
		{
			lowestPointY = smfs.GetComponent<Renderer>().bounds.min.y;
			highestPointY = smfs.GetComponent<Renderer>().bounds.max.y;
		}
		
		if(smfs_arr.Length>0)
		{
			for(int i=0;i<smfs_arr.Length;i++)
			{
				SkinnedMeshRenderer smfs_c = (SkinnedMeshRenderer) smfs_arr[i];
				
				if(smfs_c)
				{
					if(smfs_c.GetComponent<Renderer>().bounds.min.y<lowestPointY)
					{
						lowestPointY = smfs_c.GetComponent<Renderer>().bounds.min.y;
					}
					
					if(smfs_c.GetComponent<Renderer>().bounds.max.y>highestPointY)
					{
						highestPointY = smfs_c.GetComponent<Renderer>().bounds.max.y;
					}
					
					if(smfs_c.GetComponent<Renderer>().bounds.min.x<lowestPointX)
					{
						lowestPointX = smfs_c.GetComponent<Renderer>().bounds.min.x;
					}
					
					if(smfs_c.GetComponent<Renderer>().bounds.max.x>highestPointX)
					{
						highestPointX = smfs_c.GetComponent<Renderer>().bounds.max.x;
					}
					
					if(smfs_c.GetComponent<Renderer>().bounds.min.z<lowestPointZ)
					{
						lowestPointZ = smfs_c.GetComponent<Renderer>().bounds.min.z;
					}
					
					if(smfs_c.GetComponent<Renderer>().bounds.max.z>highestPointZ)
					{
						highestPointZ = smfs_c.GetComponent<Renderer>().bounds.max.z;
					}
				}
			}
		}

		if(highestPointX - lowestPointX != -20000)
		{
			finalXSize = highestPointX - lowestPointX;
		} else { finalXSize = 1.0f; divX = 1.0f; lowestPointX = 0; Debug.Log ("X Something wrong with "+obj_rs.name); }
		
		if(highestPointY - lowestPointY != -20000)
		{
			finalYSize = highestPointY - lowestPointY;
		} else { finalYSize = globalYSize; divY = 1.0f; lowestPointY = 0; Debug.Log ("Y Something wrong with "+obj_rs.name); }
		
		if(highestPointZ - lowestPointZ != -20000)
		{
			finalZSize = highestPointZ - lowestPointZ;
		} else { finalZSize = 1.0f; divZ = 1.0f; lowestPointZ = 0; Debug.Log ("Z Something wrong with "+obj_rs.name); }
		
		for(int i=0;i<trms.Length;i++)
		{
			GameObject trm_go = (GameObject) ((Transform) trms[i]).gameObject;
			trm_go.layer = 2;
		}
		
		//BoxCollider obj_bc = obj.GetComponent<BoxCollider>();
		
		//if(!obj_bc)
		//{
		GameObject behindGO = new GameObject(obj.name);
		behindGO.AddComponent<BoxCollider>();
		obj.transform.parent = behindGO.transform;
		//}
		
		if(Mathf.Approximately(finalXSize,1.0f) || finalXSize<1.0f)
		{
			if(finalXSize<1.0f)
			{
				divX=1.0f;
				lowestPointX=-1.0f;
			}

			finalXSize=1.0f;
		}
		
		if(Mathf.Approximately(finalYSize,1.0f) || finalYSize<0.1f)
		{
		//	finalYSize=1.0f;
		//	divY=1.0f;
		//	lowestPointY=-1.0f;
		}
		
		if(Mathf.Approximately(finalYSize,0.0f)) { finalYSize = 0.01f; divY = 0.1f; lowestPointY = 0.0f; }
		
		if(Mathf.Approximately(finalZSize,1.0f) || finalZSize<1.0f)
		{
			if(finalZSize<1.0f)
			{
				divZ=1.0f;
				lowestPointZ=-1.0f;
			}

			finalZSize=1.0f;
		}
		behindGO.transform.localScale = objScale;
		((BoxCollider)behindGO.GetComponent(typeof(BoxCollider))).size = new Vector3(finalXSize,finalYSize,finalZSize);
		((BoxCollider)behindGO.GetComponent(typeof(BoxCollider))).center = new Vector3(finalXSize/divX+lowestPointX,finalYSize/divY+lowestPointY,finalZSize/divZ+lowestPointZ);
		
		
		
		//if(objScale.x<0.99||objScale.x>1.01||objScale.y>1.01||objScale.y<0.99||objScale.z>1.01||objScale.z<0.99)
		//	Debug.Log ("Warning: "+"("+obj.name+") is not using (1,1,1) localScale. This might couse some problems with map editor. We suggest to always use object scale = 1,1,1 and change mesh size instead.");
		
		DisableAllExternalColliders(obj);

		List<GameObject> twoGO = new List<GameObject>();
		twoGO.Add(behindGO);
		twoGO.Add(obj);

		return twoGO;
		
		//Destroy(obj);
	}

	private void DisableAllExternalColliders(GameObject obj)
	{
		BoxCollider[] boxColls = obj.GetComponentsInChildren<BoxCollider>();

		for(int i=0;i<boxColls.Length;i++)
		{
			BoxCollider coll = (BoxCollider) boxColls[i];
			if(coll) coll.enabled = false;
		}

		MeshCollider[] mrColls = obj.GetComponentsInChildren<MeshCollider>();

		for(int i=0;i<mrColls.Length;i++)
		{
			MeshCollider coll = (MeshCollider) mrColls[i];
			if(coll) coll.enabled = false;
		}

		SphereCollider[] spColls = obj.GetComponentsInChildren<SphereCollider>();

		for(int i=0;i<spColls.Length;i++)
		{
			SphereCollider coll = (SphereCollider) spColls[i];
			if(coll) coll.enabled = false;
		}

		CapsuleCollider[] cpColls = obj.GetComponentsInChildren<CapsuleCollider>();

		for(int i=0;i<cpColls.Length;i++)
		{
			CapsuleCollider coll = (CapsuleCollider) cpColls[i];
			if(coll) coll.enabled = false;
		}
	}

	private bool CheckGUIPass()
	{
		Vector2 normalMousePosition = new Vector2(Input.mousePosition.x,Screen.height-Input.mousePosition.y);

		if(new Rect(0,0,Screen.width,40).Contains(normalMousePosition))
		{
			return false;
		}
		else if(new Rect(0,Screen.height-40,Screen.width,40).Contains(normalMousePosition))
		{
			return false;
		}
		else if(new Rect(Screen.width-100,40,100,150).Contains(normalMousePosition)&&currentTile&&!isBuildingTC&&!isBuildingMass)
		{
			return false;
		}
		else if(isBuildingTC&&checkGUIObjsColliderTC.Contains(normalMousePosition))
		{
			return false;
		}
		else if(!isBuildingTC&&checkGUIObjsCollider.Contains(normalMousePosition))
		{
			return false;
		}
		else if(isShowCameraOptions&&cameraOptionsRect.Contains(normalMousePosition))
		{
			return false;
		}

		return true;
	}

	private void ShowTopView()
	{
		if(!isInTopView)
		{
			MAIN.transform.position += new Vector3(0,5,0);
		}
		
		isInTopView = true;
		cameraMove.isInTopView = true;
		MAIN.transform.localEulerAngles = new Vector3(MAIN.transform.localEulerAngles.x,0,MAIN.transform.localEulerAngles.z);
		GameObject cameraYRot = (GameObject) GameObject.Find("MAIN/YArea");
		cameraYRot.transform.localEulerAngles = new Vector3(0,0,0);
		cameraGO.transform.localEulerAngles = new Vector3(90,cameraGO.transform.localEulerAngles.y,cameraGO.transform.localEulerAngles.z);
	}

	private IEnumerator TurnCamera90(int iternation, int count)
	{
		for(int i=0;i<iternation;i++)
		{
			MAIN.transform.Rotate(new Vector3(0,count,0));
			yield return 0;
		}

		yield return 0;
	}

	private void OnGUI()
	{
		GUI.skin = ui;

		GUI.Box(new Rect(0,0,Screen.width,40),"");
		GUI.Box(new Rect(0,Screen.height-40,Screen.width,40),"");

		if(editorIsLoading)
		{
			GUI.Label(new Rect(20,10,500,34),"Loading Assets... <size=12>(Might be slower when loading first time)</size>"); 
			GUI.Label(new Rect(20,Screen.height-30,500,34),"Click HELP for Camera Contorls and other Shortcuts");
			return;
		}

		if(isItLoad&&!editorIsLoading)
		{
			if(!_uteLM.isMapLoaded)
			{
				GUI.Label(new Rect(20,10,200,34),"Loading Assets..."); return;
			}
		}

		if(saveMap.isSaving) { GUI.Label(new Rect(20,10,200,34),"Saving Assets..."); return; }

		if(isInTopView)
		{
			GUI.Label(new Rect(Screen.width/2-30,60,60,30),"Top View");
		}

		if(isBuildingTC&&allTCTiles.Count<=0)
		{
			GUI.Label(new Rect(20,10,200,34),"[No TileConnections]");
		}
		else if(allTiles.Count<=0)
		{
			GUI.Label(new Rect(20,10,200,34),"[No Tiles found]");
		}

		if(isShowCameraOptions)
		{
			GUI.Box(cameraOptionsRect,"");
			if(GUI.Button(new Rect(Screen.width-500,Screen.height-69,100,28),"Reset"))
			{
				ResetCamera();
			}
			if(GUI.Button(new Rect(Screen.width-390,Screen.height-69,100,28),"Top View"))
			{
				ShowTopView();
			}
			if(GUI.Button(new Rect(Screen.width-280,Screen.height-69,90,28),"<- 90deg"))
			{
				StartCoroutine(TurnCamera90(9,-10));
			}
			if(GUI.Button(new Rect(Screen.width-190,Screen.height-69,90,28),"90deg ->"))
			{
				StartCoroutine(TurnCamera90(9,10));
			}
		}

		if(GUI.Button(new Rect(Screen.width-90,Screen.height-40,40,40),"+"))
		{
			if(is2D||isInTopView)
			{
				StartCoroutine(cameraMove.MoveUpDown(false,false));
			}
			else
			{
				StartCoroutine(cameraMove.MoveUpDown(false,true));
			}
		}

		if(GUI.Button(new Rect(Screen.width-50,Screen.height-40,40,40),"-"))
		{
			if(is2D||isInTopView)
			{
				StartCoroutine(cameraMove.MoveUpDown(true,false));
			}
			else
			{
				StartCoroutine(cameraMove.MoveUpDown(true,true));
			}
		}

		GUI.Label(new Rect(230,Screen.height-30,300,35),"["+newProjectName+"] Object Count: "+uteGLOBAL3dMapEditor.mapObjectCount);

		string tMode = "Tiles";

		if(isBuildingTC) tMode = "Tile-Connections";

		if(GUI.Button(new Rect(220,0,260,40),"Tile Mode: "+tMode))
		{
			if(isBuildingTC)
			{
				isBuildingTC = false;
			}
			else
			{
				isBuildingTC = true;
				isContinuesBuild = true;
				isBuildingMass = false;
			}

			if(currentTile)
			{
				Destroy(currentTile);
				ShowLineHelpers(isShowLineHelpers = false);
				currentObjectID = "-1";
				helpers_CANTBUILD.transform.position = new Vector3(-1000000.0f,0.0f,-1000000.0f);
			}
		}

		if(isBuildingTC)
		{
			if(allTCTiles.Count>0)
			{
				int selectedItemIndex = comboBoxForTileConnectionControl.GetSelectedItemIndex();
				selectedItemIndex = comboBoxForTileConnectionControl.List(GUIComboBoxColliderTC, comboBoxList_TileConnections[selectedItemIndex].text+" ^", comboBoxList_TileConnections,listStyle);

				if(lastSelectedIndexTC!=selectedItemIndex)
				{
					lastSelectedIndexTC = selectedItemIndex;
					tcGoes = ((tcInfo) allTCTiles[selectedItemIndex]).tcObjs;
					tcPrevs = ((tcInfo) allTCTiles[selectedItemIndex]).tcObjsPrevs;
					tcNames = ((tcInfo) allTCTiles[selectedItemIndex]).tcObjsNames;
					tcGuids = ((tcInfo) allTCTiles[selectedItemIndex]).tcGuidNames;
					tcRots = ((tcInfo) allTCTiles[selectedItemIndex]).tcRotsNames;
					currentTcFamilyName = comboBoxList_TileConnections[selectedItemIndex].text;

					ShowLineHelpers(isShowLineHelpers = false);
				}

				if(!comboBoxForTileConnectionControl.isClickedComboButton)
				{
					GUIObjsColliderTC = new Rect(0,0,128,tcGoes.Count*125);
					checkGUIObjsColliderTC = new Rect(0,40,128,tcGoes.Count*125);
					scrollPositionTC = GUI.BeginScrollView(new Rect(0,40,145,Screen.height-80),scrollPositionTC,GUIObjsColliderTC);
					int startDrawPoint = (int) (scrollPositionTC.y/122);
					int endDrawPoint;

					if(tcGoes.Count-startDrawPoint>=7)
					{
						endDrawPoint = startDrawPoint;
					}
					else
					{
						endDrawPoint = startDrawPoint + (tcGoes.Count-startDrawPoint);
					}

					GUI.Box(GUIObjsColliderTC,"");

					for(int i=startDrawPoint;i<endDrawPoint;i++)
					{
						if(i>=0&&i<tcGoes.Count && (GameObject) tcGoes[i])
						{
							if((Texture2D)tcPrevs[i])
							{
								previewObjTextureTC = (Texture2D) tcPrevs[i];
							}

							if(GUI.Button (new Rect(8,5+(i*122),115,115),previewObjTextureTC))
							{
								if(currentTile)
								{
									Destroy(currentTile);
								}

								currentTCID = i;
								currentTile = (GameObject) Instantiate((GameObject)tcGoes[i],new Vector3(0.0f,0.0f,0.0f),((GameObject)tcGoes[i]).transform.rotation);
								uteTagObject tempTag = (uteTagObject) currentTile.AddComponent<uteTagObject>();
								tempTag.objGUID = tcGuids[i].ToString();
								tempTag.isStatic = isCurrentStatic;

								if(isBuildingTC)
								{
									tempTag.isTC = true;
								}

								currentTile.AddComponent<Rigidbody>();
								currentTile.GetComponent<Rigidbody>().useGravity = false;
								currentTile.GetComponent<BoxCollider>().size -= new Vector3(0.01f,0.01f,0.01f);
								currentTile.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
								currentTile.GetComponent<Collider>().isTrigger = true;
								currentTile.AddComponent<uteDetectBuildCollision>();
								currentTile.layer = 2;
								currentObjectID = tcNames[i].ToString();
								isRotated = 0;
								isRotatedH = 0;
								erase = false;
							}
							
							GUI.Label(new Rect(14,7+(i*122),115,30),(string)tcNames[i]);
							
							if(!previewObjTextureTC)
							{
								GUI.Label(new Rect(18,50+(i*122),115,30),"NO PREVIEW");
							}
						}
					}

					GUI.EndScrollView();
				}
			}
		}
		else if(!isBuildingTC)
		{
			if(allTiles.Count!=0)
			{
				if(allTiles.Count>0)
				{
					int selectedItemIndex = comboBoxControl.GetSelectedItemIndex();
				
					selectedItemIndex = comboBoxControl.List(GUIComboBoxCollider, comboBoxList[selectedItemIndex].text+" ^", comboBoxList, listStyle );
					
					if(lastSelectedIndex!=selectedItemIndex)
					{
						lastSelectedIndex = selectedItemIndex;
						catGoes = ((catInfo) allTiles[selectedItemIndex]).catObjs;
						catPrevs = ((catInfo) allTiles[selectedItemIndex]).catObjsPrevs;
						catNames = ((catInfo) allTiles[selectedItemIndex]).catObjsNames;
						catGuids = ((catInfo) allTiles[selectedItemIndex]).catGuidNames;

						ShowLineHelpers(isShowLineHelpers = false);
					}
					
					if(!comboBoxControl.isClickedComboButton)
					{	
						GUIObjsCollider = new Rect(0, 0, 128, catGoes.Count*125);
						checkGUIObjsCollider = new Rect(0,40,128,catGoes.Count*125);
						scrollPosition = GUI.BeginScrollView(new Rect(0, 40, 145, Screen.height-80), scrollPosition, GUIObjsCollider);
						int startDrawPoint = (int) (scrollPosition.y/122);
						int endDrawPoint;
						
						if(catGoes.Count-startDrawPoint>=7)
						{
							endDrawPoint = startDrawPoint + 6;
						}
						else
						{
							endDrawPoint = startDrawPoint + (catGoes.Count-startDrawPoint);
						}
						
						GUI.Box (GUIObjsCollider,"");

						for(int i=startDrawPoint;i<endDrawPoint;i++)
						{	
							if(i>=0&&i<catGoes.Count && (GameObject) catGoes[i])
							{	
								if((Texture2D)catPrevs[i])
								{
									previewObjTexture = (Texture2D) catPrevs[i];
								}
								
								if(GUI.Button (new Rect(8,5+(i*122),115,115),previewObjTexture))
								{
									if(currentTile)
									{
										Destroy(currentTile);
									}
									
									if(((GameObject)catGoes[i]).transform.parent.gameObject.name.Equals("static_objs"))
									{
										isCurrentStatic = true;
									}
									else
									{
										isCurrentStatic = false;
									}

									currentTile = (GameObject) Instantiate((GameObject)catGoes[i],new Vector3(0.0f,0.0f,0.0f),((GameObject)catGoes[i]).transform.rotation);
									uteTagObject tempTag = (uteTagObject) currentTile.AddComponent<uteTagObject>();
									tempTag.objGUID = catGuids[i].ToString();
									tempTag.isStatic = isCurrentStatic;
									currentTile.AddComponent<Rigidbody>();
									currentTile.GetComponent<Rigidbody>().useGravity = false;
									currentTile.GetComponent<BoxCollider>().size -= new Vector3(0.0000001f,0.0000001f,0.0000001f);
									currentTile.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
									currentTile.GetComponent<Collider>().isTrigger = true;
									currentTile.AddComponent<uteDetectBuildCollision>();
									currentTile.layer = 2;
									currentObjectID = catNames[i].ToString();
									currentTile.name = currentObjectID;
									currentObjectGUID = catGuids[i].ToString();

									helpers_CANTBUILD.transform.position = new Vector3(-1000,0,-1000);
									helpers_CANTBUILD.transform.localScale = currentTile.GetComponent<Collider>().bounds.size + new Vector3(0.1f,0.1f,0.1f);
									helpers_CANTBUILD.transform.localRotation = currentTile.transform.localRotation;

									isRotated = 0;
									isRotatedH = 0;
									erase = false;
								}
								
								GUI.Label(new Rect(14,7+(i*122),115,30),(string)catNames[i]);
								
								if(!previewObjTexture)
								{
									GUI.Label(new Rect(18,50+(i*122),115,30),"NO PREVIEW");
								}
							}
						}
						
						GUI.EndScrollView();
					}
					else
					{
						if(currentTile)
						{
							Destroy(currentTile);
						}
					}
				}
			}
		}
		
		if(is2D)
		{
			if(GUI.Button(new Rect(Screen.width-505,Screen.height-40,130,40),"Reset Camera"))
			{
				ResetCamera();
			}
		}
		else
		{
			if(GUI.Button(new Rect(Screen.width-505,Screen.height-40,130,40),"Camera box"))
			{
				if(isShowCameraOptions)
				{
					isShowCameraOptions = false;
				}
				else
				{
					isShowCameraOptions = true;
				}
			}
		}

		if(GUI.Button (new Rect(Screen.width-370,Screen.height-40,70,40),"Eraser"))
		{
			if(currentTile)
			{
				Destroy(currentTile);
				helpers_CANTBUILD.transform.position = new Vector3(-1000000.0f,0.0f,-1000000.0f);
				ShowLineHelpers(isShowLineHelpers = false);
			}
			
			erase = true;
		}
		
		if(GUI.Button(new Rect(Screen.width-295,Screen.height-40,100,40),"Grid Up"))
		{
			StartCoroutine(gridSmoothMove(grid,true,cameraMove.gameObject));
		}
		
		if(GUI.Button(new Rect(Screen.width-195,Screen.height-40,100,40),"Grid Down"))
		{
			StartCoroutine(gridSmoothMove(grid,false,cameraMove.gameObject));
		}
		
		if(currentTile&&!isBuildingTC&&!isBuildingMass)
		{
			GUI.Box(new Rect(Screen.width-100,40,100,150),"Tile Settings");

			if(GUI.Button(new Rect(Screen.width-90,60,40,30),"<-"))
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(0.0f,-10.0f,0.0f),true));
			}
			
			if(GUI.Button(new Rect(Screen.width-50,60,40,30),"->"))
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(0.0f,10.0f,0.0f),true));
			}
			
			if(GUI.Button(new Rect(Screen.width-90,90,40,30),"Up"))
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(-10.0f,0.0f,0.0f),false));
			}
			
			if(GUI.Button(new Rect(Screen.width-50,90,40,30),"Dw"))
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(10.0f,0.0f,0.0f),false));
			}
			
			if(GUI.Button(new Rect(Screen.width-90,120,80,30),"INVERT"))
			{
				StartCoroutine(smoothRotate(currentTile,new Vector3(0.0f,0.0f,20.0f),false));
			}

			if(GUI.Button (new Rect(Screen.width-90,150,80,30),"Cancel"))
			{
				if(currentTile)
				{
					Destroy(currentTile);
					ShowLineHelpers(isShowLineHelpers = false);
					helpers_CANTBUILD.transform.position = new Vector3(-1000000.0f,0.0f,-1000000.0f);
				}
				
				erase = false;
			}
		}

		string buildMode = "Standard";

		if(isContinuesBuild)
		{
			buildMode = "Continuous";
		}
		else if(isBuildingMass)
		{
			buildMode = "Mass";
		}

		if(!isBuildingTC)
		{
			if(GUI.Button(new Rect(480,0,200,40),"Build Mode: "+buildMode))
			{
				if(isContinuesBuild)
				{
					isContinuesBuild = false;
					isBuildingMass = false;
				}
				else
				{
					if(isBuildingMass)
					{
						isContinuesBuild = true;
						isBuildingMass = false;
					}
					else
					{
						isContinuesBuild = false;
						isBuildingMass = true;

						if(currentTile)
						{
							currentTile.transform.localEulerAngles = new Vector3(0,0,0);
						}
					}
				}

				helpers_CANTBUILD.transform.position = new Vector3(-1000000.0f,0.0f,-1000000.0f);
			}
		}

		if(GUI.Button(new Rect(0,Screen.height-40,130,40),"SAVE"))
		{
			StartCoroutine(saveMap.SaveMap(newProjectName,isItNewMap));
		}
		
		if(isItNewPattern)
		{
			if(GUI.Button (new Rect(140,Screen.height-40,80,40),"Export"))
			{
				if(currentTile)
					Destroy(currentTile);
				
				exporter.isShow = true;
			}
		}

		if(GUI.Button(new Rect(Screen.width-100,0,100,40),"OPTIONS"))
		{
			if(uteOptions.isShow)
			{
				uteOptions.isShow = false;
			}
			else
			{
				uteOptions.isShow = true;
			}
		}

		if(GUI.Button(new Rect(Screen.width-200,0,90,40),"HELP"))
		{
			if(uteHelp.isShow)
			{
				uteHelp.isShow = false;
			}
			else
			{
				uteHelp.isShow = true;
			}
		}
	}

	private IEnumerator smoothRotate(GameObject obj, Vector3 dir, bool isHorizontal)
	{
		if(isHorizontal)
		{
			isRotated++;
		}
		else
		{
			isRotatedH++;
		}

		int counter = 0;
		
		while(counter++!=9)
		{
			obj.transform.Rotate(dir);
			yield return null;
		}

		if(isHorizontal)
		{
			if(isRotated>=4)
			{
				isRotated = 0;
			}
		}
		else
		{
			if(isRotatedH>=4)
			{
				isRotatedH = 0;
			}
		}
	}

	private IEnumerator gridSmoothMove(GameObject gridObj, bool isUp, GameObject cam)
	{
		canBuild = false;
		isCamGridMoving = true;
		
		Vector3 endP = gridObj.transform.position;
		Vector3 camEP = cam.transform.position;

		if(isUp)
		{
			currentLayer++;
			endP += new Vector3(0.0f,globalYSize,0.0f);
			camEP += new Vector3(0.0f,globalYSize,0.0f);
		}
		else
		{
			currentLayer--;
			endP -= new Vector3(0.0f,globalYSize,0.0f);
			camEP -= new Vector3(0.0f,globalYSize,0.0f);
		}
		
		while(true)
		{
			gridObj.transform.position = Vector3.Lerp(gridObj.transform.position,endP,Time.deltaTime * 10.0f);
			cam.transform.position = Vector3.Lerp(cam.transform.position,camEP,Time.deltaTime * 10.0f);
			
			float dist = Vector3.Distance(gridObj.transform.position,endP);
			
			if(Mathf.Abs(dist)<=0.1f)
			{
				gridObj.transform.position = endP;
				cam.transform.position = camEP;
				break;
			}
			
			yield return null;
		}
		
		canBuild = true;
		isCamGridMoving = false;
	}

	private void InitHelperLines()
	{
		helpers_LINES = new LineRenderer[12];

		GameObject mainLine = GameObject.Find("uteHELPERS/LINE1");
		helpers_LINES[0] = mainLine.GetComponent<LineRenderer>();

		for(int i=0;i<11;i++)
		{
			GameObject newLine = (GameObject) Instantiate(mainLine,transform.position,transform.rotation);
			newLine.name = "LINE"+(i+2).ToString();
			newLine.transform.parent = mainLine.transform.parent;
			helpers_LINES[i+1] = newLine.GetComponent<LineRenderer>();
		}
	}

	private void CalculateLineHelpers()
	{
		int posC = 0;
		Vector3 pos = currentTile.transform.position;
		Vector3 size = currentTile.GetComponent<Collider>().bounds.size;
		Vector3 hsize = size/2.0f;
		Vector3[] newPos = new Vector3[24];
		float lineCastSize = 500.0f;

		// x
		newPos[0] = new Vector3(pos.x-lineCastSize,pos.y+hsize.y,pos.z+hsize.z);
		newPos[1] = new Vector3(pos.x+lineCastSize,pos.y+hsize.y,pos.z+hsize.z);
		newPos[2] = new Vector3(pos.x-lineCastSize,pos.y+hsize.y,pos.z-hsize.z);
		newPos[3] = new Vector3(pos.x+lineCastSize,pos.y+hsize.y,pos.z-hsize.z);
		newPos[4] = new Vector3(pos.x-lineCastSize,pos.y-hsize.y,pos.z-hsize.z);
		newPos[5] = new Vector3(pos.x+lineCastSize,pos.y-hsize.y,pos.z-hsize.z);
		newPos[6] = new Vector3(pos.x-lineCastSize,pos.y-hsize.y,pos.z+hsize.z);
		newPos[7] = new Vector3(pos.x+lineCastSize,pos.y-hsize.y,pos.z+hsize.z);

		// y
		newPos[8] = new Vector3(pos.x+hsize.x,pos.y-lineCastSize,pos.z+hsize.z);
		newPos[9] = new Vector3(pos.x+hsize.x,pos.y+lineCastSize,pos.z+hsize.z);
		newPos[10] = new Vector3(pos.x-hsize.x,pos.y-lineCastSize,pos.z+hsize.z);
		newPos[11] = new Vector3(pos.x-hsize.x,pos.y+lineCastSize,pos.z+hsize.z);
		newPos[12] = new Vector3(pos.x-hsize.x,pos.y-lineCastSize,pos.z-hsize.z);
		newPos[13] = new Vector3(pos.x-hsize.x,pos.y+lineCastSize,pos.z-hsize.z);
		newPos[14] = new Vector3(pos.x+hsize.x,pos.y-lineCastSize,pos.z-hsize.z);
		newPos[15] = new Vector3(pos.x+hsize.x,pos.y+lineCastSize,pos.z-hsize.z);

		// z
		newPos[16] = new Vector3(pos.x+hsize.x,pos.y+hsize.y,pos.z-lineCastSize);
		newPos[17] = new Vector3(pos.x+hsize.x,pos.y+hsize.y,pos.z+lineCastSize);
		newPos[18] = new Vector3(pos.x-hsize.x,pos.y+hsize.y,pos.z-lineCastSize);
		newPos[19] = new Vector3(pos.x-hsize.x,pos.y+hsize.y,pos.z+lineCastSize);
		newPos[20] = new Vector3(pos.x-hsize.x,pos.y-hsize.y,pos.z-lineCastSize);
		newPos[21] = new Vector3(pos.x-hsize.x,pos.y-hsize.y,pos.z+lineCastSize);
		newPos[22] = new Vector3(pos.x+hsize.x,pos.y-hsize.y,pos.z-lineCastSize);
		newPos[23] = new Vector3(pos.x+hsize.x,pos.y-hsize.y,pos.z+lineCastSize);

		for(int i=0;i<12;i++)
		{
			helpers_LINES[i].SetPosition(0,newPos[posC]);
			helpers_LINES[i].SetPosition(1,newPos[posC+1]);

			posC+=2;
		}
	}

	private void ShowLineHelpers(bool isTrue)
	{
		if(isTrue)
		{
			for(int i=0;i<12;i++)
			{
				helpers_LINES[i].enabled = true;
			}
		}
		else
		{
			for(int i=0;i<12;i++)
			{
				helpers_LINES[i].enabled = false;
			}
		}
	}

	private void ResetCamera()
	{
		if(isInTopView)
		{
			cameraMove.isInTopView = false;
			isInTopView = false;
			MAIN.transform.position -= new Vector3(0,5,0);
		}
		
		GameObject cameraYRot = (GameObject) GameObject.Find("MAIN/YArea");
		cameraYRot.transform.localEulerAngles = Vector3.zero;

		if(is2D)
		{
			cameraGO.transform.localEulerAngles = new Vector3(90,0,0);
			MAIN.transform.localEulerAngles = Vector3.zero;
		}
		else
		{
			MAIN.transform.localEulerAngles = new Vector3(0,45,0);
			cameraGO.transform.localEulerAngles = new Vector3(30,0,0);
		}
	}

	private void RoundTo90(GameObject go)
	{
		Vector3 vec = go.transform.eulerAngles;
		vec.x = Mathf.Round(vec.x / 90) * 90;
		vec.y = Mathf.Round(vec.y / 90) * 90;
		vec.z = Mathf.Round(vec.z / 90) * 90;
		go.transform.eulerAngles = vec;
	}

	private float RoundTo(float point, float toRound = 2.0f)
	{
		point *= toRound;
		point = Mathf.Round(point);
		point /= toRound;

		return point;
	}
	#endif
}
