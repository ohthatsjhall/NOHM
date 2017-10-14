using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NohmWatsonManager : MonoBehaviour {

	private string welcomeMessage = "Welcome to Gnome, when you are ready to listen to music, press the app button";
	private SpeechToTextManager speechToText;
	private TextToSpeechManager textToSpeech;
	private ConversationManager conversationManager;

	void Awake () {
		speechToText = GetComponent<SpeechToTextManager>();
		textToSpeech = GetComponent<TextToSpeechManager>();
		conversationManager = GetComponent<ConversationManager>();
	}

	void Start () {
		//StartCoroutine(WelcomeDelay());
		//speechToText.StartRecording();
	}

	public void RecognizeQuestion (string finalText) {
		conversationManager.SetQuestions(finalText);
	}

	public void SayString(string speechText){
		textToSpeech.Say(speechText);
	}

	IEnumerator WelcomeDelay () {
		yield return new WaitForSeconds(3.0f);
		textToSpeech.Say(welcomeMessage);
		// yield return new WaitForSeconds(10.0f);
		// speechToText.StopRecording();
	}

	public void StartRecording() {
		speechToText.StartRecording ();
	}

	public void StopRecording() {
		speechToText.StopRecording ();
	}
}