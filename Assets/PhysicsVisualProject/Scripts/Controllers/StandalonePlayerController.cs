using UnityEngine;
using System.Collections;
using System.Timers;
//using WP.Controller;

namespace WP.Controller
{
	public class StandalonePlayerController : BaseController<StandalonePlayerController>
	{
		private Timer _actionCooldownTimer = new Timer (400);
		//private bool _actionTriggered = false;

		private void OnControlModeChanged (CONTROLLER_MODE mode)
		{
			ResetControllerCommands ();
			Cursor.lockState = mode == CONTROLLER_MODE.ACTION ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = (mode == CONTROLLER_MODE.NORMAL);
		}

		private void ReadRawInput ()
		{
			if (Input.GetKeyDown (KeyCode.F)) {
				ControllerUtility.ControllerMode = (ControllerUtility.ControllerMode == CONTROLLER_MODE.ACTION) ? CONTROLLER_MODE.NORMAL : CONTROLLER_MODE.ACTION;
			}
			if (ControllerUtility.ControllerMode == CONTROLLER_MODE.ACTION) {
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

			if (ControllerUtility.ControllerMode == CONTROLLER_MODE.NORMAL) {
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
        
			if (horizontal != 0 || vertical != 0 || ControllerUtility.ControllerMode == CONTROLLER_MODE.ACTION) {
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
			OnControlModeChanged (ControllerUtility.ControllerMode);
			ControllerUtility.OnControllerModeChanged += this.OnControlModeChanged;
			_actionCooldownTimer.Elapsed += OnActionCooldownEnd;
		}
	
		// Update is called once per frame
		void Update ()
		{
			ReadRawInput ();
			DispatchCommands ();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			ControllerUtility.OnControllerModeChanged -= this.OnControlModeChanged;
			_actionCooldownTimer.Elapsed -= OnActionCooldownEnd;
		}
	}
}
