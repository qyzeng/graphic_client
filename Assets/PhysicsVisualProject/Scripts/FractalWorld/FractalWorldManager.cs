using UnityEngine;
using System.Collections;
using FractalLibrary;
using WP.Controller;
using WP.Character;

public class FractalWorldManager : WorldManager
{
	public enum FRACTAL_EXPLORE_MODE
	{
		FLY = 0,
		WALK = 1,
	}

	MandelbrotFractal mMandBig = new MandelbrotFractal ();
	MandelbrotFractal mMandSmall = new MandelbrotFractal ();

	public Color TestColor;
	

	private const float MAX_FRACTAL_SCALE = 5f;
	private const float MIN_FRACTAL_SCALE = 0f;

	private float mFractalScale = 2f;
	public float FractalScaleSpeed = 1f;
	private Vector2 mFractalMin = new Vector2 (-1f, -1f);
	private Vector2 mFractalMax = new Vector2 (1f, 1f);
	private bool mFractalBoundsChanged = false;
	private bool mTerrainHeightsDirty = false;
	private bool mTerrainTextureDirty = false;
	public int InitialIterations = 50;
	private float[,] mFractalData = null;
	private int mCoreCount = 1;


	public float FractalBoundMoveSpeed = 5f;

	public Terrain TargetTerrain;
	private GameObject mBoundaryObject = null;
	public Bounds GameBounds;

