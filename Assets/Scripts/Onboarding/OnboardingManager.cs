using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using BestHTTP;
using IBM.Watson.DeveloperCloud.Widgets;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;

public class OnboardingManager : MonoBehaviour {

	public Canvas worldSpaceCanvas;
	public GameObject records;
	public Light pointLight;
	public GameObject recordPlayer;
	public SpeechToTextManager microphone;
	//public Canvas transitionCanvas;
	public GameObject leftController;
	public GameObject rightController;

	// Use this for initialization
	void Start () {
		//StartOnboardingTutorial ();
	}
	
	// Update is called once per frame
	void Update () {

	}

}
