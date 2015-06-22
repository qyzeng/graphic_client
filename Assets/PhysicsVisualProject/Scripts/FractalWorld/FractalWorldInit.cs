using UnityEngine;
using System.Collections;

public class FractalWorldInit : MonoBehaviour
{
	public FractalWorldManager World;
	public int PortNumber = 25000;

	// Use this for initialization
	void Start ()
	{
		Network.InitializeServer (99, PortNumber, true);
		Invoke ("Init", 1f);
	}

	void Init ()
	{
		if (World) {
			World.Init ();
			World.LateInit ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
