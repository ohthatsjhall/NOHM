using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.PostProcessing;


public class ShowKeyboard : MonoBehaviour {

	public GameObject CameraEye;

	private GameObject keyboard;
	private GameObject inputField;

	// Use this for initialization
	void Start () {
		keyboard   = gameObject.transform.GetChild (1).gameObject;
		inputField = gameObject.transform.GetChild (0).gameObject;
	}

	void OnTriggerEnter(Collider collider) {
		string colliderObject = collider.name;
		Debug.Log ("Enter: " + colliderObject);
		if (colliderObject == "SideA") {
			ToggleKeyboard (keyboard.activeInHierarchy);
		}
	}

	private void ToggleKeyboard(bool keyboardIsActive) {
		if (keyboardIsActive) {
			keyboard.SetActive (false);
			inputField.SetActive (false);
			CameraEye.GetComponent<PostProcessingBehaviour> ().enabled = true;
		} else {
			keyboard.SetActive (true);
			inputField.SetActive (true);
			CameraEye.GetComponent<PostProcessingBehaviour> ().enabled = false;
		}
	}
}
