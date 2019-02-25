using UnityEngine;
using System.Collections;

public class uteCameraMove : MonoBehaviour
{
#if UNITY_EDITOR
	private bool gF;
	private bool gB;
	private bool gR;
	private bool gL;
	private bool rL;
	private bool rR;
	private float mS;
	private float rS;
	private Camera cam;
	[HideInInspector]
	public bool is2D;
	[HideInInspector]
	public Vector3 sel;
	[HideInInspector]
	public bool isInTopView;
	
	void Start()
	{
		gF = false;
		gB = false;
		gR = false;
		gL = false;
		rL = false;
		rR = false;
		mS = 0.3f;
		rS = 70.0f;
		sel = new Vector3(500.0f,0.0f,500.0f);
		isInTopView = false;

		cam = (Camera) ((GameObject) GameObject.Find("MapEditorCamera")).GetComponent<Camera>();
	}
	
	void LateUpdate()
	{
		if(Input.GetKeyDown (KeyCode.A))
		{
			gL = true;
		}
		else if(Input.GetKeyDown (KeyCode.D))
		{
			gR = true;
		}
		
		if(Input.GetKeyDown (KeyCode.W))
		{
			gF = true;
		}
		else if(Input.GetKeyDown (KeyCode.S))
		{
			gB = true;
		}
		
		if(Input.GetKeyDown (KeyCode.E))
		{
			rL = true;
		}
		else if(Input.GetKeyDown (KeyCode.Q))
		{
			rR = true;
		}
		
		if(Input.GetKeyUp (KeyCode.A))
		{
			gL  = false;
		}
		
		if(Input.GetKeyUp (KeyCode.D))
		{
			gR = false;
		}
		
		if(Input.GetKeyUp (KeyCode.W))
		{
			gF = false;
		}
		
		if(Input.GetKeyUp (KeyCode.S))
		{
			gB = false;
		}
		
		if(Input.GetKeyUp (KeyCode.E))
		{
			rL = false;
		}
		
		if(Input.GetKeyUp (KeyCode.Q))
		{
			rR = false;
		}
		
		float scrollY = Input.GetAxis("Mouse ScrollWheel");

		if(Input.mousePosition.x<150||Input.mousePosition.x<0||Input.mousePosition.y<0||Input.mousePosition.x>Screen.width||Input.mousePosition.y>Screen.height)
		{
			scrollY = 0.0f;
		}

		if(scrollY>=0.1f)
		{
			if(is2D||isInTopView)
			{
				StartCoroutine(MoveUpDown(true,false));
				StartCoroutine(MoveUpDown(true,false));
			}
			else
			{
				StartCoroutine(MoveUpDown(true,true));
				StartCoroutine(MoveUpDown(true,true));
			}
		}
		else if(scrollY<=-0.1f)
		{
			if(is2D||isInTopView)
			{
				StartCoroutine(MoveUpDown(false,false));
				StartCoroutine(MoveUpDown(false,false));
			}
			else
			{
				StartCoroutine(MoveUpDown(false,true));
				StartCoroutine(MoveUpDown(false,true));
			}
		}

		if(Input.GetKey (KeyCode.Minus) || Input.GetKey (KeyCode.Underscore))
		{
			if(is2D||isInTopView)
			{
				StartCoroutine(MoveUpDown(true,false));
			}
			else
			{
				StartCoroutine(MoveUpDown(true,true));
			}
		}
		
		if(Input.GetKey (KeyCode.Equals) || Input.GetKey (KeyCode.Plus))
		{
			if(is2D||isInTopView)
			{
				StartCoroutine(MoveUpDown(false,false));
			}
			else
			{
				StartCoroutine(MoveUpDown(false,true));
			}
		}
	}
	
	void FixedUpdate()
	{
		if(gL)
		{
			this.transform.Translate(Vector3.left * mS);
		}
		else if(gR)
		{
			this.transform.Translate(Vector3.right * mS);
		}
		
		if(gF)
		{
			this.transform.Translate(Vector3.forward * mS);
		}
		else if(gB)
		{
			this.transform.Translate(Vector3.back * mS);
		}
		
		if(rL)
		{
			this.transform.RotateAround(cam.gameObject.transform.position,Vector3.up, rS * Time.deltaTime);
		}
		else if(rR)
		{
			this.transform.RotateAround(cam.gameObject.transform.position,Vector3.up, -rS * Time.deltaTime);
		}
	}
	
	public IEnumerator MoveUpDown(bool isUp, bool isNotGrid)
	{
		int counter = 0;
		int stopC = 3;
		
		if(!isNotGrid)
			stopC = 10;
		
		while(counter++!=stopC)
		{
			if(isUp)
			{
				gB = isNotGrid;

				if(!cam.orthographic)
					this.transform.position += new Vector3(0.0f,0.1f,0.0f);
				else
					cam.orthographicSize += 0.1f;
			}
			else
			{
				gF = isNotGrid;

				if(!cam.orthographic)
					this.transform.position -= new Vector3(0.0f,0.1f,0.0f);
				else
					cam.orthographicSize -= 0.1f;
			}
			
			yield return 0;
		}
		
		gB = false;
		gF = false;
	}
#endif
}
