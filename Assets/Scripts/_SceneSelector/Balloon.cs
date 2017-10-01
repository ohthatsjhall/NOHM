using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Balloon : VRTK_InteractableObject {

	public int levelToLoad;

	[SerializeField] GameObject player;
	[SerializeField] float balloonSpeed;

	private bool isUsing;


	protected override void Update ()
	{
		base.Update ();
		if (isUsing) {
			Vector3 floatVelocity = new Vector3 (0.0f, balloonSpeed * Time.deltaTime, 0.0f);
			FloatPlayer (floatVelocity);
		}
	}
		

	public override void StartUsing (VRTK_InteractUse currentUsingObject)
	{
		base.StartUsing (currentUsingObject);
		isUsing = true;
	}

	public override void StopUsing (VRTK_InteractUse previousUsingObject)
	{
		base.StopUsing (previousUsingObject);
		isUsing = false;
	}

	private void FloatPlayer(Vector3 floatVelocity)
	{
		player.transform.Translate (floatVelocity);
		gameObject.transform.Translate (floatVelocity);
	}
		
}
