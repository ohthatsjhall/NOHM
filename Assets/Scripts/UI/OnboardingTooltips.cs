using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class OnboardingTooltips : MonoBehaviour {

	public bool highlightBodyOnlyOnCollision = false;
	public bool pulseTriggerHighlightColor = true;
	public Color highlightColor = Color.yellow;

	private VRTK_ControllerTooltips tooltips;
	private VRTK_ControllerHighlighter highligher;
	private VRTK_ControllerEvents events;
	private Color pulseColor = Color.black;
	private Color currentPulseColor;
	private float highlightTimer = 0.5f;
	private float pulseTimer = 0.75f;
	private float dimOpacity = 0.8f;
	private float defaultOpacity = 1f;
	private bool highlighted;

	void OnEnable() {
		if (pulseTriggerHighlightColor) {
			Debug.Log ("enabled!");
			InvokeRepeating("PulseGrip", pulseTimer, pulseTimer);
		}
	}

	// Use this for initialization
	private void Start () {
		if (GetComponent<VRTK_ControllerEvents>() == null) {
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerAppearance_Example", "VRTK_ControllerEvents", "the same"));
			return;
		}

		events = GetComponent<VRTK_ControllerEvents>();
		highligher = GetComponent<VRTK_ControllerHighlighter>();
		tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
		currentPulseColor = pulseColor;
		highlighted = false;

		//Setup controller event listeners

		events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
		events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

		tooltips.ToggleTips(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void PulseGrip() {
		highligher.HighlightElement(SDK_BaseController.ControllerElements.GripRight, currentPulseColor, pulseTimer);
		highligher.HighlightElement (SDK_BaseController.ControllerElements.GripLeft, currentPulseColor, pulseTimer);
		currentPulseColor = (currentPulseColor == pulseColor ? highlightColor : pulseColor);
	}

	private void DoGripPressed(object sender, ControllerInteractionEventArgs e) {
		tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
		highligher.HighlightElement(SDK_BaseController.ControllerElements.GripLeft, highlightColor, highlightTimer);
		highligher.HighlightElement(SDK_BaseController.ControllerElements.GripRight, highlightColor, highlightTimer);
		if (pulseTriggerHighlightColor) {
			InvokeRepeating("PulseGrip", pulseTimer, pulseTimer);
		}
		VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), dimOpacity);
	}

	private void DoGripReleased(object sender, ControllerInteractionEventArgs e) {
		tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
		highligher.UnhighlightElement(SDK_BaseController.ControllerElements.GripLeft);
		highligher.UnhighlightElement(SDK_BaseController.ControllerElements.GripRight);
		if (pulseTriggerHighlightColor) {
			InvokeRepeating("PulseGrip", pulseTimer, pulseTimer);
		}
		if (!events.AnyButtonPressed()) {
			VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), defaultOpacity);
		}
	}
}
