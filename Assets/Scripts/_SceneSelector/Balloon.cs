using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Balloon : VRTK_InteractableObject {

	public float balloonSpeed;

	private bool isUsing;

	protected override void Update ()
	{
		base.Update ();
		Debug.Log ("isUsing?: " + isUsing);
		if (isUsing) {
			gameObject.transform.Translate (0.0f, balloonSpeed * Time.deltaTime, 0.0f);
		}
	}

	public override void StartUsing (VRTK_InteractUse currentUsingObject)
	{
		base.StartUsing (currentUsingObject);
		gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
		isUsing = true;
	}

	public override void StopUsing (VRTK_InteractUse previousUsingObject)
	{
		base.StopUsing (previousUsingObject);
		isUsing = false;
	}
		
}
