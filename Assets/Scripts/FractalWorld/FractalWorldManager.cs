using UnityEngine;
using System.Collections;

public class FractalWorldManager : MonoBehaviour
{

	public CharacterStateMachine Player;

	// Use this for initialization
	void Start ()
	{
		if (Player != null) {
			Player.AddController (WP.Controller.StandalonePlayerController.Singleton);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
