using UnityEngine;
using System.Collections;
using WP.Character;

public class WorldManager : MonoBehaviour
{

	protected CharacterStateMachine _playerChar;
	public CharacterStateMachine PlayerChar {
		get {
			return _playerChar;
		}
	}
	
	public delegate void PlayerCharacterReadyHandler ();
	public event PlayerCharacterReadyHandler OnPlayerReady;
	
	public GameObject ReferenceModel;
	public GameObject ReferencePlayerObject;
	
	public CameraControl CamControl;
	public CameraControl OculusCamControl;
	[SerializeField]
	protected CameraControl
		_referenceCamControlToUse;
	protected CameraControl _currentCamControl;
	
	public Vector3 PlayerSpawnPoint;

	
	private Transform _worldCenter = null;
	public Transform WorldCenter {
		get {
			if (_worldCenter == null) {
				_worldCenter = GameObject.Find ("WorldCenter").transform;
				if (_worldCenter == null) {
					_worldCenter = new GameObject ("WorldCenter").transform;
					_worldCenter.position = Vector3.zero;
				}
			}
			return _worldCenter;
		}
	}
	
	public bool UseOculus {
		get {
			return _UseOculus;
		}
		set {
			if (_UseOculus != value) {
				_UseOculus = value;
				VerifyUseOculus ();
			}
		}
	}
	[SerializeField]
	protected bool
		_UseOculus = false;

	protected void VerifyUseOculus ()
	{
		if (_UseOculus) {
			_referenceCamControlToUse = OculusCamControl;
		} else {
			_referenceCamControlToUse = CamControl;
		}
	}

#if UNITY_EDITOR
	protected virtual void OnValidate ()
	{
		VerifyUseOculus ();
	}
#endif

	protected virtual void Start ()
	{
		UseOculus = OVRManager.display.isPresent;
	}

	public void InitCharacter (CharacterStateMachine character)
	{
//		if (ReferenceModel != null) {
//			character.ModelObject = (GameObject)GameObject.Instantiate (ReferenceModel);
//			character.ModelObject.transform.parent = _playerChar.transform;
//			character.ModelObject.transform.localPosition = Vector3.zero;
//			character.ModelObject.transform.localRotation = Quaternion.identity;
//		}
		if (OnPlayerReady != null) {
			OnPlayerReady ();
		}
	}
	
	public virtual void Init ()
	{
		OVRManager.DismissHSWDisplay ();
		if (ReferencePlayerObject != null) {
			_playerChar = ((GameObject)Network.Instantiate (ReferencePlayerObject, PlayerSpawnPoint, Quaternion.identity, 0)).GetComponent<CharacterStateMachine> ();
			InitCharacter (_playerChar);
		}
		VerifyUseOculus ();
		if (_referenceCamControlToUse != null) {
			_currentCamControl = ((GameObject)GameObject.Instantiate (_referenceCamControlToUse.gameObject)).GetComponent<CameraControl> ();
		}
	}
	public virtual void LateInit ()
	{
		WP.Controller.IController controllerToUse = null;
		if (UseOculus) {
			controllerToUse = WP.Controller.OculusController.Singleton;
		} else {
			controllerToUse = WP.Controller.StandalonePlayerController.Singleton;
		}
		if (_playerChar != null) {
			_playerChar.AddController (controllerToUse);
		}
		if (_currentCamControl != null) {
			_currentCamControl.AddController (controllerToUse);
			if (_playerChar)
				_currentCamControl.LookAtTarget = _playerChar.gameObject;
		}
	}
	
	public virtual void ResetPlayer ()
	{
		if (_playerChar != null) {
			Network.Destroy (_playerChar.gameObject);
		}
		if (_currentCamControl != null)
			Destroy (_currentCamControl.gameObject);
	}
}
