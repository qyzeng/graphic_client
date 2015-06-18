﻿using UnityEngine;
using System.Collections;
using FractalLibrary;
using WP.Controller;

public class FractalWorldManager : WorldManager
{
	public enum FRACTAL_EXPLORE_MODE
	{
		FLY = 0,
		WALK = 1,
	}

	MandelbrotFractal mMandelbrotfractal = new MandelbrotFractal ();

	public Color TestColor;


	private const float MAX_FRACTAL_SCALE = 5f;
	private const float MIN_FRACTAL_SCALE = 0f;

	private float mFractalScale = 1f;
	public float FractalScaleSpeed = 1f;
	private Vector2 mFractalMin = new Vector2 (-1f, -1f);
	private Vector2 mFractalMax = new Vector2 (1f, 1f);
	private bool mFractalBoundsChanged = false;
	private bool mTerrainHeightsDirty = false;
	private bool mTerrainTextureDirty = false;

	public float FractalBoundMoveSpeed = 5f;

	public Terrain TargetTerrain;
	private GameObject mBoundaryObject = null;
	public Bounds GameBounds;

	[SerializeField]
	private FRACTAL_EXPLORE_MODE
		mExploreMode = FRACTAL_EXPLORE_MODE.FLY;

	public FRACTAL_EXPLORE_MODE ExploreMode {
		get {
			return mExploreMode;
		}
		set {
			if (mExploreMode != value) {
				mExploreMode = value;
				VerifyExploreMode ();
			}
		}
	}

	private void VerifyExploreMode ()
	{
		switch (mExploreMode) {
		case FRACTAL_EXPLORE_MODE.FLY:
			if (_playerChar) {
				_playerChar.SetState (CharacterState.FLY);
				_playerChar.RemoveController (StandalonePlayerController.Singleton);
			}
			if (_currentCamControl) {
				_currentCamControl.CamType = CameraControl.CameraType.OVERVIEW_CAM;
				_currentCamControl.LookAtTarget = WorldCenter.gameObject;
				_currentCamControl.transform.position = Vector3.up * 110f;
				_currentCamControl.OverrideRotation (Quaternion.Euler (90f * Vector3.right));
			}
			StandalonePlayerController.ControllerMode = CONTROLLER_MODE.NORMAL;
			break;
		case FRACTAL_EXPLORE_MODE.WALK:
			if (_playerChar) {
				_playerChar.SetState (CharacterState.IDLE);
				_playerChar.transform.position = 110f * Vector3.up;
				_playerChar.AddController (StandalonePlayerController.Singleton);
			}
			if (_currentCamControl) {
				_currentCamControl.CamType = CameraControl.CameraType.ORBITAL_CAM;
				_currentCamControl.LookAtTarget = _playerChar.gameObject;
			}
			StandalonePlayerController.ControllerMode = CONTROLLER_MODE.ACTION;
			break;
		}
	}

	void Awake ()
	{
		if (mBoundaryObject == null) {
			mBoundaryObject = new GameObject ("Game Boundary");
		}
		BoxCollider boundCollider = mBoundaryObject.GetComponent<BoxCollider> ();

		//VerifyExploreMode ();
	}
		
	private void OnControlModeChanged (WP.Controller.CONTROLLER_MODE mode)
	{
		ExploreMode = mode == WP.Controller.CONTROLLER_MODE.NORMAL ? FRACTAL_EXPLORE_MODE.FLY : FRACTAL_EXPLORE_MODE.WALK;
	}

	private void SetFractalScaleFactor (float scaleVal)
	{
		Vector2 midPoint = (mFractalMax + mFractalMin) * 0.5f;
		Vector2 extents = Vector2.one * 0.5f * scaleVal;
		mFractalMax = midPoint + extents;
		mFractalMin = midPoint - extents;
		mFractalBoundsChanged = true;
	}

