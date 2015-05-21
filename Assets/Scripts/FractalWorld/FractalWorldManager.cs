using UnityEngine;
using System.Collections;
using FractalLibrary;

public class FractalWorldManager : MonoBehaviour
{
	MandelbrotFractal mMandelbrotfractal = new MandelbrotFractal ();

	public CharacterStateMachine Player;

	// Use this for initialization
	void Start ()
	{
		if (Player != null) {
			Player.AddController (WP.Controller.StandalonePlayerController.Singleton);
		}
	}

	private void InitFractal ()
	{
		mMandelbrotfractal.Iterations = 100;
		mMandelbrotfractal.SetCenter (0, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
