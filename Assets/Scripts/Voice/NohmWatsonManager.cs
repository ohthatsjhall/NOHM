﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using VRTK;
using NOHM;

public class NohmWatsonManager : MonoBehaviour {

	[HideInInspector]
	public APIManager apiManager;
	[HideInInspector]
	public RadioProAPIManager radioProManager;

	private SpeechToTextManager speechToText;
	private TextToSpeechManager textToSpeech;
	private ConversationManager conversationManager;
	[HideInInspector]
	public TutorialManager tutorialManager;
	[HideInInspector]
	public int buildIndex {
		get { return UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex; }
	}

	void Awake () 
	{
		CacheComponents ();
	}


	// Use this for initialization
	void Start () 
	{
		ConfigureComponentsByBuildIndex (buildIndex);
	}

	private void CacheComponents() 
	{
		speechToText = GetComponent<SpeechToTextManager>();
		textToSpeech = GetComponent<TextToSpeechManager>();
		conversationManager = GetComponent<ConversationManager>();
	}

	public void RecognizeQuestion (string finalText)
	{
		conversationManager.SetQuestions(finalText);
	}

	public void SayString(string speechText)
	{
		textToSpeech.Say(speechText);
	}

	public void StartRecording() 
	{
		speechToText.StartRecording ();
	}

	public void StopRecording() 
	{
		speechToText.StopRecording ();
	}

	public void SearchForArtist(string artist) 
	{
		apiManager.artist = artist;
		apiManager.SearchTracksForArtist (artist);
	}

	public void LoadLevel(int index)
	{
		VRTK_SDKManager.instance.UnloadSDKSetup ();
		SceneManager.LoadSceneAsync (index);
	}

	private void ConfigureComponentsByBuildIndex(int buildIndex) 
	{
		switch (buildIndex) {
		case 1:
			tutorialManager = GetComponent<TutorialManager> ();
			break;
		case 2:
			SayString ("Hello Dad");
			apiManager = GameObject.FindObjectOfType (typeof(APIManager)) as APIManager;
			break;
		case 3:
			Debug.Log ("Chill Radio");
			radioProManager = GameObject.FindObjectOfType (typeof(RadioProAPIManager)) as RadioProAPIManager;
			break;
		default:
			Debug.Log ("Scene Selector");
			break;
		}
	}

	// Radio
	public void PlayRadioStation(string genre) {
		List<RadioStation> radioSations = radioProManager.radioStations;
		List<string> urls = new List<string>();
		foreach (RadioStation station in radioSations) {
			
			if (station.genre == genre)
				urls.Add (station.url);
			
		}

		string url = urls[Random.Range(0, urls.Count - 1)];

		radioProManager.radioPlayer.Station.Url = url;
		radioProManager.radioPlayer.Play ();
	}

}