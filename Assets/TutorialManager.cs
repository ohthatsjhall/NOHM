using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRTK;

public enum OnboardingStage {LastMoonOnboardingForNohm, PlayMusic, OnboardingGrabRecord, OnboardingClose};

public class TutorialManager : MonoBehaviour {

	[Header("Managers")]
	public OnboardingManager onboardingManager;

	[Header("DEBUGGING")]
	public int onboardingCompleted;
	public OnboardingStage onboardingStage;

	private VRTK_ControllerEvents controllerEvents;
	private NohmWatsonManager nohmWatsonManager;
	private Animator pointLightAnimator;

	void Awake() {
		nohmWatsonManager  = GetComponent<NohmWatsonManager> ();
		pointLightAnimator = onboardingManager.pointLight.GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt ("OnboardingCompleted", onboardingCompleted);
		onboardingStage = OnboardingStage.LastMoonOnboardingForNohm;
		controllerEvents = onboardingManager.rightController.GetComponent<VRTK_ControllerEvents> ();
		nohmWatsonManager.RecognizeQuestion(onboardingStage.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public IEnumerator DelayMethod(float delay, string[] values) {
		yield return new WaitForSeconds (delay);
		for (int i = 0; i < values.Length; i++) {
			OnboardingStageProceedure (onboardingStage, i, values);
		}
	}



	private void OnboardingStageProceedure(OnboardingStage onboardingStage, int index, string[] values) {

		string value = values [index];

		switch (onboardingStage) {
		case OnboardingStage.PlayMusic:
			if (index == values.Length - 1) {
				TriggerHapticPulseOnController ();
				EnableControllers (true);
			}
			break;
		default:
			Debug.Log ("default case");
			break;
		}
		nohmWatsonManager.SayString (value);
		WorldCanvasSetText (value);
	}


	// World Canvas
	private void ClearCanvas() {
		onboardingManager.worldSpaceCanvas.GetComponentInChildren<Text> ().text = "";
	}

	private void WorldCanvasSetText(string text) {
		onboardingManager.worldSpaceCanvas.GetComponentInChildren<Text> ().text = text;
	}

	private void TriggerHapticPulseOnController() {
		VRTK_ControllerHaptics.TriggerHapticPulse (
			VRTK_ControllerReference.GetControllerReference (controllerEvents.gameObject),
			10.5f);
	}

	private void EnableControllers(bool isEnabled) {
		onboardingManager.rightController.GetComponent<OnboardingControllerListener> ().enabled = isEnabled;
		onboardingManager.leftController.GetComponent<OnboardingControllerListener> ().enabled = isEnabled;
	}

}
