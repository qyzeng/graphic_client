using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ModelAudioData : MonoBehaviour 
{
    public AudioClip IdleClip;
    public AudioClip ActionClip;
    public AudioClip DieClip;
    public AudioClip MovingClip;
    public AudioClip GetHitClip;

    public static ModelAudioData GetModelAudioData(GameObject gObject)
    {
        ModelAudioData retVal = gObject.GetComponent<ModelAudioData>();
        if(retVal == null)
        {
            retVal = gObject.AddComponent<ModelAudioData>();
        }
        return retVal;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
