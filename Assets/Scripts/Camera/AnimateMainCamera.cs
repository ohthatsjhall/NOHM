using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateMainCamera : MonoBehaviour {

	public enum CameraDirection {Forward, Right, Left, Up, Back, Down};
	public CameraDirection direction;
	public bool useLocalSpace;

	[Range(0,4)]
	public float moveSpeed;
	private Camera mainCamera;
	private Vector3 cameraDirection;




	void Awake() {
		SetCameraDirection ();
	}

	// Use this for initialization
	void Start () {
		mainCamera = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		mainCamera.transform.Translate (cameraDirection * moveSpeed * Time.deltaTime);
	}

	void SetCameraDirection() {

		// add use local space for each CameraDirection

		if (direction == CameraDirection.Back) {
			if (useLocalSpace) {
				cameraDirection = transform.InverseTransformDirection (Vector3.forward);
			}
			cameraDirection = Vector3.back;
		} else if (direction == CameraDirection.Down) {
			cameraDirection = Vector3.down;
		} else if (direction == CameraDirection.Forward) {
			cameraDirection = Vector3.forward;
		} else if (direction == CameraDirection.Left) {
			cameraDirection = Vector3.left;
		} else if (direction == CameraDirection.Right) {
			cameraDirection = Vector3.right;
		} else if (direction == CameraDirection.Up) {
			cameraDirection = Vector3.up;
		}
	}
}
