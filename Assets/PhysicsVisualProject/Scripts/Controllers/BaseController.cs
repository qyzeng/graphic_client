using UnityEngine;
using System.Collections;

namespace WP.Controller
{
	public class BaseController<T> : MonoBehaviour, IController where T : MonoBehaviour, IController
	{
		public event ControllerCommandsFireHandler OnControllerCommandsFired;

		protected System.Collections.Generic.List<CommandFiredEventArgs> _commandList = new System.Collections.Generic.List<CommandFiredEventArgs> ();

		private static T _instance = default(T);
		public static T Singleton {
			get {
				if (_instance == null) {
					Component[] allcomponents = Object.FindObjectsOfType<Component> ();
					foreach (Component component in allcomponents) {
						if (component is T) {
							_instance = (T)component;
							break;
						}
					}

					if (_instance == null) {
						GameObject instObj = new GameObject (typeof(T).ToString ());
						GameObject.DontDestroyOnLoad (instObj);
						_instance = (T)instObj.AddComponent (typeof(T));
					}
				}
				return _instance;
			}
		}
		
		protected void DispatchCommands ()
		{
			if (OnControllerCommandsFired != null && _commandList.Count > 0) {
				OnControllerCommandsFired (_commandList);
			}
			_commandList.Clear ();
		}

		protected virtual void OnDestroy ()
		{
			this.OnControllerCommandsFired = null;
		}
	}
}
