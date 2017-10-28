using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRTK;

public enum OnboardingStage {LastMoonOnboardingForNohm, PlayMusic, OnboardingGrabRecord, OnboardingClose, OnboardingCompleted};

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
		for (int i = 0; i < values.Length; i++) {
			yield return new WaitForSeconds (delay);
			OnboardingStageProceedure (onboardingStage, i, values);
		}
	}

	private void OnboardingStageProceedure(OnboardingStage onboardingStage, int index, string[] values) {

		string value = values [index];

		if (index == values.Length - 1) {
			switch (onboardingStage) {
			case OnboardingStage.PlayMusic:

				TriggerHapticPulseOnController ();
				EnableControllerListeners (true);

				break;
			case OnboardingStage.OnboardingGrabRecord:

				EnableControllerListeners (false);
				onboardingManager.recordPlayer.SetActive (true);
				onboardingManager.records.GetComponent<OnboardingRecords> ().AnimateRecords (true);
				pointLightAnimator.SetInteger ("Stage", 1);
				nohmWatsonManager.RecognizeQuestion (onboardingStage.ToString ());

				break;
			case OnboardingStage.OnboardingClose:
				pointLightAnimator.SetInteger ("Stage", 2);
				nohmWatsonManager.RecognizeQuestion (onboardingStage.ToString ());
				break;
			case OnboardingStage.OnboardingCompleted:
				Debug.Log ("got emmm >> move to vinyl");
				nohmWatsonManager.LoadLevel (2);
				break;
			default:
				Debug.Log ("default case");
				break;
			}

		}
			
		WorldCanvasSetText (value);
		nohmWatsonManager.SayString (value);
	}


	// World Canvas
	public void ClearCanvas() {
		onboardingManager.worldSpaceCanvas.GetComponentInChildren<Text> ().text = "";
	}

	private void WorldCanvasSetText(string text) {
		onboardingManager.worldSpaceCanvas.GetComponentInChildren<Text> ().text = text;
	}

	// Controllers

	private void TriggerHapticPulseOnController() {
		VRTK_ControllerHaptics.TriggerHapticPulse (
			VRTK_ControllerReference.GetControllerReference (controllerEvents.gameObject),
			10.5f);
	}

	private void EnableControllerListeners(bool isEnabled) {
		onboardingManager.rightController.GetComponent<OnboardingControllerListener> ().enabled = isEnabled;
		onboardingManager.leftController.GetComponent<OnboardingControllerListener> ().enabled = isEnabled;
	}

}
