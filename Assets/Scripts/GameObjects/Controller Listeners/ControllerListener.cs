using System.Collections;
using System.Collections.Generic;
using VRTK;
using UnityEngine;

public class ControllerListener : MonoBehaviour {

	public VoiceSpawner voiceSpawner;

	// Use this for initialization
	void Start () {
		// GetComponent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);
		GetComponent<VRTK_ControllerEvents> ().ButtonTwoPressed += new ControllerInteractionEventHandler (ButtonTwoPressed);
	}

	private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e) {
		VRTK_Logger.Info("Controller on index '" + index + "' " + button + " has been " + action
			+ " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
	}

	private void ButtonTwoPressed(object sender, ControllerInteractionEventArgs e) {
		voiceSpawner.ActivateSearchArtist ();
		DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "ButtonTwo", "pressed", e);
	}
}
