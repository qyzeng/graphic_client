using UnityEngine;
using System.Collections;

public class ChaseCamera : BaseCamera
{
    public float SmoothFactor;
    private Quaternion targetRotation = Quaternion.identity;
    
    void OnValidate()
    {
        SmoothFactor = Mathf.Clamp01(SmoothFactor);
    }
    
    // Update is called once per frame
    protected override void Update()
    {
        if (TargetNode != null)
        {
            targetRotation = TargetNode.rotation;
            transform.position = Vector3.Lerp(transform.position, TargetNode.position, Mathf.Pow(Time.smoothDeltaTime, SmoothFactor));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Mathf.Pow(Time.smoothDeltaTime, SmoothFactor));

        }
    }

    public override void SetOverrideRotation(Quaternion newRotation)
    {
        if (TargetNode != null)
            TargetNode.rotation = newRotation;
    }
}
