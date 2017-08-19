using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTutorial : MonoBehaviour {

	[SerializeField]
	private VoiceSpawner voiceSpawner;

	private bool isStartingPosition;
	private Vector3 startingPosition;

	void Awake() {
		startingPosition = GetComponent<Transform> ().position;
		isStartingPosition = startingPosition == this.transform.position ? true : false;
	}

	// Use this for initialization
	void Start () {
		voiceSpawner.TextToSpeechWithString ("Welcome to Gnome! To move around the house, press and hold the touch pad. Use the cursor to choose your desired destination. Give it a try!");
	}
	
	// Update is called once per frame
	void Update () {

		if (startingPosition != this.transform.position) {
			isStartingPosition = false;
		}

		if (!isStartingPosition) {
			Debug.Log ("moved");
			voiceSpawner.TextToSpeechWithString ("well done dad! It's time to spin. Sit back, relax, and put on your favorite bjorn.");//. Press the app button to search for an artist");
			enabled = false;
		}
	}
}
