using UnityEngine;
using System.Collections;

public class uteMouseOrbit : MonoBehaviour {
#if UNITY_EDITOR
	[HideInInspector]
    public Transform target;
    [HideInInspector]
    public bool isEnabled;
    private float ySpeed = 120.0f;
    private float yMinLimit = -30f;
    private float yMaxLimit = 60;
    private float y = 0.0f;
 
	private void Start()
	{
		isEnabled = false;
        Vector3 angles = transform.eulerAngles;
        y = angles.x;
	}
 
    private void LateUpdate()
    {
   		if(target&&isEnabled)
    	{
	        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
	        y = ClampAngle(y, yMinLimit, yMaxLimit);
	        Quaternion rotation = Quaternion.Euler(y, 0, 0);
	        transform.localRotation = rotation;
    	}
 
	}
 
    private float ClampAngle(float angle, float min, float max)
    {
        if(angle<-360F)
        {
            angle += 360F;
        }
        if(angle>360F)
        {
            angle -= 360F;
        }

        return Mathf.Clamp(angle,min,max);
    }
 #endif
}
