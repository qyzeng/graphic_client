using UnityEngine;
using System.Collections; 
using System; 
using System.IO; 
using System.Net.Sockets;

public class s_TCP : MonoBehaviour
{ 
	
	internal Boolean socketReady = false;

	TcpListener mListener;
	TcpClient mClient;
	NetworkStream mClientStream;
	StreamWriter mClientWriter;
	StreamReader mClientReader;
	String Host = "localhost";
	int ClientPort = 51111;
	int ServerPort = 52111;
	System.Collections.Generic.List<Socket> mSocketList = new System.Collections.Generic.List<Socket> ();
	
	void Start ()
	{
		mListener = new TcpListener (System.Net.IPAddress.Loopback, ServerPort);
		mListener.Start ();
	}
	void Update ()
	{
		if (mListener != null) {
			if (mListener.Pending ()) {
				Socket socket = mListener.AcceptSocket ();

			}
		}
	}
	
	// ** 
	public void setupSocket ()
	{
		try {
			mClient = new TcpClient (Host, ClientPort);
			mClientStream = mClient.GetStream (); 
			mClientStream.ReadTimeout = 1;
			mClientWriter = new StreamWriter (mClientStream); 
			mClientReader = new StreamReader (mClientStream); 
			socketReady = true;
		} catch (Exception e) {
			Debug.Log ("Socket error: " + e); 
		}
	}
	
	public void writeSocket (string theLine)
	{ 
		if (!socketReady) 
			return;
		String foo = theLine + "\r\n";
		mClientWriter.Write (foo);
		mClientWriter.Flush ();
	} 
	
	public String readSocket ()
	{
		if (!socketReady) 
			return "";
		try {
			return mClientReader.ReadLine ();
		} catch (Exception e) {
			return "";
		}
	}
	
	public void closeSocket ()
	{
		if (!socketReady)
			return; 
		mClientWriter.Close ();
		mClientReader.Close ();
		mClient.Close ();
		socketReady = false;
	}

	void OnDestroy ()
	{
		mListener.Stop ();
	}

	void OnApplicationQuit ()
	{
		mListener.Stop ();
	}
	
} // end class s_TCP 
