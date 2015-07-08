using UnityEngine;
using System.Collections;

namespace WP.Controller
{
	public class OculusController : BaseController<OculusController>
	{
		private const float LR_THRESHOLD = 45f;

		private void ReadOculusData ()
		{
			OVRPose pose = OVRManager.display.GetEyePose (OVREye.Left);
			float lr_rotation = pose.orientation.eulerAngles.y;
			//Debug.Log ("OVR euler y: " + (360f - lr_rotation).ToString ());
			float yRot = 0f;
			if (lr_rotation > 180f)
				lr_rotation = lr_rotation - 360f;

			if (Mathf.Abs (lr_rotation) > LR_THRESHOLD) {
				int dir = (lr_rotation > 0) ? 1 : -1;
				yRot = dir * ((Mathf.Abs (lr_rotation)) - LR_THRESHOLD) / (180f - LR_THRESHOLD);
			}
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_HORIZONTAL, yRot));
		}

		private void ReadRawInput ()
		{
			if (Input.GetKeyDown (KeyCode.F)) {
				ControllerUtility.ControllerMode = (ControllerUtility.ControllerMode == CONTROLLER_MODE.ACTION) ? CONTROLLER_MODE.NORMAL : CONTROLLER_MODE.ACTION;
			}
			if (ControllerUtility.ControllerMode == CONTROLLER_MODE.NORMAL) {
				float camhorizontal = 0f;
				if (Input.GetKey (KeyCode.Q)) {
					camhorizontal = -1f;
				} else if (Input.GetKey (KeyCode.E)) {
					camhorizontal = 1f;
				}
				_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_HORIZONTAL, camhorizontal));
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
				if (Camera.main != null) {
					CommandFiredEventArgs playerRotate = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_ROTATION, Camera.main.transform.rotation);
					_commandList.Add (playerRotate);
				} else {
					Debug.Log ("Help");
				}
			}
		}

		private void Update ()
		{
			ReadOculusData ();
			ReadRawInput ();
			DispatchCommands ();
		}
	}
}
