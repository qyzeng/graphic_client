using UnityEngine;
using System.Collections;

namespace WP.Controller
{/*
	public class MobilePlayerController : MonoBehaviour, IController
	{
		private static MobilePlayerController _instance = null;

		public static MobilePlayerController Singleton {
			get {
				if (_instance == null) {
					GameObject instObj = new GameObject ("MobilePlayerController");
					_instance = instObj.AddComponent<MobilePlayerController> ();
					GameObject.DontDestroyOnLoad (instObj);
				}
				return _instance;
			}
		}

		public GameObject JoystickPrefabReference;
    
		private TouchJoystick _leftJoystick = null;
		private TouchJoystick _rightJoystick = null;
    
		public TouchJoystick LeftJoystick {
			get {
				if (_leftJoystick == null) {
					GameObject leftJoyObj = Instantiate (JoystickPrefabReference) as GameObject;
					GameObject.DontDestroyOnLoad (leftJoyObj);
					_leftJoystick = leftJoyObj.GetComponentInChildren<TouchJoystick> ();
				}
				return _leftJoystick;
			}
		}
    
		public TouchJoystick RightJoystick {
			get {
				if (_rightJoystick == null) {
					GameObject rightJoyObj = Instantiate (JoystickPrefabReference) as GameObject;
					GameObject.DontDestroyOnLoad (rightJoyObj);
					_rightJoystick = rightJoyObj.GetComponentInChildren<TouchJoystick> ();
				}
				return _rightJoystick;
			}
		}

		private System.Collections.Generic.List<CommandFiredEventArgs> _CommandsForDispatching = new System.Collections.Generic.List<CommandFiredEventArgs> ();

		public event ControllerCommandsFireHandler OnControllerCommandsFired;
        
		private void Awake ()
		{
			JoystickPrefabReference = Resources.Load<GameObject> ("System/TouchJoystickPrefab");
		}
    
		// Use this for initialization
		void Start ()
		{
			RightJoystick.OnControllerCommandsFired += OnRightTouchJoystickCommand;
			LeftJoystick.OnControllerCommandsFired += OnLeftTouchJoystickCommand;
			StartCoroutine (ProcessCommands ());
		}
    
		private void OnRightTouchJoystickCommand (System.Collections.Generic.List<CommandFiredEventArgs> commandList)
		{
			foreach (CommandFiredEventArgs command in commandList) {
				if (command.Command == (ushort)RAW_AXES_TYPE.HORIZONTAL) {
					CommandFiredEventArgs newCommand = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_HORIZONTAL, (command.Arguments.Length > 0) ? command.Arguments [0] : 0);
					_CommandsForDispatching.Add (newCommand);
				}
			}
		}
    
		private void OnLeftTouchJoystickCommand (System.Collections.Generic.List<CommandFiredEventArgs> commandList)
		{
			foreach (CommandFiredEventArgs command in commandList) {
				if (command.Command == (ushort)RAW_AXES_TYPE.HORIZONTAL) {
					CommandFiredEventArgs moveCommand = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_HORIZONTAL, (command.Arguments.Length > 0) ? command.Arguments [0] : 0);
					_CommandsForDispatching.Add (moveCommand);
				}
            
				if (command.Command == (ushort)RAW_AXES_TYPE.VERTICAL) {
					CommandFiredEventArgs moveCommand = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_VERTICAL, (command.Arguments.Length > 0) ? command.Arguments [0] : 0);
					_CommandsForDispatching.Add (moveCommand);
				}
			}
			CommandFiredEventArgs setPlayerRotate = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_ROTATION, Camera.main.transform.rotation);
			_CommandsForDispatching.Add (setPlayerRotate);
		}
    
		private void DispatchCommands ()
		{
			if (OnControllerCommandsFired != null) {
				OnControllerCommandsFired (_CommandsForDispatching);
			}
			_CommandsForDispatching.Clear ();
		}

		private IEnumerator ProcessCommands ()
		{
			while (true) {
				DispatchCommands ();
				yield return new WaitForEndOfFrame ();
			}
		}
	
		// Update is called once per frame
		void Update ()
		{
        
			#if UNITY_EDITOR
			if (!Input.GetMouseButton (0)) {
				LeftJoystick.gameObject.SetActive (false);
				RightJoystick.gameObject.SetActive (false);
			}
			if (Input.GetMouseButtonUp (0)) {
				if (Input.mousePosition.x < (Screen.width >> 1) && LeftJoystick != null && LeftJoystick.gameObject.activeSelf) {
					LeftJoystick.gameObject.SetActive (false);
				}
				if (Input.mousePosition.x > (Screen.width >> 1) && RightJoystick != null && RightJoystick.gameObject.activeSelf) {
					RightJoystick.gameObject.SetActive (false);
				}
			}
			if (Input.GetMouseButtonDown (0)) {
				if (Input.mousePosition.x < (Screen.width >> 1) && LeftJoystick != null && !LeftJoystick.gameObject.activeSelf) {
					LeftJoystick.gameObject.SetActive (true);
					LeftJoystick.transform.position = new Vector3 (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0);
				}
				if (Input.mousePosition.x > (Screen.width >> 1) && RightJoystick != null && !RightJoystick.gameObject.activeSelf) {
					RightJoystick.gameObject.SetActive (true);
					RightJoystick.transform.position = new Vector3 (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0);
				}
			}
#else
            
            
            UITouch[] touches = UIBindingUtility.GetTouchesNotInUI();
            if (touches.Length == 0)
            {
                LeftJoystick.gameObject.SetActive(false);
                RightJoystick.gameObject.SetActive(false);
            }
        
            foreach (UITouch touch in touches)
            {
                if (LeftJoystick != null)
                {
                    if (touch.position.x < (Screen.width >> 1))
                    {
            
                        if (LeftJoystick.gameObject.activeSelf && touch.phase == TouchPhase.Ended)
                        {
                            LeftJoystick.gameObject.SetActive(false);
                        }
            
                        if (!LeftJoystick.gameObject.activeSelf && touch.phase == TouchPhase.Began)
                        {
                            LeftJoystick.transform.position = new Vector3(touch.position.x / Screen.width, touch.position.y / Screen.height, 0);
                            LeftJoystick.gameObject.SetActive(true);    
                        
                        }
                    }
                }
                if (RightJoystick != null)
                {
                    if (touch.position.x > (Screen.width >> 1))
                    {
            
                        if (RightJoystick.gameObject.activeSelf && touch.phase == TouchPhase.Ended)
                        {
                            RightJoystick.gameObject.SetActive(false);
                        }
            
                        if (!RightJoystick.gameObject.activeSelf && touch.phase == TouchPhase.Began)
                        {
                            RightJoystick.transform.position = new Vector3(touch.position.x / Screen.width, touch.position.y / Screen.height, 0);
                            RightJoystick.gameObject.SetActive(true);
                        }
                    }
                }
            }
#endif
			//DispatchCommands();
		}
	}
	*/
}
