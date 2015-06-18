using UnityEngine;
using System.Collections;

namespace WP.Controller
{
	public class TouchJoystick : MonoBehaviour, IController
	{
		public event ControllerCommandsFireHandler OnControllerCommandsFired;
	}

}
