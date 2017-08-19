using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Vinyl : VRTK_InteractableObject {

	[HideInInspector]
	public Artist artist;
	private VRTK_ControllerEvents controllerEvents;

	protected override void Awake () {
		base.Awake ();
		GetComponent<MeshRenderer> ().materials [2].mainTextureScale = new Vector2 (3f, 3f);
	}

	public override void StartUsing (VRTK_InteractUse currentUsingObject) {
		base.StartUsing (currentUsingObject);
		controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();
	}


	public override void StopUsing (VRTK_InteractUse previousUsingObject) {
		base.StopUsing (previousUsingObject);
		controllerEvents = null;
	}

}
