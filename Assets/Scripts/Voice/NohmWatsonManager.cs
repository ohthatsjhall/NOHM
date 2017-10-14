using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NohmWatsonManager : MonoBehaviour {

	public APIManager apiManager;

	private string welcomeString = "Hello Dad";
	private SpeechToTextManager speechToText;
	private TextToSpeechManager textToSpeech;
	private ConversationManager conversationManager;

	void Awake () {
		speechToText = GetComponent<SpeechToTextManager>();
		textToSpeech = GetComponent<TextToSpeechManager>();
		conversationManager = GetComponent<ConversationManager>();
	}


	// Use this for initialization
	void Start () {
		SayString (welcomeString);
	}

	public void RecognizeQuestion (string finalText) {
		conversationManager.SetQuestions(finalText);
	}

	public void SayString(string speechText){
		textToSpeech.Say(speechText);
	}

	public void StartRecording() {
		speechToText.StartRecording ();
	}

	public void StopRecording() {
		speechToText.StopRecording ();
	}

	public void SearchForArtist(string artist) {
		apiManager.artist = artist;
		apiManager.SearchTracksForArtist (artist);
	}
}