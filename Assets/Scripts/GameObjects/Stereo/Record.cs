using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using VRTK;

public class Record : VRTK_InteractableObject {

	public bool isOnboarding;
	public Color errorColor;
	public Canvas canvas;
	public GameObject vinylPrefab;
	[HideInInspector]
	public Artist artist;
	[HideInInspector]
	public Texture2D vinylArtwork;

	private VRTK_ControllerEvents controllerEvents;
	private Animator canvasAnimator;

	protected override void Awake () {
		base.Awake ();
		canvasAnimator = canvas.GetComponent<Animator> ();
	}

	public override void StartTouching (VRTK_InteractTouch currentTouchingObject)
	{
		base.StartTouching (currentTouchingObject);
		Vector3 recordPosition = gameObject.transform.position;
		Quaternion recordRotation = gameObject.transform.rotation;
		if (isOnboarding) {
			canvas.transform.position = new Vector3 (recordPosition.x, recordPosition.y, recordPosition.z - 0.0037f);
		} else {
			canvas.transform.position = new Vector3 (recordPosition.x + 0.0037f, recordPosition.y, recordPosition.z);
		}
			
		canvas.GetComponentInChildren<Text> ().text = artist.trackName;

		controllerEvents = currentTouchingObject.GetComponent<VRTK_ControllerEvents> ();
		VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEvents.gameObject), 2.00f, 0.1f, 0.01f);

		canvasAnimator.SetBool ("isPresented", true);
	}


	public override void StopTouching (VRTK_InteractTouch previousTouchingObject)
	{
		base.StopTouching (previousTouchingObject);
		canvasAnimator.SetBool ("isPresented", false);
	}


	public override void StartUsing (VRTK_InteractUse currentUsingObject)
	{
		base.StartUsing (currentUsingObject);
		controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();

		GameObject vinyl = Instantiate (vinylPrefab) as GameObject;
		Vector3 controllerPosition = usingObject.transform.position;
		if (isOnboarding) {
			vinyl.transform.position = new Vector3 (controllerPosition.x, controllerPosition.y, controllerPosition.z - 0.045f);
		} else {
			vinyl.transform.position = new Vector3 (controllerPosition.x + 0.045f, controllerPosition.y, controllerPosition.z);
		}

		vinyl.GetComponent<Vinyl>().artist = artist;
		vinyl.GetComponent<MeshRenderer> ().materials [2].mainTexture = vinylArtwork;
	}


	public override void StopUsing (VRTK_InteractUse previousUsingObject)
	{
		base.StopUsing (previousUsingObject);
		controllerEvents = null;
	}

	public void SetTouchHighlightColorForValidURL(bool isValid) {
		if (isValid) {
			touchHighlightColor = new Color (57.0f / 2550.0f, 81.0f / 255.0f, 0.98f);
		} else {
			touchHighlightColor = errorColor;
		}
	}
		
}
