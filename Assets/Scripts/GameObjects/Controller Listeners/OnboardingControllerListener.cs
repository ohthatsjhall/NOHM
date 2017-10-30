using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class OnboardingControllerListener : MonoBehaviour {

	public NohmWatsonManager nohmWatsonManager;

	// Use this for initialization
	void Start () {
		//GetComponent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);
		GetComponent<VRTK_ControllerEvents> ().ButtonTwoPressed += new ControllerInteractionEventHandler (ButtonTwoPressed);
	}

	private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e) {
		VRTK_Logger.Info("Controller on index '" + index + "' " + button + " has been " + action
			+ " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
	}

	/*
	private void DoTriggerClicked(object sender, ControllerInteractionEventArgs e) {
		voiceSpawner.OnboardingTriggerPressed ();
		DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "clicked", e);
	}
*/
	private void ButtonTwoPressed(object sender, ControllerInteractionEventArgs e) {
		DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "Nohm Button", "pressed", e);
		nohmWatsonManager.StartRecording ();
	}

}