	private void ControlCommandReceivedHandler (System.Collections.Generic.List<CommandFiredEventArgs> commandList)
	{
		if (ExploreMode == FRACTAL_EXPLORE_MODE.FLY) {
			foreach (CommandFiredEventArgs command in commandList) {
				if (command.Command == (int)COMMAND_TYPE.PLAYER_VERTICAL) {
					MoveFractalBounds (Vector2.up * (float)(command.Arguments [0]));
				}
				if (command.Command == (int)COMMAND_TYPE.PLAYER_HORIZONTAL) {
					MoveFractalBounds (Vector2.right * (float)(command.Arguments [0]));
				}

				if (command.Command == (int)COMMAND_TYPE.CAMERA_ZOOM) {
					float deltaZoom = (float)command.Arguments [0];
					mFractalScale -= deltaZoom * Time.deltaTime;
					mFractalScale = Mathf.Clamp (mFractalScale, MIN_FRACTAL_SCALE, MAX_FRACTAL_SCALE);
					SetFractalScaleFactor (mFractalScale);
				}
			}
		}
//		if (Input.GetKey (KeyCode.UpArrow)) {
//			MoveFractalBounds (Vector2.up);
//		}
//		if (Input.GetKey (KeyCode.DownArrow)) {
//			MoveFractalBounds (-Vector2.up);
//		}
//		if (Input.GetKey (KeyCode.RightArrow)) {
//			MoveFractalBounds (Vector2.right);
//		}
//		if (Input.GetKey (KeyCode.LeftArrow)) {
//			MoveFractalBounds (-Vector2.right);
//		}
		//		float mouseScroll = Input.GetAxisRaw ("Mouse ScrollWheel");
		//		if (mouseScroll != 0f) {
		//			mFractalScale += mouseScroll * FractalScaleSpeed * Time.deltaTime;
		//			Vector2 midPoint = (mFractalMax + mFractalMin) * 0.5f;
		//			Vector2 extents = (mFractalMax - mFractalMin) * 0.5f * mFractalScale;
		//			mFractalMin = midPoint - extents;
		//			mFractalMax = midPoint + extents;
		//			mFractalBoundsChanged = true;
		//		}
		
//		if (Input.GetKey (KeyCode.O)) {
//			mFractalScale = Mathf.Lerp (mFractalScale, MAX_FRACTAL_SCALE, Time.deltaTime);
//			Vector2 midPoint = (mFractalMax + mFractalMin) * 0.5f;
//			Vector2 extents = Vector2.one * 0.5f * mFractalScale;
//			mFractalMin = midPoint - extents;
//			mFractalMax = midPoint + extents;
//			mFractalBoundsChanged = true;
//		}
//		if (Input.GetKey (KeyCode.L)) {
//			mFractalScale = Mathf.Lerp (mFractalScale, MIN_FRACTAL_SCALE, Time.deltaTime);
//			Vector2 midPoint = (mFractalMax + mFractalMin) * 0.5f;
//			Vector2 extents = Vector2.one * 0.5f * mFractalScale;
//			mFractalMin = midPoint - extents;
//			mFractalMax = midPoint + extents;
//			mFractalBoundsChanged = true;
//		}
	}

	// Use this for initialization
	void Start ()
	{
		//Cursor.lockState = CursorLockMode.Locked;
		InitFractal ();
		InitTerrain ();
		StandalonePlayerController.ControllerMode = ExploreMode == FRACTAL_EXPLORE_MODE.WALK ? CONTROLLER_MODE.ACTION : CONTROLLER_MODE.NORMAL;
		StandalonePlayerController.OnControllerModeChanged += this.OnControlModeChanged;
		OnControlModeChanged (StandalonePlayerController.ControllerMode);
		StandalonePlayerController.Singleton.OnControllerCommandsFired += ControlCommandReceivedHandler;
	}

	public override void LateInit ()
	{
		base.LateInit ();
		VerifyExploreMode ();
	}

