using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Balloon : VRTK_InteractableObject {

	[HideInInspector]
	public int levelToLoad;

	public GameObject player;
	public float balloonSpeed;

	private bool isUsing;

	void Start() {
		levelToLoad = CheckOnboardingCompleted ();
		Debug.Log ("level to load: " + CheckOnboardingCompleted());
	}

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

	private int CheckOnboardingCompleted() {
		if (PlayerPrefs.HasKey(NohmConstants.OnboardingCompleted)) {

			int completed = PlayerPrefs.GetInt (NohmConstants.OnboardingCompleted);
			if (completed == 0)
				return 1;
			else
				return 2;

		} else {
			PlayerPrefs.SetInt (NohmConstants.OnboardingCompleted, 0);
			return 1;
		}
	}
		
}