	private Texture2D mTempTexPointer = null;

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
		IController controllerToUse = null;
		if (UseOculus) {
			controllerToUse = OculusController.Singleton;
		} else {
			controllerToUse = StandalonePlayerController.Singleton;
		}
		switch (mExploreMode) {
		case FRACTAL_EXPLORE_MODE.FLY:
			if (_playerChar) {
				_playerChar.SetState (CharacterState.FLY);
				_playerChar.RemoveController (controllerToUse);
			}
			if (_currentCamControl) {
				_currentCamControl.CamType = CameraControl.CameraType.OVERVIEW_CAM;
				_currentCamControl.LookAtTarget = WorldCenter.gameObject;
				_currentCamControl.transform.position = PlayerSpawnPoint;
				_currentCamControl.OverrideRotation (Quaternion.Euler (90f * Vector3.right));
			}
			ControllerUtility.ControllerMode = CONTROLLER_MODE.NORMAL;
			break;
		case FRACTAL_EXPLORE_MODE.WALK:
			if (_playerChar) {
				_playerChar.SetState (CharacterState.IDLE);
				_playerChar.transform.position = PlayerSpawnPoint;
				_playerChar.AddController (controllerToUse);
			}
			if (_currentCamControl) {
				_currentCamControl.CamType = (UseOculus) ? CameraControl.CameraType.FPS_CAM : CameraControl.CameraType.ORBITAL_CAM;
				_currentCamControl.LookAtTarget = _playerChar.gameObject;
			}
			ControllerUtility.ControllerMode = CONTROLLER_MODE.ACTION;
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
					if (deltaZoom != 0f) {
						mFractalScale -= deltaZoom * Time.deltaTime * mFractalScale;
						mFractalScale = Mathf.Clamp (mFractalScale, MIN_FRACTAL_SCALE, MAX_FRACTAL_SCALE);
						SetFractalScaleFactor (mFractalScale);
					}
				}
			}
		}
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();
		mCoreCount = SystemInfo.processorCount;
		//Cursor.lockState = CursorLockMode.Locked;
		InitFractal ();
		InitTerrain ();
		ControllerUtility.ControllerMode = ExploreMode == FRACTAL_EXPLORE_MODE.WALK ? CONTROLLER_MODE.ACTION : CONTROLLER_MODE.NORMAL;
		ControllerUtility.OnControllerModeChanged += this.OnControlModeChanged;
		OnControlModeChanged (ControllerUtility.ControllerMode);
		if (UseOculus) {
			OculusController.Singleton.OnControllerCommandsFired += ControlCommandReceivedHandler;
		} else {
			StandalonePlayerController.Singleton.OnControllerCommandsFired += ControlCommandReceivedHandler;
		}
		StartCoroutine (CheckFractals ());
	}

	public override void LateInit ()
	{
		base.LateInit ();
		VerifyExploreMode ();
		mFractalBoundsChanged = true;
	}

	private void InitTerrain ()
	{
		if (TargetTerrain) {
			TargetTerrain.drawHeightmap = true;
			TargetTerrain.terrainData.heightmapResolution = 512;
			TargetTerrain.terrainData.size = new Vector3 (512f, (float)InitialIterations, 512f);
			TargetTerrain.transform.position = new Vector3 (-64f, 0, -64f);
		}
	}

	private void InitFractal ()
	{
		mMandBig.Iterations = InitialIterations;
		mMandSmall.Iterations = InitialIterations;

		mMandBig.SetCenter (0.7f, 0);
		mMandSmall.SetCenter (0.7f, 0);

		mMandBig.SetBounds (mFractalMin.x, mFractalMin.y, mFractalMax.x, mFractalMax.y);
		mMandSmall.SetBounds (mFractalMin.x / 2f, mFractalMin.y / 2f, mFractalMax.x / 2f, mFractalMax.y / 2f);

		mMandBig.SetDataSize (100, 100);
		mMandBig.SetDataResolution (100, 100);

		mMandSmall.SetDataSize (512, 512);
		mMandSmall.SetDataResolution (100, 100);

		mMandBig.SetInitialIterationPoint (0.3f, 0.6f);
		mMandSmall.SetInitialIterationPoint (0.3f, 0.6f);

		mMandBig.SetIteratingFunction (this.QuadJulietIterate);
		mMandBig.OnDataGenerated += OnFractalDataUpdate;
	}

	public void QuadJulietIterate (int iterations, out float returnVal, params FractalComplexNumber[] complexNos)
	{
		returnVal = -1f;
		FractalComplexNumber z = new FractalComplexNumber ();
		FractalComplexNumber c = complexNos [0];
		int iterationReached = iterations + 1;
		for (int i = 0; i < iterations + 1; ++i) {
			z = z * z + c;
			if (z.Absolute >= 2f) {
				iterationReached = i;
				break;
			}
		}
		if (iterationReached < iterations) {
			float correctionVal = Mathf.Log (Mathf.Log (z.Absolute) / Mathf.Log (2f)) / Mathf.Log (2f);
			returnVal = (float)(iterationReached + 1) - correctionVal;
		}
		returnVal = returnVal / (float)iterations;
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void FixedUpdate ()
	{
//		CheckUpdateFractal ();
//		CheckTerrainHeights ();
//		if (mTerrainTextureDirty) {
//			mTerrainTextureDirty = false;
//			CheckTerrainTexture ();
//		}
	}

	private IEnumerator CheckFractals ()
	{
		while (true) {
			yield return StartCoroutine (CheckUpdateFractal ());
			yield return StartCoroutine (CheckTerrainHeights ());
			if (mTerrainTextureDirty) {
				mTerrainTextureDirty = false;
				yield return StartCoroutine (CheckTerrainTexture ());
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	private IEnumerator CheckUpdateFractal ()
	{
		if (mFractalBoundsChanged) {
			mFractalBoundsChanged = false;
			mMandBig.Iterations = (int)((float)InitialIterations / Mathf.Pow (mFractalScale, 2));
			mMandBig.SetBounds (mFractalMin.x, mFractalMin.y, mFractalMax.x, mFractalMax.y);
			yield return null;
			//StartCoroutine (mMandBig.UnityRefreshData ());
			mMandBig.RefreshDataSamples ();
			yield return null;
		}
	}

	private void OnFractalDataUpdate (params object[] args)
	{
		mFractalData = mMandBig.Data;
		mTerrainHeightsDirty = true;
		//StartCoroutine (CustomArrayUtility.FloatArray2dResize.UnityResize2dArray (mFractalData, 512, 512, this.OnNewDataReceived));
		//Invoke ("StartResizeArray", 0);
	}

	private void OnNewDataReceived (System.Object obj)
	{
		mFractalData = (float[,])obj;
	}

	private IEnumerator CheckTerrainHeights ()
	{
		if (TargetTerrain && mTerrainHeightsDirty) {
			mTerrainHeightsDirty = false;
			mTerrainTextureDirty = true;
			yield return StartCoroutine (CustomArrayUtility.FloatArray2dResize.UnityResize2dArray (mFractalData, 512, 512, this.OnNewDataReceived));
			float maxheight = (float)mMandBig.Iterations;
			TargetTerrain.terrainData.size = new Vector3 (128f, maxheight, 128f);
			float[,] dataToUse = new float[0, 0];
			dataToUse = mFractalData.SwapXY ();
			//yield return StartCoroutine (Utility.SwapArrayXY (mFractalData, dataToUse));
			yield return null;
			TargetTerrain.terrainData.SetHeights (0, 0, dataToUse);
		}
	}

	private IEnumerator CheckTerrainTexture ()
	{
		Texture2D terrainsplat = TargetTerrain.terrainData.splatPrototypes [0].texture;
		if (terrainsplat == null) {
			terrainsplat = new Texture2D (512, 512, TextureFormat.ARGB32, true);
			TargetTerrain.terrainData.splatPrototypes [0].texture = terrainsplat;
		} else {
			terrainsplat.Resize (512, 512, TextureFormat.ARGB32, true);
		}

//		if (mTempTexPointer) {
//			mTempTexPointer.Resize (512, 512, TextureFormat.ARGB32, true);
//		} else {
//			mTempTexPointer = new Texture2D (512, 512, TextureFormat.ARGB32, true);
//		}


		TargetTerrain.terrainData.splatPrototypes [0].tileSize = new Vector2 (512f, 512f);
		for (int y = 0; y<=mFractalData.GetUpperBound(1); ++y) {
			for (int x=0; x<=mFractalData.GetUpperBound(0); ++x) {
				Color newColor = TestColor * mFractalData [x, y] * mMandBig.Iterations / InitialIterations;
				terrainsplat.SetPixel (x, y, newColor);
				//yield return null;
			}
			terrainsplat.Apply ();
			yield return null;
		}
		
		//Graphics.Blit (mTempTexPointer, TargetTerrain.terrainData.splatPrototypes [0].texture);
	}



	private void MoveFractalBounds (Vector2 direction)
	{
		if (direction.sqrMagnitude > 0f) {
			mFractalMax += direction * FractalBoundMoveSpeed * Time.deltaTime * mFractalScale;
			mFractalMin += direction * FractalBoundMoveSpeed * Time.deltaTime * mFractalScale;
			mFractalBoundsChanged = true;
		}
	}
}
