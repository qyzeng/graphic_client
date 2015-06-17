using UnityEngine;
using System.Collections;

public class ExampleNetworkUIScript : MonoBehaviour
{
	private bool _isServer = false;
	private int _portNumber = 25000;
	private string _hostString = "127.0.0.1";
	private bool _isConnected = false;

	public WorldManager WorldManager;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnGUI ()
	{
		GUILayout.BeginVertical ();
		string s_c_string = _isServer ? "Switch to Client Mode" : "Switch to Host Mode";
		if (GUILayout.Button (s_c_string)) {
			_isServer = !_isServer;
		}
		if (!_isServer) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Host");
			_hostString = GUILayout.TextField (_hostString);
			GUILayout.EndHorizontal ();
		}
		if (!_isConnected) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Port");
			_portNumber = int.TryParse (GUILayout.TextField (_portNumber.ToString ()), out _portNumber) ? _portNumber : 25000;
			GUILayout.EndHorizontal ();
		}
		string connectButton = _isConnected ? "Disconnect" : "Connect";
		if (GUILayout.Button (connectButton)) {
			if (_isConnected) {
				CloseConnection ();
			} else {
				StartConnection ();
			}
		}
		GUILayout.EndVertical ();
	}

	private void StartConnection ()
	{
		if (_isServer) {
			Network.InitializeServer (99, _portNumber, true);
			InitWorld ();
		} else {
			Network.Connect (_hostString, _portNumber, "");
		}
		_isConnected = true;
	}

	void InitWorld ()
	{
		if (WorldManager) {
			WorldManager.Init ();
			WorldManager.LateInit ();
		}
	}

	void OnConnectedToServer ()
	{
		InitWorld ();
	}
	

	void OnPlayerDisconnected (NetworkPlayer player)
	{
		Network.RemoveRPCs (player);
		Network.DestroyPlayerObjects (player);
	}

	private void CloseConnection ()
	{
		WorldManager.ResetPlayer ();
		Network.Disconnect ();
		_isConnected = false;
	}
}
