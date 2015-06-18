using UnityEngine;
using System.Collections;

public class ScreenCaptureCamera : MonoBehaviour
{
    public RenderTexture Render;

    [SerializeField]
    private GameObject _target = null;

    [SerializeField]
    private float _size = 0.4f;

    [SerializeField]
    private float _offset = 1.0f;

    [SerializeField]
    private string _lookAtNode = "Head";

    public GameObject Target
    {
        get
        {
            return _target;
        }
        set
        {
            if (_target != value)
            {
                _target = value;
                OnTargetChanged();
            }
        }
    }

    private Transform _targetNode = null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        OnTargetChanged();
    }
#endif

    private void Start()
    {
        //camera.fieldOfView = 90f;
    }

    private void OnTargetChanged()
    {
        if (_target != null)
        {
            _targetNode = GetTargetNode();
        }
    }

    //private Transform GetEstimatedHeadNode(GameObject target)
    //{
    //    Transform headNode = target.transform.Find("EstimatedHeadNode");
    //    if (headNode == null)
    //    {
    //        Transform center = target.transform.GetCenterNode();
    //        Bounds calcBounds = new Bounds(center.position, Vector3.zero);
    //        Renderer[] renders = target.GetComponentsInChildren<Renderer>();
    //        foreach (Renderer render in renders)
    //        {
    //            calcBounds.Encapsulate(render.bounds);
    //        }
    //        headNode = new GameObject("EstimatedHeadNode").transform;
    //        headNode.transform.position = center.position + 0.5f * calcBounds.extents.y * Vector3.up;
    //        headNode.transform.parent = target.transform;
    //    }
    //    return headNode;
    //}

    private Transform GetTargetNode()
    {
        Transform targetNode = null;

        // lets look for the spine node
        Transform[] transforms = _target.transform.root.GetComponentsInChildren<Transform>();

        // if we dont find the Bip01 Spine1 then we just pass the target transform, will be ideal for items
        foreach (var transform in transforms)
        {
            if (transform.gameObject.name.Contains(_lookAtNode))
            {
                targetNode = transform;
                break;
            }
        }

        if (targetNode == null)
            targetNode = _target.transform;

        return targetNode;
    }

    private void Update()
    {
        if (_targetNode != null)
        {
            SetPosition();
            SetRotation();
        }
    }

    //private float GetDesiredRenderDistanceFromCenter(GameObject target)
    //{
    //    if (target != null)
    //    {
    //        Bounds boundCheck = new Bounds(target.transform.GetCenterNode().position, Vector3.zero);
    //        Renderer[] renders = target.GetComponentsInChildren<Renderer>();
    //        foreach (Renderer render in renders)
    //        {
    //            boundCheck.Encapsulate(render.bounds);
    //        }
    //        return boundCheck.extents.z + boundCheck.extents.y / Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2);
    //    }
    //    return 0f;
    //}

    private void SetPosition()
    {
        transform.position = _targetNode.position + _offset * _target.transform.forward;
    }

    private void SetRotation()
    {
        transform.LookAt(_targetNode);
    }
}
