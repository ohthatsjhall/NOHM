using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Vinyl : VRTK_InteractableObject {

	[HideInInspector]
	public Artist artist;
	private VRTK_ControllerEvents controllerEvents;

	public override void StartUsing(GameObject usingObject) {
		base.StartUsing(usingObject);
		controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();
	}

	public override void StopUsing(GameObject previousUsingObject) {
		base.StopUsing(previousUsingObject);
		controllerEvents = null;
	}
		


}