	private void InitTerrain ()
	{
		if (TargetTerrain) {
			TargetTerrain.drawHeightmap = true;
			TargetTerrain.terrainData.heightmapResolution = 256;
			TargetTerrain.terrainData.size = new Vector3 (128f, 100f, 128f);
			TargetTerrain.transform.position = new Vector3 (-64f, 0, -64f);
		}
	}

	private void InitFractal ()
	{
		mMandelbrotfractal.Iterations = 100;
		mMandelbrotfractal.SetCenter (0.7f, 0);
		mMandelbrotfractal.SetBounds (mFractalMin.x, mFractalMin.y, mFractalMax.x, mFractalMax.y);
		mMandelbrotfractal.SetDataSize (256, 256);
		mMandelbrotfractal.SetInitialIterationPoint (0.3f, 0.6f);
		mMandelbrotfractal.SetIteratingFunction (FractalLibrary.FractalUtility.QuadMandelbrotIterate);
	}

	public void QuadJulietIterate (int iterations, out int returnVal, params FractalComplexNumber[] complexNos)
	{
		returnVal = -1;
		FractalComplexNumber z = new FractalComplexNumber ();
		FractalComplexNumber c = complexNos [0];
		for (int i = 0; i < iterations + 1; ++i) {
			z = z * z + c;
			if (z.Absolute >= 2f) {
				returnVal = i;
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void FixedUpdate ()
	{
		CheckUpdateFractal ();
		CheckTerrainHeights ();
		if (mTerrainTextureDirty) {
			mTerrainTextureDirty = false;
			StartCoroutine (CheckTerrainTexture ());
		}
	}

	private void CheckUpdateFractal ()
	{
		if (mFractalBoundsChanged) {
			mMandelbrotfractal.Iterations = (int)(100f / mFractalScale);
			mMandelbrotfractal.SetBounds (mFractalMin.x, mFractalMin.y, mFractalMax.x, mFractalMax.y);
			mMandelbrotfractal.RefreshDataSamples ();
			mTerrainHeightsDirty = true;
			mFractalBoundsChanged = false;
		}
	}

	private void CheckTerrainHeights ()
	{
		if (TargetTerrain && mTerrainHeightsDirty) {
			float maxheight = (float)mMandelbrotfractal.Iterations;
			TargetTerrain.terrainData.size = new Vector3 (128f, maxheight, 128f);
			TargetTerrain.terrainData.SetHeights (0, 0, mMandelbrotfractal.Data.SwapXY ());
			mTerrainHeightsDirty = false;
			mTerrainTextureDirty = true;

		}
	}

	private IEnumerator CheckTerrainTexture ()
	{
		Texture2D terrainsplat = TargetTerrain.terrainData.splatPrototypes [0].texture;
		if (terrainsplat) {
			terrainsplat.Resize (256, 256, TextureFormat.ARGB32, true);
		} else {
			terrainsplat = new Texture2D (256, 256, TextureFormat.ARGB32, true);
			TargetTerrain.terrainData.splatPrototypes [0].texture = terrainsplat;
		}
		TargetTerrain.terrainData.splatPrototypes [0].tileSize = new Vector2 (128f, 128f);
		for (int y = 0; y<=mMandelbrotfractal.Data.GetUpperBound(1); ++y)
			for (int x=0; x<=mMandelbrotfractal.Data.GetUpperBound(0); ++x) {
				Color newColor = TestColor * mMandelbrotfractal.Data [x, y] * mMandelbrotfractal.Iterations / 100;
				terrainsplat.SetPixel (x, y, newColor);
				//Debug.Log (string.Format ("Color at {0},{1} : {2}", x, y, terrainsplat.GetPixel (x, y).ToString ()));
				//yield return null;
			}
		terrainsplat.Apply ();
		yield return new WaitForEndOfFrame ();
	}



	private void MoveFractalBounds (Vector2 direction)
	{
		mFractalMax += direction * FractalBoundMoveSpeed * Time.deltaTime * mFractalScale;
		mFractalMin += direction * FractalBoundMoveSpeed * Time.deltaTime * mFractalScale;
		mFractalBoundsChanged = true;
	}
}