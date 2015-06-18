using UnityEngine;
using System.Collections;

public class CustomCameraController : MonoBehaviour
{
	//add update codes
	public float speed = 10.0F;
	float lookSpeed = 15.0F;
	float moveSpeed = 15.0F;
	
	float rotationX = 0.0F;
	float rotationY = 0.0F;

	private void Start ()
	{
		Init ();
	}

	public void Init ()
	{
		Vector3 initialRotEuler = gameObject.transform.rotation.eulerAngles;
		rotationX = initialRotEuler.y;
		rotationY = -initialRotEuler.x;
	}

	void Update ()
	{
		/*
		float depth = 0.0F;
		if (Input.GetKey (KeyCode.LeftShift))
			depth=1.0F;
		gameObject.transform.Translate(new Vector3(-Input.GetAxis("Horizontal") * speed * Time.deltaTime, -Input.GetAxis("Vertical") * speed * (1.0F-depth) * Time.deltaTime, 
		                                -Input.GetAxis("Vertical") *depth* speed * Time.deltaTime));*/
		
		if (Input.GetKey (KeyCode.R))
			gameObject.transform.Rotate (new Vector3 (0, 1.0F * speed * Time.deltaTime, 0));
		if (Input.GetKey (KeyCode.T))
			gameObject.transform.Rotate (new Vector3 (0, -1.0F * speed * Time.deltaTime, 0));
		if (Input.GetMouseButton (1)) {
			rotationX += Input.GetAxis ("Mouse X") * lookSpeed;
			rotationY += Input.GetAxis ("Mouse Y") * lookSpeed;
			rotationY = Mathf.Clamp (rotationY, -90, 90);
		
			transform.localRotation = Quaternion.AngleAxis (rotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis (rotationY, Vector3.left);
		}
		//transform.position += transform.forward * moveSpeed * Input.GetAxis ("Vertical") * Time.deltaTime;
		//transform.position += transform.right * moveSpeed * Input.GetAxis ("Horizontal") * Time.deltaTime;
		
	}
}

