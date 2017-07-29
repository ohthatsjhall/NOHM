using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerAnimateTurntable : MonoBehaviour {

	[SerializeField]
	Animator turntableAnimator;
	[SerializeField]
	float spinSpeed = 55.0f;
	[SerializeField]
	GameObject demoVinyl;

	// Use this for initialization
	void Start () {
		turntableAnimator.Play ("Playback");
	}
	
	// Update is called once per frame
	void Update () {
		demoVinyl.transform.Rotate (new Vector3 (0.0f, spinSpeed * Time.deltaTime, 0.0f));
	}
}
