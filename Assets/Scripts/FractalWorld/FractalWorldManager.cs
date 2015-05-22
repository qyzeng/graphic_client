using UnityEngine;
using System.Collections;
using FractalLibrary;

public class FractalWorldManager : MonoBehaviour
{
	MandelbrotFractal mMandelbrotfractal = new MandelbrotFractal ();

	public CharacterStateMachine Player;

	public Color TestColor;

	private Vector2 mFractalMin = new Vector2 (-1f, -1f);
	private Vector2 mFractalMax = new Vector2 (1f, 1f);

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
		mMandelbrotfractal.SetBounds (mFractalMin.x, mFractalMin.y, mFractalMax.x, mFractalMax.y);
		mMandelbrotfractal.SetDataSize (1024, 1024);
		mMandelbrotfractal.SetInitialIterationPoint (0.285f, 0.01f);
		mMandelbrotfractal.SetIterator (new QuadraticMandelbrotIterator ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.UpArrow)) {
			mFractalMin += Vector2.up * Time.deltaTime;
			mFractalMax += Vector2.up * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			mFractalMin -= Vector2.up * Time.deltaTime;
			mFractalMax -= Vector2.up * Time.deltaTime;
		}
	
	}
}
