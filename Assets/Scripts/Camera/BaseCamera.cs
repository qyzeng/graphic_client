using UnityEngine;
using System.Collections;

public class BaseCamera : MonoBehaviour
{
    public Transform TargetNode;
    
    // Use this for initialization
    protected virtual void Start()
    {
	
    }
	
    // Update is called once per frame
    protected virtual void Update()
    {
	
    }
    
    public virtual void RotateCamera(Vector2 rotateVector)
    {
    
    }
    
    public virtual void ResetCamera()
    {
    }
    
    public virtual void Zoom(float delta)
    {
    }

    public virtual void SetOverrideRotation(Quaternion rotation)
    {

    }
}
