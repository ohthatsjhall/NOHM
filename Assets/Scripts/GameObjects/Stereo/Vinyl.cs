using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Vinyl : VRTK_InteractableObject {

	[HideInInspector]
	public Artist artist;
	private VRTK_ControllerEvents controllerEvents;


	public override void StartUsing (VRTK_InteractUse currentUsingObject) {
		base.StartUsing (currentUsingObject);
		controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();
	}


	public override void StopUsing (VRTK_InteractUse previousUsingObject) {
		base.StopUsing (previousUsingObject);
		controllerEvents = null;
	}

	// VRTK 3.1.0
	/*

	public override void StartUsing(GameObject usingObject) {
		base.StartUsing(usingObject);
		controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();
	}

	public override void StopUsing(GameObject previousUsingObject) {
		base.StopUsing(previousUsingObject);
		controllerEvents = null;
	}
		
*/

}
