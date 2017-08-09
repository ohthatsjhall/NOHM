using System.Collections;
using System.Collections.Generic;
using VRTK;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class TrackSelector : VRTK_InteractableObject {

	public GameObject CameraEye;
	public GameObject CursorRenders;
	[SerializeField]
	APIManager manager;

	private GameObject keyboard;
	private GameObject inputField;
	private VRTK_ControllerEvents controllerEvents;
	private Animator TrackSelectorAnimator;

	protected override void Awake () {
		base.Awake ();
		TrackSelectorAnimator = GetComponent<Animator> ();
		keyboard   = gameObject.transform.GetChild (1).gameObject;
		inputField = gameObject.transform.GetChild (0).gameObject;
	}

	public override void StartUsing (GameObject currentUsingObject) {
		base.StartUsing (currentUsingObject);
		controllerEvents = currentUsingObject.GetComponent<VRTK_ControllerEvents> ();
		ToggleKeyboard (true);
	}

	public override void StopUsing(GameObject previousUsingObject) {
		base.StopUsing(previousUsingObject);
		controllerEvents = null;
		ToggleKeyboard (false);
		string searchText = inputField.GetComponentInChildren<Text> ().text;
		SearchWithText (searchText);
	}

	private void SearchWithText(string searchText) {
		manager.artist = searchText;
		//manager.FindArtistOrTrack ();
		inputField.GetComponentInChildren<Text> ().text = "";
	}

	private void ToggleKeyboard(bool shouldAnimate) {
		if (shouldAnimate) {
			CursorRenders.SetActive (true);
			CameraEye.GetComponent<PostProcessingBehaviour> ().enabled = false;
			TrackSelectorAnimator.SetBool ("keyboardPresented", true);
		} else {
			CursorRenders.SetActive (false);
			CameraEye.GetComponent<PostProcessingBehaviour> ().enabled = true;
			TrackSelectorAnimator.SetBool ("keyboardPresented", false);
		}
	}

}
