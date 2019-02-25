using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uteMassBuildEngine : MonoBehaviour {

#if UNITY_EDITOR
	public class Tile
	{
		public Vector3 pos;
		public Vector3 scale;
		public Collider coll;

		public Tile(Vector3 _pos, Vector3 _scale, Collider _coll)
		{
			pos = _pos;
			scale = _scale;
			coll = _coll;
		}
	}

	private List<Tile> newTiles = new List<Tile>();
	private List<GameObject> dummyTrash = new List<GameObject>();
	private GameObject mDummy;
	private GameObject mTile;
	private string mName;
	private string mGuid;
	private uteMapEditorEngine uteMEE;
	private Vector2 lastDistance;
	private string xzCountLabel;
	private bool isBuilding;

	private void Start()
	{
		isBuilding = false;
		lastDistance = new Vector3(-10000,-10000,-10000);
		mDummy = (GameObject) Resources.Load("uteForEditor/uteTcDummy");
		uteMEE = this.gameObject.GetComponent<uteMapEditorEngine>();
	}

	public void massBuildStart(GameObject _mTile, string _mName, string _mGuid)
	{
		isBuilding = true;
		isItFirst = true;
		mTile = _mTile;
		mName = _mName;
		mGuid = _mGuid;

		if(newTiles.Count>0)
		{
			newTiles.Clear();
		}
	}

	private bool isItFirst;
	private Vector3 startPosition;

	public IEnumerator AddTile(GameObject obj)
	{
		bool isGoodToGo = true;

		if(isItFirst)
		{
			isGoodToGo = false;
			isItFirst = false;
			startPosition = obj.transform.position;
			newTiles.Add(new Tile(startPosition,obj.transform.localScale,obj.GetComponent<Collider>()));

			GameObject newDummy = (GameObject) Instantiate(mDummy,startPosition+new Vector3(0,0.08f,0),obj.transform.rotation);
			newDummy.transform.localScale = new Vector3(obj.GetComponent<Collider>().bounds.size.x,0.1f,obj.GetComponent<Collider>().bounds.size.z);
			newDummy.AddComponent<BoxCollider>();
			dummyTrash.Add(newDummy);

			int Xdistance = (int) Mathf.Ceil(obj.transform.position.x-startPosition.x);
			int Zdistance = (int) Mathf.Ceil(obj.transform.position.z-startPosition.z);
			lastDistance = new Vector2(Xdistance,Zdistance);
		}
		else
		{
			if(newTiles.Count>0)
			{
				Vector3 lastObjectPos = ((Tile) newTiles[newTiles.Count-1]).pos;
				float distance = Vector2.Distance(new Vector2(obj.transform.position.x,obj.transform.position.z),new Vector2(lastObjectPos.x,lastObjectPos.z));

				if(distance<obj.GetComponent<Collider>().bounds.size.x||Mathf.Abs(lastObjectPos.y-obj.transform.position.y)>0.05f)
				{
					isGoodToGo = false;
				}
			}
		}

		if(isGoodToGo)
		{
			int Xdistance = (int) Mathf.Ceil(obj.transform.position.x-startPosition.x);
			int Zdistance = (int) Mathf.Ceil(obj.transform.position.z-startPosition.z);

			Vector2 newDistance = new Vector2(Xdistance,Zdistance);

			if(newDistance!=lastDistance)
			{
				if(dummyTrash.Count>0)
				{
					for(int i=0;i<dummyTrash.Count;i++)
					{
						GameObject go = (GameObject) dummyTrash[i];
						Destroy(go);
					}

					dummyTrash.Clear();
				}

				if(newTiles.Count>0)
				{
					newTiles.Clear();
				}

				lastDistance = newDistance;
				int Xfill = (int) (Mathf.Abs(Xdistance)/obj.GetComponent<Collider>().bounds.size.x);
				int Zfill = (int) (Mathf.Abs(Zdistance)/obj.GetComponent<Collider>().bounds.size.z);
				string pM = "";

				if(Xdistance>=0&&Zdistance>=0)
				{
					pM = "++";
				}
				else if(Xdistance<=0&&Zdistance<=0)
				{
					pM = "--";
				}
				else if(Xdistance<=0&&Zdistance>=0)
				{
					pM = "-+";
				}
				else if(Xdistance>=0&&Zdistance<=0)
				{
					pM = "+-";
				}

				xzCountLabel = "X:"+(Xfill+1)+",Z:"+(Zfill+1);

				for(int i=0;i<Mathf.Abs(Xfill)+1;i++)
				{
					for(int j=0;j<Mathf.Abs(Zfill)+1;j++)
					{
						Vector3 position = Vector3.zero;

						if(pM.Equals("--"))
						{
							position = new Vector3(startPosition.x-(obj.GetComponent<Collider>().bounds.size.x*i),obj.transform.position.y,startPosition.z-(obj.GetComponent<Collider>().bounds.size.z*j));
						}
						else if(pM.Equals("++"))
						{
							position = new Vector3(startPosition.x+(obj.GetComponent<Collider>().bounds.size.x*i),obj.transform.position.y,startPosition.z+(obj.GetComponent<Collider>().bounds.size.z*j));
						}
						else if(pM.Equals("-+"))
						{
							position = new Vector3(startPosition.x-(obj.GetComponent<Collider>().bounds.size.x*i),obj.transform.position.y,startPosition.z+(obj.GetComponent<Collider>().bounds.size.z*j));
						}
						else if(pM.Equals("+-"))
						{
							position = new Vector3(startPosition.x+(obj.GetComponent<Collider>().bounds.size.x*i),obj.transform.position.y,startPosition.z-(obj.GetComponent<Collider>().bounds.size.z*j));
						}

						newTiles.Add(new Tile(position,obj.transform.localScale,obj.GetComponent<Collider>()));

						GameObject newDummy = (GameObject) Instantiate(mDummy,position+new Vector3(0,0.08f,0),obj.transform.rotation);
						newDummy.transform.localScale = new Vector3(obj.transform.localScale.x,0.1f,obj.transform.localScale.z);
						newDummy.AddComponent<BoxCollider>();
						newDummy.layer = 2;
						dummyTrash.Add(newDummy);
					}
				}
			}
		}

		yield return new WaitForSeconds(0.4f);
	}

	private void OnGUI()
	{
		if(isBuilding)
		{
			GUI.Label(new Rect(Input.mousePosition.x+30,Screen.height-Input.mousePosition.y-30,100,30),xzCountLabel);
		}
	}

	public void FinishUp()
	{
		isItFirst = true;

		if(newTiles.Count>0)
		{
			for(int i=1;i<newTiles.Count;i++)
			{
				Tile tile = (Tile) newTiles[i];
				uteMEE.ApplyBuild(mTile,tile.pos,mName,mGuid,mTile.transform.localEulerAngles,"",true);
			}

			if(newTiles.Count>0)
			{
				newTiles.Clear();
			}

			if(dummyTrash.Count>0)
			{
				for(int i=0;i<dummyTrash.Count;i++)
				{
					GameObject go = (GameObject) dummyTrash[i];
					Destroy(go);
				}

				dummyTrash.Clear();
			}
		}

		isBuilding = false;
	}
#endif
}
