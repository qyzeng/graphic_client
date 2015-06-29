using UnityEngine;
using System.Collections;
using System.Timers;
//using WP.Controller;

namespace WP.Controller
{
	public enum CONTROLLER_MODE
	{
		NORMAL,
		ACTION,
	}

	public class StandalonePlayerController : MonoBehaviour , IController
	{
		private static StandalonePlayerController _instance = null;
		public static StandalonePlayerController Singleton {
			get {
				if (_instance == null) {
					_instance = GameObject.FindObjectOfType<StandalonePlayerController> ();
					if (_instance == null) {
						GameObject instObj = new GameObject ("StandalonePlayerController");
						GameObject.DontDestroyOnLoad (instObj);
						_instance = instObj.AddComponent<StandalonePlayerController> ();
					}
				}
				return _instance;
			}
		}

		private static CONTROLLER_MODE _ControllerMode = CONTROLLER_MODE.NORMAL;
		public static CONTROLLER_MODE ControllerMode {
			get {
				return _ControllerMode;
			}
			set {
				if (_ControllerMode != value) {
					_ControllerMode = value;
					Singleton.ResetControllerCommands ();
					if (OnControllerModeChanged != null) {
						OnControllerModeChanged (_ControllerMode);
					}
				}
			}
		}

		public delegate void ControllerModeChangedHandler (CONTROLLER_MODE mode);
		public static event ControllerModeChangedHandler OnControllerModeChanged;

		public event ControllerCommandsFireHandler OnControllerCommandsFired;
    
		System.Collections.Generic.List<CommandFiredEventArgs> _commandList = new System.Collections.Generic.List<CommandFiredEventArgs> ();

		private Timer _actionCooldownTimer = new Timer (400);
		//private bool _actionTriggered = false;

		private void OnControlModeChanged (CONTROLLER_MODE mode)
		{
			Cursor.lockState = mode == CONTROLLER_MODE.ACTION ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = (mode == CONTROLLER_MODE.NORMAL);
		}

		private void ReadRawInput ()
		{
			if (Input.GetKeyDown (KeyCode.F)) {
				ControllerMode = (ControllerMode == CONTROLLER_MODE.ACTION) ? CONTROLLER_MODE.NORMAL : CONTROLLER_MODE.ACTION;
			}
			if (ControllerMode == CONTROLLER_MODE.ACTION) {
				float camhorizontal = Input.GetAxis ("Mouse X");
				float camvertical = Input.GetAxis ("Mouse Y");
				_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_HORIZONTAL, camhorizontal));
				_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_VERTICAL, -camvertical));

//				if (Input.GetButtonUp ("Fire1")) {
//					_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_ACTION));
//					//_actionTriggered = true;
//					//_actionCooldownTimer.Start();
//				}
			}

			if (ControllerMode == CONTROLLER_MODE.NORMAL) {
				float camhorizontal = 0f;
				if (Input.GetKey (KeyCode.Q)) {
					camhorizontal = -1f;
				} else if (Input.GetKey (KeyCode.E)) {
					camhorizontal = 1f;
				}
				float camvertical = 0f;
				if (Input.GetMouseButton (1)) {
					camhorizontal = Input.GetAxis ("Mouse X");
					camvertical = Input.GetAxis ("Mouse Y");
				}
				_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_HORIZONTAL, camhorizontal));
				_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_VERTICAL, camvertical));
			}

			if (Input.GetKeyDown (KeyCode.Space)) {
				_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_JUMP));
			}

			float horizontal = Input.GetAxisRaw ("Horizontal");
			float vertical = Input.GetAxisRaw ("Vertical");
			CommandFiredEventArgs playerHorizontal = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_HORIZONTAL, horizontal);
			CommandFiredEventArgs playerVertical = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_VERTICAL, vertical);
			_commandList.Add (playerHorizontal);
			_commandList.Add (playerVertical);
        
			if (horizontal != 0 || vertical != 0 || ControllerMode == CONTROLLER_MODE.ACTION) {
//                CommandFiredEventArgs resetCam = CommandFiredEventArgs.GenerateArgs((ushort)COMMAND_TYPE.RESET_CAMERA);
//                _commandList.Add(resetCam);
				if (Camera.main != null) {
					CommandFiredEventArgs playerRotate = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_ROTATION, Camera.main.transform.rotation);
					_commandList.Add (playerRotate);
				}
			}
        
			float scroll = Input.GetAxis ("Mouse ScrollWheel");
			if (scroll != 0) {
				CommandFiredEventArgs camZoom = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_ZOOM, scroll);
				_commandList.Add (camZoom);
			}
		}
    
		private void DispatchCommands ()
		{
			if (OnControllerCommandsFired != null && _commandList.Count > 0) {
				OnControllerCommandsFired (_commandList);
			}
			_commandList.Clear ();
		}

		private void ResetControllerCommands ()
		{
			_commandList.Clear ();
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_HORIZONTAL, 0f));
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_VERTICAL, 0f));
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_HORIZONTAL, 0f));
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_VERTICAL, 0f));
		}

		private void DoAction ()
		{
			CommandFiredEventArgs actionArg = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_ACTION);
			_commandList.Add (actionArg);

            
		}
    
		private void OnActionCooldownEnd (object sender, ElapsedEventArgs args)
		{
			//_actionTriggered = false;
		}

		// Use this for initialization
		void Start ()
		{
			OnControlModeChanged (ControllerMode);
			OnControllerModeChanged += this.OnControlModeChanged;
			_actionCooldownTimer.Elapsed += OnActionCooldownEnd;
		}
	
		// Update is called once per frame
		void Update ()
		{
			ReadRawInput ();
			DispatchCommands ();
		}

		void OnDestroy ()
		{
			this.OnControllerCommandsFired = null;
			OnControllerModeChanged -= this.OnControlModeChanged;
			_actionCooldownTimer.Elapsed -= OnActionCooldownEnd;
		}
	}
}
